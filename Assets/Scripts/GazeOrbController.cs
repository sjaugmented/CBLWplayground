﻿using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeOrbController : MonoBehaviour
{
    [SerializeField] string messageOSC = "babyGaze";
    [SerializeField] float valueOSC = 1;
    [SerializeField] GameObject explosionFX;
    [SerializeField] float lifeSpan = 2;

    [Header("DMX controller")]
    List<int> dmxChannels;
    List<int> dmxValues;

    public float timer;
    bool OSCsent = false;
    bool DMXsent = false;
    bool exploded = false;

    OSC osc;
    DMXcontroller dmx;
    EyeTrackingTarget gaze;
    OrbHoverController hoverParent;
    
    // Start is called before the first frame update
    void Start()
    {
        osc = FindObjectOfType<OSC>();
        dmx = FindObjectOfType<DMXcontroller>();
        gaze = GetComponent<EyeTrackingTarget>();

        OrbHoverController[] hoverOrbs = FindObjectsOfType<OrbHoverController>();

        foreach (OrbHoverController orb in hoverOrbs)
        {
            if (orb.CompareTag(gameObject.tag)) hoverParent = orb;
        }

        timer = lifeSpan;
        hoverParent.babySpawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer < 0)
        {
            hoverParent.babySpawned = false;
            Destroy(gameObject);
        }

        if (gaze.IsLookedAt)
        {
            timer = lifeSpan;
        }
    }

    public void GazeSelected()
    {
        if (!OSCsent) SendOSC();
        if (!DMXsent) SendDMX();
        StartCoroutine("ExplodeOrb");
    }

    private void SendOSC()
    {
        OSCsent = true;
        OscMessage message = new OscMessage();
        message.address = messageOSC;
        message.values.Add(valueOSC);
        osc.Send(message);
    }

    private void SendDMX()
    {
        if (dmxChannels.Count == 0) return;

        if (dmxChannels.Count == dmxValues.Count)
        {
            for (int i = 0; i < dmxChannels.Count; i++)
            {
                dmx.SetAddress(dmxChannels[i], dmxValues[i]);
            }
        }
        else
        {
            Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }
    }

    IEnumerator ExplodeOrb()
    {
        if (!exploded)
        {
            GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);
            exploded = true;

            yield return new WaitForSeconds(0.2f);

            Renderer render = GetComponent<Renderer>();
            
            render.enabled = false;

            yield return new WaitForSeconds(1);

            Destroy(explosion);
            hoverParent.babySpawned = false;
            Destroy(gameObject);
        }
        else yield break;
        
    }
}
