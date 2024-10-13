using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level4 : MonoBehaviour
{
    private readonly uint[] args={0,0,0,0,0};




    private ComputeBuffer argsBuffer;
    private int count;

    public Mesh mesh;
    public Material material;

    private ComputeBuffer positionBuffer1, positionBuffer2;
    private int cacheMultiplier = 1;

    public int countMultiplier =1;
    private void Start()
    {
        count = SceneTools.GetCount*countMultiplier;

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
        SceneTools.Instance.SetCountText(count);
        SceneTools.Instance.SetNameText("GPU Instancing Indirect");


    }



    private void Update()
    {
     
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 1000), argsBuffer);

        if (Input.GetMouseButtonUp(0) && countMultiplier != cacheMultiplier)
        {
            countMultiplier = cacheMultiplier;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    private void OnDisable()
    {
        positionBuffer1?.Release();
        positionBuffer1 = null;
        positionBuffer2?.Release();
        positionBuffer2 = null;
    }
    private void UpdateBuffers()
    {
        positionBuffer1?.Release();
        positionBuffer2?.Release();

        positionBuffer1 = new ComputeBuffer(count, 16);

        positionBuffer2 = new ComputeBuffer(count, 16);


        var positions1 = new Vector4[count];

        var positions2 = new Vector4[count];
        var offset = Vector3.zero;
        var batchIndex = 0;
        var batch = 0;

        for(var i = 0; i < count; ++i)
        {

            var dir = Random.insideUnitSphere.normalized;

            positions1[i] = dir * Random.Range(10, 15) + offset;

            positions2[i] = dir * Random.Range(30, 50) + offset;

            positions1[i].w = Random.Range(-3f, 3f);
            positions2[i].w = batch;


            if (batchIndex++ == 250000)
            {
                batchIndex = 0;
                batch++;
                offset += new Vector3(90, 0, 0);
            }


        }


        positionBuffer1.SetData(positions1);
        positionBuffer2.SetData(positions2);

        material.SetBuffer("position_buffer_1", positionBuffer1);
        material.SetBuffer("position_buffer_2", positionBuffer2);

        material.SetColorArray("color_buffer", SceneTools.Instance.ColorArray);

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
    public void ApplyMultiplierUpdate(int val,bool applySliderChange = false)
    {
        sliderValueText.text = $"Multiplier: {val.ToString()}";
        cacheMultiplier = val;
        if(applySliderChange)
        {
            slider.value = val;
        }
    }

}
