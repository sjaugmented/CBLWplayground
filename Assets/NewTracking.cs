using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core{
    public class NewTracking : MonoBehaviour
    {
        [SerializeField] float marginOfError = 40;
        [SerializeField] bool printAngles = true;

        MixedRealityPose rtIndex, rtMiddle, rtThumb, rtPalm;
        MixedRealityPose ltIndex, ltMiddle, ltThumb, ltPalm;

        public enum Hands {right, left, both, none}
        public enum Formation {palmsIn, palmsOut, together, parallel, none}
        public enum Pose {pointer, peace, flat, fist, none}

        public Hands handedness = Hands.none;
        public Formation palms = Formation.none;
        public Pose rightPose = Pose.none;
        public Pose leftPose = Pose.none;

        bool foundRtIndex;
        bool foundRtMiddle;
        bool foundRtThumb;
        bool foundRtPalm;
        bool foundLtIndex;
        bool foundLtMiddle;
        bool foundLtThumb;
        bool foundLtPalm;

        Transform cam;
        Transform floor;

        void Start()
        {
            cam = Camera.main.transform;
            floor = FindObjectOfType<LevelObject>().transform;
        }


        void Update()
        {
            foundRtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndex);
            foundRtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddle);
            foundRtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumb);
            foundRtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rtPalm);
            foundLtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out rtIndex);
            foundLtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out rtMiddle);
            foundLtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out rtThumb);
            foundLtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out rtPalm);
            
            WatchHands();
            WatchFormations();
            WatchRightFingers();
            WatchLeftFingers();
        }

        private void WatchHands()
        {
            if (foundRtPalm && foundLtPalm) {
                handedness = Hands.both;
            }
            else if (foundRtPalm && !foundLtPalm) {
                handedness = Hands.right;
            }
            else if (!foundRtPalm && foundLtPalm) {
                handedness = Hands.left;
            }
            else {
                handedness = Hands.none;
            }
        }

        private void WatchFormations()
        {
            throw new NotImplementedException();
        }

        private void WatchRightFingers()
        {
            float rtIndForward = Vector3.Angle(rtIndex.Forward, rtPalm.Forward);
            float rtIndForCamFor = Vector3.Angle(rtIndex.Forward, cam.forward);
            float rtMidForward = Vector3.Angle(rtMiddle.Forward, rtPalm.Forward);
            float rtThumbForward = Vector3.Angle(rtThumb.Forward, cam.forward);
            float rtThumbForPalmFor = Vector3.Angle(rtThumb.Forward, rtPalm.Forward);
            // float rtIndMidForCamRt = Vector3.Angle(rtIndexMid.Forward, cam.right);
            // float rtIndMidForPalmFor = Vector3.Angle(rtIndexMid.Forward, rtPalm.Forward);
            // float rtIndMidUpCamFor = Vector3.Angle(rtIndexMid.Up, cam.forward);
            // float rtIndMidUpFloorFor = Vector3.Angle(rtIndexMid.Up, floor.forward);
            // float rtIndMidUpFloorUp = Vector3.Angle(rtIndexMid.Up, floor.up);
            // float rtIndKnuckForPalmFor = Vector3.Angle(rtIndexKnuckle.Forward, rtPalm.Forward);

            if (!foundRtPalm) {
                rightPose = Pose.none;
            }

            // pointer
            if (IsPosed(rtIndForward, 0) && (!foundRtIndex || IsPosed(rtMidForward, 180))) {
                rightPose = Pose.pointer;
                Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>pointer");
            }

            // peace
            else if (IsPosed(rtIndForward, 0) && (IsPosed(rtMidForward, 0))) {
                rightPose = Pose.peace;
                Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>peace");
            }

            // flat
            else if (IsPosed(rtIndForward, 0) && (IsPosed(rtMidForward, 180)) && (IsPosed(rtThumbForward, 60))) {
                rightPose = Pose.flat;
                Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>flat");
            }

            // fist
            else if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140)) {
                rightPose = Pose.fist;
                Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>fist");
            }

            else {
                rightPose = Pose.none;
            }


            #region Debug land - TODO remove
            if (printAngles) {
                Debug.Log("rtIndexForward: " + rtIndForward);
                Debug.Log("rtMiddleForward: " + rtMidForward);
                Debug.Log("rtThumbForward: " + rtThumbForward);
            }

            if (!foundRtIndex) Debug.Log("LOST RIGHT INDEX");
            if (!foundRtMiddle) Debug.Log("LOST RIGHT MIDDLE");
            if (!foundRtThumb) Debug.Log("LOST RIGHT THUMB");
            if (!foundRtPalm) Debug.Log("LOST RIGHT PALM");
            #endregion
        }

        private void WatchLeftFingers()
        {
            throw new NotImplementedException();
        }

        private bool IsPosed(float pose, float target)
        {
            bool withinRange = false;

            float top = target + marginOfError;
            float bottom = target - marginOfError;
            if (pose > bottom && pose < top) {
                withinRange = true;
            }

            return withinRange;
        }
    }

}
