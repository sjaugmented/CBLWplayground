using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RGBController : MonoBehaviour
{
    [SerializeField] GameObject xFloatObj;
    [SerializeField] TextMeshPro xFloatText;
    [SerializeField] GameObject yFloatObj;
    [SerializeField] TextMeshPro yFloatText;
    [SerializeField] GameObject zFloatObj;
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

    [Header("DMX values")]
    public int redVal = 0;
    public int greenVal = 0;
    public int blueVal = 0;


    HandTracking handTracking;
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
        dmx = FindObjectOfType<DMXcontroller>();
        dmxChan = FindObjectOfType<DMXChannels>();
        osc = FindObjectOfType<OSC>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    void OnEnable()
    {
        dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);
    }

    // Update is called once per frame
    void Update()
    {
        CalcHandPositions();

        // red
        if (handTracking.palmsOpposed && handTracking.staffCamUp90)
        {
            currentMode = RGB.red;
            float xOSCFloat;

            xOSCFloat = 1 - (indexMidDist - floatOffset) / (maxXAxisDist - floatOffset);
            if (indexMidDist > maxXAxisDist) xOSCFloat = 0;
            if (indexMidDist < floatOffset) xOSCFloat = 1;
            redVal = Mathf.RoundToInt(xOSCFloat * 255);
            var redText = Mathf.RoundToInt(redVal / 255 * 100);

            xFloatObj.SetActive(true);
            xFloatObj.transform.position = midpointIndexes;

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                SendOSCMessage(xOSCMessage, xOSCFloat);
                dmx.SetAddress(dmxChan.SkyPanel1[redChan], redVal);
                dmx.SetAddress(dmxChan.SkyPanel2[redChan], redVal);
                xFloatText.text = redText + "%".ToString();
            }
        }
        else xFloatObj.SetActive(false);
        
        // green
        if (handTracking.palmsOpposed && handTracking.staffCamUp45)
        {
            currentMode = RGB.green;
            float yOSCFloat;

            yOSCFloat = 1 - (indexMidDist - floatOffset) / (maxXAxisDist - floatOffset);
            if (indexMidDist > maxXAxisDist) yOSCFloat = 0;
            if (indexMidDist < floatOffset) yOSCFloat = 1;
            greenVal = Mathf.RoundToInt(yOSCFloat * 255);
            var greenText = Mathf.RoundToInt((greenVal / 255) * 100);

            yFloatObj.SetActive(true);
            yFloatObj.transform.position = midpointIndexes;

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                SendOSCMessage(yOSCMessage, yOSCFloat);
                dmx.SetAddress(dmxChan.SkyPanel1[greenChan], greenVal);
                dmx.SetAddress(dmxChan.SkyPanel2[greenChan], greenVal);
                yFloatText.text = greenText + "%".ToString();
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
            float zOSCFloat;

            zOSCFloat = 1 - (indexMidDist - floatOffset) / (maxXAxisDist - floatOffset);
            if (indexMidDist > maxXAxisDist) zOSCFloat = 0;
            if (indexMidDist < floatOffset) zOSCFloat = 1;
            blueVal = Mathf.RoundToInt(zOSCFloat * 255);
            var blueText = Mathf.RoundToInt((blueVal / 255) * 100);

            zFloatObj.SetActive(true);
            zFloatObj.transform.position = midpointIndexes;

            if (handTracking.rightOpen && handTracking.leftOpen)
            {
                SendOSCMessage(zOSCMessage, zOSCFloat);
                dmx.SetAddress(dmxChan.SkyPanel1[blueChan], blueVal);
                dmx.SetAddress(dmxChan.SkyPanel2[blueChan], blueVal);
                zFloatText.text = blueText + "%".ToString();
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
