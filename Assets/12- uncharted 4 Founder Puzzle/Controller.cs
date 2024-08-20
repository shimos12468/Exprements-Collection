using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace EXP.U4FOUNDERSPUZZLE
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] Puzzle puzzle;
        bool onetime = false;
        bool initialized = false;

        public int pointAIndex, pointBIndex;
       

        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CalculateTargetDistance();
                if (!initialized)
                {
                    initialized = true;
                    onetime = true;
                }
            }

            if (initialized && onetime)
            {
                puzzle.SetupGame();
                onetime = false;
                FirstTurn();
            }

            if (Input.GetMouseButtonDown(1))
            {
                puzzle.SetMove(true);
            }
        }

        private void FirstTurn()
        {


            int targetSplineIndex = 1;
            Vector3 initialPoint = puzzle.container.Splines[targetSplineIndex].Knots.ToArray()[0].Position;
            float splineLength = puzzle.container.CalculateLength(targetSplineIndex);
            Spline spline = puzzle.container.Splines[targetSplineIndex];
            for (int i = 1; i <= 3; i++)
            {
                int correctIndex = i + 1; 
                Vector3 currentPointPosition = puzzle.SplinePoints[correctIndex].obj.transform.position;

                float dist = GetDistance(currentPointPosition, initialPoint);

                GraphicalMovingPoint point = puzzle.SplinePoints[correctIndex];

                point.startDistancePercentage = dist / splineLength;
                point.currentDistancePercentage = point.startDistancePercentage;
                point.splineIndex = targetSplineIndex;
                point.splineLength = splineLength;

                float d = CalculateTargetDistance();
                float curveLength = spline.GetCurveLength(pointBIndex);

                float pointpos = curveLength / 5;
                float start = pointpos + 1;
                float pos = (d + (start * i));


                puzzle.TargetDistance[puzzle.TargetDistance.Count - i] = puzzle.TargetDistance[point.targetDistanceIndex];
                puzzle.SplinePoints[puzzle.TargetDistance.Count - i].normalized += 1;
                puzzle.TargetDistance[point.targetDistanceIndex] = pos/spline.GetLength();
            }


            
            
            puzzle.CheckIfWillHaveFullTurn(puzzle.SplinePoints);

        }

       

        private float CalculateTargetDistance()
        {
            Spline spline = puzzle.container.Splines[1];
            float totalDistance = 0;
            for(int i = 0; i < pointAIndex; i++)
            {
                totalDistance+=spline.GetCurveLength(i);
            }
            return totalDistance;
        }

        private float GetDistance(Vector3 pointA, Vector3 pointB)
        {
            return Vector3.Distance(pointA, pointB);
        }
    }
}