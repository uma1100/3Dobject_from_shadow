using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshFilter))]

public class mesh_test : MonoBehaviour
{
    [Range(1, 255)]
    //public int size;
    public float vertexDistance = 1f;
    public float vias; //y座標のバイアス
    public Material Mat;

    private void Start()
    {
        //Mesh_Make();
    }
    
    /// <summary>
    /// メッシュの明るさからの表現(推定)
    /// </summary>
    /// <param name="B">明度(random)</param>
    /// <param name="x_size">x座標</param>
    /// <param name="z_size">y座標</param>
    /// <param name="fai">下向きか上向きかの判断[0,0]座標の法線ベクトル</param>
    public void Mesh_Make(double[,] B, int x_size, int z_size, double fai)
    {
        Vector3[] vertices = new Vector3[x_size * z_size];
        ///---下向き---///
        if (fai >= 0 && fai < 180)
        {
            for (int z = 0; z < z_size; z++)
            {
                for (int x = 0; x < x_size; x++)
                {
                    //float y = Random.value * 100f;
                    double y = 0;
                    //if (x == 0)
                    //{
                    //    y = -1 * B[x, z] * 5000f;
                    //}
                    //else if (Mathf.Sqrt((float)(B[x, z] - B[x - 1, z])) * Mathf.Sqrt((float)(B[x, z] - B[x - 1, z])) >= 0.01)
                    //    y = -1 * B[x, z] * 1000f;
                    //else
                    //    y = -1 * B[x, z] * 5000f;
                    y = -1 * B[x, z] * 5000f;
                    //Debug.Log(y);
                    vertices[z * z_size + x] = new Vector3(x * vertexDistance, (float)y, -z * vertexDistance);
                }
            }
        }
        ///---上向き---///
        else
        {
            for (int z = 0; z < z_size; z++)
            {
                for (int x = 0; x < x_size; x++)
                {
                    //float y = Random.value * 100f;
                    double y = 0;
                    //if (x == 0)
                    //{
                    //    y = B[x, z] * 5000f;
                    //}
                    //else if (Mathf.Sqrt((float)(B[x, z] - B[x - 1, z])) * Mathf.Sqrt((float)(B[x, z] - B[x - 1, z])) >= 0.01)
                    //    y = B[x, z] * 1000f;
                    //else
                    //    y = B[x, z] * 5000f;
                    y = -1 * B[x, z] * 5000f;
                    //Debug.Log(y);
                    vertices[z * z_size + x] = new Vector3(x * vertexDistance, (float)y, -z * vertexDistance);
                }
            }
        }
        //Debug.Log("fai:" + fai);

        int triangleIndex = 0;
        int[] triangles = new int[(x_size - 1) * (z_size - 1) * 6];
        for (int z = 0; z < z_size - 1; z++)
        {
            for (int x = 0; x < x_size - 1; x++)
            {
                int a = z * z_size + x;
                int b = a + 1;
                int c = a + x_size;
                int d = c + 1;

                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;

                triangles[triangleIndex + 3] = c;
                triangles[triangleIndex + 4] = b;
                triangles[triangleIndex + 5] = d;

                triangleIndex += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = Mat;

        Debug.Log("ok");
        
    }
    /// <summary>
    /// 法線ベクトルから物体表現
    /// </summary>
    /// <param name="fai">推定した法線ベクトル</param>
    /// <param name="B">実際の明るさ</param>
    /// <param name="x_size"></param>
    /// <param name="z_size"></param>
    public void Mesh_from_Fai(double[,] fai, double[,] B, int x_size, int z_size)
    {
        Vector3[] vertices = new Vector3[x_size * z_size];
        double[,] y = new double [x_size,z_size];
        for (int z = 0; z < z_size; z++)
        {
            for (int x = 0; x < x_size; x++)
            {
                /// 現在の場所が明度0の場合y=0
                if (B[z, x] == 0)
                    y[z, x] = 0;
                else if (x == 0)
                    y[z, x] = vias * Mathf.Tan((float)(90.0 - fai[x, z]));
                /// 傾きが一つ前より今のほうが大きい場合(右下がり)
                else if (90.0 - fai[x - 1, z] <= 90.0 - fai[x, z])
                {
                    y[x, z] = y[x - 1, z] + vias * Mathf.Tan((float)(90.0 - fai[x, z]));
                    //y[x, z] = vias * (90.0 - fai[x, z]);
                }
                /// 傾きが一つ前より今のほうが小さい場合(右上がり)
                else if (90.0 - fai[x - 1, z] >= 90.0 - fai[x, z])
                {
                    y[x, z] = y[x - 1, z] + vias * Mathf.Tan((float)(90.0 - fai[x, z]));
                    //y[x, z] = vias * (90.0 - fai[x, z]);
                }
                /// 傾きが前と一緒の場合(平坦な時)
                else
                {
                    //y[x, z] = y[x - 1, z];
                    y[x, z] = vias * Mathf.Tan((float)(90.0 - fai[x, z]));
                }
                    //y[x, z] = y[x - 1, z] + vias * Mathf.Tan((float)(90.0 - fai[x, z]));
                    //Debug.Log(y);
                    vertices[z * z_size + x] = new Vector3(x * vertexDistance, (float)y[x,z], -z * vertexDistance);
            }
        }

        int triangleIndex = 0;
        int[] triangles = new int[(x_size - 1) * (z_size - 1) * 6];
        for (int z = 0; z < z_size - 1; z++)
        {
            for (int x = 0; x < x_size - 1; x++)
            {
                int a = z * z_size + x;
                int b = a + 1;
                int c = a + x_size;
                int d = c + 1;

                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;

                triangles[triangleIndex + 3] = c;
                triangles[triangleIndex + 4] = b;
                triangles[triangleIndex + 5] = d;

                triangleIndex += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = Mat;
        y_test(y, x_size, z_size);

        Debug.Log("ok_mesh_from_fai");
    }

    public void test()
    {
        Debug.Log("ok");
    }

    private void y_test(double[,] y_axis, int height, int width)
    {
        try
        {
            var append = false;
            //出力用のファイルを開く
            using (var sw = new System.IO.StreamWriter(@"y_axis.csv", append))
                for (int z = 0; z < height; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sw.Write(y_axis[x,z] + ",");
                    }
                    sw.WriteLine("");
                }
            Debug.Log("complete_y");
        }
        catch (System.Exception e)
        {
            //ファイルを開くのに失敗したときのエラーメッセージを表示
            Debug.Log("error");
        }
    }
}
