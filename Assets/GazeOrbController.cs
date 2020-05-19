using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeOrbController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Time before self-destruct")]
    [SerializeField] float force = 50;
    [Tooltip("Metronome flash material")]
    [SerializeField] Material flashMaterial;

    [Header("OSC")]
    [Tooltip("OSC message to receive - triggers destruction/explosion of spell orb/particle")]
    [SerializeField] string OSCtoReceive = "/metronome/";
    
    
    Material defaultMat;

    Rigidbody rigidBody;
    OSC osc;
    Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();

        osc.SetAddressHandler(OSCtoReceive, OnReceiveOSC);

        
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = new Vector3(0, 0, 1) * force;
        force--;
        if (force < 0) force = 0;
    }

    public void GazeSelected()
    {

    }

    void OnReceiveOSC(OscMessage message)
    {
        defaultMat = render.material;
        StartCoroutine("Metronome");

    }

    IEnumerator Metronome()
    {
        render.material = flashMaterial;
        yield return new WaitForSeconds(0.2f);
        render.material = defaultMat;
    }

    /*void FixedUpdate()
    {
        rigidBody.AddForce(transform.forward * force);
    }*/
}
