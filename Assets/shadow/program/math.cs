using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Math : MonoBehaviour{

    private double w_g = 3f; //最急降下法の重み
    private double w_s = 0.1f; //スムージング定数
    private double countt = 0; //何回焼きなまし法で更新されたか
    private double cold = 0.999;
    /// <summary>
    /// 最急降下法
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="angle_rand"></param>
    public double[,] Gradient_Descent(double[,] B, double[,] angle_rand, double SUN)
    {
        //S = new texture_change();
        double dif = 0;  //微分
        double update = 0;   //更新
        //int update = 0;
        ///---x,y平面上---///
        for (int z = 0; z < B.GetLength(0); z++)
        {
            for (int x = 0; x < B.GetLength(1); x++)
            {
                dif = (w_g * (2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                update = (angle_rand[z, x] - dif) % 360;
                angle_rand[z, x] = update; //更新
            }
        }
        ///---y,z平面上---///
        for (int x = 0; x < B.GetLength(1); x++)
        {
            for (int z = 0; z < B.GetLength(0); z++)
            {
                dif = (w_g * (2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                //update = angle_rand[z, x] - dif;
                //angle_rand[z, x] = update; //更新
                update = (angle_rand[z, x] - dif) % 360;
                angle_rand[z, x] = update; //更新
            }
        }
        return angle_rand;
    }

    /// <summary>
    /// エネルギー関数の計算
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="B_rand">ランダムな明るさ</param>
    public double Energy(double[,] B, double[,] B_rand)
    {
        double E = 0f;
        //S = new texture_change();
        for (int y = 0; y < B.GetLength(0); y++)
        {
            for (int x = 0; x < B.GetLength(1); x++)
            {
                E += (B[y, x] - B_rand[y, x]) * (B[y, x] - B_rand[y, x]);
            }
        }
        return E;
    }

    /// <summary>
    /// 法線ベクトルから明るさを表現
    /// </summary>
    /// <param name="angle_fai">角度φ</param>
    /// <returns></returns>
    public double[,] Fai_to_B(double[,] angle_fai, double SUN)
    {
        //S = new texture_change();
        double fai_temp = 0.0f;  //φを置き換える
        double[,] B = new double[angle_fai.GetLength(0), angle_fai.GetLength(1)];
        for (int y = 0; y < angle_fai.GetLength(0); y++)
        {
            for (int x = 0; x < angle_fai.GetLength(1); x++)
            {
                fai_temp = SUN - angle_fai[y, x];
                B[y, x] = Mathf.Cos((float)(Mathf.Deg2Rad * fai_temp));
            }
        }
        return B;
    }

    /// <summary>
    /// 最急降下法（スムージングあり）
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="angle_rand">ランダムな法線ベクトル</param>
    /// <param name="SUN">光源の向き</param>
    /// <returns></returns>
    public double[,] Gradient_Descent_Smooth(double[,] B, double[,] angle_rand, double SUN)
    {
        //S = new texture_change();
        double dif = 0;  //微分
        double update = 0;   //更新
        ///---x,y平面上---///
        for (int z = 0; z < B.GetLength(0); z++)
        {
            for (int x = 0; x < B.GetLength(1); x++)
            {
                /// まず現在地の明度が0の場合スムージングなしで計算
                if (B[z, x] == 0)
                {
                    dif = w_g * (2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN)))));
                    update = (angle_rand[z, x] - dif) % 360;
                }
                ///xがゼロのときは一つ先とスムージング
                else if (x == 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x + 1]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///xがマックスの時は一つ前とスムージング
                else if (x == (B.GetLength(1) - 1))
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x - 1]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///一つ前が明度0で一つ先が明度が0より大きいとき一つ先とスムージング
                else if (B[z, x - 1] == 0 && B[z, x + 1] >= 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x + 1]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///一つ前の明度が0より大きく一つ先が明度0の場合は一つ前とスムージング
                else if (B[z, x - 1] >= 0 && B[z, x + 1] == 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x - 1]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///それ以外
                else
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (2 * angle_rand[z, x] - angle_rand[z, x - 1] - angle_rand[z, x + 1]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                    //焼きなまし法をしてまた更新
                    //if (swing > 0.1)
                    //{
                    //    update_sa = Simulated_Annealing(B[z, x], angle_rand[z, x], angle_rand[z, x + 1]);
                    //    angle_rand[z, x] = update_sa;
                    //}
                }
                //if (x == 0)
                //    //dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x + 1]));
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                ///// 明るさが0から変わった場合スムージング項をなくす
                //else if (B[z, x - 1] == 0 && B[z, x] >= 0)
                //{
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                //}
                //else if (B[z, x] >= 0 && x < (B.GetLength(1) - 1) && B[z, x + 1] == 0) 
                //{
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                //}
                //else
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z, x - 1]));
                //update = (angle_rand[z, x] - dif) % 360;
                //angle_rand[z, x] = update; //更新
            }
        }
        ///---y,z平面上---///
        for (int x = 0; x < B.GetLength(1); x++)
        {
            for (int z = 0; z < B.GetLength(0); z++)
            {
                /// まず現在地の明度が0の場合スムージングなしで計算
                if (B[z, x] == 0)
                {
                    dif = w_g * (2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN)))));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///zがゼロのときは一つ先とスムージング
                else if (z == 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z + 1, x]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///zがマックスの時は一つ前とスムージング
                else if (z == (B.GetLength(0) - 1))
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z - 1, x]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///一つ前が明度0で一つ先が明度が0より大きいとき一つ先とスムージング
                else if (B[z - 1, x] == 0 && B[z + 1, x] >= 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z + 1, x]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///一つ前の明度が0より大きく一つ先が明度0の場合は一つ前とスムージング
                else if (B[z - 1, x] >= 0 && B[z + 1, x] == 0)
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z - 1, x]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                ///それ以外
                else
                {
                    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (2 * angle_rand[z, x] - angle_rand[z - 1, x] - angle_rand[z + 1, x]));
                    update = (angle_rand[z, x] - dif) % 360;
                    angle_rand[z, x] = update; //更新
                }
                //if (z == 0)
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z + 1, x]));
                //else if (B[z - 1, x] == 0 && B[z, x] >= 0)
                //{
                //    dif = w_g * (2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN)))));
                //}
                //else if (B[z, x] >= 0 && z < (B.GetLength(0) - 1) && B[z + 1, x] == 0) 
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))));
                //else
                //    dif = w_g * ((2 * Mathf.Sin((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))) * (B[z, x] - Mathf.Cos((float)(Mathf.Deg2Rad * (angle_rand[z, x] - SUN))))) + 2 * w_s * (angle_rand[z, x] - angle_rand[z - 1, x]));
                //update = (angle_rand[z, x] - dif) % 360;
                //angle_rand[z, x] = update; //更新
            }
        }
        return angle_rand;
    }

    /// <summary>
    /// エネルギー関数
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="B_rand">ランダムな明るさ</param>
    /// <param name="angle_fai">ランダムな法線ベクトル</param>
    /// <returns></returns>
    public double Energy_Smooth(double[,] B, double[,] B_rand, double[,] angle_fai)
    {
        double E = 0f;
        //S = new texture_change();
        for (int y = 0; y < B.GetLength(0); y++)
        {
            for (int x = 0; x < B.GetLength(1); x++)
            {
                if (x == 0)
                    E += (B[y, x] - B_rand[y, x]) * (B[y, x] - B_rand[y, x]) + (w_s * (angle_fai[y, x] - angle_fai[y, x + 1]));
                else
                    E += (B[y, x] - B_rand[y, x]) * (B[y, x] - B_rand[y, x]) + (w_s * (angle_fai[y, x] - angle_fai[y, x - 1]));
            }
        }
        return E;
    }

    /// <summary>
    /// 焼きなまし法
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="angle_fai1">現在の位置</param>
    /// <param name="angle_fai2">次の位置</param>
    /// <returns></returns>
    //public double Simulated_Annealing(double B, double angle_fai1, double angle_fai2)
    //{
    //    //太陽の定義
    //    double SUN = 90.0;
    //    //double swing = 1;   //どれぐらい揺らすか
    //    ///今の位置のエネルギーを求める
    //    double E_i = (Mathf.Cos((float)(Mathf.Deg2Rad * (angle_fai1 - SUN))) - B) * (Mathf.Cos((float)(Mathf.Deg2Rad * (angle_fai1 - SUN))) - B) + w_s * (angle_fai2 - angle_fai1) * (angle_fai2 - angle_fai1);
    //    double fai_tmp = angle_fai1 + swing;
    //    //揺らした後のエネルギーを求める
    //    double E_i_s = (Mathf.Cos((float)(Mathf.Deg2Rad * (fai_tmp - SUN))) - B) * (Mathf.Cos((float)(Mathf.Deg2Rad * (fai_tmp - SUN))) - B) + w_s * (angle_fai2 - fai_tmp) * (angle_fai2 - fai_tmp);
    //    //Δエネルギーの計算
    //    double dE = E_i_s - E_i;
    //    //1以上だとfai_tmpを採用
    //    if (Mathf.Exp((float)(-1 * dE / T)) > 1)
    //        angle_fai1 = fai_tmp;
    //    else
    //    {
    //        //確率判定
    //        bool flag = CheckRate(Mathf.Exp((float)(-1 * dE / T)) * 100.0);
    //        //trueである場合
    //        if (flag == true)
    //        {
    //            angle_fai1 = fai_tmp;
    //            countt++;
    //            if (countt % 10 == 0)
    //                Debug.Log(countt);
    //        }
    //        //falseである場合は何もしないのでそのままangle_fai1を返す
    //    }
    //    //徐冷関数
    //    double swing_tmp = swing * cold;
    //    swing = swing_tmp;
    //    return angle_fai1;
    //}

    /// <summary>
    /// 渡された引数で確率判定
    /// </summary>
    /// <param name="rate">確率</param>
    /// <returns></returns>
    public bool CheckRate(double rate)
    {
        if (Random.Range(0.0f,1.0f) * 100.0f < rate)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 温度の更新
    /// </summary>
    /// <param name="T"></param>
    /// <returns></returns>
    public double next_T(double T)
    {
        return cold * T;
    }
    /// <summary>
    /// エネルギー関数の計算
    /// </summary>
    /// <param name="B">実際の明るさ</param>
    /// <param name="angle_fai1">現在の推定した法線ベクトル</param>
    /// <param name="angle_fai2">一つ先の法線ベクトル</param>
    /// <param name="SUN">光源ベクトル</param>
    /// <returns></returns>
    public double Energy_Sim_Anneal(double B, double angle_fai1, double angle_fai2,double SUN)
    {
        double E = (Mathf.Cos((float)(Mathf.Deg2Rad * (angle_fai1 - SUN))) - B) * (Mathf.Cos((float)(Mathf.Deg2Rad * (angle_fai1 - SUN))) - B) + w_s * (angle_fai2 - angle_fai1) * (angle_fai2 - angle_fai1);
        return E;
    }

    /// <summary>
    /// スムージングなしのエネルギー関数
    /// </summary>
    public double Energy_Sim_Anneal(double B,double angle_fai,double SUN)
    {
        double E = ((Mathf.Cos((float)(Mathf.Deg2Rad*(angle_fai - SUN))) - B) * (Mathf.Cos((float)(Mathf.Deg2Rad * (angle_fai - SUN))) - B));
        return E;
    }

    /// <summary>
    /// 近傍からランダムに選ぶ
    /// </summary>
    /// <param name="angle_fai">推定した法線ベクトル</param>
    /// <returns></returns>
    public double Neighbor(double angle_fai)
    {
        if (Random.Range(0.0f, 1.0f) > 0.5)
            return angle_fai + 1;
        return angle_fai - 1;
    }

    public double Neighbor_near(double angle_fai)
    {
        if (Random.Range(0.0f, 1.0f) > 0.5)
            return angle_fai + 0.1;
        return angle_fai - 0.1;
    }
    //public float test(float S)
    //{
    //    Const_Shadow Sun;
    //    Sun = new Const_Shadow();

    //    S = Sun.SUN;
    //    return S;
    //}
}
    
