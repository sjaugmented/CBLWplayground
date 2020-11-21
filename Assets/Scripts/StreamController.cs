using UnityEngine;
using static UnityEngine.ParticleSystem;

public class StreamController : MonoBehaviour
{
    [SerializeField] int channelDMX = 0;
    [SerializeField] int valueDMX = 0;
    [SerializeField] string messageOSC = "/test/";
    [SerializeField] float valueOSC = 1f;
    [SerializeField] ParticleSystem parentStream;

    DMXcontroller dmx;
    OSC osc;

    EmissionModule emitting;

    // Start is called before the first frame update
    void Start()
    {
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();

        emitting = parentStream.emission;
        emitting.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
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
