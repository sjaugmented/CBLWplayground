using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamController : MonoBehaviour
{
    [SerializeField] int channelDMX = 0;
    [SerializeField] int valueDMX = 0;
    [SerializeField] string messageOSC = "/test/";
    [SerializeField] float valueOSC = 1f;
    [SerializeField] ParticleSystem parentStream;

    DMXcontroller dmx;
    OSC osc;
    
    // Start is called before the first frame update
    void Start()
    {
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();
    }

    // Update is called once per frame
    void Update()
    {
        var emitting = parentStream.emission;

        if (emitting.enabled == false) return;
        else
        {
            dmx.SetAddress(channelDMX, valueDMX);
            SendOSCMessage();
        }
    }

    private void SendOSCMessage()
    {
        OscMessage message = new OscMessage();
        message.address = messageOSC;
        message.values.Add(valueOSC);
        osc.Send(message);
        Debug.Log("sending OSC: " + message + valueOSC); //todo remove

    }
}
