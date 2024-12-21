using UnityEngine;

public class Octree
{
    public OctreeNode rootNode;


    public Octree(GameObject[] worldObjects,float minNodeSize)
    {
        Bounds bounds= new Bounds();

        foreach(var obj in worldObjects)
        {
            bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
        }

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = Vector3.one* maxSize;
        bounds.SetMinMax(bounds.center- (sizeVector / 2), bounds.center + (sizeVector/2));
        rootNode = new OctreeNode(bounds, minNodeSize);

        AddObjects(worldObjects);

    }


    public void AddObjects(GameObject[] worldObjects)
    {
        foreach(GameObject worldObject in worldObjects)
        {
            rootNode.AddObject(worldObject);
        }
    }
}
