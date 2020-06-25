using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [Header("Kelvin", order =0)]
    [Header("Hand HUDs", order =1)]
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

    public enum Mode { kelvin, rgb };
    public Mode currMode = Mode.rgb;

    public enum Lights { none, SkyPanel1, SkyPanel2, DJ, reset };
    public Lights gazeLight = Lights.none;
    public Lights rightControl = Lights.none;
    public Lights leftControl = Lights.none;
    public Lights rgbControl = Lights.none;

    HandTracking handTracking;
    Director director;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    OSC osc;

    bool rightTether = false;
    bool rightDimmer = false;
    bool rightKelvin = false;
    bool leftTether = false;
    bool leftDimmer = false;
    bool leftKelvin = false;

    bool kelvinMode = true;
    public bool rgbActive = false;

    #region DMX channel ID's
    int dimmerChan = 0;
    int kelvinChan = 1;
    int xOverChan = 3;
    int redChan = 4;
    int greenChan = 5;
    int blueChan = 6;
    int whiteChan = 7;
    int amberChan = 8;
    public bool hasAmber = false;
    #endregion

    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();
        director = FindObjectOfType<Director>();
        dmx = FindObjectOfType<DMXcontroller>();
        dmxChan = FindObjectOfType<DMXChannels>();
        osc = FindObjectOfType<OSC>();

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
        /*dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 0);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 0);*/
    }

    void OnDisable()
    {
        rightKelvinObj.SetActive(false);
        leftKelvinObj.SetActive(false);
    }

    

    // Update is called once per frame
    void Update()
    {
        CalcHandPositions();
        ProcessHandHUDs();
        ProcessHandRings();
        KelvinFloats();
        RGBChannels();

        if (director.readGestures)
        {
            #region Dimmer/Kelvin controls
            if (handTracking.rightPoint || handTracking.leftPoint)
            {
                currMode = Mode.kelvin;
                SetKelvinXover();

            }
            else
            {
                currMode = Mode.rgb;
            }
            #endregion

            #region RGB controls
            if (!handTracking.rightPoint && !handTracking.leftPoint && handTracking.twoHands /*&& handTracking.palmsOpposed*/)
            {
                

            }
            else
            {
                rgbActive = false;
                redChanObj.SetActive(false);
                greenChanObj.SetActive(false);
                blueChanObj.SetActive(false);
                whiteChanObj.SetActive(false);
                amberChanObj.SetActive(false);
                globalDimmerObj.SetActive(false);
            }
            #endregion

            
        }
        else
        {
            rightHandHUD.SetActive(false);
            leftHandHUD.SetActive(false);
            rightRingParent.SetActive(false);
            leftRingParent.SetActive(false);
        }
    }

    private void SetKelvinXover()
    {
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

    private void SetRGBXover()
    {
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

    public bool initialCheckLeft;
    public int timerLeft = 60;

    private void KelvinFloats()
    {
        if (currMode == Mode.kelvin)
        {
            // right hand control
            if (handTracking.rightPoint)
            {
                ToggleRightTether();
            }
            else
            {
                rightTether = false;
            }

            if (rightTether)
            {
                if (handTracking.rightPoint && handTracking.rtPalmUpFloorUp >= 0 && handTracking.rtPalmUpFloorUp < 70 && handTracking.rtPalmRtCamRt >= 0 && handTracking.rtPalmRtCamRt < 50)
                {
                    rightDimmer = true;
                    ProcessRightHandControls();
                }
                else rightDimmer = false;

                if (handTracking.rightPoint && handTracking.rtPalmUpFloorUp >= 70 && handTracking.rtPalmUpFloorUp <= 135 && handTracking.rtPalmRtCamRt >= 60 && handTracking.rtPalmRtCamRt < 115)
                {
                    rightKelvin = true;
                    ProcessRightHandControls();
                }
                else rightKelvin = false;
            }

            // left hand control
            if (handTracking.leftPoint)
            {
                if (!initialCheckLeft)
                {
                    initialCheckLeft = true;
                }
                else
                {
                    ToggleLeftTether();

                    if (timerLeft > 0)
                    {
                        timerLeft--;
                    }
                    else
                    {
                        initialCheckLeft = false;
                        timerLeft = 60;
                    }
                }

                
                //ToggleLeftTether();
            }
            else
            {
                leftTether = false;
                initialCheckLeft = false;
                timerLeft = 60;
            }

            if (leftTether)
            {
                if (handTracking.leftPoint && handTracking.ltPalmUpFloorUp >= 0 && handTracking.ltPalmUpFloorUp < 50 && handTracking.ltPalmRtCamRt >= 0 && handTracking.ltPalmRtCamRt < 50)
                {
                    leftDimmer = true;
                    ProcessLeftHandControls();
                }
                else leftDimmer = false;

                if (handTracking.leftPoint && handTracking.ltPalmUpFloorUp >= 70 && handTracking.ltPalmUpFloorUp <= 135 && handTracking.ltPalmRtCamRt >= 70 && handTracking.ltPalmRtCamRt < 105)
                {
                    leftKelvin = true;
                    ProcessLeftHandControls();
                }
                else leftKelvin = false;
            }
        }
        else return;
    }

    private void ProcessHandHUDs()
    {
        if (currMode == Mode.kelvin)
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
        else
        {
            rightHandHUD.SetActive(false);
            leftHandHUD.SetActive(false);
        }
        

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

    /// <summary>
    /// RGB MODE
    /// </summary>
    #region RGB
    #region Inspector Fields
    [Header("RGB", order =0)]
    [Header("Hand Rings", order =1)]
    [SerializeField] GameObject rightRingParent;
    [SerializeField] GameObject leftRingParent;
    [SerializeField] Renderer rightRing;
    [SerializeField] Renderer leftRing;
    [SerializeField] Material redLive;
    [SerializeField] Material greenLive;
    [SerializeField] Material blueLive;
    [SerializeField] Material whiteLive;
    [SerializeField] Material amberLive;
    [SerializeField] Material redStealth;
    [SerializeField] Material greenStealth;
    [SerializeField] Material blueStealth;
    [SerializeField] Material whiteStealth;
    [SerializeField] Material amberStealth;

    [Header("Floats and Float objects")]
    [SerializeField] GameObject redChanObj;
    [SerializeField] TextMeshPro redChanText;
    [SerializeField] GameObject redLiveBox;
    [SerializeField] GameObject greenChanObj;
    [SerializeField] TextMeshPro greenChanText;
    [SerializeField] GameObject greenLiveBox;
    [SerializeField] GameObject blueChanObj;
    [SerializeField] TextMeshPro blueChanText;
    [SerializeField] GameObject blueLiveBox;
    [SerializeField] GameObject whiteChanObj;
    [SerializeField] TextMeshPro whiteChanText;
    [SerializeField] GameObject whiteLiveBox;
    [SerializeField] GameObject amberChanObj;
    [SerializeField] TextMeshPro amberChanText;
    [SerializeField] GameObject amberLiveBox;
    [SerializeField] GameObject globalDimmerObj;
    [SerializeField] TextMeshPro globalDimmerText;
    [SerializeField] GameObject globalLiveBox;

    [SerializeField] [Range(0.2f, 0.6f)] float maxFloatDist = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] float floatOffset = 0.05f;
    [SerializeField] Vector3 palmMidpointOffset;

    [Header("OSC controller")]
    [SerializeField] string redOSCMessage = "/redOSCfloat/";
    [SerializeField] string greenOSCMessage = "/greenOSCfloat/";
    [SerializeField] string blueOSCMessage = "/blueOSCfloat/";
    [SerializeField] string whiteOSCMessage = "/whiteOSCfloat/";
    [SerializeField] string amberOSCMessage = "/amberOSCfloat/";
    [SerializeField] string globalOSCMessage = "/globalOSCfloat/";

    public enum RGB { red, green, blue, white, amber, all };
    public RGB currColor = RGB.red;

    [Header("DMX values")]
    public int redVal = 0;
    public int greenVal = 0;
    public int blueVal = 0;
    public int whiteVal = 0;
    public int amberVal = 0;
    public int globalDimmerVal = 0;
    #endregion

    bool live = false;
    float palmDist;
    float indexMidDist;
    int floatScale;
    Vector3 midpointIndexes;
    Vector3 masterOrbPos;

    private void RGBChannels()
    {
        if (director.readGestures)
        {
            if (currMode == Mode.rgb)
            {
                // global dimmer
                if (handTracking.staffFloorFor00)
                {
                    currColor = RGB.all;
                    float allFloat;

                    allFloat = 1 - (indexMidDist - floatOffset) / (0.2f - floatOffset);
                    if (indexMidDist > 0.2f) allFloat = 0;
                    if (indexMidDist < floatOffset) allFloat = 1;
                    var globalText = Mathf.RoundToInt(allFloat * 100);

                    globalDimmerObj.SetActive(true);
                    globalDimmerObj.transform.position = midpointIndexes;
                    globalDimmerObj.transform.rotation = Camera.main.transform.rotation;
                    globalDimmerText.text = globalText + "%".ToString();

                    if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                    {
                        live = true;
                        globalLiveBox.SetActive(true);
                        globalDimmerVal = Mathf.RoundToInt(allFloat * 255);
                        SendOSC(globalOSCMessage, allFloat);

                        if (rgbControl == Lights.SkyPanel1)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[redChan], Mathf.RoundToInt(redVal * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[greenChan], Mathf.RoundToInt(greenChan * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[blueChan], Mathf.RoundToInt(blueChan * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[whiteChan], Mathf.RoundToInt(whiteChan * allFloat));
                            if (hasAmber) dmx.SetAddress(dmxChan.SkyPanel1[amberChan], Mathf.RoundToInt(redVal * allFloat));
                        }
                        if (rgbControl == Lights.SkyPanel2)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[redChan], Mathf.RoundToInt(redVal * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[greenChan], Mathf.RoundToInt(greenChan * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[blueChan], Mathf.RoundToInt(blueChan * allFloat));
                            dmx.SetAddress(dmxChan.SkyPanel1[whiteChan], Mathf.RoundToInt(whiteChan * allFloat));
                            if (hasAmber) dmx.SetAddress(dmxChan.SkyPanel1[amberChan], Mathf.RoundToInt(redVal * allFloat));
                        }
                        else return;
                    }
                    else
                    {
                        live = false;
                        globalLiveBox.SetActive(false);
                    }
                }
                else
                {
                    globalDimmerObj.SetActive(false);
                }

                // red
                if (handTracking.palmsOpposed && handTracking.staffCamUp90 && handTracking.staffFloorFor90)
                {
                    currColor = RGB.red;
                    float redFloat;

                    redFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
                    if (indexMidDist > maxFloatDist) redFloat = 0;
                    if (indexMidDist < floatOffset) redFloat = 1;
                    var redText = Mathf.RoundToInt(redFloat * 100);

                    redChanObj.SetActive(true);
                    redChanObj.transform.position = midpointIndexes;
                    redChanObj.transform.rotation = Camera.main.transform.rotation;
                    redChanText.text = redText + "%".ToString();

                    if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                    {
                        live = true;
                        redLiveBox.SetActive(true);
                        redVal = Mathf.RoundToInt(redFloat * 255);
                        SendOSC(redOSCMessage, redFloat);

                        if (rgbControl == Lights.SkyPanel1)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[redChan], redVal);
                        }
                        if (rgbControl == Lights.SkyPanel2)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel2[redChan], redVal);
                        }
                        if (rgbControl == PrecisionController.Lights.none)
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
                else redChanObj.SetActive(false);

                // green
                if (handTracking.palmsOpposed && handTracking.staffCamUp45 && handTracking.staffFloorFor90)
                {
                    currColor = RGB.green;
                    float greenFloat;

                    greenFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
                    if (indexMidDist > maxFloatDist) greenFloat = 0;
                    if (indexMidDist < floatOffset) greenFloat = 1;
                    var greenText = Mathf.RoundToInt(greenFloat * 100);

                    greenChanObj.SetActive(true);
                    greenChanObj.transform.position = midpointIndexes;
                    greenChanObj.transform.rotation = Camera.main.transform.rotation;
                    greenChanText.text = greenText + "%".ToString();

                    if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                    {
                        live = true;
                        greenLiveBox.SetActive(true);
                        greenVal = Mathf.RoundToInt(greenFloat * 255);
                        SendOSC(greenOSCMessage, greenFloat);
                        if (rgbControl == Lights.SkyPanel1)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[greenChan], greenVal);
                        }
                        if (rgbControl == Lights.SkyPanel2)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel2[greenChan], greenVal);
                        }
                        if (rgbControl == Lights.none)
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
                    greenChanObj.SetActive(false);
                }

                // blue
                if (handTracking.palmsOpposed && handTracking.staffCamUp135 && handTracking.staffFloorFor90)
                {
                    currColor = RGB.blue;
                    float blueFloat;

                    blueFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
                    if (indexMidDist > maxFloatDist) blueFloat = 0;
                    if (indexMidDist < floatOffset) blueFloat = 1;
                    var blueText = Mathf.RoundToInt(blueFloat * 100);

                    blueChanObj.SetActive(true);
                    blueChanObj.transform.position = midpointIndexes;
                    blueChanObj.transform.rotation = Camera.main.transform.rotation;
                    blueChanText.text = blueText + "%".ToString();

                    if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                    {
                        live = true;
                        blueLiveBox.SetActive(true);
                        blueVal = Mathf.RoundToInt(blueFloat * 255);
                        SendOSC(blueOSCMessage, blueFloat);
                        if (rgbControl == Lights.SkyPanel1)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[blueChan], blueVal);
                        }
                        if (rgbControl == Lights.SkyPanel2)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel2[blueChan], blueVal);
                        }
                        if (rgbControl == Lights.none)
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
                    blueChanObj.SetActive(false);
                }

                // white
                if (handTracking.palmsOpposed && handTracking.staffCamUp00 && handTracking.staffFloorFor90)
                {
                    currColor = RGB.white;
                    float whiteFloat;

                    whiteFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
                    if (indexMidDist > maxFloatDist) whiteFloat = 0;
                    if (indexMidDist < floatOffset) whiteFloat = 1;
                    var whiteText = Mathf.RoundToInt(whiteFloat * 100);

                    whiteChanObj.SetActive(true);
                    whiteChanObj.transform.position = midpointIndexes;
                    whiteChanObj.transform.rotation = Camera.main.transform.rotation;
                    whiteChanText.text = whiteText + "%".ToString();

                    if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                    {
                        live = true;
                        whiteLiveBox.SetActive(true);
                        whiteVal = Mathf.RoundToInt(whiteFloat * 255);
                        SendOSC(whiteOSCMessage, whiteFloat);
                        if (rgbControl == Lights.SkyPanel1)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel1[whiteChan], whiteVal);
                        }
                        if (rgbControl == Lights.SkyPanel2)
                        {
                            dmx.SetAddress(dmxChan.SkyPanel2[whiteChan], whiteVal);
                        }
                        if (rgbControl == Lights.none)
                        {
                            return;
                        }
                    }
                    else
                    {
                        live = false;
                        whiteLiveBox.SetActive(false);
                    }
                }
                else
                {
                    whiteChanObj.SetActive(false);
                }

                // amber
                if (hasAmber)
                {
                    if (handTracking.palmsOpposed && handTracking.staffCamUp180 && handTracking.staffFloorFor90)
                    {
                        currColor = RGB.amber;
                        float amberFloat;

                        amberFloat = 1 - (indexMidDist - floatOffset) / (maxFloatDist - floatOffset);
                        if (indexMidDist > maxFloatDist) amberFloat = 0;
                        if (indexMidDist < floatOffset) amberFloat = 1;
                        var amberText = Mathf.RoundToInt(amberFloat * 100);

                        amberChanObj.SetActive(true);
                        amberChanObj.transform.position = midpointIndexes;
                        amberChanObj.transform.rotation = Camera.main.transform.rotation;
                        amberChanText.text = amberText + "%".ToString();

                        if (handTracking.rightHand && handTracking.leftHand && handTracking.rightOpen && handTracking.leftOpen)
                        {
                            live = true;
                            amberLiveBox.SetActive(true);
                            amberVal = Mathf.RoundToInt(amberFloat * 255);
                            SendOSC(amberOSCMessage, amberFloat);
                            if (rgbControl == Lights.SkyPanel1)
                            {
                                dmx.SetAddress(dmxChan.SkyPanel1[amberChan], amberVal);
                            }
                            if (rgbControl == Lights.SkyPanel2)
                            {
                                dmx.SetAddress(dmxChan.SkyPanel2[amberChan], amberVal);
                            }
                            if (rgbControl == Lights.none)
                            {
                                return;
                            }
                        }
                        else
                        {
                            live = false;
                            amberLiveBox.SetActive(false);
                        }
                    }
                    else
                    {
                        amberChanObj.SetActive(false);
                    }
                }                   
                else
                {
                    return;
                }
            }
            else return;
        }
        else return;
    }

    private void CalcHandPositions()
    {
        palmDist = Vector3.Distance(handTracking.rightPalm.Position, handTracking.leftPalm.Position);
        indexMidDist = Vector3.Distance(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position);
        midpointIndexes = Vector3.Lerp(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position, 0.5f);

        var midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
    }

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
                if (currColor == RGB.white)
                {
                    if (!live) rightRing.material = whiteStealth;
                    else rightRing.material = whiteLive;
                }
                if (currColor == RGB.amber)
                {
                    if (!live) rightRing.material = amberStealth;
                    else rightRing.material = amberLive;
                }
                if (currColor == RGB.all)
                {
                    StartCoroutine("CycleRingColors", rightRing);
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
                if (currColor == RGB.white)
                {
                    if (!live) leftRing.material = whiteStealth;
                    else leftRing.material = whiteLive;
                }
                if (currColor == RGB.amber)
                {
                    if (!live) leftRing.material = amberStealth;
                    else leftRing.material = amberLive;
                }
                if (currColor == RGB.all)
                {
                    StartCoroutine("CycleRingColors", leftRing);
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

    IEnumerator CycleRingColors(Renderer ring)
    {
        while (!live && currColor == RGB.all)
        {
            ring.material = redStealth;
            yield return new WaitForSeconds(0.4f);
            ring.material = greenStealth;
            yield return new WaitForSeconds(0.4f);
            ring.material = blueStealth;
            yield return new WaitForSeconds(0.4f);
            ring.material = whiteStealth;
            yield return new WaitForSeconds(0.4f);
            if (hasAmber) ring.material = amberStealth;
            yield return new WaitForSeconds(0.4f);
        }

        while (live && currColor == RGB.all)
        {
            ring.material = redLive;
            yield return new WaitForSeconds(0.5f);
            ring.material = greenLive;
            yield return new WaitForSeconds(0.5f);
            ring.material = blueLive;
            yield return new WaitForSeconds(0.5f);
            ring.material = whiteLive;
            yield return new WaitForSeconds(0.5f);
            if (hasAmber) ring.material = amberLive;
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

}
