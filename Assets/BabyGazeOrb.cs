using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyGazeOrb : MonoBehaviour
{
    [SerializeField] string messageOSC = "babyGaze";
    [SerializeField] float valueOSC = 1;
    [SerializeField] GameObject explosionFX;
    [SerializeField] float lifeSpan = 2;

    public float timer;

    OSC osc;
    EyeTrackingTarget gaze;
    GazeOrbController gazeOrb;
    
    // Start is called before the first frame update
    void Start()
    {
        osc = FindObjectOfType<OSC>();
        gaze = GetComponent<EyeTrackingTarget>();
        gazeOrb = FindObjectOfType<GazeOrbController>();

        timer = lifeSpan;
        gazeOrb.babySpawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer < 0)
        {
            Destroy(gameObject);
        }

        if (gaze.IsLookedAt)
        {
            timer = lifeSpan;
        }
    }

    public void GazeSelected()
    {
        SendOSC();
        StartCoroutine("ExplodeOrb");
    }

    private void SendOSC()
    {
        OscMessage message = new OscMessage();
        message.address = messageOSC;
        message.values.Add(valueOSC);
        osc.Send(message);
    }

    IEnumerator ExplodeOrb()
    {
        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(explosion);
        gazeOrb.babySpawned = false;
        Destroy(gameObject);
    }
}
