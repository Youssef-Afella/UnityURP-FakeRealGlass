using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotate;

    private void Start()
    {
        Application.targetFrameRate = int.MaxValue;
    }
    void Update()
    {
        transform.Rotate(rotate * Time.deltaTime, Space.World);
    }
}
