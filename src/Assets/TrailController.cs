using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrailController : MonoBehaviour
{
    private const int FRAME_MAX = 10;
    private List<Vector3> Points0 = new List<Vector3>();// 根本
    private List<Vector3> Points1 = new List<Vector3>();// 先端

    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (FRAME_MAX <= Points0.Count)
        {
            Points0.RemoveAt(0);
            Points1.RemoveAt(0);
        }

        Points0.Add(transform.position);
        Points1.Add(transform.TransformPoint(new Vector3(0.0f, 1.0f, 0.0f)));

        if (2 <= Points0.Count)// エッジ一つでは帯は作れない
        {
            CreateMeshSubDevided(4);
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t * t2;
        float c0 = -0.5f * t3 + t2 - 0.5f * t;
        float c1 = 1.5f * t3 -2.5f * t2 + 1.0f;
        float c2 = -1.5f * t3 + 2.0f * t2 + 0.5f * t;
        float c3 = 0.5f * t3 - 0.5f * t2;

        return p0 * c0 + p1 * c1 + p2 * c2 + p3 * c3;
    }

    private void CreateMesh()
    {
        mesh.Clear();　//　メッシュのクリア

        // メモリ確保
        int n = Points0.Count;
        Vector3[] vertexArray = new Vector3[2 * n];
        Vector2[] uvArray = new Vector2[2 * n];
        int[] indexArray = new int[6 * (n - 1)];

        int idV = 0, idI = 0;
        float dUv = 1.0f / ((float)n - 1.0f);
        for (int i = 0; i < n; i++)
        {
            // 頂点座標
            vertexArray[idV + 0] = transform.InverseTransformPoint(Points0[i]);
            vertexArray[idV + 1] = transform.InverseTransformPoint(Points1[i]);
            // UV座標
            uvArray[idV + 0].x =
            uvArray[idV + 1].x = 1.0f - dUv * (float)i;
            uvArray[idV + 0].y = 0.0f;
            uvArray[idV + 1].y = 1.0f;

            // インデックス
            if (i == n - 1) break;// 最後の辺では帯を作らない
            indexArray[idI + 0] = idV;
            indexArray[idI + 1] = idV + 1;
            indexArray[idI + 2] = idV + 2;
            indexArray[idI + 3] = idV + 2;
            indexArray[idI + 4] = idV + 1;
            indexArray[idI + 5] = idV + 3;

            idV += 2;
            idI += 6;
        }

        mesh.vertices = vertexArray;
        mesh.uv = uvArray;
        mesh.triangles = indexArray;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void CreateMeshSubDevided(int s/* 分割数 */)
    {
        mesh.Clear();　//　メッシュのクリア

        // メモリ確保
        int n = Points0.Count;
        Vector3[] vertexArray = new Vector3[2 * s * (n - 1) + 2];
        Vector2[] uvArray     = new Vector2[2 * s * (n - 1) + 2];
        int[] indexArray = new int[6 * s * (n - 1)];

        int idV = 0, idI = 0;
        float dUv = 1.0f / ((float)s * (float)(n - 1.0f));
        int id0 = 0, id1 = 0, id2 = 1, id3 = 2;
        if (n - 1 < id3) id3 = n - 1;

        for (int i = 0; i < n - 1; i++)// N.B. 最後の一個手前まで
        {
            for (int j = 0; j < s; j++)
            {
                // 頂点座標
                float t = (float)j / (float)s;
                Vector3 p0 = CatmullRom(Points0[id0], Points0[id1], Points0[id2], Points0[id3], t);
                Vector3 p1 = CatmullRom(Points1[id0], Points1[id1], Points1[id2], Points1[id3], t);
                vertexArray[idV + 0] = transform.InverseTransformPoint(p0);
                vertexArray[idV + 1] = transform.InverseTransformPoint(p1);

                // UV座標
                uvArray[idV + 0].x =
                uvArray[idV + 1].x = 1.0f - dUv * (float)(i*s+j);
                uvArray[idV + 0].y = 0.0f;
                uvArray[idV + 1].y = 1.0f;

                // インデックス
                indexArray[idI + 0] = idV;
                indexArray[idI + 1] = idV + 1;
                indexArray[idI + 2] = idV + 2;
                indexArray[idI + 3] = idV + 2;
                indexArray[idI + 4] = idV + 1;
                indexArray[idI + 5] = idV + 3;

                idI += 6;
                idV += 2;
            }
            if(i != 0) id0++;
            if(n - 1 < ++id1) id1 = n - 1;
            if(n - 1 < ++id2) id2 = n - 1;
            if(n - 1 < ++id3) id3 = n - 1;
        }

        // 最後の辺
        vertexArray[idV + 0] = transform.InverseTransformPoint(Points0[n - 1]);
        vertexArray[idV + 1] = transform.InverseTransformPoint(Points1[n - 1]);
        uvArray[idV + 0].x = uvArray[idV + 1].x = 0.0f;
        uvArray[idV + 0].y = 0.0f;
        uvArray[idV + 1].y = 1.0f;

        mesh.vertices = vertexArray;
        mesh.uv = uvArray;
        mesh.triangles = indexArray;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
