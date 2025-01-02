using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Octree
{
    public class Graph
	{
		public  readonly Dictionary<OctreeNode, Node> nodes = new();

		public readonly HashSet<Edge>edges= new();

		List<Node> pathList = new();
		
		public int GetPathLength()=>pathList.Count;

		public OctreeNode GetPathNode(int index)
		{
			if (pathList == null) return null;

			if (index < 0 || index >= pathList.Count)
			{
				Debug.Log("Index Out Of Bounds");
				return null;
			}
			return pathList[index].octreeNode;

		}


		public float maxItirations = 100000;

		public bool AStar(OctreeNode startNode , OctreeNode endNode)
		{
			pathList.Clear();

			Node start = FindNode(startNode);
			Node end = FindNode(endNode);

			if (start == null || end == null)
			{

				Debug.Log("The node you passed is null");
				return false;
			}


			SortedSet<Node> openSet = new(new NodeComparer());
            HashSet<Node> closedSet = new();
			int iterationCount = 0;

			start.g = 0;
			start.h = Heuristic(start, end);
			start.f = start.g+start.h;
			start.from = null;
			openSet.Add(start);

			float min = Mathf.Infinity;
			Node q= null;
			while (openSet.Count > 0)
			{
				if (++iterationCount > maxItirations)
				{
					Debug.Log("Exceeded the max Iterations");
					return false;
				}

				Node current = openSet.First();
				openSet.Remove(current);
				if (current.Equals(end))
				{
					ReconstructPath(current);
					return true;
				}
				closedSet.Add(current);

                foreach (Edge edge in current.edges)
                {
					Node neighbor = Equals(current ,edge.a)?edge.b:edge.a;

					if (closedSet.Contains(neighbor)) continue;


					float tentative_gScore = current.g+Heuristic(current,neighbor);

					if (tentative_gScore < neighbor.g || !openSet.Contains(neighbor))
					{
						neighbor.g = tentative_gScore;
						neighbor.h = Heuristic(neighbor, end);
						neighbor.f = neighbor.g+neighbor.h;
						neighbor.from = current;
						openSet.Add(neighbor);
					}
                }


            }

				Debug.Log("No Path Found");

				return false;

        }

        private void ReconstructPath(Node current)
        {
			if (current != null)
			{

				
                pathList.Add(current);
                while (current.from != null)
				{
					pathList.Add(current.from);
					current = current.from;
				}
				pathList.Reverse();
			}
        }

        private float Heuristic(Node start, Node end)
        {
			return (start.octreeNode.bounds.center - end.octreeNode.bounds.center).sqrMagnitude;
        }

        public class NodeComparer : IComparer<Node>
        {
            public int Compare(Node x, Node y)
            {
				if (x == null || y == null) return 0;

				int comp = x.f.CompareTo(y.f);
				if (comp == 0)
				{
					comp =  x.id.CompareTo(y.id);
				}
				return comp;
            }
        }


        public void AddNode(OctreeNode node)
		{
			if (!nodes.ContainsKey(node))
			{
				nodes.Add(node, new Node(node));
			}

		}

		public void AddEdge(OctreeNode nodeA , OctreeNode nodeB)
		{
			Node a = FindNode(nodeA);
            Node b = FindNode(nodeB);

			if (a == null || b == null) return;

			var edge = new Edge(a,b);

			if (edges.Add(edge))
			{
				a.edges.Add(edge);
                b.edges.Add(edge);
            }


        }

		public void DrawGraph()
		{
			Gizmos.color = Color.red;

			foreach(Edge edge in edges)
			{
				Gizmos.DrawLine(edge.a.octreeNode.bounds.center, edge.b.octreeNode.bounds.center);
			}


			foreach(Node node in nodes.Values)
			{
				Gizmos.DrawSphere(node.octreeNode.bounds.center, 0.2f);
			}

		}

		public Node FindNode(OctreeNode octreeNode)
		{
			nodes.TryGetValue(octreeNode, out Node node);
			return node;
		}


	}
}