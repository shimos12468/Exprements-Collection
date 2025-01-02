using System.Collections.Generic;


namespace Octree
{
    public class Node
	{
		static int nextId;
		public readonly int id;

		public float f, g,h;

		public Node from;

		public List<Edge> edges = new();

		public OctreeNode octreeNode;

		public Node(OctreeNode node)
		{
			id = nextId++;

			this.octreeNode = node;
		}

        public override bool Equals(object obj)
        {
            return obj is Node node &&node.id==id;
        }
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }


    } 
}