using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Original_Mesh : MonoBehaviour {

    [Range(1, 255)]
    //public int size;
    public float vertexDistance = 1f;
    public float vias = 10000f; //y座標のバイアス
    public Material Mat;

    /// <summary>
    /// メッシュの明るさからの表現(画像)
    /// </summary>
    /// <param name="B">明度(random)</param>
    /// <param name="x_size">x座標</param>
    /// <param name="z_size">y座標</param>
    /// <param name="fai">下向きか上向きかの判断[0,0]座標の法線ベクトル</param>
    public void Mesh_Make_Ori(double[,] B, int x_size, int z_size, double fai)
    {
        Vector3[] vertices = new Vector3[x_size * z_size];
        ///---下向き---///
        if (fai >= 90 && fai < 270)
        {
            for (int z = 0; z < z_size; z++)
            {
                for (int x = 0; x < x_size; x++)
                {
                    //float y = Random.value * 100f;
                    double y = -1 * B[x, z] * 5000f;
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
                    double y = B[x, z] * 5000f;
                    //Debug.Log(y);
                    vertices[z * z_size + x] = new Vector3(x * vertexDistance, (float)y, -z * vertexDistance);
                }
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

        Debug.Log("ok");

    }
}
