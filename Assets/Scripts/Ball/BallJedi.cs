using LW.Core;
using UnityEngine;

namespace LW.Ball
{
    public enum Force { push, pull, right, left, idle }

    public class BallJedi : MonoBehaviour
    {
        [SerializeField] float masterForce = 3;
        [SerializeField] float recallMultiplier = 6;
        [SerializeField] float holdDistance = 0.5f;
        [SerializeField] float gingerLift = 0.001f;
        public bool noJedi = false;

        float minDistance = 0.3f;

        public float GingerLift { get { return gingerLift; } }

        public bool spin;
        public bool Spin { get { return spin; } set { spin = value; } }
        public float HoldDistance
        {
            get { return holdDistance; }
        }
        public float MinDistance
        {
            get { return minDistance; }
        }
        public float MasterForce { get { return masterForce; } }
        public float RecallForce
        {
            get { return recallMultiplier; }
        }
        public float RelativeHandDist { get; set; }
        public bool Held { get; set; }
        public bool Moving { get; set; }
        public bool Recall { get; set; }
        public bool Reset { get; set; }
        public bool NoJedi 
        { 
            get { return noJedi; }
            set { noJedi = value; }
        }
        public float LevelUpTimer
        {
            get { return recallPunchTimer; }
        }
        public HandPose HoldPose = HandPose.none;
        public Force Primary = Force.idle;
        public Force Secondary = Force.idle;

        bool lassoReady, resetReady;
        float lassoTimer, resetTimer, recallPunchTimer, forceTimer = Mathf.Infinity;

        BallDirector director;
        NewTracking tracking;
        CastOrigins origins;
        Rigidbody rigidbody;
        Ball ball;
        MultiAxisController multiAxis;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            rigidbody = GetComponent<Rigidbody>();
            ball = GetComponent<Ball>();
            multiAxis = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<MultiAxisController>();
        }

        void Update()
        {
            //Debug.Log(NoJedi);
            bool gravityCondition = !Held && ball.State == BallState.Active;
            
            RelativeHandDist = (origins.PalmsDist - MinDistance * ball.transform.localScale.x) / (HoldDistance - MinDistance * ball.transform.localScale.x);

            lassoTimer += Time.deltaTime;
            resetTimer += Time.deltaTime;
            recallPunchTimer += Time.deltaTime;
            forceTimer = Time.deltaTime;


            rigidbody.useGravity = gravityCondition;
            GetComponent<ConstantForce>().enabled = gravityCondition;

            if (!NoJedi && !director.KillJedi)
            {

                #region multiAxis Axis Control
                if (tracking.handedness == Hands.both)
                {
                    if (tracking.palmsRel == Formation.together)
                    {
                        forceTimer = 0;

                        #region Hold Poses
                        if (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                        {
                            HoldPose = HandPose.flat;
                        }
                        else if (tracking.rightPose == HandPose.pointer && tracking.leftPose == HandPose.pointer)
                        {
                            HoldPose = HandPose.pointer;
                        }
                        else if (tracking.rightPose == HandPose.peace && tracking.leftPose == HandPose.peace)
                        {
                            HoldPose = HandPose.peace;
                        }
                        else if (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
                        {
                            HoldPose = HandPose.thumbsUp;
                        }
                        else
                        {
                            HoldPose = HandPose.none;
                        }
                        #endregion
                    }
                    else if (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                    {
                        #region Jedi
                        if (multiAxis.StaffForwardCamForward > (90 + multiAxis.DeadZone / 2))
                        {
                            Secondary = Force.right;
                        }
                        else if (multiAxis.StaffForwardCamForward < (90 - multiAxis.DeadZone / 2))
                        {
                            Secondary = Force.left;
                        }
                        else
                        {
                            Secondary = Force.idle;
                        }

                        if (multiAxis.PalmRightStaffForward > (90 + multiAxis.DeadZone))
                        {
                            Primary = Force.pull;
                        }
                        else if (multiAxis.PalmRightStaffForward < (90 - multiAxis.DeadZone))
                        {
                            Primary = Force.push;
                        }
                        else
                        {
                            Primary = Force.idle;
                        }
                        #endregion
                    }
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

                Spin =
                    ball.State == BallState.Active &&
                    tracking.handedness == Hands.both &&
                    ((tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat) ||
                    (tracking.rightPose == HandPose.pointer && tracking.leftPose == HandPose.pointer) ||
                    (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp));
                
                Held =
                    !Moving &&
                    Primary == Force.idle &&
                    tracking.handedness == Hands.both &&
                    ((tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat) ||
                    (tracking.rightPose == HandPose.pointer && tracking.leftPose == HandPose.pointer) ||
                    (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp));
            }

            Moving = Primary != Force.idle || Recall;
            
            #endregion



            #region Forces
            //if (forceTimer < 3)
            //{
            //    if (tracking.handedness == Hands.both && (tracking.rightPose == HandPose.flat || tracking.rightPose == HandPose.peace) && (tracking.leftPose == HandPose.flat || tracking.leftPose == HandPose.peace))
            //    {
            //        if (multiAxis.StaffForward > (90 + multiAxis.DeadZone / 2))
            //        {
            //            Secondary = Force.right;
            //        }
            //        else if (multiAxis.StaffForward < (90 - multiAxis.DeadZone / 2))
            //        {
            //            Secondary = Force.left;
            //        }
            //        else/* if (multiAxis.StaffForward >= (90 - multiAxis.DeadZone) && multiAxis.StaffForward <= (90 + multiAxis.DeadZone))*/
            //        {
            //            Secondary = Force.idle;
            //        }

            //        if (multiAxis.StaffRight > (90 + multiAxis.DeadZone))
            //        {
            //            Primary = Force.pull;
            //        }
            //        else if (multiAxis.StaffRight < (90 - multiAxis.DeadZone))
            //        {
            //            Primary = Force.push;
            //        }
            //        else/* if (multiAxis.StaffRight >= (90 - multiAxis.DeadZone) && multiAxis.StaffRight <= (90 + multiAxis.DeadZone))*/
            //        {
            //            Primary = Force.idle;
            //        }
            //    }
            //    else
            //    {
            //        Primary = Force.idle;
            //        Secondary = Force.idle;
            //    }
            //}
            //else
            //{
            //    Primary = Force.idle;
            //    Secondary = Force.idle;
            //}

            //Spin = ball.State == BallState.Still && tracking.handedness == Hands.both && (tracking.rightPose == HandPose.flat || tracking.rightPose == HandPose.peace) && (tracking.leftPose == HandPose.flat || tracking.leftPose == HandPose.peace);
            #endregion

            #region Recall
            if (ball.IsNotQuiet && tracking.rightPose == HandPose.fist && tracking.rightPalmRel == Direction.palmOut)
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

            if (ball.IsNotQuiet && lassoTimer < 3 && tracking.rightPose == HandPose.flat && tracking.rightPalmRel == Direction.palmOut)
            {
                Recall = true;
                recallPunchTimer = 0;
            }

            if (tracking.rightPose != HandPose.flat)
            {
                Recall = false;
            }
            #endregion

            #region Reset
            if (ball.DominantDir == Direction.up && ball.DominantPose == HandPose.fist)
            {
                resetTimer = 0;
            }
            else
            {
                resetReady = false;
            }

            if (resetTimer < 0.5 && ball.DominantDir == Direction.up && ball.DominantPose == HandPose.flat)
            {
                Reset = true;
            }
            else
            {
                Reset = false;
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

