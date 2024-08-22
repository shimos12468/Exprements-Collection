using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;


namespace EXP.U4FOUNDERSPUZZLE
{
    public class Puzzle : MonoBehaviour
    {
        public SplineContainer container;

        public GameObject prefab;

        public List<GraphicalMovingPoint> splinePoints = new List<GraphicalMovingPoint>();
        private List<GraphicalMovingPoint> points;
        public List<GraphicalMovingPoint> SplinePoints
        {
            get
            {
                return splinePoints;
            }
            set
            {
                splinePoints = value;
            }
        }
    
        public List<float> targetDistance = new List<float>();
        public List<float> TargetDistance
        {
            get
            {
                return targetDistance;
            }
            set
            {
                targetDistance = value;
            }
        }
        

        public List<float> distanceplaces= new List<float>();
        private List<Vector2> calculatedCirclePoints = new List<Vector2>();


        public float speed = 1f;

        public int numPoints = 6;
        public float radius = 0.5f;
        public bool isCircle = false;
        private bool moving = false;

       
       
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < calculatedCirclePoints.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(calculatedCirclePoints[i].x, 0, calculatedCirclePoints[i].y) * transform.localScale.x, transform.localScale.x);
            }

        }
        
        public void SetupGame()
        {
            splinePoints.Clear();
            targetDistance.Clear();
            distanceplaces.Clear();
            points = null; 

            CalculateCirclePoints(numPoints, radius);
            InitializePoints();
            SetupPoints();
        }

        private void CalculateCirclePoints(int numPoints, float radius)
        {
            if (calculatedCirclePoints.Count == 0)
            {
                radius *= transform.localScale.x;
                float angleStep = isCircle ? 2 * Mathf.PI / (numPoints) : Mathf.PI / (numPoints);
   
                for (int i = 0; i < numPoints; i++)
                {
                    float angle = angleStep * i;
                    float x = radius * Mathf.Cos(angle);
                    float y = radius * Mathf.Sin(angle);
                    calculatedCirclePoints.Add(new Vector2(x, y));
                }
            }

        }

        private void InitializePoints()
        {

            if (splinePoints.Count == 0)
            {
                for (int i = 0; i < calculatedCirclePoints.Count; i++)
                {
                    GameObject g = Instantiate(prefab);
                    g.transform.position = new Vector3(calculatedCirclePoints[i].x, 0, calculatedCirclePoints[i].y) * transform.localScale.x;
                    g.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    g.transform.localScale = g.transform.localScale * transform.localScale.x;
                    g.GetComponentInChildren<TMP_Text>().SetText(i.ToString());
                    splinePoints.Add(new GraphicalMovingPoint());

                    //assign initial game object
                    splinePoints[i].obj = g;
                }

            }

        }

        private void SetupPoints()
        {
            int num1 = 0, num0 = 0;

            for (int i = 0; i < calculatedCirclePoints.Count; i++)
            {
                if (splinePoints[i].splineIndex == 0)
                {
                    splinePoints[i].startDistance = ((container.CalculateLength(splinePoints[i].splineIndex) / calculatedCirclePoints.Count) * num0) / container.CalculateLength(splinePoints[i].splineIndex);
                    num0++;
                }
                else
                {
                    splinePoints[i].startDistance = ((container.CalculateLength(splinePoints[i].splineIndex) / calculatedCirclePoints.Count) * num1) / container.CalculateLength(splinePoints[i].splineIndex);
                    num1++;
                }

                targetDistance.Add(splinePoints[i].startDistance);
                distanceplaces.Add(splinePoints[i].startDistance);
                splinePoints[i].currentDistance = splinePoints[i].startDistance;
                splinePoints[i].distanceIndex = i;
                Vector3 currentPosition = container.EvaluatePosition(splinePoints[i].splineIndex, splinePoints[i].currentDistance);
                splinePoints[i].obj.transform.position = currentPosition;
                splinePoints[i].obj.transform.localScale = Vector3.one * transform.localScale.x;
            }

            for (int i = 0; i < splinePoints.Count; i++)
            {
                splinePoints[i].splineLength = container.CalculateLength(splinePoints[i].splineIndex);
            }

        }

        public void CheckIfWillHaveFullTurn(List<GraphicalMovingPoint> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].currentDistance >= targetDistance[points[i].distanceIndex])
                {
                    points[i].turns += 1;
                }
                else
                {
                    points[i].turns += 0;

                }
                points[i].splineLength = container.CalculateLength(points[i].splineIndex);
            }
        }

        private void Update()
        {
            if (moving)
            {
                if (GetPointsList() == null)
                {
                    SetupPointsList();
                }
                Move();
            }
        }

        private void Move()
        {

            for (int i = 0; i < points.Count; i++)
            {
                bool arrivedToDistination = CheckArrivingToDesiredPoint(i);
                if (arrivedToDistination)
                {
                    i--;
                    continue;
                }

                if (!arrivedToDistination)
                {
                    Vector3 currentPosition = UpdatePosition(i);
                    UpdateLookDirection(i, currentPosition);
                    NormalizePoint(i);
                }
            }
            CloseTurn();
        }

        private bool CheckArrivingToDesiredPoint(int i)
        {

            
            if (points[i].switching)
            {
                if (Vector3.Distance(points[i].obj.transform.position, container.Splines[0].Knots.ToArray()[1].Position) <=0.4f)
                {
                    points[i].switching = false;
                    points[i].splineLength = container.CalculateLength(0);
                    points[i].splineIndex = 0;
                    points[i].currentDistance = 0.1f;


                    if (points[i].currentDistance >= targetDistance[points[i].distanceIndex])
                    {
                        points[i].turns = 1;
                    }
                    else
                    {
                        points[i].turns = 0;

                    }

                }
            }

            
            if (points[i].currentDistance >= targetDistance[points[i].distanceIndex] && points[i].turns<=0)
            {
                points.RemoveAt(i);
                return true;
            }
            return false;
        }

        private Vector3 UpdatePosition(int i)
        {
            points[i].currentDistance += speed * Time.deltaTime / points[i].splineLength;
            float frac = points[i].currentDistance - Mathf.Floor(points[i].currentDistance);

            Vector3 currentPosition = container.EvaluatePosition(points[i].splineIndex, frac);
            points[i].obj.transform.position = currentPosition;
            return currentPosition;
        }

        private void UpdateLookDirection(int i, Vector3 currentPosition)
        {
            Vector3 nextPosition = container.EvaluatePosition(points[i].splineIndex, points[i].currentDistance + 0.05f);
            Vector3 direction = nextPosition - currentPosition;
            points[i].obj.transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }

        private void NormalizePoint(int i)
        {

            if (points[i].turns>0)
            {
                if (points[i].currentDistance >= 1)
                {
                    points[i].currentDistance = points[i].currentDistance - 1;
                    points[i].turns--;
                }
            }

        }

        public void CloseTurn()
        {
            if (points.Count == 0)
            {

                for (int j = 0; j < splinePoints.Count; j++)
                {
                    if (splinePoints[j].currentDistance >= targetDistance[splinePoints[j].distanceIndex])
                    {
                        //splinePoints[j].normalized += 1;
                    }
                    else
                    {
                        //splinePoints[j].normalized += 0;

                    }
                   
                }
                points = null;
                moving = false;
            }
        }
        
        public List<GraphicalMovingPoint> GetPointsList()
        {
            return points;
        }
        
        public void SetupPointsList()
        {
            points = new List<GraphicalMovingPoint>(splinePoints);
        }

        public List<Vector2> GetCalculatedCirclePointsList()
        {
            return calculatedCirclePoints;
        }

        public bool GetMove()
        {
            return moving;
        }

        public void SetMove(bool value)
        {
            moving = value;
        }

    }

    [Serializable]
    public class GraphicalMovingPoint
    {
        public GameObject obj;
        
        public float splineLength = 0;
        
        public float currentDistance = 0;
        
        public float startDistance = 0;
        
        public int splineIndex = 0;
        
        public int distanceIndex =0;
        
        public int turns = 0;
        
        public bool switching = false;
    }

}


