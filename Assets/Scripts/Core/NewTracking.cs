using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core {
    
    public enum Hands {right, left, both, none}
    public enum Direction {up, down, palmOut, palmIn, side, none};
    public enum Formation {palmsIn, palmsOut, together, palmsUp, palmsDown, none}
    public enum HandPose {pointer, peace, flat, fist, gun, rockOn, any, none}

    public class NewTracking : MonoBehaviour
    {
        [SerializeField] float marginOfError = 40;
        [SerializeField] bool printAngles = true;

        public Hands handedness = Hands.none;
        public Formation palms = Formation.none;
        public Direction rightPalm = Direction.none;
        public Direction leftPalm = Direction.none;
        public HandPose rightPose = HandPose.none;
        public HandPose leftPose = HandPose.none;
        
        MixedRealityPose rtIndex, rtMiddle, rtPinky, rtThumb, rtPalm;
        MixedRealityPose ltIndex, ltMiddle, ltPinky, ltThumb, ltPalm;

        bool foundRtIndex, foundRtMiddle, foundRtPinky, foundRtThumb, foundRtPalm;
        bool foundLtIndex, foundLtMiddle, foundLtPinky, foundLtThumb, foundLtPalm;
        
        Vector3 staff;
        float staffForward, staffUp, staffRight, staffFloorUp, staffFloorForward;
        float rtPalmForward, rtPalmUp, rtPalmIn, rtLauncher, ltPalmForward, ltPalmUp, ltPalmIn, ltLauncher;
        
        #region Getters
        public MixedRealityPose GetRtPalm { get { return rtPalm; } }
        public MixedRealityPose GetLtPalm { get {return ltPalm;} }
        public MixedRealityPose GetRtIndex { get {return rtIndex;} }
        public MixedRealityPose GetRtMiddle { get { return rtMiddle; } }
        public MixedRealityPose GetLtIndex { get {return ltIndex;} }
        public MixedRealityPose GetLtMiddle { get { return ltMiddle; } }
        public MixedRealityPose GetRtPinky { get { return rtPinky; } }
        public MixedRealityPose GetLtPinky { get { return ltPinky; } }
        public bool FoundRightHand { get { return foundRtPalm; } }
        public bool FoundLeftHand { get { return foundLtPalm; } }
        public float StaffUp { get { return staffUp; } }
        public float StaffForward { get { return staffForward; } }
        public float RtLauncher { get { return rtLauncher; } }
        public float LtLauncher { get { return ltLauncher; } }
        #endregion

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

            WatchPalmDirections();

            WatchStaff();
            WatchFormations();
            
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

        private void WatchPalmDirections() 
        {
            rtPalmForward = Vector3.Angle(rtPalm.Up, cam.forward);
            rtPalmUp = Vector3.Angle(rtPalm.Up, floor.up);
            rtPalmIn = Vector3.Angle(rtPalm.Up, cam.right);
            rtLauncher = Vector3.Angle(rtPalm.Right, cam.up); // TODO move launchers off of NewTracker

            ltPalmForward = Vector3.Angle(ltPalm.Up, cam.forward);
            ltPalmUp = Vector3.Angle(ltPalm.Up, floor.up);
            ltPalmIn = Vector3.Angle(ltPalm.Up, cam.right * -1); // inverted for symmetry
            ltLauncher = Vector3.Angle(ltPalm.Right, cam.up);

            if (!foundRtPalm) {
                rightPalm = Direction.none;
            }
            if (!foundLtPalm) {
                leftPalm = Direction.none;
            }

            if (IsPosed(rtPalmForward, 20)) {
                rightPalm = Direction.palmIn;
            }
            if (IsPosed(ltPalmForward, 20)) {
                leftPalm = Direction.palmIn;
            }

            if (IsPosed(rtPalmForward, 150)) {
                rightPalm = Direction.palmOut;
            }
            if (IsPosed(ltPalmForward, 150)) {
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

            if (IsPosed(rtPalmIn, 0)) {
                rightPalm = Direction.side;
            }
            if (IsPosed(ltPalmIn, 0)) {
                leftPalm = Direction.side;
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

        private void WatchFormations()
        {
            float palmToPalm = Vector3.Angle(rtPalm.Up, ltPalm.Up);

            if (!foundRtPalm || !foundLtPalm || ((IsPosed(staffUp, 90) || IsPosed(staffRight, 0)) && ((rightPalm == Direction.up && leftPalm == Direction.down) || (rightPalm == Direction.down && leftPalm == Direction.up)))) 
            { 
                palms = Formation.none; 
            }

            else if (IsPosed(palmToPalm, 180)) {
                palms = Formation.together;
            }

            else if (rightPalm == Direction.palmOut && leftPalm == Direction.palmOut) {
                palms = Formation.palmsOut;
            }

            else if (rightPalm == Direction.palmIn && leftPalm == Direction.palmIn) {
                palms = Formation.palmsIn;
            }

            else if (rightPalm == Direction.up && leftPalm == Direction.up) {
                palms = Formation.palmsUp;
            }

            else if (rightPalm == Direction.down && leftPalm == Direction.down) {
                palms = Formation.palmsDown;
            }

            else
            {
                palms = Formation.none;
            }
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

            if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = HandPose.fist;
            }
            else if (IsPosed(rtIndForward, 0) && IsPosed(rtMidForward, 0) && IsPosed(rtPinkyForward, 0) && IsPosed(rtThumbOut, 30)) {
                rightPose = HandPose.flat;
            }
            else if (IsPosed(rtIndForward, 0) && IsPosed(rtMidForward, 0) && (IsPosed(rtPinkyForward, 180) || !IsPosed(rtPinkyForward, 0)) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = HandPose.peace;
            }
            else if (IsPosed(rtIndForward, 0) && (!IsPosed(rtMidForward, 0) || IsPosed(rtMidForward, 180)) && IsPosed(rtPinkyForward, 0))
            {
                rightPose = HandPose.rockOn;                          
            }
            else if (IsPosed(rtIndForward, 0) && (!IsPosed(rtMidForward, 0) || IsPosed(rtMidForward, 180)) && (IsPosed(rtPinkyForward, 180) || !IsPosed(rtPinkyForward, 0)) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = HandPose.pointer;
            }
            else { rightPose = HandPose.any; }
            
            if (IsPosed(ltIndForward, 140) && IsPosed(ltMidForward, 140) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = HandPose.fist;
            }
            else if (IsPosed(ltIndForward, 0) && IsPosed(ltMidForward, 0) && IsPosed(ltPinkyForward, 0) && IsPosed(ltThumbOut, 30)) {
                leftPose = HandPose.flat;
            }
            else if (IsPosed(ltIndForward, 0) && IsPosed(ltMidForward, 0) && (IsPosed(ltPinkyForward, 180) || !IsPosed(ltPinkyForward, 0)) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = HandPose.peace;
            }
            else if (IsPosed(ltIndForward, 0) && (!IsPosed(ltMidForward, 0) || IsPosed(ltMidForward, 180)) && IsPosed(ltPinkyForward, 0))
            {
                leftPose = HandPose.rockOn;
            }
            else if (IsPosed(ltIndForward, 0) && (!IsPosed(ltMidForward, 0)) && (IsPosed(ltPinkyForward, 180) || !IsPosed(ltPinkyForward, 0)) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = HandPose.pointer;
            }
            else { leftPose = HandPose.any; }

            #region Debug land - TODO remove
            if (printAngles) {
                //Debug.Log("rtIndexForward: " + rtIndForward);
                //Debug.Log("rtMiddleForward: " + rtMidForward);
                Debug.Log("rtPinkyForward: " + rtPinkyForward);
                //Debug.Log("rtThumbOut: " + rtThumbOut);
                //Debug.Log("ltIndexForward: " + ltIndForward);
                //Debug.Log("ltMiddleForward: " + ltMidForward);
                Debug.Log("ltPinkyForward: " + ltPinkyForward);
                //Debug.Log("ltThumbOut: " + ltThumbOut);
            }
            #endregion
        }

        public bool IsPosed(float pose, float target)
        {
            return (pose < target + marginOfError && pose > target - marginOfError);
        }
    }
}
