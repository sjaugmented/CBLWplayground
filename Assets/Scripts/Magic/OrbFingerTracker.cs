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
    [Tooltip("Margins of error for poses")]
    [SerializeField] int forgivingMargin = 50;
    [SerializeField] int preciseMargin = 25;
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
    public float palmDist;

    

    Transform floor;

    // Start is called before the first frame update
    void Start()
    {
        floor = FindObjectOfType<LevelObject>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHands();
    }

    private void ProcessHands()
    {
        Transform cam = Camera.main.transform;

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
        #endregion

        #region Finger Ref Angles
        // get right finger angles
        float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
        float rtIndForCamFor = Vector3.Angle(rtIndexTip.Forward, cam.forward);
        float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
        float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
        float rtThumbForCamFor = Vector3.Angle(rtThumbTip.Forward, cam.forward);
        float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);
        float rtIndMidForPalmFor = Vector3.Angle(rtIndexMid.Forward, rightPalm.Forward);
        float rtIndKnuckForPalmFor = Vector3.Angle(rtIndexKnuckle.Forward, rightPalm.Forward);

        // get left finger angles
        float ltIndForPalmFor = Vector3.Angle(ltIndexTip.Forward, leftPalm.Forward);
        float ltIndForCamFor = Vector3.Angle(ltIndexTip.Forward, cam.forward);
        float ltMidForPalmFor = Vector3.Angle(ltMiddleTip.Forward, leftPalm.Forward);
        float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
        float ltThumbForCamFor = Vector3.Angle(ltThumbTip.Forward, cam.forward);
        float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
        float ltIndMidForPalmFor = Vector3.Angle(ltIndexMid.Forward, leftPalm.Forward);
        float ltIndKnuckForPalmFor = Vector3.Angle(ltIndexKnuckle.Forward, leftPalm.Forward);
        #endregion

        // look for only right fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip))
        {
            // look for rockOn
            if (IsWithinRange(rtIndForPalmFor, 0, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 0, forgivingMargin) && !IsWithinRange(rtMidForPalmFor, 0, forgivingMargin))
            {
                rockOnRight = true;
            }
            else
            {
                rockOnRight = false;
            }
        }
        else
        {
            rockOnRight = false;
        }

        // look for only left fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip))
        {
            // look for rockOn
            if (IsWithinRange(ltIndForPalmFor, 0, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 0, forgivingMargin) && !IsWithinRange(ltMidForPalmFor, 0, forgivingMargin))
            {
                rockOnLeft = true;
            }
            else
            {
                rockOnLeft = false;
            }
        }
        else
        {
            rockOnLeft = false;
        }

        // look for two palms
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            twoHands = true;

            palmDist = Vector3.Distance(rightPalm.Position, leftPalm.Position);

            // look for fingers both hands
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip))
            {
                // look for palms in
                if (IsWithinRange(p2pUp, 180, preciseMargin) && IsWithinRange(p2pRt, 180, preciseMargin) && IsWithinRange(p2pFor, 0, preciseMargin) && IsWithinRange(rtPalmUpCamFor, 90, preciseMargin) && IsWithinRange(ltPalmUpCamFor, 90, preciseMargin) && IsWithinRange(rtPalmUpFloorUp, 90, preciseMargin) && IsWithinRange(ltPalmUpFloorUp, 90, preciseMargin) && IsWithinRange(rtIndForPalmFor, 0, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 0, forgivingMargin) && IsWithinRange(rtMidForPalmFor, 0, forgivingMargin) && IsWithinRange(ltIndForPalmFor, 0, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 0, forgivingMargin) && IsWithinRange(ltMidForPalmFor, 0, forgivingMargin))
                {
                    palmsIn = true;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for fists in
                else if (IsWithinRange(p2pUp, 180, preciseMargin) && IsWithinRange(p2pRt, 180, preciseMargin) && IsWithinRange(p2pFor, 0, preciseMargin) && IsWithinRange(rtPalmUpCamFor, 90, preciseMargin) && IsWithinRange(ltPalmUpCamFor, 90, preciseMargin) && IsWithinRange(rtPalmUpFloorUp, 90, preciseMargin) && IsWithinRange(ltPalmUpFloorUp, 90, preciseMargin) && IsWithinRange(rtIndMidForPalmFor, 170, forgivingMargin) && IsWithinRange(rtIndKnuckForPalmFor, 70, forgivingMargin) && IsWithinRange(ltIndMidForPalmFor, 170, forgivingMargin) && IsWithinRange(ltIndKnuckForPalmFor, 70, forgivingMargin) 
                    ||
                    //debug: unity standard airtap
                    IsWithinRange(rtIndMidForPalmFor, 83, forgivingMargin) && IsWithinRange(rtMidForPalmFor, 160, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 129, forgivingMargin) && IsWithinRange(ltIndMidForPalmFor, 83, forgivingMargin) && IsWithinRange(ltMidForPalmFor, 160, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 129, forgivingMargin))
                {
                    palmsIn = false;
                    fistsIn = true;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for palmsOut, neutral fingers
                else if (IsWithinRange(p2pUp, 0, forgivingMargin) && IsWithinRange(p2pRt, 0, forgivingMargin) && IsWithinRange(p2pFor, 0, forgivingMargin) && IsWithinRange(rtPalmUpCamFor, 180, forgivingMargin) && IsWithinRange(ltPalmUpCamFor, 180, forgivingMargin) && IsWithinRange(rtIndForPalmFor, 20, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(rtMidForPalmFor, 20, forgivingMargin) && IsWithinRange(ltIndForPalmFor, 20, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(ltMidForPalmFor, 20, forgivingMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = true;
                    pullUps = false;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for pull up fists
                else if (IsWithinRange(p2pUp, 0, preciseMargin) && IsWithinRange(p2pRt, 180, preciseMargin) && IsWithinRange(p2pFor, 0, preciseMargin) && IsWithinRange(rtPalmUpCamFor, 0, preciseMargin) && IsWithinRange(ltPalmUpCamFor, 0, preciseMargin) && IsWithinRange(rtIndMidForPalmFor, 170, forgivingMargin) && IsWithinRange(rtIndKnuckForPalmFor, 70, forgivingMargin) && IsWithinRange(ltIndMidForPalmFor, 170, forgivingMargin) && IsWithinRange(ltIndKnuckForPalmFor, 70, forgivingMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = true;
                    verticalStack = false;
                    forwardStack = false;
                }

                // look for vertical stack
                else if (IsWithinRange(p2pUp, 0, forgivingMargin) && IsWithinRange(p2pRt, 180, forgivingMargin) && IsWithinRange(p2pFor, 180, forgivingMargin) && IsWithinRange(rtPalmUpCamFor, 90, forgivingMargin) && IsWithinRange(ltPalmUpCamFor, 90, forgivingMargin) && IsWithinRange(rtPalmRtCamFor, 0, forgivingMargin) && IsWithinRange(ltPalmRtCamFor, 180, forgivingMargin) && IsWithinRange(rtIndForPalmFor, 20, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(rtMidForPalmFor, 20, forgivingMargin) && IsWithinRange(ltIndForPalmFor, 20, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(ltMidForPalmFor, 20, forgivingMargin))
                {
                    palmsIn = false;
                    fistsIn = false;
                    palmsOut = false;
                    pullUps = false;
                    verticalStack = true;
                    forwardStack = false;
                }

                // look for forward stack
                else if (IsWithinRange(p2pUp, 0, forgivingMargin) && IsWithinRange(p2pRt, 180, forgivingMargin) && IsWithinRange(p2pFor, 180, forgivingMargin) && IsWithinRange(rtPalmUpFloorFor, 0, forgivingMargin) && IsWithinRange(ltPalmUpFloorFor, 0, forgivingMargin) && IsWithinRange(rtPalmRtFloorFor, 90, forgivingMargin) && IsWithinRange(ltPalmRtFloorFor, 90, forgivingMargin) && IsWithinRange(rtIndForPalmFor, 20, forgivingMargin) && IsWithinRange(rtPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(rtMidForPalmFor, 20, forgivingMargin) && IsWithinRange(ltIndForPalmFor, 20, forgivingMargin) && IsWithinRange(ltPinkForPalmFor, 20, forgivingMargin) && IsWithinRange(ltMidForPalmFor, 20, forgivingMargin))
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
