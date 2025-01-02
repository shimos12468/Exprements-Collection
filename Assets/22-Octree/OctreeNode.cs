using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeNode
	{
	    public List<OctreeObject> objects = new();
	    
	    float minSize;
	    
	    static int nextId;
	    public readonly int Id;
	   
	    public Bounds bounds;
	    Bounds[] childBounds = new Bounds[8];
	    
	    public OctreeNode[] childrenList;
	
	    public bool isLeaf =>childrenList==null;
	
	    public OctreeNode(Bounds bounds, float minSize)
	    {
	        Id= nextId++;
	        this.bounds = bounds;
	        this.minSize = minSize;
	
	        Vector3 newSize = bounds.size *0.5f;
	        Vector3 centerOffset = bounds.size *0.25f;
	        Vector3 parentCenter = bounds.center;
	       
	
	        for(int i = 0; i < 8; i++)
	        {
	            Vector3 childCenter = parentCenter;
	            childCenter.x += centerOffset.x * ((i & 1) == 0 ? -1 : 1);
	            childCenter.y += centerOffset.y * ((i & 2) == 0 ? -1 : 1);
	            childCenter.z += centerOffset.z * ((i & 4) == 0 ? -1 : 1);
	            childBounds[i] = new Bounds(childCenter,newSize);
	        }
	    }
	    public void Divide(GameObject worldObject)
	    {
	        Divide(new OctreeObject(worldObject));
	    }
	
	    private void Divide(OctreeObject octreeObject)
	    {
	        if (bounds.size.x <= minSize)
	        {
	            AddWorldObject(octreeObject);
	            return;
	        }



            childrenList ??= new OctreeNode[8];

	        bool intersected = false;
	
	        for(int i = 0; i < 8; i++)
	        {
	             childrenList[i] ??= new OctreeNode(childBounds[i], minSize);
	            if (childBounds[i].Intersects(octreeObject.bounds))
	            {
	                intersected = true;
	                childrenList[i].Divide(octreeObject);
	            }
	        }
	        
	        if (!intersected)
	        {
	            AddWorldObject(octreeObject);
	        }
	
	    }
	
	
	    void AddWorldObject(OctreeObject obj) => objects.Add(obj);
	
	    public void Draw()
	    {
	        Gizmos.color = Color.green;
	        Gizmos.DrawWireCube(bounds.center, bounds.size);
	
	        Gizmos.color = Color.red;
	       
	
	        foreach(var obj in objects)
	        {
	            Gizmos.DrawCube(obj.bounds.center, obj.bounds.size);
	        }
	
	        if (childrenList != null)
	        { 
	            for(int i = 0; i < childrenList.Length; i++)
	            {
	                if (childrenList[i] != null)
	                {
	                    
	                    childrenList[i].Draw();
	                }
	
	            }
	        }
	    }
	
	  
	}
}


namespace Octree
{


}