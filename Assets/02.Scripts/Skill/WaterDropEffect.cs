using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropEffect : MonoBehaviour
{
    bool isActivate = false;

    private void OnEnable()
    {
        isActivate = true;
    }
    private void Update()
    {
        if (isActivate)
        {
            transform.position += Vector3.up * 0.5f * Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        isActivate = false;
    }
}
