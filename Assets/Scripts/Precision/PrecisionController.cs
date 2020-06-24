using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [Header("Hand HUDs")]
    [SerializeField] GameObject rightHandHUD;
    [SerializeField] GameObject leftHandHUD;
    [SerializeField] TextMeshPro rightHandText;
    [SerializeField] TextMeshPro leftHandText;
    [SerializeField] Material skyPanel1Mat;
    [SerializeField] Material skyPanel2Mat;

    [Header("Float GameObjects")]
    [SerializeField] GameObject rightDimmerObj;
    [SerializeField] GameObject rightKelvinObj;
    [SerializeField] GameObject leftDimmerObj;
    [SerializeField] GameObject leftKelvinObj;
    [SerializeField] Transform rightDimmerMin;
    [SerializeField] Transform rightKelvinMin;
    [SerializeField] Transform rightKelvinMax;
    [SerializeField] Transform leftDimmerMin;
    [SerializeField] Transform leftKelvinMin;
    [SerializeField] Transform leftKelvinMax;

    [SerializeField] bool tetherOverride = false;
    bool hasMadeRightFist = false;
    bool rightTether = false;
    bool rightDimmer = false;
    bool rightKelvin = false;

    public enum Mode { kelvin, rgb };
    public Mode currMode = Mode.rgb;

    public enum Lights { none, SkyPanel1, SkyPanel2, DJ, reset };
    public Lights gazeLight = Lights.none;
    public Lights rightControl = Lights.none;
    public Lights leftControl = Lights.none;
    public Lights rgbControl = Lights.none;



    HandTracking handTracking;
    Director director;
    RGBController rgb;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    OSC osc;

    GameObject rgbComponents;

    bool hasMadeLeftFist = false;
    bool leftTether = false;
    bool leftDimmer = false;
    bool leftKelvin = false;

    int dimmerChan = 0;
    int kelvinChan = 1;
    int xOverChan = 3;
    int redChan = 4;
    int greenChan = 5;
    int blueChan = 6;
    int whiteChan = 7;

    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();
        director = FindObjectOfType<Director>();
        rgb = FindObjectOfType<RGBController>();
        dmx = FindObjectOfType<DMXcontroller>();
        dmxChan = FindObjectOfType<DMXChannels>();
        osc = FindObjectOfType<OSC>();

        rgbComponents = FindObjectOfType<RGBController>().gameObject;

        rightDimmerObj.SetActive(false);
        rightKelvinObj.SetActive(false);
        leftDimmerObj.SetActive(false);
        leftKelvinObj.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
   
    }

    void OnEnable()
    {
        dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 0);
    }

    void OnDisable()
    {
        rightKelvinObj.SetActive(false);
        leftKelvinObj.SetActive(false);
    }

    bool kelvinMode = true;
    public bool rgbActive = false;

    // Update is called once per frame
    void Update()
    {
        if (director.readGestures)
        {
            #region Dimmer/Kelvin controls
            if (handTracking.rightThumbsUp || handTracking.leftThumbsUp)
            {
                currMode = Mode.kelvin;
                rgb.enabled = false;
                rgbComponents.SetActive(false);
                rgbActive = false;
                if (!kelvinMode)
                {
                    dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 0);
                    dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 0);
                    dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 0);
                    dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 0);
                    kelvinMode = true;
                }
            }
            else
            {
                currMode = Mode.rgb;
            }

            // right hand control
            if (handTracking.rightThumbsUp/* && !hasMadeRightFist*/)
            {
                ToggleRightTether();
                //hasMadeRightFist = true;
            }
            else
            {
                rightTether = false;
            }

            //if (!handTracking.rightFist) hasMadeRightFist = false;

            if (rightTether)
            {
                if (handTracking.rightThumbsUp && handTracking.rtPalmUpFloorUp >= 0 && handTracking.rtPalmUpFloorUp < 50) rightDimmer = true;
                else rightDimmer = false;

                if (handTracking.rightThumbsUp && handTracking.rtPalmUpFloorUp >= 70 && handTracking.rtPalmUpFloorUp <= 135) rightKelvin = true;
                else rightKelvin = false;
            }

            // left hand control
            if (handTracking.leftThumbsUp /*&& !hasMadeLeftFist*/)
            {
                ToggleLeftTether();
                //hasMadeLeftFist = true;
            }
            else
            {
                leftTether = false;
            }

            //if (!handTracking.leftFist) hasMadeLeftFist = false;

            if (leftTether)
            {
                if (/*handTracking.leftOpen && */handTracking.ltPalmUpFloorUp >= 0 && handTracking.ltPalmUpFloorUp < 50) leftDimmer = true;
                else leftDimmer = false;

                if (/*handTracking.leftOpen && */handTracking.ltPalmUpFloorUp >= 70 && handTracking.ltPalmUpFloorUp <= 135) leftKelvin = true;
                else leftKelvin = false;
            }
            #endregion

            if (!handTracking.rightThumbsUp && !handTracking.leftThumbsUp && handTracking.twoHands && handTracking.palmsOpposed)
            {
                rgb.enabled = true;
                rgbComponents.SetActive(true);
                kelvinMode = false;
                if (!rgbActive)
                {
                    dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
                    dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
                    dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
                    dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);
                    rgbActive = true;
                }

            }
            else
            {
                rgb.enabled = false;
                rgbComponents.SetActive(false);
                rgbActive = false;
            }

            ProcessRightHandControls();
            ProcessLeftHandControls();
        }
        else return;

        ProcessHandRings();
        ProcessHUD();
    }

    private void ToggleRightTether()
    {
        /*if (!rightTether)
        {
            if (gazeLight == Lights.SkyPanel1) rightControl = Lights.SkyPanel1;
            else if (gazeLight == Lights.SkyPanel2) rightControl = Lights.SkyPanel2;
            else if (rightControl == Lights.none)
            {
                if (gazeLight == Lights.SkyPanel1)
                {
                    rightControl = Lights.SkyPanel1;
                }

                else if (gazeLight == Lights.SkyPanel2)
                {
                    rightControl = Lights.SkyPanel2;
                }

                else return;
            }

            rightTether = true;

        }
        else
        {
            if (gazeLight == Lights.reset) rightControl = Lights.none;
            rightTether = false;
        }*/

        if (gazeLight == Lights.SkyPanel1) rightControl = Lights.SkyPanel1;
        else if (gazeLight == Lights.SkyPanel2) rightControl = Lights.SkyPanel2;
        else if (rightControl == Lights.none)
        {
            if (gazeLight == Lights.SkyPanel1)
            {
                rightControl = Lights.SkyPanel1;
            }

            else if (gazeLight == Lights.SkyPanel2)
            {
                rightControl = Lights.SkyPanel2;
            }

            else return;
        }

        rightTether = true;
    }

    private void ToggleLeftTether()
    {
        /*if (!leftTether)
        {
            if (gazeLight == Lights.SkyPanel1) leftControl = Lights.SkyPanel1;
            else if (gazeLight == Lights.SkyPanel2) leftControl = Lights.SkyPanel2;
            else if (leftControl == Lights.none)
            {
                if (gazeLight == Lights.SkyPanel1)
                {
                    leftControl = Lights.SkyPanel1;
                }

                else if (gazeLight == Lights.SkyPanel2)
                {
                    leftControl = Lights.SkyPanel2;
                }

                else return;
            }

            leftTether = true;

        }
        else
        {
            if (gazeLight == Lights.reset) leftControl = Lights.none;
            leftTether = false;
        }*/

        if (gazeLight == Lights.SkyPanel1) leftControl = Lights.SkyPanel1;
        else if (gazeLight == Lights.SkyPanel2) leftControl = Lights.SkyPanel2;
        else if (leftControl == Lights.none)
        {
            if (gazeLight == Lights.SkyPanel1)
            {
                leftControl = Lights.SkyPanel1;
            }

            else if (gazeLight == Lights.SkyPanel2)
            {
                leftControl = Lights.SkyPanel2;
            }

            else return;
        }

        leftTether = true;
    }


    #region right globals
    float rightDimmerFloat;
    float rightDimmerYPos;
    bool rightDimmerYLocked = false;

    float rightKelvinFloat;
    bool rightKelvinLocked = false;
    Vector3 rightKelvinPos;
    #endregion

    private void ProcessRightHandControls()
    {
        if (rightDimmer)
        {
            rightKelvinLocked = false;
            
            // activate right hand float
            rightDimmerObj.SetActive(true);
            rightKelvinObj.SetActive(false);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!rightDimmerYLocked)
            {
                rightDimmerYPos = handTracking.rightPalm.Position.y;
                rightDimmerYLocked = true;
            }

            rightDimmerObj.transform.position = new Vector3(handTracking.rightPalm.Position.x, rightDimmerYPos, handTracking.rightPalm.Position.z);

            // determine float using pose.position.y
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightDimmerMin.position, handTracking.rightPalm.Position);


            rightDimmerFloat = handDistToMin / maxDistance;
            if (rightDimmerFloat > 1) rightDimmerFloat = 1;
            if (handTracking.rightPalm.Position.y < rightDimmerObj.transform.position.y - 0.125) rightDimmerFloat = 0;
            

            // display in HUD
            rightHandText.text = Mathf.RoundToInt(rightDimmerFloat * 100) + "%".ToString();

            // convert float to DMX
            int dimmerVal = Mathf.RoundToInt(rightDimmerFloat * 255);

            // send DMX & OSC
            if (rightControl == Lights.SkyPanel1)
            {
                dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], dimmerVal);
                SendOSC("/SkyPanel1Dimmer/", rightDimmerFloat);
            }
            else if (rightControl == Lights.SkyPanel2)
            {
                dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], dimmerVal);
                SendOSC("/SkyPanel2Dimmer/", rightDimmerFloat);
            }

        }

        else if (rightKelvin)
        {
            rightDimmerYLocked = false;

            // set float.position.x to pose.position.x and store in memory - float.position.y/z tracks to pose.position.y/z
            if (!rightKelvinLocked)
            {
                rightKelvinPos = handTracking.rightPalm.Position;
                rightKelvinLocked = true;
            }

            // activate right hand float, position, and rotate
            rightDimmerObj.SetActive(false);
            rightKelvinObj.SetActive(true);
            rightKelvinObj.transform.position = rightKelvinPos;


            // determine float using pose.position.x
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightKelvinMin.position, handTracking.rightPalm.Position);

            rightKelvinFloat = handDistToMin / maxDistance;
            if (rightKelvinFloat > 1) rightKelvinFloat = 1;
            if (Vector3.Distance(handTracking.rightPalm.Position, rightKelvinMax.position) > maxDistance) rightKelvinFloat = 0;

            // display in HUD
            rightHandText.text = Mathf.RoundToInt(Mathf.Clamp(rightKelvinFloat * 10000, 2800, 10000)) + "k".ToString();

            // convert float to DMX
            int kelvinVal = Mathf.RoundToInt(rightKelvinFloat * 255);

            // send DMX & OSC
            if (rightControl == Lights.SkyPanel1)
            {
                dmx.SetAddress(dmxChan.SkyPanel1[kelvinChan], kelvinVal);
                SendOSC("/SkyPanel1kelvin/", rightKelvinFloat);
            }
            else if (rightControl == Lights.SkyPanel2)
            {
                dmx.SetAddress(dmxChan.SkyPanel2[kelvinChan], kelvinVal);
                SendOSC("/SkyPanel2Kelvin/", rightKelvinFloat);
            }
        }
        else
        {
            rightDimmerObj.SetActive(false);
            rightKelvinObj.SetActive(false);
        }
    }


    #region left globals
    float leftDimmerFloat;
    float leftDimmerYPos;
    bool leftDimmerYLocked = false;

    float leftKelvinFloat;
    bool leftKelvinLocked = false;
    Vector3 leftKelvinPos;
    #endregion

    private void ProcessLeftHandControls()
    {
        if (leftDimmer)
        {
            leftKelvinLocked = false;

            // activate right hand float
            leftDimmerObj.SetActive(true);
            leftKelvinObj.SetActive(false);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!leftDimmerYLocked)
            {
                leftDimmerYPos = handTracking.leftPalm.Position.y;
                leftDimmerYLocked = true;
            }

            leftDimmerObj.transform.position = new Vector3(handTracking.leftPalm.Position.x, leftDimmerYPos, handTracking.leftPalm.Position.z);

            // determine float using pose.position.y
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(leftDimmerMin.position, handTracking.leftPalm.Position);


            leftDimmerFloat = handDistToMin / maxDistance;
            if (leftDimmerFloat > 1) leftDimmerFloat = 1;
            if (handTracking.leftPalm.Position.y < leftDimmerObj.transform.position.y - 0.125) leftDimmerFloat = 0;

            // display in HUD
            leftHandText.text = Mathf.RoundToInt(leftDimmerFloat * 100) + "%".ToString();

            // convert float to DMX
            int dimmerVal = Mathf.RoundToInt(leftDimmerFloat * 255);

            // send DMX & OSC
            if (leftControl == Lights.SkyPanel1)
            {
                dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], dimmerVal);
                SendOSC("/SkyPanel1Dimmer/", rightDimmerFloat);
            }
            else if (leftControl == Lights.SkyPanel2)
            {
                dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], dimmerVal);
                SendOSC("/SkyPanel2Dimmer/", rightDimmerFloat);
            }
            
        }

        else if (leftKelvin)
        {
            leftDimmerYLocked = false;

            // activate right hand float
            leftDimmerObj.SetActive(false);
            leftKelvinObj.SetActive(true);

            // set float.positionto pose.position and store in memory
            if (!leftKelvinLocked)
            {
                leftKelvinPos = handTracking.leftPalm.Position;
                leftKelvinLocked = true;
            }

            leftKelvinObj.transform.position = leftKelvinPos;

            // determine float using pose.position.x
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(leftKelvinMin.position, handTracking.leftPalm.Position);

            leftKelvinFloat = handDistToMin / maxDistance;
            if (leftKelvinFloat > 1) leftKelvinFloat = 1;
            if (Vector3.Distance(handTracking.leftPalm.Position, leftKelvinMax.position) > maxDistance) leftKelvinFloat = 0;

            // display in HUD
            leftHandText.text = Mathf.RoundToInt(Mathf.Clamp(leftKelvinFloat * 10000, 2800, 10000)) + "k".ToString();

            // convert float to DMX
            int kelvinVal = Mathf.RoundToInt(leftKelvinFloat * 255);

            // send DMX & OSC
            if (leftControl == Lights.SkyPanel1)
            {
                dmx.SetAddress(dmxChan.SkyPanel1[kelvinChan], kelvinVal);
                SendOSC("/SkyPanel1kelvin/", rightKelvinFloat);
            }
            else if (leftControl == Lights.SkyPanel2)
            {
                dmx.SetAddress(dmxChan.SkyPanel2[kelvinChan], kelvinVal);
                SendOSC("/SkyPanel2Kelvin/", rightKelvinFloat);
            }
            
        }
        else
        {
            leftDimmerObj.SetActive(false);
            leftKelvinObj.SetActive(false);
        }
    }

    private void SendOSC(string messageOSC, float val)
    {
        OscMessage message = new OscMessage();
        message.address = messageOSC;
        message.values.Add(val);
        osc.Send(message);
        Debug.Log("OSC sending: " + message); // remove
    }

    #region Button/Gaze hookups
    public void GazeAtSkyPanel1()
    {
        gazeLight = Lights.SkyPanel1;
        rgbControl = Lights.SkyPanel1;
    }

    public void GazeAtSkyPanel2()
    {
        gazeLight = Lights.SkyPanel2;
        rgbControl = Lights.SkyPanel2;
    }

    public void NoGaze()
    {
        gazeLight = Lights.none;
    }

    public void ResetHand()
    {
        gazeLight = Lights.reset;
    }
    #endregion

    ////////// RGB /////////////
    [Header("Hand Rings")]
    [SerializeField] GameObject rightRingParent;
    [SerializeField] GameObject leftRingParent;
    [SerializeField] Renderer rightRing;
    [SerializeField] Renderer leftRing;
    [SerializeField] Material redLive;
    [SerializeField] Material greenLive;
    [SerializeField] Material blueLive;
    [SerializeField] Material redStealth;
    [SerializeField] Material greenStealth;
    [SerializeField] Material blueStealth;

    public enum RGB { red, green, blue };
    public RGB currColor = RGB.red;

    bool live = false;

    private void ProcessHandRings()
    {
        if (currMode == Mode.rgb)
        {
            // right hand
            if (handTracking.rightHand)
            {
                rightRingParent.SetActive(true);

                if (currColor == RGB.red)
                {
                    if (!live) rightRing.material = redStealth;
                    else rightRing.material = redLive;

                }
                if (currColor == RGB.green)
                {
                    if (!live) rightRing.material = greenStealth;
                    else rightRing.material = greenLive;
                }
                if (currColor == RGB.blue)
                {
                    if (!live) rightRing.material = blueStealth;
                    else rightRing.material = blueLive;
                }
            }
            else rightRingParent.SetActive(false);

            // left hand
            if (handTracking.leftHand)
            {
                leftRingParent.SetActive(true);
                if (currColor == RGB.red)
                {
                    if (!live) leftRing.material = redStealth;
                    else leftRing.material = redLive;

                }
                if (currColor == RGB.green)
                {
                    if (!live) leftRing.material = greenStealth;
                    else leftRing.material = greenLive;
                }
                if (currColor == RGB.blue)
                {
                    if (!live) leftRing.material = blueStealth;
                    else leftRing.material = blueLive;
                }
            }
            else leftRingParent.SetActive(false);
        }
        else
        {
            rightRingParent.SetActive(false);
            leftRingParent.SetActive(false);
        }
    }

    private void ProcessHUD()
    {
        // right hand
        if (handTracking.rightHand)
        {
            if (rightTether)
            {
                if (rightControl == Lights.SkyPanel1)
                {
                    rightHandHUD.SetActive(true);
                    rightHandHUD.GetComponentInChildren<Renderer>().material = skyPanel1Mat;
                }
                else if (rightControl == Lights.SkyPanel2)
                {
                    rightHandHUD.SetActive(true);
                    rightHandHUD.GetComponentInChildren<Renderer>().material = skyPanel2Mat;
                }
                else if (rightControl == Lights.none)
                {
                    rightHandHUD.SetActive(false);
                }
            }
            else
            {
                rightHandHUD.SetActive(false);
            }

            if (rightTether && rightDimmer)
            {
                rightHandText.gameObject.SetActive(true);
            }

            else if (rightTether && rightKelvin)
            {
                rightHandText.gameObject.SetActive(true);

            }
            else rightHandText.gameObject.SetActive(false);
        }
        else rightHandHUD.SetActive(false);



        // left hand
        if (handTracking.leftHand)
        {
            if (leftTether)
            {

                if (leftControl == Lights.SkyPanel1)
                {
                    leftHandHUD.SetActive(true);
                    leftHandHUD.GetComponentInChildren<Renderer>().material = skyPanel1Mat;
                }
                else if (leftControl == Lights.SkyPanel2)
                {
                    leftHandHUD.SetActive(true);
                    leftHandHUD.GetComponentInChildren<Renderer>().material = skyPanel2Mat;
                }

                else if (leftControl == Lights.none)
                {
                    leftHandHUD.SetActive(false);
                }
            }
            else
            {
                leftHandHUD.SetActive(false);
            }

            if (leftTether && leftDimmer)
            {
                leftHandText.gameObject.SetActive(true);
            }

            else if (leftTether && leftKelvin)
            {
                leftHandText.gameObject.SetActive(true);

            }
            else leftHandText.gameObject.SetActive(false);
        }
        else leftHandHUD.SetActive(false);
        


    }
}
