using System.Collections;
using UnityEngine;

public class OrbHoverController : MonoBehaviour
{
    [SerializeField] GameObject babyOrbPrefab;

    [Header("General")]
    [SerializeField] float force = 5;
    [SerializeField] float rateOfDecel = 0.5f;
    [SerializeField] Vector3 spawnOffset = new Vector3(0, 0.1f, 0);
    [Tooltip("Metronome flash material")]
    [SerializeField] Material flashMaterial;

    [Header("OSC")]
    [Tooltip("OSC message to receive - triggers destruction/explosion of spell orb/particle")]
    [SerializeField] string OSCtoReceive = "/metronome/";

    public bool babySpawned = false;

    Material defaultMat;

    Rigidbody rigidBody;
    OSC osc;
    Renderer render;
    Collider thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        osc = FindObjectOfType<OSC>();

        thisCollider = GetComponent<SphereCollider>();
        thisCollider.enabled = true;

        osc.SetAddressHandler(OSCtoReceive, OnReceiveOSC);
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = transform.forward * force;
        force -= rateOfDecel;
        if (force < 0) force = 0;
    }

    public void SpawnBabyOrb()
    {
        if (!babySpawned) Instantiate(babyOrbPrefab, transform.position + spawnOffset, Quaternion.identity);
        else return;
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
