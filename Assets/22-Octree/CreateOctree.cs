using UnityEngine;

namespace Octree
{
	public class CreateOctree : MonoBehaviour
	{
	    public GameObject[] worldObjects;
	    public float minNodeSize;
	    public Octree octree;

		public readonly Graph waypoints = new();

	    void Awake()
	    {
	        octree = new Octree(worldObjects, minNodeSize ,waypoints);
	
	    }
	
	   // private void OnDrawGizmos()
	   // {
	   //     if (Application.isPlaying)
	   //     {
	
	   //         Gizmos.color = Color.green;
	
	   //         Gizmos.DrawWireCube(octree.bounds.center, octree.bounds.size);
	   //         octree.rootNode.Draw();

				//waypoints.DrawGraph();
	   //     }
	   // }
	
	
	
	}
}
