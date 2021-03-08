using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    private GameObject[] cameras;

    private void Start()
    {
        cameras= GameObject.FindGameObjectsWithTag("MainCamera");
        EnableCamera(0);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EnableCamera(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnableCamera(1); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EnableCamera(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EnableCamera(3);
        }

    }

    private void EnableCamera(int n)
    {
        foreach(GameObject cam in cameras){
            cam.SetActive(false);
        }
        cameras[n].SetActive(true);
    }
}