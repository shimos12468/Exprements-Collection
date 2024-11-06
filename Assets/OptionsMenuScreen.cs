using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuScreen : MonoBehaviour
{
    public bool rotate;
    public Transform optionsContainer;
    public Transform obj;

    public List<Mesh>filters= new List<Mesh>();


    void Start()
    {
        FreeRotation();
    }

    // Update is called once per frame
    void Update()
    {
        RotateOptions();

    }

    private void FreeRotation()
    {
        obj.DORotate(new Vector3(360, 0, 360), 5, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetRelative()
            .SetEase(Ease.Linear);
    }

    private void RotateOptions()
    {
        if (rotate)
        {
            rotate = false;

            Vector3 currentRotation = optionsContainer.rotation.eulerAngles;
            currentRotation.z += 90;
            optionsContainer.DORotate(currentRotation, 0.2f).SetEase(Ease.InExpo).OnComplete(Completed);
            index = index + 1;

        }
    }
    public int index = 0;
    private void Completed()
    {
        if(index>=filters.Count)index= 0;
        print("FSDAFSADF");
        obj.GetComponent<MeshFilter>().sharedMesh = filters[index];

        

    }
}
