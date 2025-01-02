using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer splinesContainer;
    public float speed = 1f;

    public List<GameObject> woodenPlates = new List<GameObject>();
    public List<float> splineLength;
    public List<int> splineIndex;
    public List<float> distancePercentages;

    public GameObject prefab;

    int index = 1;


    void Start()
    {
        //PopulatePlates();


    }

    // Update is called once per frame
    void Update()
    {


    }

    public void MoveAlongeSplines(List<GameObject> woodenPlates, SplineContainer splinesContainer)
    {

        Initialize();

        for (int i = 0; i < woodenPlates.Count; i++)
        {

            distancePercentages[i] += speed * Time.deltaTime / splineLength[i];

            Vector3 currentPosition = splinesContainer.EvaluatePosition(splineIndex[i], distancePercentages[i]);

            woodenPlates[i].transform.position = currentPosition;

            if (Vector3.Distance(woodenPlates[i].transform.position, splinesContainer.Splines[1].Knots.ToArray()[0].Position) <= 0.01f && splineIndex[i] == 0)
            {
                //print("hey ? ");
                //splineIndex[i] = 1;
                //splineLength[i] = splinesContainer.CalculateLength(splineIndex[i]);
                //distancePercentages[i] = 0;

            }
            else if (distancePercentages[i] > 1f)
            {

                //distancePercentage = 0;
                float min = Mathf.Infinity;
                for (int k = 0; k < splinesContainer.Splines[0].Knots.ToArray().Length; k++)
                {

                    float newmin = Mathf.Min(Vector3.Distance(splinesContainer.Splines[0].Knots.ToArray()[k].Position, splinesContainer.Splines[1].Knots.ToArray()[splinesContainer.Splines[1].Knots.ToArray().Length - 1].Position), min);

                    if (newmin < min)
                    {
                        min = newmin;
                        index = k;
                    }


                }
                splineIndex[i] = 0;
                splineLength[i] = splinesContainer.CalculateLength(splineIndex[i]);
                distancePercentages[i] = Vector3.Distance(splinesContainer.Splines[0].Knots.ToArray()[0].Position, splinesContainer.Splines[0].Knots.ToArray()[index].Position * splinesContainer.gameObject.transform.localScale) / splineLength[i];

                Vector3 nextPosition = splinesContainer.Splines[0].Knots.ToArray()[index].Position;
                Vector3 direction = nextPosition - currentPosition;
                woodenPlates[i].transform.rotation = Quaternion.LookRotation(direction, transform.up);
            }
            else
            {
                Vector3 nextPosition = splinesContainer.EvaluatePosition(splineIndex[i], distancePercentages[i] + 0.05f);
                Vector3 direction = nextPosition - currentPosition;
                woodenPlates[i].transform.rotation = Quaternion.LookRotation(direction, transform.up);

            }

        }
    }

    public int initialAngle = 45;

    int numberOfObjects = 10;
    float radius = 0.5f;
    float mm = Mathf.Infinity;
    int ind;
    float dist = 0;
    public void PopulatePlates()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            var angle = i * Mathf.PI * 2 / numberOfObjects;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            GameObject go = Instantiate(prefab, pos, Quaternion.identity);

            for (int j = 0; j < splinesContainer.Splines[0].Knots.ToArray().Length; j++)
            {
                float min = Vector3.Distance(go.transform.position, splinesContainer.Splines[0].Knots.ToArray()[j].Position);
                if (min <= mm)
                {

                    mm = min;
                    ind = j;


                }


            }
            for (int k = 0; k < ind; k++)
            {
                dist += Vector3.Distance(splinesContainer.Splines[0].Knots.ToArray()[k].Position, splinesContainer.Splines[0].Knots.ToArray()[k + 1].Position);
            }
            distancePercentages.Add(dist / splinesContainer.CalculateLength(0));
            go.transform.position = splinesContainer.Splines[0].Knots.ToArray()[ind].Position;
            woodenPlates.Add(go);
            mm = Mathf.Infinity;
            dist = 0;
        }


        Initialize();
    }


    void Initialize()
    {
        splineLength.Capacity = woodenPlates.Count;
        splineIndex.Capacity = woodenPlates.Count;
        //distancePercentages.Capacity = woodenPlates.Count;

        for (int i = 0; i < woodenPlates.Count; i++)
        {
            splineIndex.Add(0);
        }

        for (int i = 0; i < woodenPlates.Count; i++)
        {
            splineLength.Add(splinesContainer.CalculateLength(splineIndex[i]));
        }
    }

}
