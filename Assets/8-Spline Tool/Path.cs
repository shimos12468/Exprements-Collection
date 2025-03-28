using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField, HideInInspector]
    public List<Vector2> points;

    [SerializeField, HideInInspector]
    public bool isClosed;


    [SerializeField, HideInInspector]
    public bool autoSetControlPoints;

    public Path(Vector2 center)
    {
        points = new List<Vector2>() {
        center+Vector2.left,
        center+(Vector2.left+Vector2.up)*0.5f,
        center+(Vector2.right+Vector2.down)*0.5f,
        center+Vector2.right
        };
    }

    public bool IsClosed
    {
        get
        {
            return isClosed;
        }
        set
        {
            if (isClosed != value)
            {
                isClosed = value;

                if (isClosed)
                {
                    points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
                    points.Add(points[0] * 2 - points[1]);
                    if (autoSetControlPoints)
                    {
                        AutoSetAnchorControlPoints(0);
                        AutoSetAnchorControlPoints(points.Count - 3);
                    }
                }
                else
                {
                    points.RemoveRange(points.Count - 2, 2);
                    if (autoSetControlPoints)
                    {
                        AutoSetStartAndEndControls();
                    }
                }
            }

        }
    }
    public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return autoSetControlPoints;
        }
        set
        {
            if (autoSetControlPoints != value)
            {
                autoSetControlPoints = value;
                if (autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public int NumOfPoints
    {
        get
        {
            return points.Count;
        }
    }
    public int NumOfSegments
    {
        get
        {
            return points.Count / 3;
        }
    }

    public void AddSegment(Vector2 anchor)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchor) * 0.5f);
        points.Add(anchor);
        if (autoSetControlPoints)
        {
            AutoSetAffectedControlPoints(points.Count - 1);
        }
    }

    public void SplitSegment(Vector2 anchorPosition, int segmentIndex)
    {
        points.InsertRange(segmentIndex * 3 + 2, new Vector2[] { Vector2.zero, anchorPosition, Vector2.zero });
        if (autoSetControlPoints)
        {
            AutoSetAffectedControlPoints(segmentIndex * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
        }
    }
    public void DeleteSegment(int anchorIndex)
    {

        if (NumOfSegments > 2 || !isClosed && NumOfSegments > 1)
        {
            if (anchorIndex == 0)
            {

                if (isClosed)
                {
                    points[points.Count - 1] = points[2];
                }
                points.RemoveRange(0, 3);
            }
            else if (anchorIndex == points.Count - 1 && !isClosed)
            {
                points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                points.RemoveRange(anchorIndex - 1, 3);
            }

        }

    }

    public Vector2[] GetPointsInSegment(int index)
    {
        return new Vector2[] { points[index * 3], points[index * 3 + 1], points[index * 3 + 2], points[LoopIndex(index * 3 + 3)] };
    }

    public void MovePoint(int i, Vector2 pos)
    {
        Vector2 deltaMove = pos - points[i];

        if (i % 3 == 0 || !autoSetControlPoints)
        {
            points[i] = pos;

            if (autoSetControlPoints)
            {
                AutoSetAffectedControlPoints(i);
            }
            else
            {

                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count || isClosed)
                    {
                        points[LoopIndex(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0 || isClosed)
                    {
                        points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count || isClosed)
                    {
                        float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector2 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                        points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }

    }

    public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
    {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(points[0]);
        Vector2 previousPoint = points[0];
        float distanceSinceLastEvenPoint = 0;
        for (int i = 0; i < NumOfSegments; i++)
        {
            Vector2[] points = GetPointsInSegment(i);

            float controlNetLength = Vector2.Distance(points[0], points[1]) + Vector2.Distance(points[1], points[2]) + Vector2.Distance(points[2], points[3]);
            float estimatedCurveLength = Vector2.Distance(points[0], points[3]) + controlNetLength * .5f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f / divisions;
                Vector2 pointOnCurve = Bezier.EvaluateCubic(points[0], points[1], points[2], points[3], t);

                distanceSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (distanceSinceLastEvenPoint >= spacing)
                {
                    float overShoot = distanceSinceLastEvenPoint - spacing;
                    Vector2 spacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overShoot;
                    evenlySpacedPoints.Add(spacedPoint);
                    distanceSinceLastEvenPoint = overShoot;
                    previousPoint = spacedPoint;
                }

                previousPoint = pointOnCurve;
            }

        }
        return evenlySpacedPoints.ToArray();
    }

    public void AutoSetAffectedControlPoints(int anchorIndex)
    {
        for (int i = anchorIndex - 3; i <= anchorIndex + 3; i += 3)
        {
            if (i >= 0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    public void AutoSetAllControlPoints()
    {
        for (int i = 0; i < points.Count; i += 3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }


    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector2 anchorPos = points[anchorIndex];
        Vector2 dir = Vector2.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
        if (anchorIndex + 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
            }
        }
    }

    public void AutoSetStartAndEndControls()
    {
        if (!isClosed)
        {
            points[1] = (points[0] + points[2]) * 0.5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
        }

    }

    public int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}
