using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGPUInstancing : MonoBehaviour
{

    public Mesh mesh;
    public Material material;

    private float[] cubeOffsets;
    private Matrix4x4[] matrices; 

    private Vector3[] positions;

    private RenderParams renderParams;


    // Start is called before the first frame update
    void Start()
    {


        var count = SceneTools.GetCount;

        cubeOffsets = new float[count];
        positions= new Vector3[count];
        matrices = new Matrix4x4[count];

        SceneTools.LoopPositions((i, p) =>
        {

            cubeOffsets[i] = p.y;
            positions[i] = p;

        });

        renderParams = new RenderParams(material);

        SceneTools.Instance.SetCountText(count);
        SceneTools.Instance.SetNameText("Managed Cubes");

    }

    // Update is called once per frame
    void Update()
    {
        var time = Time.time;

        for (int i = 0; i < positions.Length; i++)
        {

            
            var (pos, rot) = positions[i].CalculatePos(cubeOffsets[i], time);

            matrices[i].SetTRS(pos, rot, SceneTools.CubeScale);

            positions[i].y = pos.y;

        }

        Graphics.RenderMeshInstanced(renderParams, mesh, 0, matrices);
    }
}
