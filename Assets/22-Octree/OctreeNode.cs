using System;
using UnityEngine;

public class OctreeNode 
{
    Bounds bounds;
    float minSize;
    Bounds[] childBounds; 

    public OctreeNode(Bounds bounds ,float minSize)
    {
        this.bounds = bounds;
        this.minSize = minSize;

        float quarter = bounds.size.y / 4;
        float childLength = bounds.size.y / 2;
        Vector3 childSize = childLength * Vector3.one;
        Debug.Log(childLength);
        Debug.Log(quarter);
        childBounds = new Bounds[8];
        
        //1,1,1
        childBounds[0] = new Bounds(bounds.center + new Vector3(quarter,quarter,quarter), childSize);
        
        
        //1,-1,1
        childBounds[1] = new Bounds(bounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        
        //1,1,-1
        childBounds[2] = new Bounds(bounds.center + new Vector3(quarter, quarter,-quarter), childSize);
        
        //1 ,-1 ,-1
        childBounds[3] = new Bounds(bounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        
        //-1,1,1
        childBounds[4] = new Bounds(bounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        
        //-1 ,-1,1
        childBounds[5] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        
        //-1,1,-1
        childBounds[6] = new Bounds(bounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        
        //-1,-1,-1
        childBounds[7] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);




    }
    public void AddObject(GameObject worldObject)
    {
        DivideAndAdd(worldObject);
    }

    OctreeNode[]childrenList =null;

    private void DivideAndAdd(GameObject worldObject)
    {
        if (bounds.size.y <= minSize)
        {
            return;
        }
        if (childrenList == null)
        {
            childrenList = new OctreeNode[8];
        }
        bool divide = false;

        for(int i = 0; i < 8; i++)
        {
            if (childrenList[i] == null)
            {
                childrenList[i] = new OctreeNode(childBounds[i], minSize);
            }
            if (childBounds[i].Intersects(worldObject.GetComponent<Collider>().bounds))
            {
                divide = true;
                childrenList[i].DivideAndAdd(worldObject);
            }

            

        }
        if (divide == false)
        {
            childrenList = null;
        }


    }

    public void Draw()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (childrenList != null)
        {

            for(int i = 0; i < 8; i++)
            {
                if (childrenList[i] != null)
                {
                    childrenList[i].Draw();
                }
            }
        }

    }

    
}
