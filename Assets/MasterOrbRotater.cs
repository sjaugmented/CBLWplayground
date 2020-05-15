using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterOrbRotater : MonoBehaviour
{
    public float xRotation = 1f;
    public float yRotation = 1f;
    public float zRotation = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(xRotation, yRotation, zRotation);
    }
}
