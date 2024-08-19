using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class Puzzle : MonoBehaviour
{
    public float V = 0.002f;
    public SplineContainer container;
    public List<Vector2>pointsOfCircle = new List<Vector2>();
 
   
    
    
    public int numPoints = 6;
    public float radius = 0.5f;
    public bool isCircle = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        for (int i = 0; i < pointsOfCircle.Count; i++)
        {
            Gizmos.DrawSphere(new Vector3(pointsOfCircle[i].x,0, pointsOfCircle[i].y) * transform.localScale.x, transform.localScale.x);
        }
    }

    
    private void DisplayObjects()
    {
       
        if (PointsOnSpline.Count== 0)
        {
            for (int i = 0; i < pointsOfCircle.Count; i++)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = new Vector3(pointsOfCircle[i].x, 0, pointsOfCircle[i].y) * transform.localScale.x;
                g.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                g.transform.localScale = g.transform.localScale * transform.localScale.x;
                PointsOnSpline.Add(new Intstance());
                //assign initial game object
                PointsOnSpline[i].obj = g;
            }
        }
       
    }

    private void InitializeGameObjects()
    {

        float mm = Mathf.Infinity;

        float dist = 0;
        for (int i = 0; i < pointsOfCircle.Count; i++)
        {
            int ind = -1;
            for (int j = 0; j < container.Splines[0].Knots.ToArray().Length; j++)
            {
                float min = Vector3.Distance(PointsOnSpline[i].obj.transform.position, container.Splines[0].Knots.ToArray()[j].Position);
                if (min <= mm)
                {

                    mm = min;
                    ind = j;


                }


            }
            for (int k = 0; k < ind; k++)
            {
                dist += Vector3.Distance(container.Splines[0].Knots.ToArray()[k].Position, container.Splines[0].Knots.ToArray()[k + 1].Position);
            }

            // assign initial distance
            PointsOnSpline[i].initialDistance = (float)Math.Round(dist / container.CalculateLength(0), 3) + V * i;
            PointsOnSpline[i].distancePercentage = PointsOnSpline[i].initialDistance;

            PointsOnSpline[i].numOfTurns = 0;
            // assign the initial position
            Vector3 currentPosition = container.EvaluatePosition(PointsOnSpline[i].splineIndex, PointsOnSpline[i].distancePercentage);
            PointsOnSpline[i].obj.transform.position = currentPosition;

            mm = Mathf.Infinity;
            dist = 0;

        }
    }

    public void MakeCirclePoints(int numPoints ,float radius)
    {
        if (pointsOfCircle.Count == 0)
        {
            radius *= transform.localScale.x;
            float angleStep = isCircle ? 2 * Mathf.PI / (numPoints) : Mathf.PI / (numPoints);
            print(angleStep);
            for (int i = 0; i < numPoints; i++)
            {
                float angle = angleStep * i;
                float x = radius * Mathf.Cos(angle);
                float y = radius * Mathf.Sin(angle);

                x = (float)Math.Round(x, 2);
                y = (float)Math.Round(y, 2);
                pointsOfCircle.Add(new Vector2(x, y));
            }
        }
       
    }


    public List<Intstance> PointsOnSpline;
    public float speed = 1f;

    [Serializable]
    public class Intstance
    {
        public GameObject obj;
        public float splineLength = 0;
        public int splineIndex = 0;
        public float distancePercentage =0;
        public float initialDistance = 0;
        public int numOfTurns = 0;
        public bool closing = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        MakeCirclePoints(numPoints, radius);
        DisplayObjects();
        InitializeGameObjects();
        Initialize();
    }

    public bool oneTurn = false;
    // Update is called once per frame
    void Update()
    {

        if(oneTurn)
            MoveForATurn();



    }
    public int number = 0;
   

    private void MoveForATurn()
    {
        for (int i = 0; i < PointsOnSpline.Count; i++)
        {
            if (PointsOnSpline[i].distancePercentage >= PointsOnSpline[i].initialDistance && PointsOnSpline[i].closing == true)
            {
                number++;
                PointsOnSpline[i].closing = false;
                if (number == numPoints)
                {
                    
                    for(int j = 0; j < PointsOnSpline.Count; j++)
                    {
                        PointsOnSpline[j].numOfTurns++;
                        PointsOnSpline[j].distancePercentage = PointsOnSpline[j].initialDistance;
                    }
                    number = 0;
                    oneTurn = false;
                    break;
                }
                continue;
            }

            PointsOnSpline[i].distancePercentage += speed * Time.deltaTime / PointsOnSpline[i].splineLength;


            Vector3 currentPosition = container.EvaluatePosition(PointsOnSpline[i].splineIndex, PointsOnSpline[i].distancePercentage);
            PointsOnSpline[i].obj.transform.position = currentPosition;


            Vector3 nextPosition = container.EvaluatePosition(PointsOnSpline[i].splineIndex, PointsOnSpline[i].distancePercentage + 0.05f);
            Vector3 direction = nextPosition - currentPosition;
            PointsOnSpline[i].obj.transform.rotation = Quaternion.LookRotation(direction, transform.up);

            if (PointsOnSpline[i].distancePercentage >= 1f)
            {
                PointsOnSpline[i].distancePercentage = PointsOnSpline[i].distancePercentage-1;
                PointsOnSpline[i].closing = true;
            }
        }
    }

    void Initialize()
    {
          
       
        for (int i = 0; i < PointsOnSpline.Count; i++)
        {
            PointsOnSpline[i].splineIndex=0;
        }

        for (int i = 0; i < PointsOnSpline.Count; i++)
        {
            PointsOnSpline[i].splineLength = container.CalculateLength(PointsOnSpline[i].splineIndex);
        }
    }
}
