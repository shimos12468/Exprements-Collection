using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquares : MonoBehaviour
{
    public float res = 0.2f;
    public int width = 100, height = 100, depth = 100;
    public float noiseResolution = 1;
    public float threshold = 0.5f;
    float[,,] points;


    List<Vector3>vertices = new List<Vector3>();
    List<int>triangles= new List<int>();
    public MeshFilter meshFilter;


    // Start is called before the first frame update
    void Start()
    {

        meshFilter = GetComponent<MeshFilter>();    
        StartCoroutine(UpdateAll());
    }

    IEnumerator UpdateAll()
    {
        while (true)
        {
            SetPointHeight();
            MrachCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    private void SetMesh()
    {
        Mesh mesh= new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();   
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void MrachCubes()
    {
        print("hello");
        vertices.Clear();
        triangles.Clear();  
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height ; y++)
            {
                for (int z = 0; z < depth ; z++)
                {
                    float [] cubeCorners = new float[8];

                    for(int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = points[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigurationIndex(cubeCorners));
                }

            }

        }
    }

    private void MarchCube(Vector3 position, int confiurationIndex)
    {
        if (confiurationIndex == 1 || confiurationIndex == 255) return;

        int edgeIndex = 0;

        for(int t = 0; t < 5; t++)
        {
            for(int v = 0; v < 3; v++)
            {
                int triangleTableValue=MarchingTable.Triangles[confiurationIndex ,edgeIndex];
                
                if(triangleTableValue == -1)
                {
                    return;
                }

                 Vector3 edgeStart= (position*res)+MarchingTable.Edges[triangleTableValue, 0];
                 Vector3 edgeEnd = (position * res) + MarchingTable.Edges[triangleTableValue, 0];
                 Vector3 vertex = (edgeStart+edgeEnd) /2;
                 vertices.Add(vertex);
                 triangles.Add(vertices.Count-1);
                 edgeIndex++;
            }
        }

    }

    int GetConfigurationIndex(float[] cubeCorners)
    {
        int confiurationIndex = 0;
        for(int i = 0; i < cubeCorners.Length; i++)
        {
            if (cubeCorners[i] > threshold)
            {
                confiurationIndex |= 1<<i;
            }
        }
        return confiurationIndex;
    }
    void SetPointHeight()
    {

        points = new float[width + 1, height + 1, depth + 1];

        for (int x = 0; x < width+1; x++)
        {
            for (int y = 0; y < height+1; y++)
            {
                for (int z = 0; z < depth + 1; z++)
                {
                    float currentHeight = height*Mathf.PerlinNoise(x*noiseResolution, z* noiseResolution);

                    float newHeight;

                    if (y <= currentHeight - (res/2))
                    {
                        newHeight = 0f *res;
                    }

                    else if (y >= currentHeight + (res / 2))
                    {
                        newHeight = 1f*res;
                    }
                    else if (y > currentHeight)
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }
                    
                    points[x, y, z] = newHeight;

                }

            }

        }
    }
    // Update is called once per frame
    void Update()
    {

    }

 
}
