using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    [Header("Thresholds")]
    [Tooltip("Min Velocity at which spells are cast")]
    [SerializeField] float minVelocity = 2f;
    [Tooltip("Max Velocity at which spells are cast")]
    [SerializeField] float maxVelocity = 10f;
    [Tooltip("How far forward the finger must point before casting can happen")]
    [SerializeField] float fingerForwardThreshold = 0.7f;
    [Tooltip("Margins of error for poses")]
    [SerializeField] int bigMargin = 50;
    [SerializeField] int smallMargin = 25;
    //[SerializeField] bool fingerCasting = true;

    // right hand joints
    public MixedRealityPose rightPalm, rtIndexTip, rtIndexMid, rtIndexKnuckle, rtMiddleTip, rtMiddleKnuckle, rtPinkyTip, rtThumbTip;
    // left hand joints
    public MixedRealityPose leftPalm, ltIndexTip, ltIndexMid, ltIndexKnuckle, ltMiddleTip, ltMiddleKnuckle, ltPinkyTip, ltThumbTip;
    float castFingerUpThresh = 0.3f;
    bool castFingerOut = false;

    public bool twoHands = false;
    public bool palmsIn = false;
    public bool fistsIn = false;
    public bool pullUps = false;
    public bool palmsOut = false;
    public bool verticalStack = false;
    public bool forwardStack = false;
    public bool rockOnRight = false;
    public bool rockOnLeft = false;
    public bool rightFist = false;
    public bool leftFist = false;
    public bool rightFlatHand = false;
    public bool rightKnifeHand = false;
    public bool rightThrower;
    public bool leftFlatHand = false;
    public bool leftKnifeHand = false;



    Transform cam;
    Transform floor;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        floor = FindObjectOfType<LevelObject>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHands();
    }

    private void ProcessHands()
    {
        #region Palm Ref Angles
        // get reference angles
        // palm to palm
        float p2pUp = Vector3.Angle(rightPalm.Up, leftPalm.Up);
        float p2pRt = Vector3.Angle(rightPalm.Right, leftPalm.Right);
        float p2pFor = Vector3.Angle(rightPalm.Forward, leftPalm.Forward);

        // right palm
        float rtPalmUpCamFor = Vector3.Angle(rightPalm.Up, cam.forward);
        float rtPalmUpFloorUp = Vector3.Angle(rightPalm.Up, floor.up);
        float rtPalmUpFloorFor = Vector3.Angle(rightPalm.Up, floor.forward);
        float rtPalmForCamFor = Vector3.Angle(rightPalm.Forward, cam.forward);
        float rtPalmRtCamFor = Vector3.Angle(rightPalm.Right, cam.forward);
        float rtPalmRtCamRt = Vector3.Angle(rightPalm.Right, cam.right);
        float rtPalmRtCamUp = Vector3.Angle(rightPalm.Right, cam.up);
        float rtPalmForFloorFor = Vector3.Angle(rightPalm.Forward, floor.forward);
        float rtPalmRtFloorFor = Vector3.Angle(rightPalm.Right, floor.forward);
        float rtPalmUpCamRt = Vector3.Angle(rightPalm.Up, cam.right);
        float rtPalmForCamRt = Vector3.Angle(rightPalm.Forward, cam.right);
        float rtPalmRtFloorRt = Vector3.Angle(rightPalm.Right, floor.right);


        // left palm
        float ltPalmUpCamFor = Vector3.Angle(leftPalm.Up, cam.forward);
        float ltPalmUpFloorUp = Vector3.Angle(leftPalm.Up, floor.up);
        float ltPalmUpFloorFor = Vector3.Angle(leftPalm.Up, floor.forward);
        float ltPalmForCamFor = Vector3.Angle(leftPalm.Forward, cam.forward);
        float ltPalmRtCamFor = Vector3.Angle(leftPalm.Right, cam.forward);
        float ltPalmRtCamRt = Vector3.Angle(leftPalm.Right, cam.right);
        float ltPalmRtCamUp = Vector3.Angle(leftPalm.Right, cam.up);
        float ltPalmForFloorFor = Vector3.Angle(leftPalm.Forward, floor.forward);
        float ltPalmRtFloorFor = Vector3.Angle(leftPalm.Right, floor.forward);
        float ltPalmUpCamRt = Vector3.Angle(leftPalm.Up, cam.right);
        float ltPalmForCamRt = Vector3.Angle(leftPalm.Forward, cam.right);
        float ltPalmRtFloorRt = Vector3.Angle(leftPalm.Right, floor.right);

        #endregion

        #region Finger Ref Angles
        // get right finger angles
        float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
        float rtIndForCamFor = Vector3.Angle(rtIndexTip.Forward, cam.forward);
        float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
        float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
        float rtThumbForCamFor = Vector3.Angle(rtThumbTip.Forward, cam.forward);
        float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);
        float rtIndMidForCamRt = Vector3.Angle(rtIndexMid.Forward, cam.right);
        float rtIndMidForPalmFor = Vector3.Angle(rtIndexMid.Forward, rightPalm.Forward);
        float rtIndMidUpCamFor = Vector3.Angle(rtIndexMid.Up, cam.forward);
        float rtIndMidUpFloorFor = Vector3.Angle(rtIndexMid.Up, floor.forward);
        float rtIndMidUpFloorUp = Vector3.Angle(rtIndexMid.Up, floor.up);
        float rtIndKnuckForPalmFor = Vector3.Angle(rtIndexKnuckle.Forward, rightPalm.Forward);

        // get left finger angles
        float ltIndForPalmFor = Vector3.Angle(ltIndexTip.Forward, leftPalm.Forward);
        float ltIndForCamFor = Vector3.Angle(ltIndexTip.Forward, cam.forward);
        float ltMidForPalmFor = Vector3.Angle(ltMiddleTip.Forward, leftPalm.Forward);
        float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
        float ltThumbForCamFor = Vector3.Angle(ltThumbTip.Forward, cam.forward);
        float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
        float ltIndMidForCamRt = Vector3.Angle(ltIndexMid.Forward, cam.right);
        float ltIndMidForPalmFor = Vector3.Angle(ltIndexMid.Forward, leftPalm.Forward);
        float ltIndMidUpCamFor = Vector3.Angle(ltIndexMid.Up, cam.forward);
        float ltIndMidUpFloorFor = Vector3.Angle(ltIndexMid.Up, floor.forward);
        float ltIndMidUpFloorUp = Vector3.Angle(ltIndexMid.Up, floor.up);
        float ltIndKnuckForPalmFor = Vector3.Angle(ltIndexKnuckle.Forward, leftPalm.Forward);

        // compare fingers on both hands
        float rtIndMidForLtIndMidFor = Vector3.Angle(rtIndexMid.Forward, ltIndexMid.Forward);
        float rtIndMidUpLtIndMidUp = Vector3.Angle(rtIndexMid.Up, ltIndexMid.Up);
        #endregion

        // look for only right fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm))
        {
            // look for rockOn
            if (IsWithinRange(rtIndForPalmFor, 0, bigMargin) && IsWithinRange(rtPinkForPalmFor, 0, bigMargin) && !IsWithinRange(rtMidForPalmFor, 0, bigMargin))
            {
                rockOnRight = true;
            }
            else
            {
                rockOnRight = false;
            }

            // look for right fist
            if (IsWithinRange(rtIndMidForPalmFor, 140, bigMargin) && IsWithinRange(rtMidForPalmFor, 140, bigMargin) && IsWithinRange(rtPinkForPalmFor, 130, bigMargin) ||
            //unity standard tap for debug
            IsWithinRange(rtIndMidForPalmFor, 83, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin))
            {

                rightFist = true;
            }
            else rightFist = false;

            // look for right flat
            if (IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 0, bigMargin) && IsWithinRange(rtPinkForPalmFor, 0, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 0, bigMargin) && IsWithinRange(rtPalmRtFloorRt, 0, bigMargin))
            {
                rightFlatHand = true;
            }
            else rightFlatHand = false;

            // look for right knife
            if (IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 0, bigMargin) && IsWithinRange(rtPinkForPalmFor, 0, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 90, bigMargin) && IsWithinRange(rtPalmRtFloorRt, 90, bigMargin))
            {
                rightKnifeHand = true;
            }
            else rightKnifeHand = false;

            // look for palm out throw
            if (IsWithinRange(rtIndMidForPalmFor, 20, bigMargin) && IsWithinRange(rtMidForPalmFor, 20, bigMargin) && IsWithinRange(rtPinkForPalmFor, 20, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 60, bigMargin) && IsWithinRange(rtPalmRtFloorRt, 0, bigMargin))
            {
                rightThrower = true;
            }
            else rightThrower = false;
        }
        else
        {
            rockOnRight = false;
            rightFist = false;
            rightFlatHand = false;
            rightKnifeHand = false;
            rightThrower = false;
        }

        // look for only left fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            // look for rockOn
            if (IsWithinRange(ltIndForPalmFor, 0, bigMargin) && IsWithinRange(ltPinkForPalmFor, 0, bigMargin) && !IsWithinRange(ltMidForPalmFor, 0, bigMargin))
            {
                rockOnLeft = true;
            }
            else
            {
                rockOnLeft = false;
            }

            // look for left fist
            if (IsWithinRange(ltIndMidForPalmFor, 140, bigMargin) && IsWithinRange(ltMidForPalmFor, 140, bigMargin) && IsWithinRange(ltPinkForPalmFor, 130, bigMargin) ||
                // debug unity standard tap
                IsWithinRange(ltIndMidForPalmFor, 83, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
            {

                leftFist = true;
            }
            else leftFist = false;

            // look for left flat
            if (IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 0, bigMargin) && IsWithinRange(ltPinkForPalmFor, 0, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 0, bigMargin) && IsWithinRange(ltPalmRtFloorRt, 0, bigMargin))
            {
                leftFlatHand = true;
            }
            else leftFlatHand = false;

            // look for left knife
            if (IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 0, bigMargin) && IsWithinRange(ltPinkForPalmFor, 0, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 90, bigMargin) && IsWithinRange(ltPalmRtFloorRt, 90, bigMargin))
            {
                leftKnifeHand = true;
            }
            else leftKnifeHand = false;
        }
        else
        {
            rockOnLeft = false;
            leftFist = false;
            leftFlatHand = false;
            leftKnifeHand = false;
        }

        // look for two palms
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            twoHands = true;

            // look for fingers both hands
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip))
            {
                // look for palms in
                if (IsWithinRange(p2pUp, 180, bigMargin) && IsWithinRange(p2pRt, 180, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 90, bigMargin) && IsWithinRange(ltPalmUpCamFor, 90, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 90, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 90, bigMargin) && IsWithinRange(rtIndForPalmFor, 0, bigMargin) && IsWithinRange(rtPinkForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 0, bigMargin) && IsWithinRange(ltIndForPalmFor, 0, bigMargin) && IsWithinRange(ltPinkForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 0, bigMargin))
                {
                    palmsIn = true;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for fists in
                else if (IsWithinRange(p2pUp, 180, bigMargin) && IsWithinRange(p2pRt, 180, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 90, smallMargin) && IsWithinRange(ltPalmUpCamFor, 90, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 90, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 90, bigMargin) && IsWithinRange(rtIndMidForPalmFor, 170, bigMargin) && IsWithinRange(rtIndKnuckForPalmFor, 70, bigMargin) && IsWithinRange(ltIndMidForPalmFor, 170, bigMargin) && IsWithinRange(ltIndKnuckForPalmFor, 70, bigMargin) 
                    ||
                    //debug: unity standard airtap
                    IsWithinRange(p2pUp, 180, bigMargin) && IsWithinRange(p2pRt, 180, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 90, bigMargin) && IsWithinRange(ltPalmUpCamFor, 90, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 90, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 90, bigMargin) && IsWithinRange(rtIndMidForPalmFor, 83, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin) && IsWithinRange(ltIndMidForPalmFor, 83, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
                {
                    palmsIn = false;
                    fistsIn = true;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for palmsOut, neutral fingers
                else if (IsWithinRange(p2pUp, 0, bigMargin) && IsWithinRange(p2pRt, 0, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 180, bigMargin) && IsWithinRange(ltPalmUpCamFor, 180, bigMargin) && IsWithinRange(rtIndForPalmFor, 20, bigMargin) && IsWithinRange(rtPinkForPalmFor, 20, bigMargin) && IsWithinRange(rtMidForPalmFor, 20, bigMargin) && IsWithinRange(ltIndForPalmFor, 20, bigMargin) && IsWithinRange(ltPinkForPalmFor, 20, bigMargin) && IsWithinRange(ltMidForPalmFor, 20, bigMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = true;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for pull ups
                else if (IsWithinRange(p2pUp, 0, bigMargin) && IsWithinRange(p2pRt, 0, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 0, bigMargin) && IsWithinRange(ltPalmUpCamFor, 0, bigMargin) && IsWithinRange(rtIndMidForPalmFor, 170, bigMargin) && IsWithinRange(rtIndKnuckForPalmFor, 70, bigMargin) && IsWithinRange(ltIndMidForPalmFor, 170, bigMargin) && IsWithinRange(ltIndKnuckForPalmFor, 70, bigMargin) ||
                    //debug: unity standard airtap
                    IsWithinRange(p2pUp, 0, bigMargin) && IsWithinRange(p2pRt, 0, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 0, bigMargin) && IsWithinRange(ltPalmUpCamFor, 0, bigMargin) && IsWithinRange(rtIndMidForPalmFor, 83, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin) && IsWithinRange(ltIndMidForPalmFor, 83, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = true;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for vertical stack
                else if (IsWithinRange(rtIndMidForLtIndMidFor, 180, bigMargin) && IsWithinRange(rtIndMidUpLtIndMidUp, 0, bigMargin) && IsWithinRange(rtIndMidUpFloorUp, 0, bigMargin) && IsWithinRange(ltIndMidUpFloorUp, 0, bigMargin) && IsWithinRange(rtIndMidForCamRt, 180, bigMargin) && IsWithinRange(ltIndMidForCamRt, 0, bigMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = true;
                    forwardStack = false;
                }

                // look for forward stack
                else if (IsWithinRange(rtIndMidForLtIndMidFor, 180, bigMargin) && IsWithinRange(rtIndMidUpLtIndMidUp, 0, bigMargin) && IsWithinRange(rtIndMidUpFloorFor, 0, bigMargin) && IsWithinRange(ltIndMidUpFloorFor, 0, bigMargin) && IsWithinRange(rtIndMidForCamRt, 180, bigMargin) && IsWithinRange(ltIndMidForCamRt, 0, bigMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = true;
                }

                else
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }
            }
            else
            {
                palmsIn = false;
                fistsIn = false;
                palmsOut = false;
                pullUps = false;
                verticalStack = false;
                forwardStack = false;
            }

        }
        else
        {
            twoHands = false;
            fistsIn = false;
            palmsIn = false;
            palmsOut = false;
            pullUps = false;
            verticalStack = false;
            forwardStack = false;
        }
    }

    private bool IsWithinRange(float testVal, float target, int marginOfError)
    {
        bool withinRange = false;

        if (target == 0)
        {
            if (testVal <= target + marginOfError) withinRange = true;
        }
        else if (target == 180)
        {
            if (testVal >= 180 - marginOfError) withinRange = true;
        }
        else if (target > 0 && target < 180)
        {
            if (testVal >= target - marginOfError && testVal <= target + marginOfError) withinRange = true;
        }
        else withinRange = false;

        return withinRange;
    }
}
