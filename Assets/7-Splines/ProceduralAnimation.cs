using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ProceduralAnimation : MonoBehaviour
{
    public float distance = 1;
    public List<GameObject> points = new List<GameObject>();
    public SplineContainer spline;
    public float length = 0;
    public GameObject prefab;
    void Start()
    {
        splineLength = spline.CalculateLength(0);
        for(int i = 0; i < length; i++)
        {
            Instantiate(prefab, transform);
            points.Add(transform.GetChild(i).gameObject);
        }
    }
    public Vector3 ConstrainDistance(Vector3 point, Vector3 anchor,float distance)
    {
        Vector3 dis = (point - anchor); 
        dis = dis.normalized;
        return (dis * distance) + anchor;
    }
    float distancePercentage;
    public float speed;
    float splineLength;
    float desiredDotProduct = 0.8f;
    public bool enable = false;

    void Update()
    {
        ApplyDistanceConstraintsOnPoints();
        for(int i = 2; i < points.Count; i++)
        {
            ApplyAngleConstraint(i);
        }
    }

    private void ApplyDistanceConstraintsOnPoints()
    {
        AttachToSpline();

        for (int i = 0; i < points.Count - 1; i++)
        {
            points[i + 1].transform.position = ConstrainDistance(points[i + 1].transform.position, points[i].transform.position, distance);
            
            AlignForwardAndRightVectors(i);
            Vector3 dir = points[i].transform.position - points[i + 1].transform.position;
            points[i + 1].transform.rotation = Quaternion.LookRotation(dir, transform.up);
        }
    }

    public float radius = 10;
    void ApplyAngleConstraint(int index)
    {
         // Radius of the circle
        float maxAngle = 2 * Mathf.Asin(distance / (2 * radius)) * Mathf.Rad2Deg; // Convert to degrees

        Vector3 currentDirection = points[index].transform.position - points[index - 1].transform.position;
        Vector3 previousDirection = points[index - 1].transform.position - points[index - 2].transform.position;

        // Calculate the angle between the two segments
        float angle = Vector3.Angle(previousDirection, currentDirection);

        // Check if the angle exceeds the maxAngle
        if (angle > maxAngle)
        {
            // Calculate the axis of rotation
            Vector3 rotationAxis = Vector3.Cross(previousDirection, currentDirection).normalized;

            // Calculate the desired angle change to bring the current angle within the maxAngle
            float angleCorrection = angle - maxAngle;

            // Rotate the current segment to satisfy the angle constraint
            Quaternion rotation = Quaternion.AngleAxis(-angleCorrection, rotationAxis);
            Vector3 newDirection = rotation * currentDirection;

            // Update the position of the current point
            points[index].transform.position = points[index - 1].transform.position + newDirection.normalized * distance;
        }
    }



    private void AlignForwardAndRightVectors(int i)
    {
        Vector3 adjustedForward = Vector3.Project(points[i + 1].transform.forward, points[i].transform.forward);
        points[i + 1].transform.forward = adjustedForward;
        Vector3 adjustedRight = Vector3.Project(points[i + 1].transform.right, points[i].transform.right);
        points[i + 1].transform.right = adjustedRight;
    }

    private void AttachToSpline()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(0, distancePercentage);
        points[0].transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0;
        }

        Vector3 nextPosition = spline.EvaluatePosition(0, distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;

        points[0].transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}
