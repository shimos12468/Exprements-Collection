using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ChainSimulation : MonoBehaviour
{
    public int length;
    public Vector3 [] segmentPoses;
    private Vector3[] segmentV;
    public Transform anchor;

    public GameObject prefab;
    public float targetDist;
    public float smoothSpeed;
    public float trailSpeed;
    void Start()
    {
        splineLength = spline.CalculateLength(0);
        for (int i = 0; i < length; i++)
        {
            Instantiate(prefab, transform);
        }
            segmentPoses = new Vector3[length];
            segmentV = new Vector3[length];
        
    }
    
    void Update()
    {


        segmentPoses[0] = anchor.position;

        for(int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i-1]).normalized*targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i],targetPos,ref segmentV[i],smoothSpeed+i/trailSpeed);
            transform.GetChild(i-1).position = segmentPoses[i];
        }
        
        AttachToSpline();

        for (int i = 0; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed + i / trailSpeed);
            transform.GetChild(i - 1).position = segmentPoses[i];
        }




    }

    float distancePercentage;
    public float speed;
    float splineLength;
    public SplineContainer spline;
    private void AttachToSpline()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(0, distancePercentage);
        anchor.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0;
        }

        Vector3 nextPosition = spline.EvaluatePosition(0, distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;

        anchor.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}
