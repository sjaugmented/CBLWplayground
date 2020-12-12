using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core
{
    public class HandTracking : MonoBehaviour
    {
        [Tooltip("Margins of error for poses")]
        [SerializeField] int bigMargin = 50;
        [SerializeField] int smallMargin = 25;
        //[SerializeField] bool fingerCasting = true;

        // right hand joints
        public MixedRealityPose rightPalm, rtIndexTip, rtIndexMid, rtIndexKnuckle, rtMiddleTip, rtMiddleKnuckle, rtRingTip, rtPinkyTip, rtThumbTip, rtThumbDistal;
        // left hand joints
        public MixedRealityPose leftPalm, ltIndexTip, ltIndexMid, ltIndexKnuckle, ltMiddleTip, ltMiddleKnuckle, ltRingTip, ltPinkyTip, ltThumbTip, ltThumbDistal;

        #region public position booleans
        // staff
        public bool staffCamUp00 = false;
        public bool staffCamUp45 = false;
        public bool staffCamUp90 = false;
        public bool staffCamUp135 = false;
        public bool staffCamUp180 = false;
        public bool staffFloorFor00 = false;
        public bool staffFloorFor90 = false;

        // palms
        public bool twoHands = false;
        public bool palmsOpposed = false;
        public bool palmsIn = false;
        public bool palmsParallel = false;
        public bool palmsOut = false;

        // right fingers
        public bool rightHand = false;
        public bool rightRockOn = false;
        public bool rightFist = false;
        public bool rightOpen = false;
        public bool rightThrower = false;
        public bool rightPoint = false;
        public bool rightThumbsUp = false;
        public bool rightL = false;
        public bool rightPeace = false;

        // left fingers
        public bool leftHand = false;
        public bool leftRockOn = false;
        public bool leftFist = false;
        public bool leftOpen = false;
        public bool leftThrower = false;
        public bool leftPoint = false;
        public bool leftThumbsUp = false;
        public bool leftL = false;
        public bool leftPeace = false;
        #endregion

        public float rtPalmUpFloorUp;
        public float rtPalmUpCamFor;
        public float rtPalmRtCamRt;
        public float rtPalmRtFloorUp;
        public float rtPalmForCamRt;
        public float ltPalmUpFloorUp;
        public float ltPalmUpCamFor;
        public float ltPalmRtCamRt;
        public float ltPalmRtFloorUp;
        public float ltPalmForCamRt;

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
            ProcessStaffAngle();
            ProcessPalmToPalm();
            ProcessFingers();
        }

        // public for StaffTester Vector scene - todo remove
        float staffForCamUp;
        float staffForCamFor;
        float staffForFloorFor;
        float staffForFloorUp;
        float staffForCamRight;

        public float GetStaffForCamUp()
        {
            return staffForCamUp;
        }

        private void ProcessStaffAngle()
        {
            // wizard staff angles
            Vector3 wizardStaff = rightPalm.Position - leftPalm.Position;
            staffForCamUp = Vector3.Angle(wizardStaff, cam.up);
            staffForCamFor = Vector3.Angle(wizardStaff, cam.forward);
            staffForFloorFor = Vector3.Angle(wizardStaff, floor.forward);
            staffForFloorUp = Vector3.Angle(wizardStaff, floor.up);
            staffForCamRight = Vector3.Angle(wizardStaff, cam.right);

            if (twoHands)
            {
                if (staffForCamUp >= 0 && staffForCamUp < 30)
                {
                    staffCamUp00 = true;
                }
                else staffCamUp00 = false;

                if (staffForCamUp >= 30 && staffForCamUp < 75)
                {
                    staffCamUp45 = true;
                }
                else staffCamUp45 = false;

                if (staffForCamUp >= 75 && staffForCamUp < 105)
                {
                    staffCamUp90 = true;
                }
                else staffCamUp90 = false;

                if (staffForCamUp >= 105 && staffForCamUp < 135)
                {
                    staffCamUp135 = true;
                }
                else staffCamUp135 = false;

                if (staffForCamUp >= 135 && staffForCamUp <= 180)
                {
                    staffCamUp180 = true;
                }
                else staffCamUp180 = false;

                if (staffForFloorFor >= 0 && staffForFloorFor < 30)
                {
                    staffFloorFor00 = true;
                }
                else staffFloorFor00 = false;

                if (staffForFloorFor >= 75 && staffForFloorFor <= 105)
                {
                    staffFloorFor90 = true;
                }
                else staffFloorFor90 = false;
            }
            else
            {
                staffCamUp00 = false;
                staffCamUp45 = false;
                staffCamUp90 = false;
                staffCamUp135 = false;
                staffCamUp180 = false;
            }

        }

        private void ProcessPalmToPalm()
        {
            #region Palm Ref Angles
            // get reference angles

            // palm to palm angles
            float p2pUp = Vector3.Angle(rightPalm.Up, leftPalm.Up);
            float p2pRt = Vector3.Angle(rightPalm.Right, leftPalm.Right);
            float p2pFor = Vector3.Angle(rightPalm.Forward, leftPalm.Forward);

            // right palm angles
            rtPalmUpCamFor = Vector3.Angle(rightPalm.Up, cam.forward);
            rtPalmUpFloorUp = Vector3.Angle(rightPalm.Up, floor.up);
            rtPalmRtFloorUp = Vector3.Angle(rightPalm.Right, floor.up);
            float rtPalmUpFloorFor = Vector3.Angle(rightPalm.Up, floor.forward);
            float rtPalmForCamFor = Vector3.Angle(rightPalm.Forward, cam.forward);
            float rtPalmRtCamFor = Vector3.Angle(rightPalm.Right, cam.forward);
            rtPalmRtCamRt = Vector3.Angle(rightPalm.Right, cam.right);
            float rtPalmRtCamUp = Vector3.Angle(rightPalm.Right, cam.up);
            float rtPalmForFloorFor = Vector3.Angle(rightPalm.Forward, floor.forward);
            float rtPalmRtFloorFor = Vector3.Angle(rightPalm.Right, floor.forward);
            float rtPalmUpCamRt = Vector3.Angle(rightPalm.Up, cam.right);
            rtPalmForCamRt = Vector3.Angle(rightPalm.Forward, cam.right);
            float rtPalmRtFloorRt = Vector3.Angle(rightPalm.Right, floor.right);


            // left palm angles
            ltPalmUpCamFor = Vector3.Angle(leftPalm.Up, cam.forward);
            ltPalmUpFloorUp = Vector3.Angle(leftPalm.Up, floor.up);
            ltPalmRtFloorUp = Vector3.Angle(leftPalm.Right, floor.up);
            float ltPalmUpFloorFor = Vector3.Angle(leftPalm.Up, floor.forward);
            float ltPalmForCamFor = Vector3.Angle(leftPalm.Forward, cam.forward);
            float ltPalmRtCamFor = Vector3.Angle(leftPalm.Right, cam.forward);
            ltPalmRtCamRt = Vector3.Angle(leftPalm.Right, cam.right);
            float ltPalmRtCamUp = Vector3.Angle(leftPalm.Right, cam.up);
            float ltPalmForFloorFor = Vector3.Angle(leftPalm.Forward, floor.forward);
            float ltPalmRtFloorFor = Vector3.Angle(leftPalm.Right, floor.forward);
            float ltPalmUpCamRt = Vector3.Angle(leftPalm.Up, cam.right);
            ltPalmForCamRt = Vector3.Angle(leftPalm.Forward, cam.right);
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

            // look for two palms
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) /*&& HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleKnuckle, Handedness.Right, out rtMiddleKnuckle)*/ && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm) /*&& HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleKnuckle, Handedness.Left, out ltMiddleKnuckle)*/)
            {
                twoHands = true;

                // look for palmsOpposed
                if (IsWithinRange(p2pUp, 180, bigMargin))
                {
                    palmsOpposed = true;

                }
                else palmsOpposed = false;

                // look for palmsOut, neutral fingers
                if (IsWithinRange(p2pUp, 0, bigMargin) && IsWithinRange(p2pRt, 0, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 180, bigMargin) && IsWithinRange(ltPalmUpCamFor, 180, bigMargin))
                {
                    palmsOut = true;
                }
                else palmsOut = false;

                // look for palmsIn, neutral fingers
                if (IsWithinRange(p2pUp, 0, bigMargin) && IsWithinRange(p2pRt, 0, bigMargin) && IsWithinRange(p2pFor, 0, bigMargin) && IsWithinRange(rtPalmUpCamFor, 0, bigMargin) && IsWithinRange(ltPalmUpCamFor, 0, bigMargin))
                {
                    palmsIn = true;
                }
                else palmsIn = false;

                // look for palmsParallel
                if (IsWithinRange(p2pUp, 0, bigMargin))
                {
                    palmsParallel = true; // TODO reconsider
                }
                else palmsParallel = false;
            }
            else
            {
                twoHands = false;
                palmsOpposed = false;
                palmsOut = false;
                palmsIn = false;
                palmsParallel = false;
            }
        }

        private void ProcessFingers()
        {
            #region Palm Ref Angles
            // get reference angles

            // palm to palm angles
            float p2pUp = Vector3.Angle(rightPalm.Up, leftPalm.Up);
            float p2pRt = Vector3.Angle(rightPalm.Right, leftPalm.Right);
            float p2pFor = Vector3.Angle(rightPalm.Forward, leftPalm.Forward);

            // right palm angles
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


            // left palm angles
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
            float rtRingForPalmFor = Vector3.Angle(rtRingTip.Forward, rightPalm.Forward);
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
            float ltRingForPalmFor = Vector3.Angle(ltRingTip.Forward, leftPalm.Forward);
            float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
            float ltThumbForCamFor = Vector3.Angle(ltThumbTip.Forward, cam.forward);
            float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
            float ltIndMidForCamRt = Vector3.Angle(ltIndexMid.Forward, cam.right);
            float ltIndMidForPalmFor = Vector3.Angle(ltIndexMid.Forward, leftPalm.Forward);
            float ltIndMidUpCamFor = Vector3.Angle(ltIndexMid.Up, cam.forward);
            float ltIndMidUpFloorFor = Vector3.Angle(ltIndexMid.Up, floor.forward);
            float ltIndMidUpFloorUp = Vector3.Angle(ltIndexMid.Up, floor.up);
            float ltIndKnuckForPalmFor = Vector3.Angle(ltIndexKnuckle.Forward, leftPalm.Forward);

            // thumb angles
            Vector3 rtThumbVector = rtThumbTip.Position - rtThumbDistal.Position;
            Vector3 ltThumbVector = ltThumbTip.Position - ltThumbDistal.Position;
            float rtThumbVecPalmFor = Vector3.Angle(rtThumbVector, rightPalm.Forward);
            float rtThumbVecPalmRight = Vector3.Angle(rtThumbVector, rightPalm.Right);
            float ltThumbVecPalmFor = Vector3.Angle(ltThumbVector, leftPalm.Forward);
            float ltThumbVecPalmRight = Vector3.Angle(ltThumbVector, leftPalm.Right);

            // compare fingers on both hands
            float rtIndMidForLtIndMidFor = Vector3.Angle(rtIndexMid.Forward, ltIndexMid.Forward);
            float rtIndMidUpLtIndMidUp = Vector3.Angle(rtIndexMid.Up, ltIndexMid.Up);
            #endregion

            // right hand
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rtIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out rtIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out rtIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rtMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out rtRingTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rtPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rtThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Right, out rtThumbDistal) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm))
            {
                rightHand = true;

                // look for rockOn
                if (IsWithinRange(rtIndForPalmFor, 0, bigMargin) && IsWithinRange(rtPinkForPalmFor, 0, bigMargin) && !IsWithinRange(rtMidForPalmFor, 0, 90) && !IsWithinRange(rtRingForPalmFor, 0, 90))
                {
                    rightRockOn = true;
                }
                else
                {
                    rightRockOn = false;
                }

                // look for right fist
                if (IsWithinRange(rtIndMidForPalmFor, 140, bigMargin) && IsWithinRange(rtMidForPalmFor, 140, bigMargin) && IsWithinRange(rtPinkForPalmFor, 130, bigMargin) && !IsWithinRange(rtThumbVecPalmRight, 180, 80) ||
                //unity standard tap for debug
                IsWithinRange(rtIndMidForPalmFor, 83, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin) && !IsWithinRange(rtThumbVecPalmRight, 160, 70))
                {

                    rightFist = true;
                }
                else rightFist = false;

                // look for right thumbs up
                if (IsWithinRange(rtThumbVecPalmRight, 180, 90) && !IsWithinRange(rtIndMidForPalmFor, 0, 100) && !IsWithinRange(rtMidForPalmFor, 0, 100) && !IsWithinRange(rtPinkForPalmFor, 0, 100)
                    // unity standard thumbs up
                    //IsWithinRange(rtThumbVecPalmFor, 61, 10)
                    )
                {
                    rightThumbsUp = true;
                }
                else rightThumbsUp = false;

                // look for right flat
                if (IsWithinRange(rtIndMidForPalmFor, 0, 70) && IsWithinRange(rtMidForPalmFor, 0, 70) && IsWithinRange(rtPinkForPalmFor, 0, 70))
                {
                    rightOpen = true;
                }
                else rightOpen = false;

                // look for palm out throw
                if (IsWithinRange(rtIndMidForPalmFor, 20, bigMargin) && IsWithinRange(rtMidForPalmFor, 20, bigMargin) && IsWithinRange(rtPinkForPalmFor, 20, bigMargin) && IsWithinRange(rtPalmUpFloorUp, 60, bigMargin) && IsWithinRange(rtPalmRtFloorRt, 0, bigMargin))
                {
                    rightThrower = true;
                }
                else rightThrower = false;

                // look for right point
                if (!IsWithinRange(rtThumbVecPalmRight, 180, 80) && IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtRingForPalmFor, 145, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin) ||
                    // unity editor pose
                    IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtThumbForPalmFor, 36, smallMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin))
                {
                    rightPoint = true;
                }
                else rightPoint = false;

                if (IsWithinRange(rtThumbVecPalmRight, 180, 80) && IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 160, bigMargin) && IsWithinRange(rtRingForPalmFor, 145, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin))
                {
                    rightL = true;
                }
                else rightL = false;
                
                if (!IsWithinRange(rtThumbVecPalmRight, 180, 80) && IsWithinRange(rtIndMidForPalmFor, 0, bigMargin) && IsWithinRange(rtIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(rtMidForPalmFor, 0, bigMargin) && IsWithinRange(rtRingForPalmFor, 145, bigMargin) && IsWithinRange(rtPinkForPalmFor, 129, bigMargin))
                {
                    rightPeace = true;
                }
                else rightPeace = false;
            }
            else
            {
                rightHand = false;
                rightRockOn = false;
                rightFist = false;
                rightOpen = false;
                rightThrower = false;
                rightPoint = false;
                rightThumbsUp = false;
                rightL = false;
                rightPeace = false;
            }

            // left hand
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out ltIndexTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Left, out ltIndexMid) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out ltIndexKnuckle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out ltMiddleTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Left, out ltRingTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Left, out ltPinkyTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out ltThumbTip) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Left, out ltThumbDistal) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
            {
                leftHand = true;

                // look for rockOn
                if (IsWithinRange(ltIndForPalmFor, 0, bigMargin) && IsWithinRange(ltPinkForPalmFor, 0, bigMargin) && !IsWithinRange(ltMidForPalmFor, 0, 90) && !IsWithinRange(ltRingForPalmFor, 0, 90))
                {
                    leftRockOn = true;
                }
                else
                {
                    leftRockOn = false;
                }

                // look for left fist
                if (IsWithinRange(ltIndMidForPalmFor, 140, bigMargin) && IsWithinRange(ltMidForPalmFor, 140, bigMargin) && IsWithinRange(ltPinkForPalmFor, 130, bigMargin) && !IsWithinRange(ltThumbVecPalmRight, 0, 80) ||
                    // debug unity standard tap
                    IsWithinRange(ltIndMidForPalmFor, 83, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin) && !IsWithinRange(ltThumbVecPalmRight, 20, bigMargin))
                {

                    leftFist = true;
                }
                else leftFist = false;

                // look for left thumbs up
                if (IsWithinRange(ltThumbVecPalmRight, 0, 80) && !IsWithinRange(ltIndMidForPalmFor, 0, 100) && !IsWithinRange(ltMidForPalmFor, 0, 100) && !IsWithinRange(ltPinkForPalmFor, 0, 100) /*||
                // unity standard thumbs up
                IsWithinRange(ltThumbVecPalmFor, 61, 10)*/)
                {
                    leftThumbsUp = true;
                }
                else leftThumbsUp = false;

                // look for left flat
                if (IsWithinRange(ltIndMidForPalmFor, 0, 70) && IsWithinRange(ltMidForPalmFor, 0, 70) && IsWithinRange(ltPinkForPalmFor, 0, 70))
                {
                    leftOpen = true;
                }
                else leftOpen = false;

                // look for palm out throw
                if (IsWithinRange(ltIndMidForPalmFor, 20, bigMargin) && IsWithinRange(ltMidForPalmFor, 20, bigMargin) && IsWithinRange(ltPinkForPalmFor, 20, bigMargin) && IsWithinRange(ltPalmUpFloorUp, 60, bigMargin) && IsWithinRange(ltPalmRtFloorRt, 0, bigMargin))
                {
                    leftThrower = true;
                }
                else leftThrower = false;

                // look for left point
                if (!IsWithinRange(ltThumbVecPalmRight, 0, 80) && IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltRingForPalmFor, 145, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin) ||
                    // unity editor pose
                    IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltThumbForPalmFor, 36, smallMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
                {
                    leftPoint = true;
                }
                else leftPoint = false;

                // look for left L
                if (IsWithinRange(ltThumbVecPalmRight, 0, 80) && IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 160, bigMargin) && IsWithinRange(ltRingForPalmFor, 145, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
                {
                    leftL = true;
                }
                else leftL = false;

                if (!IsWithinRange(ltThumbVecPalmRight, 0, 80) && IsWithinRange(ltIndMidForPalmFor, 0, bigMargin) && IsWithinRange(ltIndKnuckForPalmFor, 0, bigMargin) && IsWithinRange(ltMidForPalmFor, 0, bigMargin) && IsWithinRange(ltRingForPalmFor, 145, bigMargin) && IsWithinRange(ltPinkForPalmFor, 129, bigMargin))
                {
                    leftPeace = true;
                }
                else leftPeace = false;
            }
            else
            {
                leftHand = false;
                leftRockOn = false;
                leftFist = false;
                leftOpen = false;
                leftThrower = false;
                leftPoint = false;
                leftThumbsUp = false;
                leftL = false;
                leftPeace = false;
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

        public float GetFloorUp()
        {
            return staffForFloorUp;
        }

        public float GetCamRight()
        {
            return staffForCamRight;
        }

        public float GetCamUp()
        {
            return staffForCamUp;
        }

        public float GetCamForward()
        {
            return staffForCamFor;
        }
    }

}

