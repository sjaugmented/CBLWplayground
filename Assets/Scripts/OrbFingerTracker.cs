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

    // right hand joints
    MixedRealityPose rightPalm, rtIndexTip, rtIndexMid, rtIndexKnuckle, rtMiddleTip, rtMiddleKnuckle, rtPinkyTip, rtThumbTip;
    // left hand joints
    MixedRealityPose leftPalm, ltIndexTip, ltIndexMid, ltIndexKnuckle, ltMiddleTip, ltMiddleKnuckle, ltPinkyTip, ltThumbTip;
    float castFingerUpThresh = 0.3f;
    bool castFingerOut = false;

    public bool twoPalms = false;
    public bool touchDown = false;
    public bool palmsForward = false;
    public bool palmsIn = false;
    public bool rockOnRight = false;
    public bool fingerGunRight = false;
    public bool rockOnLeft = false;
    public bool fingerGunLeft = false;
    float palmDist;

    public bool palmsParallel = false;
    public bool fists = false;

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
        
        // look for two palms
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            twoPalms = true;
            fingerGunRight = false;
            fingerGunLeft = false;
            rockOnRight = false;
            rockOnLeft = false;

            palmDist = Vector3.Distance(rightPalm.Position, leftPalm.Position);

            #region Joint Reference Angles
            // get reference angles
            // palm to palm
            float p2pUp = Vector3.Angle(rightPalm.Up, leftPalm.Up);
            float p2pRt = Vector3.Angle(rightPalm.Right, leftPalm.Right);
            float p2pFor = Vector3.Angle(rightPalm.Forward, leftPalm.Forward);

            // right palm
            float rtPalmUpCamFor = Vector3.Angle(rightPalm.Up, cam.forward);
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
            float ltPalmForCamFor = Vector3.Angle(leftPalm.Forward, cam.forward);
            float ltPalmRtCamFor = Vector3.Angle(leftPalm.Right, cam.forward);
            float ltPalmRtCamRt = Vector3.Angle(leftPalm.Right, cam.right);
            float ltPalmRtCamUp = Vector3.Angle(leftPalm.Right, cam.up);
            float ltPalmForFloorFor = Vector3.Angle(leftPalm.Forward, floor.forward);
            float ltPalmRtFloorFor = Vector3.Angle(leftPalm.Right, floor.forward);
            float ltPalmUpCamRt = Vector3.Angle(leftPalm.Up, cam.right);
            float ltPalmForCamRt = Vector3.Angle(leftPalm.Forward, cam.right);
            #endregion


            // look for palms parrallel
            if (IsWithinRange(p2pUp, 180) && IsWithinRange(p2pRt, 180) && IsWithinRange(p2pFor, 0) && IsWithinRange(rtPalmUpCamFor, 90) && IsWithinRange(ltPalmUpCamFor, 90) && IsWithinRange(rtPalmUpCamRt, 0) && IsWithinRange(ltPalmUpCamRt, 180) && IsWithinRange(rtPalmForCamRt, 90) && IsWithinRange(ltPalmForCamRt, 90))
            {
                palmsParallel = true;
                palmsForward = false;

                // look for fingers both hands
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip))
                {
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

                    if (IsWithinRange(rtIndMidForPalmFor, 170) && IsWithinRange(rtIndKnuckForPalmFor, 70) && IsWithinRange(ltIndMidForPalmFor, 170) && IsWithinRange(ltIndKnuckForPalmFor, 70))
                    {
                        fists = true;
                    }
                    else fists = false;
                }
            } 

            // look for palmsOut 
            else if (IsWithinRange(p2pUp, 0) && IsWithinRange(p2pRt, 0) && IsWithinRange(p2pFor, 0))
            {
                palmsParallel = false;
                palmsForward = true;
                
            }
            else
            {
                palmsParallel = false;
                palmsForward = false;
            }
        }
        else
        {
            palmsParallel = false;
            palmsForward = false;
        }
        // look for right fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip))
        {
            #region Finger Ref Angles
            // get finger angles
            float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
            float rtIndForCamFor = Vector3.Angle(rtIndexTip.Forward, cam.forward);
            float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
            float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
            float rtThumbForCamFor = Vector3.Angle(rtThumbTip.Forward, cam.forward);
            float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);
            #endregion

            // look for rockOn
            if (IsWithinRange(rtIndForPalmFor, 20) && IsWithinRange(rtPinkForPalmFor, 20) && !IsWithinRange(rtMidForPalmFor, 0))
            {
                rockOnRight = true;
                fingerGunRight = false;
            }
            // look for fingerGun
            else if (IsWithinRange(rtIndForCamFor, 0) && IsWithinRange(rtIndForPalmFor, 20) && IsWithinRange(rtThumbForCamFor, 90) && IsWithinRange(rtMidForPalmFor, 180) && IsWithinRange(rtPinkForPalmFor, 180))
            {
                rockOnRight = false;
                fingerGunRight = true;
            }
            else
            {
                rockOnRight = false;
                fingerGunRight = false;
            }
        }
        else
        {
            rockOnRight = false;
            fingerGunRight = false;
        }
        // look for left fingers
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip))
        {
            #region Finger Ref Angles
            // get finger angles
            float ltIndForPalmFor = Vector3.Angle(ltIndexTip.Forward, leftPalm.Forward);
            float ltIndForCamFor = Vector3.Angle(ltIndexTip.Forward, cam.forward);
            float ltMidForPalmFor = Vector3.Angle(ltMiddleTip.Forward, leftPalm.Forward);
            float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
            float ltThumbForCamFor = Vector3.Angle(ltThumbTip.Forward, cam.forward);
            float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
            #endregion

            // look for rockOn
            if (IsWithinRange(ltIndForPalmFor, 20) && IsWithinRange(ltPinkForPalmFor, 20) && !IsWithinRange(ltMidForPalmFor, 0))
            {
                rockOnLeft = true;
                fingerGunLeft = false;
            }
            // look for fingerGun
            else if (IsWithinRange(ltIndForCamFor, 0) && IsWithinRange(ltIndForPalmFor, 20) && IsWithinRange(ltThumbForCamFor, 90) && IsWithinRange(ltMidForPalmFor, 180) && IsWithinRange(ltPinkForPalmFor, 180))
            {
                rockOnLeft = false;
                fingerGunLeft = true;
            }
            else
            {
                rockOnLeft = false;
                fingerGunLeft = false;
            }
        }
        else
        {
            rockOnLeft = false;
            fingerGunLeft = false;
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

    #region Public Getters
    public bool GetPalmsParallel()
    {
        return palmsParallel;
    }

    public bool GetFists()
    {
        return fists;
    }

    public Vector3 GetRtPalmPos()
    {
        return rightPalm.Position;
    }

    public Vector3 GetLtPalmPos()
    {
        return leftPalm.Position;
    }

    public Quaternion GetRtPalmRot()
    {
        return rightPalm.Rotation;
    }

    public Quaternion GetLtPalmRot()
    {
        return leftPalm.Rotation;
    }

    public Vector3 GetRtIndexPos()
    {
        return rtIndexTip.Position;
    }

    public Vector3 GetLtIndexPos()
    {
        return ltIndexTip.Position;
    }

    public Vector3 GetRtPinkyPos()
    {
        return rtPinkyTip.Position;
    }

    public Vector3 GetLtPinkyPos()
    {
        return ltPinkyTip.Position;
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

    public bool GetRockOnRight()
    {
        return rockOnRight;
    }

    public bool GetFingerGunRight()
    {
        return fingerGunRight;
    }

    public bool GetRockOnLeft()
    {
        return rockOnLeft;
    }

    public bool GetFingerGunLeft()
    {
        return fingerGunLeft;
    }

    public Quaternion GetRtIndRot()
    {
        return rtIndexTip.Rotation;
    }

    public Quaternion GetLtIndRot()
    {
        return ltIndexTip.Rotation;
    }
    #endregion
}
