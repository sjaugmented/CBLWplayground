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

        public enum Hands {right, left, both, none}
        public enum Direction {up, down, palmOut, palmIn, side, none};
        public enum Formation {palmsIn, palmsOut, together, palmsUp, palmsDown, none}
        public enum Pose {pointer, peace, flat, fist, none}

        public Hands handedness = Hands.none;
        public Formation palms = Formation.none;
        public Direction rightPalm = Direction.none;
        public Direction leftPalm = Direction.none;
        public Pose rightPose = Pose.none;
        public Pose leftPose = Pose.none;
        
        MixedRealityPose rtIndex, rtMiddle, rtThumb, rtPalm;
        MixedRealityPose ltIndex, ltMiddle, ltThumb, ltPalm;

        bool foundRtIndex, foundRtMiddle, foundRtThumb, foundRtPalm;
        bool foundLtIndex, foundLtMiddle, foundLtThumb, foundLtPalm;

        Vector3 staff;
        public float staffForward, staffUp, staffRight, staffFloorUp, staffFloorForward;
        public float rtPalmForward, rtPalmUp, rtPalmRt, ltPalmForward, ltPalmUp, ltPalmRt;

        Transform cam;
        Transform floor;

        void Start()
        {
            cam = Camera.main.transform;
            floor = FindObjectOfType<LevelObject>().transform;
        }


        void Update()
        {
            FindJoints();
            WatchHands();
            WatchRightPalm();
            WatchLeftPalm();
            WatchFormations();
            WatchStaff();
            WatchRightFingers();
            WatchLeftFingers();
        }

        private void FindJoints()
        {
            foundRtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndex);
            foundRtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddle);
            foundRtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumb);
            foundRtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rtPalm);

            foundLtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndex);
            foundLtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddle);
            foundLtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumb);
            foundLtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out ltPalm);
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

        private void WatchRightPalm() 
        {
            rtPalmForward = Vector3.Angle(rtPalm.Up, cam.forward);
            rtPalmUp = Vector3.Angle(rtPalm.Up, floor.up);
            rtPalmRt = Vector3.Angle(rtPalm.Up, cam.right);

            if (!foundRtPalm) {
                rightPalm = Direction.none;
            }

            if (IsPosed(rtPalmForward, 0)) {
                rightPalm = Direction.palmIn;
            }

            if (IsPosed(rtPalmForward, 180)) {
                rightPalm = Direction.palmOut;
            }
            
            if (IsPosed(rtPalmUp, 0)) {
                rightPalm = Direction.down;
            }
            
            if (IsPosed(rtPalmUp, 180)) {
                rightPalm = Direction.up;
            }

            if (IsPosed(rtPalmRt, 0)) {
                rightPalm = Direction.side;
            }
        }

        private void WatchLeftPalm() 
        {
            ltPalmForward = Vector3.Angle(ltPalm.Up, cam.forward);
            ltPalmUp = Vector3.Angle(ltPalm.Up, floor.up);
            ltPalmRt = Vector3.Angle(ltPalm.Up, cam.right);

            if (!foundLtPalm) {
                leftPalm = Direction.none;
            }

            if (IsPosed(ltPalmForward, 0)) {
                leftPalm = Direction.palmIn;
            }

            if (IsPosed(ltPalmForward, 180)) {
                leftPalm = Direction.palmOut;
            }
            
            if (IsPosed(ltPalmUp, 0)) {
                leftPalm = Direction.down;
            }
            
            if (IsPosed(ltPalmUp, 180)) {
                leftPalm = Direction.up;
            }

            if (IsPosed(ltPalmRt, 180)) {
                leftPalm = Direction.side;
            }
        }

        private void WatchFormations()
        {
            if (!foundRtPalm || !foundLtPalm) { palms = Formation.none; }
            
            if (rightPalm == Direction.side && leftPalm == Direction.side) {
                palms = Formation.together;
            }

            if (rightPalm == Direction.palmOut && leftPalm == Direction.palmOut) {
                palms = Formation.palmsOut;
            }

            if (rightPalm == Direction.palmIn && leftPalm == Direction.palmIn) {
                palms = Formation.palmsIn;
            }

            if (rightPalm == Direction.up && leftPalm == Direction.up) {
                palms = Formation.palmsUp;
            }
            
            if (rightPalm == Direction.down && leftPalm == Direction.down) {
                palms = Formation.palmsDown;
            }
        }

        private void WatchStaff() 
        {
            if (handedness != Hands.both) { return; }
            
            staff = rtPalm.Position - ltPalm.Position;
            staffUp = Vector3.Angle(staff, cam.up);
            staffForward = Vector3.Angle(staff, cam.forward);
            staffRight = Vector3.Angle(staff, cam.right);
            staffFloorForward = Vector3.Angle(staff, floor.forward);
            staffFloorUp = Vector3.Angle(staff, floor.up);
        }

        private void WatchRightFingers()
        {
            float rtIndForward = Vector3.Angle(rtIndex.Forward, rtPalm.Forward);
            float rtIndForCamFor = Vector3.Angle(rtIndex.Forward, cam.forward);
            float rtMidForward = Vector3.Angle(rtMiddle.Forward, rtPalm.Forward);
            float rtThumbOut = Vector3.Angle(rtThumb.Forward, rtPalm.Right);
            float rtThumbForward = Vector3.Angle(rtThumb.Forward, rtPalm.Forward);

            if (!foundRtPalm) {
                rightPose = Pose.none;
            }

            if (IsPosed(rtIndForward, 0) && (!foundRtMiddle || !IsPosed(rtMidForward, 0))) {
                rightPose = Pose.pointer;
            }

            else if (IsPosed(rtIndForward, 0) &&(IsPosed(rtMidForward, 0) || !IsPosed(rtMidForward, 180) )&& (IsPosed(rtThumbOut, 20) || IsPosed(rtThumbOut, 50))) {
                rightPose = Pose.peace;
            }

            else if (IsPosed(rtIndForward, 0) && (IsPosed(rtMidForward, 0)) && (IsPosed(rtThumbOut, 130))) {
                rightPose = Pose.flat;
            }

            else if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140) && (IsPosed(rtThumbOut, 20) || IsPosed(rtThumbOut, 50))) {
                rightPose = Pose.fist;
            }

            else {
                rightPose = Pose.none;
            }


            #region Debug land - TODO remove
            if (printAngles) {
                Debug.Log("rtIndexForward: " + rtIndForward);
                Debug.Log("rtMiddleForward: " + rtMidForward);
                Debug.Log("rtThumbOut: " + rtThumbOut);
            }

            // if (!foundRtIndex) Debug.Log("LOST RIGHT INDEX");
            // if (!foundRtMiddle) Debug.Log("LOST RIGHT MIDDLE");
            // if (!foundRtThumb) Debug.Log("LOST RIGHT THUMB");
            // if (!foundRtPalm) Debug.Log("LOST RIGHT PALM");
            #endregion
        }

        private void WatchLeftFingers()
        {
            float ltIndForward = Vector3.Angle(ltIndex.Forward, ltPalm.Forward);
            float ltIndForCamFor = Vector3.Angle(ltIndex.Forward, cam.forward);
            float ltMidForward = Vector3.Angle(ltMiddle.Forward, ltPalm.Forward);
            float ltThumbOut = Vector3.Angle(ltThumb.Forward, ltPalm.Right);
            float ltThumbForward = Vector3.Angle(ltThumb.Forward, ltPalm.Forward);

            if (!foundLtPalm) {
                leftPose = Pose.none;
            }

            // pointer
            if (IsPosed(ltIndForward, 0) && (!foundLtMiddle || !IsPosed(ltMidForward, 0))) {
                leftPose = Pose.pointer;
            }

            // peace
            else if (IsPosed(ltIndForward, 0) && (IsPosed(ltMidForward, 0) || !IsPosed(ltMidForward, 180)) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = Pose.peace;
            }

            // flat
            else if (IsPosed(ltIndForward, 0) && (IsPosed(ltMidForward, 0)) && (IsPosed(ltThumbOut, 30))) {
                leftPose = Pose.flat;
            }

            // fist
            else if (IsPosed(ltIndForward, 140) && IsPosed(ltMidForward, 140) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = Pose.fist;
            }

            else {
                leftPose = Pose.none;
            }


            #region Debug land - TODO remove
            if (printAngles) {
                Debug.Log("ltIndexForward: " + ltIndForward);
                Debug.Log("ltMiddleForward: " + ltMidForward);
                Debug.Log("ltThumbOut: " + ltThumbOut);
            }

            // if (!foundLtIndex) Debug.Log("LOST LEFT INDEX");
            // if (!foundLtMiddle) Debug.Log("LOST LEFT MIDDLE");
            // if (!foundLtThumb) Debug.Log("LOST LEFT THUMB");
            // if (!foundLtPalm) Debug.Log("LOST LEFT PALM");
            #endregion
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
