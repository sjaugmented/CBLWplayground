using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RGBController : MonoBehaviour
{
    [SerializeField] GameObject horStackObj;
    [SerializeField] TextMeshPro xFloatText;
    [SerializeField] GameObject vertStackObj;
    [SerializeField] TextMeshPro yFloatText;
    [SerializeField] GameObject forStackObj;
    [SerializeField] TextMeshPro zFloatText;

    [SerializeField] [Range(0.2f, 0.6f)] float maxXAxisDist = 0.5f;
    [SerializeField] [Range(0.2f, 0.6f)] float maxYAxisDist = 0.3f;
    [SerializeField] [Range(0.2f, 0.6f)] float maxZAxisDist = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] float floatOffset = 0.05f;
    [SerializeField] Vector3 palmMidpointOffset;

    [Header("OSC controller")]
    [SerializeField] string xOSCMessage = "/xOSCfloat/";
    [SerializeField] string yOSCMessage = "/yOSCfloat/";
    [SerializeField] string zOSCMessage = "/zOSCfloat/";

    [Header("DMX controllers")]
    [SerializeField] int redDMX;
    [SerializeField] [Range(0, 255)] int redVal;
    [SerializeField] int greenDMX;
    [SerializeField] [Range(0, 255)] int greenVal;
    [SerializeField] int blueDMX;
    [SerializeField] [Range(0, 255)] int blueVal;


    HandTracking handTracking;
    DMXcontroller dmx;
    OSC osc;
    private float palmDist;
    private float indexMidDist;
    private int floatScale;
    private Vector3 midpointIndexes;
    private Vector3 masterOrbPos;


    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    void OnEnable()
    {
        dmx.SetAddress(50, 255);
        dmx.SetAddress(53, 255);
    }

    // Update is called once per frame
    void Update()
    {
        CalcHandPositions();

        if (handTracking.palmsIn)
        {
            float xOSCFloat;

            xOSCFloat = 1 - (indexMidDist - floatOffset) / (maxXAxisDist - floatOffset);
            if (indexMidDist > maxXAxisDist) xOSCFloat = 0;
            if (indexMidDist < floatOffset) xOSCFloat = 1;
            redVal = Mathf.RoundToInt(xOSCFloat * 255);

            SendOSCMessage(yOSCMessage, xOSCFloat);
            dmx.SetAddress(redDMX, redVal);

            horStackObj.SetActive(true);
            horStackObj.transform.position = midpointIndexes;
            xFloatText.text = xOSCFloat.ToString();
        }
        else horStackObj.SetActive(false);
        
        if (handTracking.verticalStack)
        {
            float yOSCFloat;

            yOSCFloat = 1 - (indexMidDist - floatOffset) / (maxYAxisDist - floatOffset);
            if (indexMidDist > maxYAxisDist) yOSCFloat = 0;
            if (indexMidDist < floatOffset) yOSCFloat = 1;
            redVal = Mathf.RoundToInt(yOSCFloat * 255);

            SendOSCMessage(yOSCMessage, yOSCFloat);
            dmx.SetAddress(greenDMX, greenVal);

            vertStackObj.SetActive(true);
            vertStackObj.transform.position = midpointIndexes;
            yFloatText.text = yOSCFloat.ToString();
        }
        else
        {
            vertStackObj.SetActive(false);
        }

        if (handTracking.forwardStack)
        {
            float zOSCFloat;

            zOSCFloat = 1 - (indexMidDist - floatOffset) / (maxZAxisDist - floatOffset);
            if (indexMidDist > maxZAxisDist) zOSCFloat = 0;
            if (indexMidDist < floatOffset) zOSCFloat = 1;
            redVal = Mathf.RoundToInt(zOSCFloat * 255);

            SendOSCMessage(zOSCMessage, zOSCFloat);
            dmx.SetAddress(blueDMX, blueVal);

            forStackObj.SetActive(true);
            forStackObj.transform.position = midpointIndexes;
            zFloatText.text = zOSCFloat.ToString();
        }
        else
        {
            forStackObj.SetActive(false);
        }
    }

    private void CalcHandPositions()
    {
        palmDist = Vector3.Distance(handTracking.rightPalm.Position, handTracking.leftPalm.Position);
        indexMidDist = Vector3.Distance(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position);
        midpointIndexes = Vector3.Lerp(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position, 0.5f);

        var midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
        /*rightStreamPos = Vector3.Lerp(handTracking.rtIndexTip.Position, handTracking.rtPinkyTip.Position, 0.5f);
        leftStreamPos = Vector3.Lerp(handTracking.ltIndexTip.Position, handTracking.ltPinkyTip.Position, 0.5f);*/
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
