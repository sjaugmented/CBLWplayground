using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RGBController : MonoBehaviour
{
    [SerializeField] GameObject vertStackText;
    [SerializeField] TextMeshPro yFloatText;
    [SerializeField] GameObject forStackText;
    [SerializeField] TextMeshPro zFloatText;

    [SerializeField] [Range(0.2f, 0.6f)] float maxXAxisDist = 0.5f;
    [SerializeField] float maxYAxisDist = 0.3f;
    [SerializeField] float maxZAxisDist = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] float palmDistOffset = 0.05f;
    [SerializeField] Vector3 palmMidpointOffset;

    [Header("OSC controller")]
    [SerializeField] string xOSCMessage = "/xOSCfloat/";
    [SerializeField] string yOSCMessage = "/yOSCfloat/";
    [SerializeField] string zOSCMessage = "/zOSCfloat/";

    [Header("DMX controllers")]
    public List<int> lightChannels;
    public List<int> lightValues;
    public List<int> fireChannels;
    public List<int> fireValues;
    public List<int> waterChannels;
    public List<int> waterValues;
    public List<int> iceChannels;
    public List<int> iceValues;

    OrbFingerTracker handTracking;
    DMXcontroller dmx;
    OSC osc;

    private float indexMidDist;
    private int floatScale;
    private Vector3 midpointIndexes;

    


    // Start is called before the first frame update
    void Start()
    {
        handTracking = FindObjectOfType<OrbFingerTracker>();
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();
    }

    // Update is called once per frame
    void Update()
    {
        if (handTracking.palmsIn)
        {

        }
        
        if (handTracking.verticalStack)
        {
            float yOSCFloat;

            yOSCFloat = 1 - indexMidDist / maxYAxisDist;
            if (indexMidDist > maxYAxisDist) floatScale = 0;

            SendOSCMessage(yOSCMessage, yOSCFloat);

            vertStackText.SetActive(true);
            vertStackText.transform.position = midpointIndexes;
            yFloatText.text = yOSCFloat.ToString();
        }
        else
        {
            vertStackText.SetActive(false);
        }

        if (handTracking.forwardStack)
        {
            float zOSCFloat;

            zOSCFloat = 1 - indexMidDist / maxZAxisDist;
            if (indexMidDist > maxZAxisDist) floatScale = 0;

            SendOSCMessage(zOSCMessage, zOSCFloat);

            forStackText.SetActive(true);
            forStackText.transform.position = midpointIndexes;
            zFloatText.text = zOSCFloat.ToString();
        }
        else
        {
            forStackText.SetActive(false);
        }
    }



    private void SendOSCMessage(string address, float value)
    {
        OscMessage message = new OscMessage();
        message.address = address;
        message.values.Add(value);
        osc.Send(message);
        Debug.Log("Sending OSC: " + address + " " + value); // todo remove
    }

    private void LiveDMX()
    {
        // todo
        
    }
}
