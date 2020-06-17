using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [Header("Right Hand HUD")]
    [SerializeField] GameObject rightSkyPanel;
    [SerializeField] GameObject rightSpot;
    [SerializeField] GameObject rightNone;
    [SerializeField] GameObject rightDimmerText;
    [SerializeField] GameObject rightKelvinText;
    [SerializeField] TextMeshPro rightDimmerVal;
    [SerializeField] TextMeshPro rightKelvinVal;

    [Header("Left Hand HUD")]
    [SerializeField] GameObject leftSkyPanel;
    [SerializeField] GameObject leftSpot;
    [SerializeField] GameObject leftNone;
    [SerializeField] GameObject leftDimmerText;
    [SerializeField] GameObject leftKelvinText;
    [SerializeField] TextMeshPro leftDimmerVal;
    [SerializeField] TextMeshPro leftKelvinVal;

    [Header("Float GameObjects")]
    [SerializeField] GameObject rightHandFloat;
    [SerializeField] GameObject leftHandFloat;
    [SerializeField] Transform rightHandMin;
    [SerializeField] Transform leftHandMin;

    [SerializeField] bool tetherOverride = false;
    bool hasMadeRightFist = false;
    bool rightTether = false;
    bool rightDimmer = false;
    bool rightKelvin = false;

    bool hasMadeLeftFist = false;
    bool leftTether = false;
    bool leftDimmer = false;
    bool leftKelvin = false;

    public enum Lights { none, SkyPanel, Spot, DJ, reset };
    public Lights gazeLight = Lights.none;
    public Lights rightControl = Lights.none;
    public Lights leftControl = Lights.none;

    public bool hudOn = false; // todo make private


    HandTracking handTracking;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    OSC osc;

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
        

        rightHandFloat.SetActive(false);
        leftHandFloat.SetActive(false);

        
    }

    void OnEnable()
    {
        dmx.SetAddress(dmxChan.skyPanelXOver, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // right hand control
        if (handTracking.rightFist && !hasMadeRightFist)
        {
            ToggleRightTether();
            hasMadeRightFist = true;
        }

        if (!handTracking.rightFist) hasMadeRightFist = false;

        if (rightTether)
        {
            if (handTracking.rightFlatHand) rightDimmer = true;
            else rightDimmer = false;

            if (handTracking.rightKnifeHand) rightKelvin = true;
            else rightKelvin = false;
        }

        // left hand control
        if (handTracking.leftFist && !hasMadeLeftFist)
        {
            ToggleLeftTether();
            hasMadeLeftFist = true;
        }

        if (!handTracking.leftFist) hasMadeLeftFist = false;

        if (leftTether)
        {
            if (handTracking.leftFlatHand) leftDimmer = true;
            else leftDimmer = false;

            if (handTracking.leftKnifeHand) leftKelvin = true;
            else leftKelvin = false;
        }

        
        ProcessRightHandControls();
        ProcessLeftHandControls();

        if (hudOn)
        {
            ProcessRightHUD();
            ProcessLeftHud();
        }
        else
        {
            rightNone.SetActive(false);
            rightSkyPanel.SetActive(false);
            rightSpot.SetActive(false);
            rightDimmerText.SetActive(false);
            rightKelvinText.SetActive(false);

            leftNone.SetActive(false);
            leftSpot.SetActive(false);
            leftSkyPanel.SetActive(false);
            leftDimmerText.SetActive(false);
            leftKelvinText.SetActive(false);
        }
    }

    private void ToggleRightTether()
    {
        if (!rightTether)
        {
            if (gazeLight == Lights.reset) rightControl = Lights.none;
            else if (gazeLight == Lights.SkyPanel) rightControl = Lights.SkyPanel;
            else if (gazeLight == Lights.Spot) rightControl = Lights.Spot;
            
            else if (rightControl == Lights.none)
            {
                if (gazeLight == Lights.SkyPanel)
                {
                    rightControl = Lights.SkyPanel;
                }

                else if (gazeLight == Lights.Spot)
                {
                    rightControl = Lights.Spot;
                }

                else if (gazeLight == Lights.DJ)
                {
                    rightControl = Lights.DJ;
                }

                else return;
            }

            rightTether = true;

        }
        else
        {
            if (gazeLight == Lights.reset) rightControl = Lights.none;
            rightTether = false;
        }
    }

    private void ToggleLeftTether()
    {
        if (!leftTether)
        {
            if (gazeLight == Lights.reset) leftControl = Lights.none;

            if (leftControl == Lights.none)
            {
                if (gazeLight == Lights.SkyPanel)
                {
                    leftControl = Lights.SkyPanel;
                }

                else if (gazeLight == Lights.Spot)
                {
                    leftControl = Lights.Spot;
                }

                else if (gazeLight == Lights.DJ)
                {
                    leftControl = Lights.DJ;
                }

                else return;
            }

            leftTether = true;

        }
        else
        {
            if (gazeLight == Lights.reset) leftControl = Lights.none;
            leftTether = false;
        }
    }


    #region right globals
    float rightDimmerFloat;
    float rightDimmerYPos;
    bool rightDimmerYLocked = false;

    float rightKelvinFloat;
    bool rightKelvinXLocked = false;
    float rightKelvinXPos;
    #endregion

    private void ProcessRightHandControls()
    {
        if (rightDimmer)
        {
            rightKelvinXLocked = false;
            
            // activate right hand float
            rightHandFloat.SetActive(true);
            rightHandFloat.transform.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!rightDimmerYLocked)
            {
                rightDimmerYPos = handTracking.rightPalm.Position.y;
                rightDimmerYLocked = true;
            }

            rightHandFloat.transform.position = new Vector3(handTracking.rightPalm.Position.x, rightDimmerYPos, handTracking.rightPalm.Position.z);

            // determine float using pose.position.y
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightHandMin.position, handTracking.rightPalm.Position);


            rightDimmerFloat = handDistToMin / maxDistance;
            if (rightDimmerFloat > 1) rightDimmerFloat = 1;
            if (handTracking.rightPalm.Position.y < rightHandFloat.transform.position.y - 0.125) rightDimmerFloat = 0;
            

            // display in HUD
            rightDimmerVal.text = Mathf.RoundToInt(rightDimmerFloat * 255).ToString();

            // convert float to DMX
            int dimmerVal = Mathf.RoundToInt(rightDimmerFloat * 255);

            // send DMX & OSC
            if (rightControl == Lights.SkyPanel)
            {
                SendDMX(dmxChan.skyPanelDimmer, dimmerVal);
                SendOSC("/SkyPanelDimmer/", rightDimmerFloat);
            }
            else if (rightControl == Lights.Spot)
            {
                SendDMX(dmxChan.spotWhite, dimmerVal);
                SendOSC("/SpotDimmer/", rightDimmerFloat);
            }

        }

        else if (rightKelvin)
        {
            rightDimmerYLocked = false;
            var cam = Camera.main.transform;
            var camRelative = cam.InverseTransformPoint(handTracking.rightPalm.Position);

            // set float.position.x to pose.position.x and store in memory - float.position.y/z tracks to pose.position.y/z
            if (!rightKelvinXLocked)
            {
                rightKelvinXPos = handTracking.rightPalm.Position.x;
                rightKelvinXLocked = true;
            }

            // activate right hand float, position, and rotate
            rightHandFloat.SetActive(true);
            rightHandFloat.transform.localRotation = Quaternion.Euler(0, 0, -90);
            rightHandFloat.transform.position = new Vector3(rightKelvinXPos, handTracking.rightPalm.Position.y, handTracking.rightPalm.Position.z);


            // determine float using pose.position.x
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightHandMin.position, handTracking.rightPalm.Position);

            rightKelvinFloat = handDistToMin / maxDistance;
            if (rightKelvinFloat > 1) rightKelvinFloat = 1;
            if (handTracking.rightPalm.Position.x < rightHandFloat.transform.position.x - 0.125) rightKelvinFloat = 0;

            // display in HUD
            rightKelvinVal.text = rightKelvinFloat.ToString();

            // convert float to DMX
            int kelvinVal = Mathf.RoundToInt(rightKelvinFloat * 255);

            // send DMX & OSC
            if (rightControl == Lights.SkyPanel)
            {
                SendDMX(dmxChan.skyPanelKelvin, kelvinVal);
                SendOSC("/SkyPanelkelvin/", rightKelvinFloat);
            }
            else if (rightControl == Lights.Spot)
            {
                SendDMX(dmxChan.spotBlue, kelvinVal);
                SendOSC("/SpotKelvin/", rightKelvinFloat);
            }
        }
        else rightHandFloat.SetActive(false);

    }


    #region left globals
    float leftDimmerFloat;
    float leftDimmerYPos;
    bool leftDimmerYLocked = false;

    float leftKelvinFloat;
    bool leftKelvinXLocked = false;
    float leftKelvinXPos;
    #endregion

    private void ProcessLeftHandControls()
    {
        if (leftDimmer)
        {
            leftKelvinXLocked = false;

            // activate right hand float
            leftHandFloat.SetActive(true);
            leftHandFloat.transform.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!leftDimmerYLocked)
            {
                leftDimmerYPos = handTracking.ltMiddleTip.Position.y;
                leftDimmerYLocked = true;
            }

            leftHandFloat.transform.position = new Vector3(handTracking.ltMiddleTip.Position.x, leftDimmerYPos, handTracking.ltMiddleTip.Position.z);

            // determine float using pose.position.y
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(leftHandMin.position, handTracking.ltMiddleTip.Position);


            leftDimmerFloat = handDistToMin / maxDistance;
            if (leftDimmerFloat > 1) leftDimmerFloat = 1;
            if (handTracking.ltMiddleTip.Position.y < leftHandFloat.transform.position.y - 0.125) leftDimmerFloat = 0;


            // display in HUD
            leftDimmerVal.text = Mathf.RoundToInt(leftDimmerFloat * 255).ToString();

            // convert float to DMX
            int dimmerVal = Mathf.RoundToInt(leftDimmerFloat * 255);

            // send DMX & OSC
            if (leftControl == Lights.SkyPanel)
            {
                SendDMX(dmxChan.skyPanelDimmer, dimmerVal);
                SendOSC("/SkyPanelDimmer/", rightDimmerFloat);
            }
            else if (leftControl == Lights.Spot)
            {
                SendDMX(dmxChan.spotWhite, dimmerVal);
                SendOSC("/SpotDimmer/", rightDimmerFloat);
            }
            
        }

        else if (leftKelvin)
        {
            leftDimmerYLocked = false;

            // activate right hand float
            leftHandFloat.SetActive(true);
            leftHandFloat.transform.transform.localRotation = Quaternion.Euler(0, 0, 90);

            // set float.position.x to pose.position.x and store in memory - float.position.y/z tracks to pose.position.y/z
            if (!leftKelvinXLocked)
            {
                leftKelvinXPos = handTracking.ltMiddleTip.Position.x;
                leftKelvinXLocked = true;
            }

            leftHandFloat.transform.position = new Vector3(leftKelvinXPos, handTracking.ltMiddleTip.Position.y, handTracking.ltMiddleTip.Position.z);

            // determine float using pose.position.x
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(leftHandMin.position, handTracking.ltMiddleTip.Position);

            leftKelvinFloat = handDistToMin / maxDistance;
            if (leftKelvinFloat > 1) leftKelvinFloat = 1;
            if (handTracking.ltMiddleTip.Position.x > leftHandFloat.transform.position.x + 0.125) leftKelvinFloat = 0;

            // display in HUD
            leftKelvinVal.text = leftKelvinFloat.ToString();

            // convert float to DMX
            int kelvinVal = Mathf.RoundToInt(leftKelvinFloat  * 255);

            // send DMX & OSC
            if (leftControl == Lights.SkyPanel)
            {
                SendDMX(dmxChan.skyPanelKelvin, kelvinVal);
                SendOSC("/SkyPanelkelvin/", rightKelvinFloat);
            }
            else if (leftControl == Lights.Spot)
            {
                SendDMX(dmxChan.spotBlue, kelvinVal);
                SendOSC("/SpotKelvin/", rightKelvinFloat);
            }
            
        }
        else leftHandFloat.SetActive(false);

    }

    private void SendDMX(int channel, int val)
    {
        dmx.SetAddress(channel, val);
        Debug.Log("Sending DMX: channel " + channel + ", value " + val); // remove
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
    public void GazeAtSkyPanel()
    {
        gazeLight = Lights.SkyPanel;
    }

    public void GazeAtSpot()
    {
        gazeLight = Lights.Spot;
    }

    public void GazeAtDJ()
    {
        gazeLight = Lights.DJ;
    }

    public void NoGaze()
    {
        gazeLight = Lights.none;
    }

    public void ResetHand()
    {
        gazeLight = Lights.reset;
    }

    public void ToggleHUD()
    {
        hudOn = !hudOn;
    }
    #endregion

    private void ProcessRightHUD()
    {
        if (rightTether)
        {
            if (rightControl == Lights.SkyPanel)
            {
                rightSkyPanel.SetActive(true);
                rightSpot.SetActive(false);
                rightNone.SetActive(false);
            }
            else if (rightControl == Lights.Spot)
            {
                rightSkyPanel.SetActive(false);
                rightSpot.SetActive(true);
                rightNone.SetActive(false);
            }
            else if (rightControl == Lights.DJ)
            {
                // add HUD text
            }
            else if (rightControl == Lights.none)
            {
                rightSkyPanel.SetActive(false);
                rightSpot.SetActive(false);
                rightNone.SetActive(true);
            }
        }
        else
        {
            rightSkyPanel.SetActive(false);
            rightSpot.SetActive(false);
            rightNone.SetActive(true);
        }

        if (rightTether && rightDimmer)
        {
            rightDimmerText.SetActive(true);
        }
        else rightDimmerText.SetActive(false);

        if (rightTether && rightKelvin)
        {
            rightKelvinText.SetActive(true);
        }
        else rightKelvinText.SetActive(false);
    }

    private void ProcessLeftHud()
    {
        if (leftTether)
        {
            if (leftControl == Lights.SkyPanel)
            {
                leftSkyPanel.SetActive(true);
                leftSpot.SetActive(false);
                leftNone.SetActive(false);
            }
            else if (leftControl == Lights.Spot)
            {
                leftSkyPanel.SetActive(false);
                leftSpot.SetActive(true);
                leftNone.SetActive(false);
            }
            else if (leftControl == Lights.DJ)
            {
                // add HUD text
            }
            else if (leftControl == Lights.none)
            {
                leftSkyPanel.SetActive(false);
                leftSpot.SetActive(false);
                leftNone.SetActive(true);
            }
        }
        else
        {
            leftSkyPanel.SetActive(false);
            leftSpot.SetActive(false);
            leftNone.SetActive(true);
        }

        if (leftTether && leftDimmer)
        {
            leftDimmerText.SetActive(true);
        }
        else leftDimmerText.SetActive(false);

        if (leftTether && leftKelvin)
        {
           leftKelvinText.SetActive(true);
        }
        else leftKelvinText.SetActive(false);
    }
}
