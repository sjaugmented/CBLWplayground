using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core {
    public enum Hands {right, left, both, none}
    public enum Direction {up, down, palmOut, palmIn, side, none };
    public enum Formation {palmsIn, palmsOut, together, palmsUp, palmsDown, none}
    public enum HandPose {pointer, peace, flat, fist, gun, thumbsUp, rockOn, any, none}

    [RequireComponent(typeof(CastOrigins))]
    public class NewTracking : MonoBehaviour
    {
        public static NewTracking Instance;
        
        [SerializeField] float strictMargin = 20;
        [SerializeField] bool printAngles = false;
        [SerializeField] bool showStaff = false;

        public Hands handedness = Hands.none;
        public Formation palmsRel = Formation.none;
        public Formation palmsAbs = Formation.none;
        public Direction rightPalmRel = Direction.none;
        public Direction rightPalmAbs = Direction.none;
        public Direction leftPalmRel = Direction.none;
        public Direction leftPalmAbs = Direction.none;
        public HandPose rightPose = HandPose.none;
        public HandPose leftPose = HandPose.none;
        
        MixedRealityPose rtIndex, rtMiddle, rtPinky, rtThumb, rtPalm;
        MixedRealityPose ltIndex, ltMiddle, ltPinky, ltThumb, ltPalm;

        bool foundRtIndex, foundRtMiddle, foundRtPinky, foundRtThumb, foundRtPalm;
        bool foundLtIndex, foundLtMiddle, foundLtPinky, foundLtThumb, foundLtPalm;
        
        Vector3 staff;
        float staffForward, staffUp, staffRight, staffFloorUp, staffFloorForward;
        float rtPalmForwardRel, rtPalmForwardAbs, rtPalmUpRel, rtPalmUpAbs, rtPalmInRel, rtLauncher, ltPalmForwardRel,ltPalmForwardAbs, ltPalmUpRel, ltPalmUpAbs, ltPalmInRel, ltLauncher;
        
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
        public float StaffRight { get { return staffRight; } }
        public float RtLauncher { get { return rtLauncher; } }
        public float LtLauncher { get { return ltLauncher; } }
        public Vector3 Staff { get { return staff; } }
        #endregion

        Transform cam;
        Transform floor;

        private void Awake()
        {
            cam = Camera.main.transform;
            floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<LevelObject>().transform;
        }

        private void Start()
        {
            Instance = this;
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
            rtPalmForwardRel = Vector3.Angle(rtPalm.Up, cam.forward);
            rtPalmForwardAbs = Vector3.Angle(rtPalm.Up, floor.forward);
            rtPalmUpRel = Vector3.Angle(rtPalm.Up, cam.up);
            rtPalmUpAbs = Vector3.Angle(rtPalm.Up, floor.up);
            rtPalmInRel = Vector3.Angle(rtPalm.Up, cam.right);
            rtLauncher = Vector3.Angle(rtPalm.Right, cam.up); // TODO move launchers off of NewTracker

            ltPalmForwardRel = Vector3.Angle(ltPalm.Up, cam.forward);
            ltPalmForwardAbs = Vector3.Angle(ltPalm.Up, floor.forward);
            ltPalmUpRel = Vector3.Angle(ltPalm.Up, cam.up);
            ltPalmUpAbs = Vector3.Angle(ltPalm.Up, floor.up);
            ltPalmInRel = Vector3.Angle(ltPalm.Up, cam.right * -1); // inverted for symmetry
            ltLauncher = Vector3.Angle(ltPalm.Right, cam.up);

            if (!foundRtPalm) {
                rightPalmRel = Direction.none;
                rightPalmAbs = Direction.none;
            }
            if (!foundLtPalm) {
                leftPalmRel = Direction.none;
                leftPalmAbs = Direction.none;
            }

            #region RELATIVES
            if (IsPosed(rtPalmForwardRel, 20)) {
                rightPalmRel = Direction.palmIn;
            }
            if (IsPosed(ltPalmForwardRel, 20)) {
                leftPalmRel = Direction.palmIn;
            }

            if (IsPosed(rtPalmForwardRel, 150)) {
                rightPalmRel = Direction.palmOut;
            }
            if (IsPosed(ltPalmForwardRel, 150)) {
                leftPalmRel = Direction.palmOut;
            }

            if (IsPosed(rtPalmUpRel, 0)) {
                rightPalmRel = Direction.down;
            }
            if (IsPosed(ltPalmUpRel, 0)) {
                leftPalmRel = Direction.down;
            }

            if (IsPosed(rtPalmUpRel, 180)) {
                rightPalmRel = Direction.up;
            }
            if (IsPosed(ltPalmUpRel, 180)) {
                leftPalmRel = Direction.up;
            }

            if (IsPosed(rtPalmInRel, 0))
            {
                rightPalmRel = Direction.side;
                rightPalmAbs = Direction.none;
            }
            if (IsPosed(ltPalmInRel, 0))
            {
                leftPalmRel = Direction.side;
                leftPalmAbs = Direction.none;
            }
            #endregion

            #region ABSOLUTES
            if (IsPosed(rtPalmForwardAbs, 20))
            {
                rightPalmAbs = Direction.palmIn;
            }
            if (IsPosed(ltPalmForwardAbs, 20))
            {
                leftPalmAbs = Direction.palmIn;
            }

            if (IsPosed(rtPalmForwardAbs, 150))
            {
                rightPalmAbs = Direction.palmOut;
            }
            if (IsPosed(ltPalmForwardAbs, 150))
            {
                leftPalmAbs = Direction.palmOut;
            }

            if (IsPosed(rtPalmUpAbs, 0))
            {
                rightPalmAbs = Direction.down;
            }
            if (IsPosed(ltPalmUpAbs, 0))
            {
                leftPalmAbs = Direction.down;
            }

            if (IsPosed(rtPalmUpAbs, 180))
            {
                rightPalmAbs = Direction.up;
            }
            if (IsPosed(ltPalmUpAbs, 180))
            {
                leftPalmAbs = Direction.up;
            }
            #endregion
        }

        private void WatchStaff() 
        {
            if (handedness != Hands.both) { return; }
            
            staff = (rtPalm.Position - ltPalm.Position).normalized;
            if (showStaff)
            {
                Debug.DrawLine(ltPalm.Position, ltPalm.Position + staff * 2, Color.red, Mathf.Infinity);
            }
            staffUp = Vector3.Angle(staff, cam.up);
            staffForward = Vector3.Angle(staff, cam.forward);
            staffRight = Vector3.Angle(staff, cam.right);
            staffFloorForward = Vector3.Angle(staff, floor.forward);
            staffFloorUp = Vector3.Angle(staff, floor.up);
        }

        private void WatchFormations()
        {
            float palmToPalm = Vector3.Angle(rtPalm.Up, ltPalm.Up);

            if (!foundRtPalm || !foundLtPalm || ((IsPosed(staffUp, 90) || IsPosed(staffRight, 0)) && ((rightPalmRel == Direction.up && leftPalmRel == Direction.down) || (rightPalmRel == Direction.down && leftPalmRel == Direction.up))))
            {
                palmsRel = Formation.none;
                palmsAbs = Formation.none;
            }

            else if (IsPosed(palmToPalm, 180)) {
                palmsRel = Formation.together;
                palmsAbs = Formation.together;
            }

            #region RELATIVES
            else if (rightPalmRel == Direction.palmOut && leftPalmRel == Direction.palmOut) {
                palmsRel = Formation.palmsOut;
            }

            else if (rightPalmRel == Direction.palmIn && leftPalmRel == Direction.palmIn) {
                palmsRel = Formation.palmsIn;
            }

            else if (rightPalmRel == Direction.up && leftPalmRel == Direction.up) {
                palmsRel = Formation.palmsUp;
            }

            else if (rightPalmRel == Direction.down && leftPalmRel == Direction.down) {
                palmsRel = Formation.palmsDown;
            }
            else
            {
                palmsRel = Formation.none;
            }
            #endregion

            #region ABSOLUTES
            if (rightPalmAbs == Direction.palmOut && leftPalmAbs == Direction.palmOut)
            {
                palmsAbs = Formation.palmsOut;
            }

            else if (rightPalmAbs == Direction.palmIn && leftPalmAbs == Direction.palmIn)
            {
                palmsAbs = Formation.palmsIn;
            }

            else if (rightPalmAbs == Direction.up && leftPalmAbs == Direction.up)
            {
                palmsAbs = Formation.palmsUp;
            }

            else if (rightPalmAbs == Direction.down && leftPalmAbs == Direction.down)
            {
                palmsAbs = Formation.palmsDown;
            }
            else
            {
                palmsAbs = Formation.none;
            }
            #endregion


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

            // RIGHT
            if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140) && (IsPosed(rtThumbOut, 150) || IsPosed(rtThumbOut, 130))) {
                rightPose = HandPose.fist;
            }
            else if (IsPosed(rtIndForward, 140) && IsPosed(rtMidForward, 140) && (IsPosed(rtThumbOut, 30) || IsPosed(rtThumbOut, 50)))
            {
                rightPose = HandPose.thumbsUp;
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
            
            // LEFT
            if (IsPosed(ltIndForward, 140) && IsPosed(ltMidForward, 140) && (IsPosed(ltThumbOut, 150) || IsPosed(ltThumbOut, 130))) {
                leftPose = HandPose.fist;
            }
            else if (IsPosed(ltIndForward, 125) && IsPosed(ltMidForward, 100) && (IsPosed(ltThumbOut, 30) || IsPosed(ltThumbOut, 50)))
            {
                leftPose = HandPose.thumbsUp;
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
                Debug.Log("rtIndexForward: " + rtIndForward);
                Debug.Log("rtMiddleForward: " + rtMidForward);
                //Debug.Log("rtPinkyForward: " + rtPinkyForward);
                Debug.Log("rtThumbOut: " + rtThumbOut);
                Debug.Log("ltIndexForward: " + ltIndForward);
                Debug.Log("ltMiddleForward: " + ltMidForward);
                //Debug.Log("ltPinkyForward: " + ltPinkyForward);
                Debug.Log("ltThumbOut: " + ltThumbOut);
            }
            #endregion
        }

        public bool IsPosed(float pose, float target, float marginOfError = 40)
        {
            return (pose < target + marginOfError && pose > target - marginOfError);
        }
    }
}
