using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlacer : MonoBehaviour
{
    public float spacing = 0.1f;
    public float resolution = 1f;
    void Start()
    {
       
        Vector2[] points= FindObjectOfType<PathCreator>().path.CalculateEvenlySpacedPoints(spacing, resolution);

        foreach (var point in points) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = point;
            go.transform.localScale = Vector3.one * spacing * 0.5f;

        }


    }

  
}
