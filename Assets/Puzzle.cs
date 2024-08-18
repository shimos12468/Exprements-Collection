using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class Puzzle : MonoBehaviour
{

    public SplineContainer container;
    public List<Vector2>pointsOfCircle = new List<Vector2>();
    public int numPoints = 6;
    public float radius = 0.5f;
    public bool isCircle = false;
    List<BezierKnot> knots = new List<BezierKnot>();

    private void OnDrawGizmos()
    {

        
        
    }

    private void OnValidate()
    {
        DisplayObjects();

    }

    private void DisplayObjects()
    {
        knots = container.Splines[0].Knots.ToList();
        for (int i = 0; i < knots.Count; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Quad);
            g.transform.position = knots[i].Position*transform.localScale.x;
            g.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
            g.transform.localScale = Vector3.one*5;
        }
    }

    private void OnEnable()
    {
        
    }

    public void MakeCirclePoints(int numPoints ,float radius)
    {
        radius *= transform.localScale.x;
        float angleStep = isCircle ? 2 * Mathf.PI / (numPoints - 1) : Mathf.PI / (numPoints - 1);
        print(angleStep);
        for (int i = 0; i < numPoints; i++)
        {
            float angle = angleStep * i;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            x = (float)Math.Round(x, 2);
            y = (float)Math.Round(y, 2);
            Debug.Log($"X{i} = {x}");
            Debug.Log($"Y{i} = {y}");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
