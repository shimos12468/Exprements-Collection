using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Octree
{
	public class Octree
	{
	    public OctreeNode rootNode;
	    public Bounds bounds;
		public Graph graph;
	    public List<OctreeNode> emptyLeaves = new();


	    public Octree(GameObject[] worldObjects, float minNodeSize ,Graph graph)
	    {
	
			this.graph = graph;
	        CalculateBounds(worldObjects);
	        CreateTree(worldObjects, minNodeSize);
	        GetEmptyLeaves(rootNode);
			GetEdges();
	        int count = emptyLeaves.Count;
	        Debug.Log(count);
	
	    }

		public OctreeNode FindClosestNode(Vector3 position) => FindClosestNode(rootNode, position);

		public OctreeNode FindClosestNode(OctreeNode node,Vector3 position)
		{
            if (node.isLeaf&&node.bounds.Contains(position))
            {
                return node;
            }

            for (int i = 0; i < node.childrenList.Length; i++)
			{
				if (node.childrenList[i].bounds.Contains(position))
				{
                   return FindClosestNode(node.childrenList[i], position);
                }
				
			}
			return null;
		}

		public void GetEdges()
		{
			foreach(OctreeNode octreeNodeA in emptyLeaves)
			{

                foreach (OctreeNode octreeNodeB in emptyLeaves)
                {
					if (octreeNodeA.bounds.Intersects(octreeNodeB.bounds))
					{
						graph.AddEdge(octreeNodeA, octreeNodeB);
					}
                }

            }
		}
	
	    private  void CalculateBounds(GameObject[] worldObjects)
	    {
	        foreach (var obj in worldObjects)
	        {
	            bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
	        }
	        Vector3 sizeVector = Vector3.one * Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y),bounds.size.z) * 0.6f;
	        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
	
	    }
	
	    public void GetEmptyLeaves(OctreeNode node)
	    {
	        if (node.isLeaf&&node.objects.Count==0)
	        {
	
	            emptyLeaves.Add(node);
				graph.AddNode(node);
	            return;
	        }
			if (node.childrenList == null) return;

	        for(int i = 0; i < node.childrenList.Length; i++)
	        {
	                GetEmptyLeaves(node.childrenList[i]);
	        }

			for(int i = 0; i < node.childrenList.Length; i++)
			{
				for(int j = i+1; j < node.childrenList.Length; j++)
				{
					
					graph.AddEdge(node.childrenList[i], node.childrenList[j]);
				}
			}


	    } 
	    public void CreateTree(GameObject[] worldObjects, float minNodeSize)
	    {
	        rootNode = new OctreeNode(bounds, minNodeSize);
	
	        foreach (GameObject worldObject in worldObjects)
	        {
	            rootNode.Divide(worldObject);
	        }
	
	    }
	
	    public void AddObjects(GameObject[] worldObjects)
	    {
	       
	    }
	}
}
