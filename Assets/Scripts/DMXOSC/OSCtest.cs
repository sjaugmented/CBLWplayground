using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCtest : MonoBehaviour
{
    OSC osc;

    [SerializeField] string OSCmessage = "/fart";
    [SerializeField] int messageValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        osc = FindObjectOfType<OSC>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendOSCMessage()
    {
        OscMessage message = new OscMessage();
        message.address = OSCmessage;
        message.values.Add(messageValue);
        osc.Send(message);
    }
}
