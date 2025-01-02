using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace EXP.U4FOUNDERSPUZZLE
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] Puzzle puzzle;
        bool initialized = false;



        public List<GraphicalMovingPoint> PointsToGetOut = new List<GraphicalMovingPoint>();

        public List<GraphicalMovingPoint> pointsToGetIn = new List<GraphicalMovingPoint>();


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
                PointsToGetOut.Clear();

                initialized = false;

            }

            if (Input.GetMouseButtonDown(1))
            {
                Turn();
                puzzle.value = 4;
                puzzle.SetMove(true);
            }
        }

        private void Turn()
        {
            int targetSplineIndex = 1;

            Vector3 initialPoint = puzzle.container.Splines[targetSplineIndex].Knots.ToArray()[0].Position;

            float splineLength = puzzle.container.CalculateLength(targetSplineIndex);

            Spline spline = puzzle.container.Splines[targetSplineIndex];

            //List<float> values = new List<float>(puzzle.TargetDistance);

            float d = CalculateTargetDistance();

            float curveLength = spline.GetCurveLength(6);

            float pointpos = curveLength / 4;

            float start = pointpos;

            RemoveFromSpline();

            EnterSpline(targetSplineIndex, initialPoint, splineLength, spline, d, start);

            ShiftPoints();


            puzzle.CheckIfWillHaveFullTurn(puzzle.SplinePoints);

        }

        private void RemoveFromSpline()
        {
            for (int i = 0; i < PointsToGetOut.Count; i++)
            {
                int index = puzzle.splinePoints.FindIndex(x => x == PointsToGetOut[i]);

                puzzle.splinePoints[index].switching = true;
            }
            PointsToGetOut.Clear();
        }

        private void EnterSpline(int targetSplineIndex, Vector3 initialPoint, float splineLength, Spline spline, float d, float start)
        {
            List<GraphicalMovingPoint> pointsToMove = new List<GraphicalMovingPoint>();
            pointsToMove.Add(Find(puzzle.splinePoints, 0.2f));
            pointsToMove.Add(Find(puzzle.splinePoints, 0.3f));
            pointsToMove.Add(Find(puzzle.splinePoints, 0.4f));

            for (int i = 0; i < pointsToMove.Count; i++)
            {
                int correctIndex = i;

                Vector3 currentPointPosition = pointsToMove[correctIndex].obj.transform.position;

                float dist = GetDistance(currentPointPosition, initialPoint);

                GraphicalMovingPoint point = pointsToMove[correctIndex];

                point.startDistance = dist / splineLength;
                point.currentDistance = point.startDistance;
                point.splineIndex = targetSplineIndex;
                point.splineLength = splineLength;

                float pos = (d) + (start * (i + 1));

                puzzle.TargetDistance[point.distanceIndex] = pos / spline.GetLength();

                PointsToGetOut.Add(point);

            }
        }


        public GraphicalMovingPoint Find(List<GraphicalMovingPoint> list, float num)
        {

            for (int i = 0; i < list.Count; i++)
            {
                if (Mathf.Approximately(puzzle.targetDistance[puzzle.splinePoints[i].distanceIndex], num))
                {
                    return list[i];
                }

            }

            return null;
        }



        bool v = true;
        private void ShiftPoints()
        {
            for (int i = 0; i < puzzle.targetDistance.Count; i++)
            {

                if (puzzle.splinePoints[i].splineIndex != 0)
                {
                    continue;
                }

                if (v)
                    puzzle.TargetDistance[i] = (puzzle.TargetDistance[i] + 0.5f) % 1;
                else
                    puzzle.TargetDistance[i] = (puzzle.TargetDistance[i] + 0.6f) % 1;
            }
            v = !v;
        }


        private float CalculateTargetDistance()
        {
            Spline spline = puzzle.container.Splines[1];
            float totalDistance = 0;
            for (int i = 0; i <= 5; i++)
            {
                totalDistance += spline.GetCurveLength(i);
            }
            return totalDistance;
        }

        private float GetDistance(Vector3 pointA, Vector3 pointB)
        {
            return Vector3.Distance(pointA, pointB);
        }
    }
}