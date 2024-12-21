using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCreature : MonoBehaviour
{
    public List<Renderer> erodeObject;

    public float erodeRate = 0.03f;
    public float erodeRefreshRate = 0.1f;
    public float erodeDelay = 1.25f;
    public bool finished = false;
    public GameObject spawnSkin, reguralSkin;
    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        if (erodeObject.Count > 0)
        {
            print("effect");
            for (int i = 0; i < erodeObject.Count; i++)
            {

                StartCoroutine(ErodeObject(i));
            }

        }
    }

    private void Update()
    {
        if (finished)
        {
            spawnSkin.SetActive(false);
            reguralSkin.SetActive(true);
        }
    }

    IEnumerator ErodeObject(int index)
    {
        yield return new WaitForSeconds(1);

        float t = 0;
        while (t < 1)
        {
            t += erodeRate;
            foreach (var mat in erodeObject[index].materials)
            {
                mat.SetFloat("_Errosion", 1f - t);
            }

            yield return new WaitForSeconds(erodeRefreshRate);

        }
        finished = true;

    }


}