using UnityEngine;


namespace Octree
{


public class OctreeObject
{

    public Bounds bounds;
    public OctreeObject(GameObject worldObject)
    {
        bounds= worldObject.GetComponent<Collider>().bounds;
    }

    public bool Intersects(Bounds boundsToCheck) => bounds.Intersects(boundsToCheck);


}
}