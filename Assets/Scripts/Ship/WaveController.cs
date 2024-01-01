using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public float dalgaGenisligi = 1.0f;
    public float dalgaYuksekligi = 0.1f;
    public float dalgaHizi = 1.0f;

    private void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x / dalgaGenisligi;
            float z = vertices[i].z / dalgaGenisligi;

            float y = Mathf.Sin((x + z + Time.time) * dalgaHizi) * dalgaYuksekligi;

            vertices[i].y = y;
        }

        mesh.vertices = vertices;
    }
}
