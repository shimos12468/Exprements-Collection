using System.Linq;
using UnityEngine;


namespace Octree
{
    class Mover : MonoBehaviour
	{
		public float speed=5f;
		public float accuracy=1f;
		public float turnSpeed=5f;

		int currentWaypoint;
		OctreeNode currentNode;
		Vector3 distnation;

		public CreateOctree octreeGenerator;
		Graph graph;

        private void Start()
        {
			graph = octreeGenerator.waypoints;
			currentNode = GetClosestNode(transform.position);
			print(currentNode == null);
			GetRandomDistenation();


        }


        private void Update()
        {
			if (graph == null) return;

			if (graph.GetPathLength() == 0 || currentWaypoint > graph.GetPathLength())
			{
				GetRandomDistenation();
			}
			if(Vector3.Distance(transform.position , graph.GetPathNode(currentWaypoint).bounds.center) < accuracy)
			{
				currentWaypoint++;
				Debug.Log($"Current Waypoint : {currentWaypoint}");
            }
			if (currentWaypoint < graph.GetPathLength())
			{
				currentNode = graph.GetPathNode(currentWaypoint);
				distnation = currentNode.bounds.center;

				Vector3 direction = distnation-transform.position ;
				direction.Normalize();

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
				transform.Translate(0,0,speed*Time.deltaTime);

			}
			else
			{
				GetRandomDistenation();
			}
        }

        private void GetRandomDistenation()
        {
			OctreeNode distnationNode;

            do
            {
              distnationNode = graph.nodes.ElementAt(Random.Range(0, graph.nodes.Count - 1)).Key;
            } while (!graph.AStar(currentNode, distnationNode));

			currentWaypoint= 0;
        }

        private OctreeNode GetClosestNode(Vector3 position)
        {


			return octreeGenerator.octree.FindClosestNode(position);
            //OctreeNode closestNode = null;
            //float minDistance = Mathf.Infinity;
            //foreach (OctreeNode node in graph.nodes.Keys)
            //{
            //    float distance = (node.bounds.center - position).sqrMagnitude;
            //    if (distance < minDistance)
            //    {
            //        closestNode = node;
            //        minDistance = distance;
            //    }
            //}

            //return closestNode;
        }


        private void OnDrawGizmos()
        {
			if (graph == null || graph.GetPathLength() == 0) return;



            Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(graph.GetPathNode(0).bounds.center, 0.7f);



            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(graph.GetPathNode(graph.GetPathLength()-1).bounds.center, 0.7f);

            Gizmos.color = Color.green;

			for(int i = 0; i < graph.GetPathLength(); i++)
            {
                Gizmos.DrawWireSphere(graph.GetPathNode(i).bounds.center, 0.5f);
				if (i < graph.GetPathLength() - 1)
				{
					Vector3 start = graph.GetPathNode(i).bounds.center;
                    Vector3 end = graph.GetPathNode(i+1).bounds.center;
					Gizmos.DrawLine(start, end);
				
				}
            }



        }

    }


}