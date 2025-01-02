using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Level5 : MonoBehaviour
{

    [SerializeField] private Transform _pusher;
    [SerializeField] private float _pusherSpeed = 20;
    public ComputeShader compute;
    public Mesh mesh;
    public Material material;
    public int countMultiplier = 1;

    private readonly uint[] args = { 0, 0, 0, 0, 0 };
    private ComputeBuffer argsBuffer;
    private ComputeBuffer _meshPropertiesBuffer;
    int kernal;
    int count;


    private int cacheMultiplier = 1;


    // Start is called before the first frame update
    void Start()
    {
        kernal = compute.FindKernel("CSMain");
        count = SceneTools.GetCount * countMultiplier;
        ApplyMultiplierUpdate(countMultiplier, true);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);


        UpdateBuffers();

        SceneTools.Instance.SetCountText(count);
        SceneTools.Instance.SetNameText("GPU Instancing Indirect Interaction");

    }

    // Update is called once per frame
    void Update()
    {
        var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _pusher.Translate(dir * (_pusherSpeed * Time.deltaTime));
        compute.SetVector("pusher_position", _pusher.position);
        compute.Dispatch(kernal, Mathf.CeilToInt(count / 64), 1, 1);

        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 1000), argsBuffer);

        if (Input.GetMouseButtonUp(0) && countMultiplier != cacheMultiplier)
        {
            countMultiplier = cacheMultiplier;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }



    private void UpdateBuffers()
    {

        var offset = Vector3.zero;

        var data = new MeshData[count];

        for (int i = 0; i < count; i++)
        {
            var pos = Random.insideUnitSphere.normalized * Random.Range(10, 50) + offset;
            var rot = Quaternion.Euler(Random.insideUnitSphere.normalized);

            data[i] = new MeshData
            {
                basePos = pos,
                mat = Matrix4x4.TRS(pos, rot, SceneTools.CubeScale),
                amount = 0
            };
        }


        _meshPropertiesBuffer = new ComputeBuffer(count, 80);
        _meshPropertiesBuffer.SetData(data);

        compute.SetBuffer(kernal, "data", _meshPropertiesBuffer);
        material.SetBuffer("data", _meshPropertiesBuffer);

        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)count;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);

        argsBuffer.SetData(args);

    }

    public void UpdateMultiplier(float val)
    {
        ApplyMultiplierUpdate(Mathf.CeilToInt(val));
    }

    public Slider slider;
    public TMP_Text sliderValueText;
    public void ApplyMultiplierUpdate(int val, bool applySliderChange = false)
    {
        sliderValueText.text = $"Multiplier: {val.ToString()}";
        cacheMultiplier = val;
        if (applySliderChange)
        {
            slider.value = val;
        }
    }
    private struct MeshData
    {
        public float3 basePos;
        public Matrix4x4 mat;
        public float amount;
    }
}
