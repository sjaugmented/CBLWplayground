using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterOrbRotater : MonoBehaviour
{
    public float xRotation = 1f;
    public float yRotation = 1f;
    public float zRotation = 1f;

    public bool scalerActive;

    SpellManager spellManager;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<SpellManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spellManager.fromOrbScaler) transform.Rotate(xRotation, yRotation, zRotation);
        else transform.forward = Camera.main.transform.forward;
    }
}
