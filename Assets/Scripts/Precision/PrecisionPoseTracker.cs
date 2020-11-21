using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class PrecisionPoseTracker : MonoBehaviour
{
    // right hand joints
    public MixedRealityPose rightPalm, rtIndexTip, rtIndexMid, rtIndexKnuckle, rtMiddleTip, rtMiddleKnuckle, rtPinkyTip, rtThumbTip;
    // left hand joints
    public MixedRealityPose leftPalm, ltIndexTip, ltIndexMid, ltIndexKnuckle, ltMiddleTip, ltMiddleKnuckle, ltPinkyTip, ltThumbTip;
    public bool rightFist = false;
    public bool leftFist = false;
    public bool rightFlatHand = false;
    public bool rightKnifeHand = false;
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
        #region Palm Ref Angles
        // right palm
        float rtPalmUpFloorUp = Vector3.Angle(rightPalm.Up, floor.up);
        float rtPalmUpFloorFor = Vector3.Angle(rightPalm.Up, floor.forward);
        float rtPalmUpFloorRt = Vector3.Angle(rightPalm.Up, floor.right);
        float rtPalmForFloorUp = Vector3.Angle(rightPalm.Forward, floor.up);
        float rtPalmForFloorFor = Vector3.Angle(rightPalm.Forward, floor.forward);
        float rtPalmForFloorRt = Vector3.Angle(rightPalm.Forward, floor.right);
        float rtPalmRtFloorUp = Vector3.Angle(rightPalm.Right, floor.up);
        float rtPalmRtFloorFor = Vector3.Angle(rightPalm.Right, floor.forward);
        float rtPalmRtFloorRt = Vector3.Angle(rightPalm.Right, floor.right);


        // left palm
        float ltPalmUpFloorUp = Vector3.Angle(leftPalm.Up, floor.up);
        float ltPalmUpFloorFor = Vector3.Angle(leftPalm.Up, floor.forward);
        float ltPalmUpFloorRt = Vector3.Angle(leftPalm.Up, floor.right);
        float ltPalmForFloorUp = Vector3.Angle(leftPalm.Forward, floor.up);
        float ltPalmForFloorFor = Vector3.Angle(leftPalm.Forward, floor.forward);
        float ltPalmForFloorRt = Vector3.Angle(leftPalm.Forward, floor.right);
        float ltPalmRtFloorUp = Vector3.Angle(leftPalm.Right, floor.up);
        float ltPalmRtFloorFor = Vector3.Angle(leftPalm.Right, floor.forward);
        float ltPalmRtFloorRt = Vector3.Angle(leftPalm.Right, floor.right);
        #endregion

        #region Finger Ref Angles
        // get right finger angles
        float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
        float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
        float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
        float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);
        float rtIndMidForPalmFor = Vector3.Angle(rtIndexMid.Forward, rightPalm.Forward);
        float rtIndKnuckForPalmFor = Vector3.Angle(rtIndexKnuckle.Forward, rightPalm.Forward);

        // get left finger angles
        float ltIndForPalmFor = Vector3.Angle(ltIndexTip.Forward, leftPalm.Forward);
        float ltMidForPalmFor = Vector3.Angle(ltMiddleTip.Forward, leftPalm.Forward);
        float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
        float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
        float ltIndMidForPalmFor = Vector3.Angle(ltIndexMid.Forward, leftPalm.Forward);
        float ltIndKnuckForPalmFor = Vector3.Angle(ltIndexKnuckle.Forward, leftPalm.Forward);
        #endregion

        // look for right hand
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm))
        {

            // debug - unity standard tap
            if (IsWithinRange(rtIndMidForPalmFor, 83) && IsWithinRange(rtMidForPalmFor, 160) && IsWithinRange(rtPinkForPalmFor, 129))
            {
                rightFist = true;
                rightFlatHand = false;
                rightKnifeHand = false;
            }

            // look for right fist
            else if (IsWithinRange(rtIndMidForPalmFor, 140) && IsWithinRange(rtMidForPalmFor, 140) && IsWithinRange(rtPinkForPalmFor, 130))
            {

                rightFist = true;
                rightFlatHand = false;
                rightKnifeHand = false;
            }

            // look for right flat
            else if (IsWithinRange(rtIndMidForPalmFor, 0) && IsWithinRange(rtMidForPalmFor, 0) && IsWithinRange(rtPinkForPalmFor, 0) && IsWithinRange(rtPalmUpFloorUp, 0) && IsWithinRange(rtPalmRtFloorRt, 0))
            {
                rightFist = false;
                rightFlatHand = true;
                rightKnifeHand = false;
            }

            // look for right knife
            else if (IsWithinRange(rtIndMidForPalmFor, 0) && IsWithinRange(rtMidForPalmFor, 0) && IsWithinRange(rtPinkForPalmFor, 0) && IsWithinRange(rtPalmUpFloorUp, 90) && IsWithinRange(rtPalmRtFloorRt, 90))
            {
                rightFist = false;
                rightFlatHand = false;
                rightKnifeHand = true;
            }

            // if no hand positions match
            else
            {
                rightFist = false;
                rightFlatHand = false;
                rightKnifeHand = false;
            }
        }
        // if no right hand
        else
        {
            rightFist = false;
            rightFlatHand = false;
            rightKnifeHand = false;
        }

        // look for left hand
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {

            // debug - unity standard tap
            if (IsWithinRange(ltIndMidForPalmFor, 83) && IsWithinRange(ltMidForPalmFor, 160) && IsWithinRange(ltPinkForPalmFor, 129))
            {
                leftFist = true;
                leftFlatHand = false;
                leftKnifeHand = false;
            }

            // look for left fist
            else if (IsWithinRange(ltIndMidForPalmFor, 140) && IsWithinRange(ltMidForPalmFor, 140) && IsWithinRange(ltPinkForPalmFor, 130))
            {

                leftFist = true;
                leftFlatHand = false;
                leftKnifeHand = false;
            }

            // look for left flat
            else if (IsWithinRange(ltIndMidForPalmFor, 0) && IsWithinRange(ltMidForPalmFor, 0) && IsWithinRange(ltPinkForPalmFor, 0) && IsWithinRange(ltPalmUpFloorUp, 0) && IsWithinRange(ltPalmRtFloorRt, 0))
            {
                leftFist = false;
                leftFlatHand = true;
                leftKnifeHand = false;
            }

            // look for left knife
            else if (IsWithinRange(ltIndMidForPalmFor, 0) && IsWithinRange(ltMidForPalmFor, 0) && IsWithinRange(ltPinkForPalmFor, 0) && IsWithinRange(ltPalmUpFloorUp, 90) && IsWithinRange(ltPalmRtFloorRt, 90))
            {
                leftFist = false;
                leftFlatHand = false;
                leftKnifeHand = true;
            }

            // if no hand positions match
            else
            {
                leftFist = false;
                leftFlatHand = false;
                leftKnifeHand = false;
            }
        }

        // if no left hand
        else
        {
            leftFist = false;
            leftFlatHand = false;
            leftKnifeHand = false;
        }
    }

    private bool IsWithinRange(float testVal, float target)
    {
        bool withinRange;
        if (testVal >= target - 50 && testVal <= target + 50) withinRange = true;
        else withinRange = false;

        return withinRange;
    }
}
