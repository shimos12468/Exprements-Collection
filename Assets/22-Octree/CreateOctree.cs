using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public float minNodeSize;
    Octree octree;
    void Start()
    {
        octree = new Octree(worldObjects, minNodeSize);
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {

            octree.rootNode.Draw();
        }
    }

   

}
