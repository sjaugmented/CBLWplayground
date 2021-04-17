﻿using LW.Core;
using UnityEngine;

namespace LW.Ball
{
    public enum Force { push, pull, right, left, idle }

    public class BallJedi : MonoBehaviour
    {
        [SerializeField] float masterForce = 3;
        //[SerializeField] float pushMultiplier = 5;
        //[SerializeField] float pullMultiplier = 5;
        //[SerializeField] float liftMultiplier = 3;
        [SerializeField] float recallMultiplier = 6;
        float holdDistance = 0.7f;
        float minDistance = 0.3f;

        public bool Spin { get; set; }
        public float HoldDistance
        {
            get { return holdDistance; }
        }
        public float MinDistance
        {
            get { return minDistance; }
        }
        public float MasterForce { get { return masterForce; } }
        //public float PushForce
        //{
        //    get { return pushMultiplier; }
        //}
        //public float PullForce
        //{
        //    get { return pullMultiplier; }
        //}
        //public float LiftForce
        //{
        //    get { return liftMultiplier; }
        //}
        public float RecallForce
        {
            get { return recallMultiplier; }
        }
        public float RelativeHandDist { get; set; }
        public bool Held { get; set; }
        public bool Moving { get; set; }
        public bool Recall { get; set; }
        public float LevelUpTimer
        {
            get { return recallPunchTimer; }
        }
        public HandPose ControlPose = HandPose.none;
        public Force Primary = Force.idle;
        public Force Secondary = Force.idle;

        bool lassoReady;
        float lassoTimer, recallPunchTimer, forceTimer = Mathf.Infinity;

        NewTracking tracking;
        CastOrigins origins;
        Rigidbody rigidbody;
        Ball ball;
        MultiAxis multiAxis;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            rigidbody = GetComponent<Rigidbody>();
            ball = GetComponent<Ball>();
            multiAxis = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<MultiAxis>();
        }

        void Update()
        {
            RelativeHandDist = (origins.PalmsDist - MinDistance * ball.transform.localScale.x) / (HoldDistance - MinDistance * ball.transform.localScale.x);

            lassoTimer += Time.deltaTime;
            recallPunchTimer += Time.deltaTime;
            forceTimer = Time.deltaTime;

            bool gravityCondition = !Held && ball.State == BallState.Active && Primary == Force.idle;

            rigidbody.useGravity = gravityCondition;
            GetComponent<ConstantForce>().enabled = gravityCondition;

            #region ControlPoses
            if (tracking.palmsRel == Formation.together)
            {
                Held = true;
                forceTimer = 0;

                if (tracking.rightPose == HandPose.pointer && tracking.leftPose == HandPose.pointer)
                {
                    ControlPose = HandPose.pointer;
                }
                else if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    ControlPose = HandPose.fist;
                }
                else
                {
                    ControlPose = HandPose.none;
                }
            }
            else
            {
                Held = false;
            }
            #endregion

            Moving = Primary != Force.idle;

            #region Forces
            if (forceTimer < 1)
            {
                if (tracking.handedness == Hands.both && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    if (multiAxis.StaffForward > (90 + multiAxis.DeadZone / 2))
                    {
                        Secondary = Force.right;
                    }
                    else if (multiAxis.StaffForward < (90 - multiAxis.DeadZone / 2))
                    {
                        Secondary = Force.left;
                    }
                    else/* if (multiAxis.StaffForward >= (90 - multiAxis.DeadZone) && multiAxis.StaffForward <= (90 + multiAxis.DeadZone))*/
                    {
                        Secondary = Force.idle;
                    }

                    if (multiAxis.StaffRight > (90 + multiAxis.DeadZone))
                    {
                        Primary = Force.pull;
                    }
                    else if (multiAxis.StaffRight < (90 - multiAxis.DeadZone))
                    {
                        Primary = Force.push;
                    }
                    else/* if (multiAxis.StaffRight >= (90 - multiAxis.DeadZone) && multiAxis.StaffRight <= (90 + multiAxis.DeadZone))*/
                    {
                        Primary = Force.idle;
                    }
                }
                //else if (tracking.handedness == Hands.both && tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
                //{
                //    Primary = PrimaryForce.spin;
                //}
                else
                {
                    Primary = Force.idle;
                    Secondary = Force.idle;
                }
            }
            else
            {
                Primary = Force.idle;
                Secondary = Force.idle;
            }

            Spin = tracking.handedness == Hands.both && tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp;
            #endregion

            #region Recall
            if (ball.HasSpawned && tracking.rightPose == HandPose.fist && tracking.rightPalmRel == Direction.palmOut)
            {
                if (!lassoReady)
                {
                    lassoTimer = 0;
                    lassoReady = true;
                }
            }
            else
            {
                lassoReady = false;
            }

            if (ball.HasSpawned && lassoTimer < 3 && tracking.rightPose == HandPose.flat && tracking.rightPalmRel == Direction.palmOut)
            {
                Recall = true;
                recallPunchTimer = 0;
            }

            if (tracking.rightPose != HandPose.flat)
            {
                Recall = false;
            }
            #endregion

            #region Forces (old)
            //if (tracking.palmsRel == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            //{
            //    Power = TheForce.push;
            //}
            //else if (tracking.palmsAbs == Formation.palmsUp && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            //{
            //    Power = TheForce.lift;
            //}
            //else if (tracking.palmsRel == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            //{
            //    Power = TheForce.pull;
            //}
            //else if (tracking.palmsAbs == Formation.palmsDown && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            //{
            //    Power = TheForce.down;
            //}
            //if (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
            //{
            //    Power = TheForce.spin;
            //}
            //else
            //{
            //    Power = TheForce.idle;
            //}
            #endregion
        }
    }

}

