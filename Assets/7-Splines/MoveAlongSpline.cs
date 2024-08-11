using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer splinesContainer;
    public float speed = 1f;
    float distancePercentage = 0f;
    float splineLength;
    int index = 1;
    int splineIndex = 1;
    public Transform spline2Start;

    // Start is called before the first frame update
    void Start()
    {
        splineLength = splinesContainer.CalculateLength(splineIndex);




    }

    // Update is called once per frame
    void Update()
    {


       
        distancePercentage += speed * Time.deltaTime / splineLength;
        Vector3 currentPosition = splinesContainer.EvaluatePosition(splineIndex, distancePercentage);
        transform.position = currentPosition;
        if (Vector3.Distance(transform.position, spline2Start.position) <= 0.01f && splineIndex == 0)
        {
            print("hey ? ");
            splineIndex = 1;
            splineLength = splinesContainer.CalculateLength(splineIndex);
            distancePercentage = 0;

        }
        else if (distancePercentage > 1f)
        {

            //distancePercentage = 0;
            float min = Mathf.Infinity;
            for (int i = 0; i < splinesContainer.Splines[0].Knots.ToArray().Length; i++)
            {

                float newmin = Mathf.Min(Vector3.Distance(splinesContainer.Splines[0].Knots.ToArray()[i].Position, splinesContainer.Splines[1].Knots.ToArray()[splinesContainer.Splines[1].Knots.ToArray().Length - 1].Position), min);

                if (newmin < min)
                {
                    min = newmin;
                    index = i;
                }


            }
            splineIndex = 0;
            splineLength = splinesContainer.CalculateLength(splineIndex);
            distancePercentage = Vector3.Distance(splinesContainer.Splines[0].Knots.ToArray()[0].Position, splinesContainer.Splines[0].Knots.ToArray()[index].Position) / splineLength;

            Vector3 nextPosition = splinesContainer.Splines[0].Knots.ToArray()[index].Position;
            Vector3 direction = nextPosition - currentPosition;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }
        else
        {
            Vector3 nextPosition = splinesContainer.EvaluatePosition(splineIndex, distancePercentage + 0.05f);
            Vector3 direction = nextPosition - currentPosition;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);

        }


    }
}
