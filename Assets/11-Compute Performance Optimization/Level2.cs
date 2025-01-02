using UnityEngine;

public class Level2 : MonoBehaviour
{
    public GameObject cubePrefab;

    private float[] cubeOffsets;
    private Transform[] spawnedCubes;
    // Start is called before the first frame update
    void Start()
    {


        var count = SceneTools.GetCount;
        cubeOffsets = new float[count];
        spawnedCubes = new Transform[count];

        SceneTools.LoopPositions((i, p) =>
        {

            cubeOffsets[i] = p.y;
            spawnedCubes[i] = Instantiate(cubePrefab, p, Quaternion.identity, transform).transform;

        });

        SceneTools.Instance.SetCountText(count);
        SceneTools.Instance.SetNameText("Managed Cubes");

    }

    // Update is called once per frame
    void Update()
    {
        var time = Time.time;

        for (int i = 0; i < spawnedCubes.Length; i++)
        {

            var cube = spawnedCubes[i];
            var (pos, rot) = cube.position.CalculatePos(cubeOffsets[i], time);
            cube.SetPositionAndRotation(pos, rot);

        }
    }
}
