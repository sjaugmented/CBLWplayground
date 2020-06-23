using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RGBController : MonoBehaviour
{
    [SerializeField] GameObject xFloatObj;
    [SerializeField] TextMeshPro xFloatText;
    [SerializeField] GameObject redLiveBox;
    [SerializeField] GameObject yFloatObj;
    [SerializeField] TextMeshPro yFloatText;
    [SerializeField] GameObject greenLiveBox;
    [SerializeField] GameObject zFloatObj;
    [SerializeField] TextMeshPro zFloatText;
    [SerializeField] GameObject blueLiveBox;

    [SerializeField] [Range(0.2f, 0.6f)] float maxFloatDist = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] float floatOffset = 0.05f;
    [SerializeField] Vector3 palmMidpointOffset;

    [Header("OSC controller")]
    [SerializeField] string xOSCMessage = "/xOSCfloat/";
    [SerializeField] string yOSCMessage = "/yOSCfloat/";
    [SerializeField] string zOSCMessage = "/zOSCfloat/";

    [Header("DMX values")]
    public int redVal = 0;
    public int greenVal = 0;
    public int blueVal = 0;


    HandTracking handTracking;
    PrecisionController precision;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    OSC osc;
    private float palmDist;
    private float indexMidDist;
    private int floatScale;
    private Vector3 midpointIndexes;
    private Vector3 masterOrbPos;

    int dimmerChan = 0;
    int kelvinChan = 1;
    int xOverChan = 3;
    int redChan = 4;
    int greenChan = 5;
    int blueChan = 6;
    int whiteChan = 7;

    public enum RGB { red, green, blue };
    public RGB currentMode = RGB.red;


    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();
        precision = FindObjectOfType<PrecisionController>();
        dmx = FindObjectOfType<DMXcontroller>();
        dmxChan = FindObjectOfType<DMXChannels>();
        osc = FindObjectOfType<OSC>();
        //DisableRings();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    void OnEnable()
    {
        /*dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);*/
    }

    void OnDisable()
    {
        //DisableRings();
        /*dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 0);*/
    }

    bool live = false;

    // Update is called once per frame
    void Update()
    {
        CalcHandPositions();

        // red
        if (handTracking.palmsOpposed && handTracking.staffCamUp90)
        {
            currentMode = RGB.red;
            float xFloat;

            xFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
            if (indexMidDist > maxFloatDist) xFloat = 0;
            if (indexMidDist < floatOffset) xFloat = 1;
            var redText = Mathf.RoundToInt(xFloat * 100);

            xFloatObj.SetActive(true);
            xFloatObj.transform.position = midpointIndexes;
            xFloatObj.transform.rotation = Camera.main.transform.rotation;
            xFloatText.text = redText + "%".ToString();

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                live = true;
                redLiveBox.SetActive(true);
                redVal = Mathf.RoundToInt(xFloat * 255);
                SendOSCMessage(xOSCMessage, xFloat);

                if (precision.rgbControl == PrecisionController.Lights.SkyPanel1)
                {
                    dmx.SetAddress(dmxChan.SkyPanel1[redChan], redVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.SkyPanel2)
                {
                    dmx.SetAddress(dmxChan.SkyPanel2[redChan], redVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.none)
                {
                    return;
                }
            }
            else
            {
                live = false;
                redLiveBox.SetActive(false);
            }
        }
        else xFloatObj.SetActive(false);

        // green
        if (handTracking.palmsOpposed && handTracking.staffCamUp45)
        {
            currentMode = RGB.green;
            float yFloat;

            yFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
            if (indexMidDist > maxFloatDist) yFloat = 0;
            if (indexMidDist < floatOffset) yFloat = 1;
            var greenText = Mathf.RoundToInt(yFloat * 100);

            yFloatObj.SetActive(true);
            yFloatObj.transform.position = midpointIndexes;
            yFloatObj.transform.rotation = Camera.main.transform.rotation;
            yFloatText.text = greenText + "%".ToString();

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                live = true;
                greenLiveBox.SetActive(true);
                greenVal = Mathf.RoundToInt(yFloat * 255);
                SendOSCMessage(yOSCMessage, yFloat);
                if (precision.rgbControl == PrecisionController.Lights.SkyPanel1)
                {
                    dmx.SetAddress(dmxChan.SkyPanel1[greenChan], greenVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.SkyPanel2)
                {
                    dmx.SetAddress(dmxChan.SkyPanel2[greenChan], greenVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.none)
                {
                    return;
                }
            }
            else
            {
                live = false;
                greenLiveBox.SetActive(false);
            }
        }
        else
        {
            yFloatObj.SetActive(false);
        }

        // blue
        if (handTracking.palmsOpposed && handTracking.staffCamUp135)
        {
            currentMode = RGB.blue;
            float zFloat;

            zFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
            if (indexMidDist > maxFloatDist) zFloat = 0;
            if (indexMidDist < floatOffset) zFloat = 1;
            var blueText = Mathf.RoundToInt(zFloat * 100);

            zFloatObj.SetActive(true);
            zFloatObj.transform.position = midpointIndexes;
            zFloatObj.transform.rotation = Camera.main.transform.rotation;
            zFloatText.text = blueText + "%".ToString();

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                live = true;
                blueLiveBox.SetActive(true);
                blueVal = Mathf.RoundToInt(zFloat * 255);
                SendOSCMessage(zOSCMessage, zFloat);
                if (precision.rgbControl == PrecisionController.Lights.SkyPanel1)
                {
                    dmx.SetAddress(dmxChan.SkyPanel1[blueChan], blueVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.SkyPanel2)
                {
                    dmx.SetAddress(dmxChan.SkyPanel2[blueChan], blueVal);
                }
                if (precision.rgbControl == PrecisionController.Lights.none)
                {
                    return;
                }
            }
            else
            {
                live = false;
                blueLiveBox.SetActive(false);
            }
        }
        else
        {
            zFloatObj.SetActive(false);
        }
    }

    private void CalcHandPositions()
    {
        palmDist = Vector3.Distance(handTracking.rightPalm.Position, handTracking.leftPalm.Position);
        indexMidDist = Vector3.Distance(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position);
        midpointIndexes = Vector3.Lerp(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position, 0.5f);

        var midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
    }


    private void SendOSCMessage(string address, float value)
    {
        OscMessage message = new OscMessage();
        message.address = address;
        message.values.Add(value);
        osc.Send(message);
        Debug.Log("Sending OSC: " + address + " " + value); // todo remove
    }
}
