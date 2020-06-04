using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [Header("Right Hand HUD")]
    [SerializeField] GameObject rightTetherOnText;
    [SerializeField] GameObject rightTetherOffText;
    [SerializeField] GameObject rightDimmerText;
    [SerializeField] GameObject rightKelvinText;
    [SerializeField] TextMeshPro rightDimmerVal;
    [SerializeField] TextMeshPro rightKelvinVal;

    [Header("Left Hand HUD")]
    [SerializeField] GameObject leftTetherOnText;
    [SerializeField] GameObject leftTetherOffText;
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
    bool rightFisted = false;
    bool rightTether = false;
    bool rightDimmer = false;
    bool rightKelvin = false;

    bool leftFisted = false;
    bool leftTether = false;
    bool leftDimmer = false;
    bool leftKelvin = false;


    PrecisionPoseTracker poseTracker;
    DMXcontroller dmx;
    OSC osc;

    // Start is called before the first frame update
    void Start()
    {
        poseTracker = GetComponent<PrecisionPoseTracker>();
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();

        rightHandFloat.SetActive(false);
        leftHandFloat.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // right hand control
        if (poseTracker.rightFist && !rightFisted)
        {
            rightTether = !rightTether;
            rightFisted = true;
        }

        if (!poseTracker.rightFist) rightFisted = false;

        if (rightTether)
        {
            if (poseTracker.rightFlatHand) rightDimmer = true;
            else rightDimmer = false;

            if (poseTracker.rightKnifeHand) rightKelvin = true;
            else rightKelvin = false;
        }

        // left hand control
        if (poseTracker.leftFist && !leftFisted)
        {
            leftTether = !leftTether;
            leftFisted = true;
        }

        if (!poseTracker.leftFist) leftFisted = false;

        if (leftTether)
        {
            if (poseTracker.leftFlatHand) leftDimmer = true;
            else leftDimmer = false;

            if (poseTracker.leftKnifeHand) leftKelvin = true;
            else leftKelvin = false;
        }

        ProcessRightHUD();
        ProcessLeftHud();
        ProcessRightHandControls();
    }

    float rightDimmerFloat;
    float dimmerYPos;
    bool dimmerYLocked = false;

    float rightKelvinFloat;
    bool kelvinXLocked = false;
    float kelvinXPos;

    private void ProcessRightHandControls()
    {
        if (rightDimmer)
        {
            kelvinXLocked = false;
            
            // activate right hand float
            rightHandFloat.SetActive(true);
            rightHandFloat.transform.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!dimmerYLocked)
            {
                dimmerYPos = poseTracker.rtMiddleTip.Position.y;
                dimmerYLocked = true;
            }

            rightHandFloat.transform.position = new Vector3(poseTracker.rtMiddleTip.Position.x, dimmerYPos, poseTracker.rtMiddleTip.Position.z);

            // determine float using pose.position.y
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightHandMin.position, poseTracker.rtMiddleTip.Position);


            rightDimmerFloat = handDistToMin / maxDistance;
            if (rightDimmerFloat > 1) rightDimmerFloat = 1;
            if (poseTracker.rtMiddleTip.Position.y < rightHandFloat.transform.position.y - 0.125) rightDimmerFloat = 0;
            

            // display in HUD
            rightDimmerVal.text = rightDimmerFloat.ToString();

            // convert float to DMX

            // send DMX

            // send OSC
        }

        if (rightKelvin)
        {
            dimmerYLocked = false;

            // activate right hand float
            rightHandFloat.SetActive(true);
            rightHandFloat.transform.transform.localRotation = Quaternion.Euler(0, 0, -90);

            // set float.position.y to pose.position.y and store in memory - float.position.x/z tracks to pose.position.x/z
            if (!kelvinXLocked)
            {
                kelvinXPos = poseTracker.rtMiddleTip.Position.x;
                kelvinXLocked = true;
            }

            rightHandFloat.transform.position = new Vector3(kelvinXPos, poseTracker.rtMiddleTip.Position.y, poseTracker.rtMiddleTip.Position.z);

            // determine float using pose.position.x
            float maxDistance = 0.25f;
            float handDistToMin = Vector3.Distance(rightHandMin.position, poseTracker.rtMiddleTip.Position);

            rightKelvinFloat = handDistToMin / maxDistance;
            if (rightKelvinFloat > 1) rightKelvinFloat = 1;
            if (poseTracker.rtMiddleTip.Position.x < rightHandFloat.transform.position.x - 0.125) rightKelvinFloat = 0;

            // display in HUD
            rightKelvinVal.text = rightKelvinFloat.ToString();

            // convert float to DMX

            // send DMX

            // send OSC
        }
    }

    private void DimmerControl()
    {

    }

    private void KelvinControl()
    {

    }

    private void ProcessRightHUD()
    {
        if (rightTether)
        {
            rightTetherOnText.SetActive(true);
            rightTetherOffText.SetActive(false);
        }
        else
        {
            rightTetherOnText.SetActive(false);
            rightTetherOffText.SetActive(true);
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
            leftTetherOnText.SetActive(true);
            leftTetherOffText.SetActive(false);
        }
        else
        {
            leftTetherOnText.SetActive(false);
            leftTetherOffText.SetActive(true);
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
