using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsDebugBuild : MonoBehaviour
{
    private void Start()
    {
        if (Debug.isDebugBuild)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}