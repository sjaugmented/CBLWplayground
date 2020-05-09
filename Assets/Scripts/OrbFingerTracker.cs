using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFingerTracker : MonoBehaviour
{
    [Header("Thresholds")]
    [Tooltip("Min Velocity at which spells are cast")]
    [SerializeField] float minVelocity = 2f;
    [Tooltip("Max Velocity at which spells are cast")]
    [SerializeField] float maxVelocity = 10f;
    [Tooltip("How far forward the finger must point before casting can happen")]
    [SerializeField] float fingerForwardThreshold = 0.7f;
    [Tooltip("Margin between hero angles of 0 and 180")]
    [SerializeField] float angleMargin = 50f;
    //[SerializeField] bool fingerCasting = true;

    // used for index tracking & velocity
    MixedRealityPose rightPalm, leftPalm, rtIndexTip, rtMiddleTip, rtPinkyTip, rtThumbTip;
    float castFingerUpThresh = 0.3f;
    bool castFingerOut = false;

    public bool twoPalms = false;
    public bool touchDown = false;
    public bool palmsForward = false;
    public bool palmsIn = false;
    public bool rockOn = false;
    public bool fingerGun = false;
    float palmDist;
    float prevHandCamDist;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessHands();
    }

    private void ProcessHands()
    {
        Transform cam = Camera.main.transform;
        
        // look for two palms
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            twoPalms = true;
            palmDist = Vector3.Distance(rightPalm.Position, leftPalm.Position);

            // get reference angles
            // palm to palm
            float p2pUp = Vector3.Angle(rightPalm.Up, leftPalm.Up);
            float p2pRt = Vector3.Angle(rightPalm.Right, leftPalm.Right);

            // right palm
            float rtPalmUpCamFor = Vector3.Angle(rightPalm.Up, cam.forward);
            float rtPalmForCamFor = Vector3.Angle(rightPalm.Forward, cam.forward);
            float rtPalmRtCamFor = Vector3.Angle(rightPalm.Right, cam.forward);
            float rtPalmRtCamRt = Vector3.Angle(rightPalm.Right, cam.right);
            float rtPalmRtCamUp = Vector3.Angle(rightPalm.Right, cam.up);

            // left palm
            float ltPalmUpCamFor = Vector3.Angle(leftPalm.Up, cam.forward);
            float ltPalmForCamFor = Vector3.Angle(leftPalm.Forward, cam.forward);
            float ltPalmRtCamFor = Vector3.Angle(leftPalm.Right, cam.forward);
            float ltPalmRtCamRt = Vector3.Angle(leftPalm.Right, cam.right);
            float ltPalmRtCamUp = Vector3.Angle(leftPalm.Right, cam.up);

            // look for touchDown 
            if (IsWithinRange(p2pUp, 180) && IsWithinRange(p2pRt, 180) && IsWithinRange(rtPalmUpCamFor, 90) && IsWithinRange(ltPalmUpCamFor, 90) && IsWithinRange(rtPalmForCamFor, 90) && IsWithinRange(ltPalmForCamFor, 90) && IsWithinRange(rtPalmRtCamFor, 30) && IsWithinRange(ltPalmRtCamFor, 150) && IsWithinRange(rtPalmRtCamRt, 90) && IsWithinRange(ltPalmRtCamRt, 90))
            {
                touchDown = true;
                palmsIn = false;
                palmsForward = false;
               
            }

            // look for palmsIn 
            else if (IsWithinRange(p2pUp, 180) && IsWithinRange(p2pRt, 180) && IsWithinRange(rtPalmUpCamFor, 90) && IsWithinRange(ltPalmUpCamFor, 90) && IsWithinRange(rtPalmForCamFor, 20) && IsWithinRange(ltPalmForCamFor, 20) && IsWithinRange(rtPalmRtCamFor, 90) && IsWithinRange(ltPalmRtCamFor, 90) && IsWithinRange(rtPalmRtCamRt, 90) && IsWithinRange(ltPalmRtCamRt, 90))
            {
                touchDown = false;
                palmsIn = true;
                palmsForward = false;
               
            }

            // look for palmsOut 
            else if (IsWithinRange(p2pUp, 0) && IsWithinRange(p2pRt, 0) && IsWithinRange(rtPalmUpCamFor, 180) && IsWithinRange(ltPalmUpCamFor, 180) && IsWithinRange(rtPalmForCamFor, 90) && IsWithinRange(ltPalmForCamFor, 90) && IsWithinRange(rtPalmRtCamFor, 90) && IsWithinRange(ltPalmRtCamFor, 90) && IsWithinRange(rtPalmRtCamRt, 0) && IsWithinRange(ltPalmRtCamRt, 0))
            {
                touchDown = false;
                palmsIn = false;
                palmsForward = true;
                
            }
            else
            {
                touchDown = false;
                palmsIn = false;
                palmsForward = false;
            }
        }
        // look for fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip))
        {
            // get finger angles
            float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
            float rtIndForCamFor = Vector3.Angle(rtIndexTip.Forward, cam.forward);
            float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
            float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
            float rtThumbForCamFor = Vector3.Angle(rtThumbTip.Forward, cam.forward);
            float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);

            // look for rockOn
            if (IsWithinRange(rtIndForPalmFor, 0) && IsWithinRange(rtPinkForPalmFor, 0) && !IsWithinRange(rtMidForPalmFor, 0))
            {
                rockOn = true;
                fingerGun = false;
            }
            // look for fingerGun
            else if (IsWithinRange(rtIndForCamFor, 0) && IsWithinRange(rtIndForPalmFor, 0) && IsWithinRange(rtThumbForPalmFor, 60) && IsWithinRange(rtMidForPalmFor, 180) && IsWithinRange(rtPinkForPalmFor, 180))
            {
                rockOn = false;
                fingerGun = true;
            }
            else
            {
                rockOn = false;
                fingerGun = false;
            }
        }
        else
        {
            twoPalms = false;
            touchDown = false;
            palmsIn = false;
            palmsForward = false;
            rockOn = false;
            fingerGun = false;
        }
    }

    private bool IsWithinRange(float testVal, float target)
    {
        bool withinRange = false;

        if (target == 0)
        {
            if (testVal <= target + angleMargin) withinRange = true;
        }
        else if (target == 180)
        {
            if (testVal >= 180 - angleMargin) withinRange = true;
        }
        else if (target > 0 && target < 180)
        {
            if (testVal >= target - angleMargin && testVal <= target + angleMargin) withinRange = true;
        }
        else withinRange = false;

        return withinRange;
    }

    public Vector3 GetPalm1Pos()
    {
        return rightPalm.Position;
    }

    public Vector3 GetPalm2Pos()
    {
        return leftPalm.Position;
    }

    public Vector3 GetRtIndexPos()
    {
        return rtIndexTip.Position;
    }

    public Vector3 GetRtPinkyPos()
    {
        return rtPinkyTip.Position;
    }

    public bool GetPalmsForward()
    {
        return palmsForward;
    }

    public bool GetTouchdown()
    {
        return touchDown;
    }

    public bool GetPalmsIn()
    {
        return palmsIn;
    }

    public float GetPalmDist()
    {
        return palmDist;
    }

    public bool GetTwoPalms()
    {
        return twoPalms;
    }

    public bool GetRockOn()
    {
        return rockOn;
    }

    public bool GetFingerGun()
    {
        return fingerGun;
    }

    public Quaternion GetRtPalmRot()
    {
        return rightPalm.Rotation;
    }
}
