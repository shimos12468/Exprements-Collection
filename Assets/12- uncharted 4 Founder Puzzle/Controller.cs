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
        bool initialized = false;

        public int pointAIndex, pointBIndex;
       
        public List<GraphicalMovingPoint>selectedPoints = new List<GraphicalMovingPoint>();

        public int shiftStart;
        public int shiftCount;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CalculateTargetDistance();
                if (!initialized)
                {
                    initialized = true;
                }
            }

            if (initialized)
            {
               puzzle.SetupGame();
                selectedPoints.Clear();
               
               initialized = false;
                
            }

            if (Input.GetMouseButtonDown(1))
            {
                Turn();
                puzzle.SetMove(true);
            }
        }

        private void Turn()
        {
            int targetSplineIndex = 1;

            Vector3 initialPoint = puzzle.container.Splines[targetSplineIndex].Knots.ToArray()[0].Position;

            float splineLength = puzzle.container.CalculateLength(targetSplineIndex);

            Spline spline = puzzle.container.Splines[targetSplineIndex];

            List<float> values = new List<float>(puzzle.TargetDistance);

            float d = CalculateTargetDistance();

            float curveLength = spline.GetCurveLength(pointBIndex);

            float pointpos = curveLength / 4;

            float start = pointpos;


            if (selectedPoints.Count > 0)
            {
                
                for (int i = 0; i < selectedPoints.Count; i++)
                {
                    int index= puzzle.splinePoints.FindIndex(x => x == selectedPoints[i]);

                    puzzle.splinePoints[index].switching = true;
                    puzzle.splinePoints[index].splineIndex = 1;
                    puzzle.targetDistance[puzzle.splinePoints[index].distanceIndex] = i*0.1f+0.4f;

                }
                puzzle.targetDistance[2] = 0.2f;
                puzzle.targetDistance[puzzle.splinePoints[8].distanceIndex] = 0.3f;
                puzzle.targetDistance[puzzle.splinePoints[9].distanceIndex] = 0.4f;
                selectedPoints.Clear();
            }


            ShiftPoints(values, shiftStart,shiftCount);
            
            SwitchToSpline(targetSplineIndex, initialPoint, splineLength, spline, d, start);

            for (int i = 0; i < puzzle.targetDistance.Count - 3; i++)
            {
                int correctIndex = i + 5;
                int index = correctIndex % puzzle.targetDistance.Count;
                int valueIndex = (correctIndex + 5) % puzzle.targetDistance.Count;

                GraphicalMovingPoint point = puzzle.splinePoints[index];

                puzzle.splinePoints[index] = puzzle.splinePoints[valueIndex];

                puzzle.splinePoints[valueIndex] = point;

            }

            puzzle.CheckIfWillHaveFullTurn(puzzle.SplinePoints);

        }

        private void SwitchToSpline(int targetSplineIndex, Vector3 initialPoint, float splineLength, Spline spline, float d, float start)
        {
            for (int i = shiftStart; i < shiftStart+shiftCount; i++)
            {
                int correctIndex = i;
                Vector3 currentPointPosition = puzzle.SplinePoints[correctIndex].obj.transform.position;

                float dist = GetDistance(currentPointPosition, initialPoint);

                GraphicalMovingPoint point = puzzle.SplinePoints[correctIndex];

                point.startDistance = dist / splineLength;
                point.currentDistance = point.startDistance;
                point.splineIndex = targetSplineIndex;
                point.splineLength = splineLength;

                float pos = (d) + (start * (i - 1));

                puzzle.TargetDistance[point.distanceIndex] = pos / spline.GetLength();

                selectedPoints.Add(point);

            }
        }

        private void ShiftPoints(List<float> values , int count ,int start)
        {

            for (int i = start+count; i < puzzle.targetDistance.Count+count; i++)
            {

                print(i % puzzle.targetDistance.Count);
                
                int index = i % puzzle.targetDistance.Count;
                int valueIndex = (i + 5) % puzzle.targetDistance.Count;
                puzzle.TargetDistance[index] = values[valueIndex];
            }
        }


        private float CalculateTargetDistance()
        {
            Spline spline = puzzle.container.Splines[1];
            float totalDistance = 0;
            for(int i = 0; i <= pointAIndex; i++)
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