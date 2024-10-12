using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Level1 : MonoBehaviour
{

    public GameObject cubePrefab;

        // Start is called before the first frame update
    void Start()
    {
        SceneTools.LoopPositions((i, p) =>
        {

            Instantiate(cubePrefab, new Vector3(p.x, p.y * SceneTools.DEPTH_OFFSET, p.z), Quaternion.identity, transform)
            .AddComponent<Level1Cube>()
            .Init(p.y );

        });


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



public static class CubeHelpers
{
    public static (Vector3 pos, Quaternion rot) CalculatePos(this Vector3 pos, float yOffset, float time)
    {
        var t = Mathf.InverseLerp(yOffset, SceneTools.HEIGHT_SCALE + yOffset, pos.y);
        var rot = Quaternion.Slerp(quaternion.identity, SceneTools.RotGoal, t);
        pos.y = SceneTools.HEIGHT_SCALE * Mathf.PerlinNoise(pos.x * SceneTools.NOISE_SCALE + time, pos.z * SceneTools.NOISE_SCALE + time) + yOffset * SceneTools.DEPTH_OFFSET;
        return (pos, rot);
    }

    public static (float3 pos, Quaternion rot) CalculatePosBurst(this float3 pos, float yOffset, float time)
    {
        var t = math.unlerp(yOffset, SceneTools.BURST_HEIGHT_SCALE + yOffset, pos.y);
        pos.y = SceneTools.BURST_HEIGHT_SCALE * noise.cnoise(new float2(pos.x * SceneTools.NOISE_SCALE + time, pos.z * SceneTools.NOISE_SCALE + time)) +
                yOffset * SceneTools.DEPTH_OFFSET;
        var rot = math.nlerp(quaternion.identity, SceneTools.RotGoal, t);
        return (pos, rot);
    }
}

public class Level1Cube : MonoBehaviour
{

    private float yOffset;

    private void Update()
    {

        var (pos, rot) = transform.position.CalculatePos(yOffset, Time.time);
        transform.SetPositionAndRotation(pos, rot);

    }

    public void Init(float p)
    {
        yOffset = p;
    }
}



