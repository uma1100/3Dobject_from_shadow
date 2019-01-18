using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texture_change : MonoBehaviour
{
    public int x_size;
    public int z_size;
    private double T = 100.0;
    private int count_sa = 0;
    private int count_t = 0;
    private SpriteRenderer spRen;
    private Math math1;
    //private mesh_test Mesh_Create; //メッシュ作成

    GameObject Mesh_Cre; //メッシュ作成コードオブジェクトが入る変数
    GameObject Mesh_Cre_Ori;
    mesh_test mesh_script; //メッシュ作成コードが入る変数
    Original_Mesh mesh_script_ori;

    //グローバルでSpriteRendererの変数を定義
    // Use this for initialization
    void Start()
    {
        spRen = GetComponent<SpriteRenderer>();
        math1 = new Math();
        //Mesh_Create = new mesh_test();
        //メッシュ作成オブジェクトを探して変数に入れる
        Mesh_Cre = GameObject.Find("Mesh_Test");
        Mesh_Cre_Ori = GameObject.Find("Mesh_Original");
        //Mesh_Testの中にあるスクリプトを取得して変数に格納
        mesh_script = Mesh_Cre.GetComponent<mesh_test>();
        mesh_script_ori = Mesh_Cre_Ori.GetComponent<Original_Mesh>();

        //ゲームが始まったらspRenに画像をコンポーネント
        //Get();
        StartCoroutine(coroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //Get();
    }

    void Get()
    {
    }

    /// <summary>
    /// 画像から明度を受け取る
    /// </summary>
    private IEnumerator coroutine()
    {
        int count = 0;
        float hue, S, B;
        double Ene = 0f; //エネルギー
        double E_Sim = 0f;  //焼きなましで使うエネルギー
        double E_Nei = 0f;  //隣のエネルギー
        double E_total = 0.0;
        double Sun = 90.0f;  //光源の向き
        var tex = spRen.sprite.texture; //変数texに画像のtextureデータを読み込み
        var list_da = new List<double>();   //どのように遷移しているかチェック
        //縦幅と横幅の設定
        z_size = tex.height;
        x_size = tex.width;
        Color[] pixels = tex.GetPixels();   //画像からピクセル情報取得
        double[,] B_array = new double[tex.height,tex.width]; //影の明るさの情報
        double[,] B_array_rand = new double[tex.height, tex.width];   //ランダムな影の明るさ
        double[,] angle_fai = new double[tex.height, tex.width];  //法線ベクトルの情報
        double[,] angle_fai_rand = new double[tex.height, tex.width]; //ランダムな法線ベクトル
        double[,] angle_fai_rand_re = new double[tex.height, tex.width];  //最急降下法をしたあとの置き換える用
        double[,] angle_a_rand = new double[tex.height, tex.width];   //ランダムな明るさの角度α
        //Color[] pixels_hsv = Color.RGBToHSV(pixels[], out hue, out S, out B); //置き換え用
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                var color = pixels[(tex.width * y) + x];
                //change_pixels[(tex.width * y) + x] = new Color(color.g, color.b, color.r);  //色変え
                //Debug.Log(color.g);
                //string Color_Code = color.ToString();
                Color.RGBToHSV(color, out hue, out S, out B);   //Bは0~1.0の間
                B_array[y,x] = B;
                angle_fai[y, x] = Sun - Mathf.Rad2Deg * Mathf.Acos(B);    //角度φ
                //B_array_rand[y, x] = Random.Range(0.0f, 1.0f);  //ランダムな明るさを代入
                //angle_fai_rand[y, x] = Sun - Mathf.Rad2Deg * Mathf.Acos(B_array_rand[y, x]);    //ランダムな法線ベクトル
                angle_a_rand[y, x] = Random.Range(0f,360f);   //ランダムな角度φを生成
                angle_fai_rand[y, x] = Sun - angle_a_rand[y, x];    //ランダムな法線ベクトルφを生成
                B_array_rand[y, x] = Mathf.Cos((float)(Mathf.Deg2Rad * angle_a_rand[y, x])); //ランダムな明るさを生成
            }
        }

        //最急降下法を用いた推定方法
        //while (true)
        //{
        //    //最急降下法によりランダムな角度φたちをそろえる
        //    //angle_fai_rand_re = math1.Gradient_Descent(B_array, angle_fai_rand, Sun);
        //    angle_fai_rand_re = math1.Gradient_Descent_Smooth(B_array, angle_fai_rand, Sun);
        //    //角度φの更新
        //    angle_fai_rand = angle_fai_rand_re;
        //    //更新したランダムな角度φを用いて明るさを表現
        //    B_array_rand = math1.Fai_to_B(angle_fai_rand, Sun);
        //    //エネルギーの計算
        //    //Ene = math1.Energy(B_array, B_array_rand);
        //    Ene = math1.Energy_Smooth(B_array, B_array_rand, angle_fai_rand);
        //    //if (Ene < 10) break;
        //    //Nn = math1.test(Nn);
        //    //if (Ene <= 5)
        //    //{
        //    //    Debug.Log(count);
        //    //    Debug.Log(Ene);
        //    //    break;
        //    //}
        //    if (count >= 5000)
        //    {
        //        Debug.Log(Ene);
        //        break;
        //    }
        //    if (count % 1000 == 0)
        //    {
        //        Debug.Log(Ene);
        //    }
        //    count++;
        //}
        //焼きなまし法を用いた推定方法
        while(T>0.1)
        {
            double angle_tmp = 0.0;
            double delta = 0.0;
            E_total = 0.0;   //エネルギーの合計
            //横に移動
            for (int z = 0; z < tex.height; z++) 
            {
                for (int x = 0; x < tex.width; x++)
                {
                    if (list_da.Count < 200)
                    {
                        E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], Sun);
                        angle_tmp = math1.Neighbor(angle_fai_rand[z, x]);
                        E_Nei = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, Sun);
                        delta = (E_Nei - E_Sim);
                    }
                    else
                    {
                        //スムージングをしたくないため
                        if (B_array[z, x] == 0)
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Nei = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, Sun);
                            delta = (E_Nei - E_Sim);
                        }
                        //xが最大値のときは一つ前と見比べる
                        else if (x == tex.width - 1)
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z, x - 1], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z, x - 1], Sun);
                            delta = E_Nei - E_Sim;
                        }
                        //一つ先が明度0の場合は一つ前と見比べる
                        else if (B_array[z, x + 1] == 0)
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z, x - 1], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z, x - 1], Sun);
                            delta = E_Nei - E_Sim;
                        }
                        //それ以外
                        else
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z, x + 1], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z, x + 1], Sun);
                            delta = E_Nei - E_Sim;
                        }
                    }

                    //遷移するかしないかの判断
                    if (delta < 0)
                    angle_fai_rand[z, x] = angle_tmp;
                    else
                    {
                        //確率計算
                        bool flag = math1.CheckRate(-1 * delta / T);
                        if (flag == true)
                        {
                            angle_fai_rand[z, x] = angle_tmp;
                        }
                        if (x == 20 && z == 20 && list_da.Count < 400)
                        {
                            //Debug.Log(-100 * delta / T);
                        }
                    }

                    if (x == 20 && z == 20 && list_da.Count < 300) 
                    {
                        //Debug.Log(Mathf.Cos((float)(angle_fai_rand[z,x]-Sun)) + ":" + B_array[z,x]);    //明るさの比較
                        //Debug.Log(angle_fai_rand[z, x] + ":" + angle_tmp + ":" + angle_fai[z, x] + ":delta=" + delta + ":実際の明るさ=" + B_array[z, x] + ":");    //比較
                        list_da.Add(angle_fai_rand[z,x]);
                    }
                }
            }
            //縦に移動
            for (int x = 0; x < tex.width; x++)
            {
                for (int z = 0; z < tex.height; z++)
                {
                    //スムージングをしたくないため
                    //if (B_array[z, x] == 0)
                    if (count_t < 200)
                    {
                        E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], Sun);
                        angle_tmp = math1.Neighbor(angle_fai_rand[z, x]);
                        E_Nei = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, Sun);
                        delta = E_Nei - E_Sim;
                    }
                    else
                    {
                        //zが最大値のときは一つ前と見比べる
                        if (z == tex.height - 1)
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z - 1, x], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z - 1, x], Sun);
                            delta = E_Nei - E_Sim;
                        }
                        //zが0のときは一つ先と見比べる
                        else if (z == 0)
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z + 1, x], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z + 1, x], Sun);
                            delta = E_Nei - E_Sim;
                        }
                        //一つ先が明度0の場合は一つ前と見比べる
                        else if (B_array[z + 1, x] == 0) 
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z - 1, x], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z - 1, x], Sun);
                            delta = E_Nei - E_Sim;
                        }
                        //それ以外
                        else
                        {
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_fai_rand[z, x], angle_fai_rand[z + 1, x], Sun);
                            angle_tmp = math1.Neighbor_near(angle_fai_rand[z, x]);
                            E_Sim = math1.Energy_Sim_Anneal(B_array[z, x], angle_tmp, angle_fai_rand[z + 1, x], Sun);
                            delta = E_Nei - E_Sim;
                        }
                    }


                    //遷移するかしないかの判断
                    if (delta < 0)
                    {
                        angle_fai_rand[z, x] = angle_tmp;
                    }
                    else
                    {
                        //確率計算
                        bool flag = math1.CheckRate(-1 * delta / T);
                        if (flag == true)
                        {
                            angle_fai_rand[z, x] = angle_tmp;
                        }
                    }

                    //切り替え
                    if (x == 20 && z == 20)
                        count_t++;
                    //        //E_total += E_Sim;
                }
            }
                    //Tの更新
                    T = math1.next_T(T);
        }
        //Debug.Log(count_t);
        //Debug.Log("count_sa" + count_sa);
        //Brightness_to_csv(B_array, tex.height, tex.width);
        //angle_fai_to_csv(angle_fai, tex.height, tex.width);
        //B_rand_to_csv(B_array_rand, tex.height, tex.width);
        angle_fai_rand_to_csv(angle_fai_rand, tex.height, tex.width);
        //test(angle_fai_rand, tex.height, tex.width);

        //更新したランダムな角度φを用いて明るさを表現
        B_array_rand = math1.Fai_to_B(angle_fai_rand, Sun);
        //エネルギー関数の計算
        Ene = math1.Energy(B_array, B_array_rand);
        Debug.Log("E=" + Ene);

        //メッシュ生成を実行
        //mesh_script.Mesh_from_Fai(angle_fai_rand,B_array,x_size,z_size);  //推定(法線ベクトルから)
        mesh_script.Mesh_Make(B_array_rand, x_size, z_size, angle_fai[0, 0]); //推定
        mesh_script_ori.Mesh_Make_Ori(B_array, x_size, z_size, angle_fai[0, 0]);    //元の画像

        //状態遷移
        //for (int i = 0; i < list_da.Count; i++)
        //{
        //    Debug.Log(list_da[i]);
        //}

        yield return null;
    }

    /// <summary>
    /// csvファイルで出力(明度)
    /// </summary>
    /// <param name="B"></param>
    private void Brightness_to_csv(double[,] B_array, int height, int width)
    {
        try
        {
            var append = false;
            //出力用のファイルを開く
            using (var sw = new System.IO.StreamWriter(@"brightness.csv", append))
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sw.Write(B_array[y,x] + ",");
                    }
                    sw.WriteLine("");
                }
            Debug.Log("complete_B");
        }
        catch(System.Exception e)
        {
            //ファイルを開くのに失敗したときのエラーメッセージを表示
            Debug.Log("error");
        }
    }

    /// <summary>
    /// csvファイルで出力(法線ベクトル)
    /// </summary>
    /// <param name="B"></param>
    private void angle_fai_to_csv(double[,] angle, int height, int width)
    {
        try
        {
            var append = false;
            //出力用のファイルを開く
            using (var sw = new System.IO.StreamWriter(@"angle_fai.csv", append))
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sw.Write(angle[y,x] + ",");
                    }
                    sw.WriteLine("");
                }
            Debug.Log("complete_angle_fai");
        }
        catch (System.Exception e)
        {
            //ファイルを開くのに失敗したときのエラーメッセージを表示
            Debug.Log("error");
        }
    }

    /// <summary>
    /// csvファイルで出力(明度)ランダムで生成したバージョン
    /// </summary>
    /// <param name="B_array_rand">ランダムな値で生成した明るさ</param>
    private void B_rand_to_csv(double[,] B_array_rand, int height, int width)
    {
        //int y = 50;
        try
        {
            var append = false;
            //出力用のファイルを開く
            using (var sw = new System.IO.StreamWriter(@"brightness_rand.csv", append))
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sw.Write(B_array_rand[y, x] + ",");
                    }
                    sw.WriteLine("");
                }
            Debug.Log("complete_B_rand");
        }
        catch (System.Exception e)
        {
            //ファイルを開くのに失敗したときのエラーメッセージを表示
            Debug.Log("error");
        }
    }
    /// <summary>
    /// csvファイルで出力(法線ベクトル)
    /// </summary>
    /// <param name="B"></param>
    private void angle_fai_rand_to_csv(double[,] angle, int height, int width)
    {
        try
        {
            var append = false;
            //出力用のファイルを開く
            using (var sw = new System.IO.StreamWriter(@"angle_fai_rand.csv", append))
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sw.Write(angle[y, x] + ",");
                    }
                    sw.WriteLine("");
                }
            Debug.Log("complete_angle_fai_rand");
        }
        catch (System.Exception e)
        {
            //ファイルを開くのに失敗したときのエラーメッセージを表示
            Debug.Log("error");
        }
    }

    /// <summary>
    /// テスト用
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void posiotion_coord()
    {

    }
}


