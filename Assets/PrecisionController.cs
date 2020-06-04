using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [SerializeField] GameObject rightTetherOnText;
    [SerializeField] GameObject rightTetherOffText;
    [SerializeField] GameObject rightDimmerText;
    [SerializeField] GameObject rightKelvinText;

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

    // Start is called before the first frame update
    void Start()
    {
        poseTracker = GetComponent<PrecisionPoseTracker>();
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

        ProcessTextObjects();
        
    }

    private void ProcessTextObjects()
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
}
