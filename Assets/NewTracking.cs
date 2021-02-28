﻿using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core{
    public class NewTracking : MonoBehaviour
    {
        [SerializeField] float marginOfError = 40;
        [SerializeField] bool printAngles = true;

        // TODO make private
        public enum Hands {right, left, both, none}
        public enum Direction {up, down, palmOut, palmIn, side, none};
        public enum Formation {palmsIn, palmsOut, together, palmsUp, palmsDown, none}
        public enum Pose {pointer, peace, flat, fist, gun, rockOn, any, none}

        // TODO make private
        public Hands handedness = Hands.none;
        public Formation palms = Formation.none;
        public Direction rightPalm = Direction.none;
        public Direction leftPalm = Direction.none;
        public Pose rightPose = Pose.none;
        public Pose leftPose = Pose.none;
        
        MixedRealityPose rtIndex, rtMiddle, rtPinky, rtThumb, rtPalm;
        MixedRealityPose ltIndex, ltMiddle, ltPinky, ltThumb, ltPalm;

        bool foundRtIndex, foundRtMiddle, foundRtPinky, foundRtThumb, foundRtPalm;
        bool foundLtIndex, foundLtMiddle, foundLtPinky, foundLtThumb, foundLtPalm;

        Vector3 staff;
        float staffForward, staffUp, staffRight, staffFloorUp, staffFloorForward;
        float rtPalmForward, rtPalmUp, rtPalmRt, ltPalmForward, ltPalmUp, ltPalmRt;

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
            FindHands();

            WatchPalms();

            WatchFormations();
            
            WatchStaff();

            WatchPoses();
        }

        private void FindJoints()
        {
            foundRtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndex);
            foundRtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddle);
            foundRtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumb);
            foundRtPinky = HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinky);
            foundRtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rtPalm);

            foundLtIndex = HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndex);
            foundLtMiddle = HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddle);
            foundLtThumb = HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumb);
            foundLtPinky = HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinky);
            foundLtPalm = HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out ltPalm);

            // TODO create test, if any of the above are ever false, trigger a HUD element
        }

        private void FindHands()
        {
            if (foundRtPalm && foundLtPalm) {
                handedness = Hands.both;
            }
            else if (foundRtPalm) {
                handedness = Hands.right;
            }
            else if (foundLtPalm) {
                handedness = Hands.left;
            }
            else {
                handedness = Hands.none;
            }
        }

        private void WatchPalms() 
        {
            rtPalmForward = Vector3.Angle(rtPalm.Up, cam.forward);
            rtPalmUp = Vector3.Angle(rtPalm.Up, floor.up);
            rtPalmRt = Vector3.Angle(rtPalm.Up, cam.right);

            ltPalmForward = Vector3.Angle(ltPalm.Up, cam.forward);
            ltPalmUp = Vector3.Angle(ltPalm.Up, floor.up);
            ltPalmRt = Vector3.Angle(ltPalm.Up, cam.right);

            if (!foundRtPalm) {
                rightPalm = Direction.none;
            }
            if (!foundLtPalm) {
                leftPalm = Direction.none;
            }

            if (IsPosed(rtPalmForward, 0)) {
                rightPalm = Direction.palmIn;
            }
            if (IsPosed(ltPalmForward, 0)) {
                leftPalm = Direction.palmIn;
            }

            if (IsPosed(rtPalmForward, 180)) {
                rightPalm = Direction.palmOut;
            }
            if (IsPosed(ltPalmForward, 180)) {
                leftPalm = Direction.palmOut;
            }
            
            if (IsPosed(rtPalmUp, 0)) {
                rightPalm = Direction.down;
            }
            if (IsPosed(ltPalmUp, 0)) {
                leftPalm = Direction.down;
            }
            
            if (IsPosed(rtPalmUp, 180)) {
                rightPalm = Direction.up;
            }
            if (IsPosed(ltPalmUp, 180)) {
                leftPalm = Direction.up;
            }

            if (IsPosed(rtPalmRt, 0)) {
                rightPalm = Direction.side;
            }
            if (IsPosed(ltPalmRt, 180)) {
                leftPalm = Direction.side;
            }
        }

        private void WatchFormations()
        {
            float palmToPalm = Vector3.Angle(rtPalm.Up, ltPalm.Up);
            
            if (!foundRtPalm || !foundLtPalm) { palms = Formation.none; }
            
            if (IsPosed(palmToPalm, 180)) {
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

        private void WatchPoses()
        {
            #region finger angles
            float rtIndForward = Vector3.Angle(rtIndex.Forward, rtPalm.Forward);
            float rtIndForCamFor = Vector3.Angle(rtIndex.Forward, cam.forward);
            float rtMidForward = Vector3.Angle(rtMiddle.Forward, rtPalm.Forward);
            float rtPinkyForward = Vector3.Angle(rtPinky.Forward, rtPalm.Forward);
            float rtThumbOut = Vector3.Angle(rtThumb.Forward, rtPalm.Right * -1); // inverted for symmetry with left thumb values

            float ltIndForward = Vector3.Angle(ltIndex.Forward, ltPalm.Forward);
            float ltIndForCamFor = Vector3.Angle(ltIndex.Forward, cam.forward);
            float ltMidForward = Vector3.Angle(ltMiddle.Forward, ltPalm.Forward);
            float ltPinkyForward = Vector3.Angle(ltPinky.Forward, ltPalm.Forward);
            float ltThumbOut = Vector3.Angle(ltThumb.Forward, ltPalm.Right);
            #endregion

            if (IsPosed(rtIndForward, 0) && (IsPosed(rtMidForward, 0) || !IsPosed(rtMidForward, 180)) && (IsPosed(rtPinkyForward, 180) || !IsPosed(rtPinkyForward, 0)) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = Pose.peace;
            }
            else if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = Pose.fist;
            }
            else if (IsPosed(rtIndForward, 0) && (IsPosed(rtMidForward, 0)) && IsPosed(rtPinkyForward, 0) && IsPosed(rtThumbOut, 30)) {
                rightPose = Pose.flat;
            }
            else if (IsPosed(rtIndForward, 0) && (!IsPosed(rtMidForward, 0))) {
                rightPose = Pose.pointer;
            }
            else { rightPose = Pose.any; }
            
            if (IsPosed(ltIndForward, 0) && (IsPosed(ltMidForward, 0) || !IsPosed(ltMidForward, 180)) && (IsPosed(ltPinkyForward, 180) || !IsPosed(ltPinkyForward, 0)) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = Pose.peace;
            }
            else if (IsPosed(ltIndForward, 140) && IsPosed(ltMidForward, 140) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = Pose.fist;
            }
            else if (IsPosed(ltIndForward, 0) && IsPosed(ltMidForward, 0) && IsPosed(ltPinkyForward, 0) && IsPosed(ltThumbOut, 30)) {
                leftPose = Pose.flat;
            }
            else if (IsPosed(ltIndForward, 0) && (!IsPosed(ltMidForward, 0))) {
                leftPose = Pose.pointer;
            }
            else { leftPose = Pose.any; }

            #region Debug land - TODO remove
            if (printAngles) {
                Debug.Log("rtIndexForward: " + rtIndForward);
                Debug.Log("rtMiddleForward: " + rtMidForward);
                Debug.Log("rtPinkyForward: " + rtPinkyForward);
                Debug.Log("rtThumbOut: " + rtThumbOut);
                Debug.Log("ltIndexForward: " + ltIndForward);
                Debug.Log("ltMiddleForward: " + ltMidForward);
                Debug.Log("ltPinkyForward: " + ltPinkyForward);
                Debug.Log("ltThumbOut: " + ltThumbOut);
            }
            #endregion
        }

        private bool IsPosed(float pose, float target)
        {
            return (pose < target + marginOfError && pose > target - marginOfError);
        }
    }
}
