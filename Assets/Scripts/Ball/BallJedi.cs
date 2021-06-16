using LW.Core;
using Photon.Pun;
using UnityEngine;

namespace LW.Ball
{
    public enum Force { push, pull, right, left, idle }

    public class BallJedi : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] float deadZone = 0.5f;
        [SerializeField] float fieldOfControl = 60f;
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
        public bool RecallRight { get; set; }
        public bool RecallLeft { get; set; }
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

        bool rightLasso, leftLasso, resetReady;
        float rLassoTimer, lLassoTimer, resetTimer, recallPunchTimer, forceTimer = Mathf.Infinity;

        BallDirector director;
        NewTracking tracking;
        CastOrigins origins;
        Rigidbody rigidbody;
        Ball ball;
        MultiAxisController multiAxis;

        #region Photon

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Primary);
                stream.SendNext(Secondary);
                stream.SendNext(Spin);
                stream.SendNext(RecallRight);
                stream.SendNext(RecallLeft);
                stream.SendNext(Reset);
                stream.SendNext(Held);
                stream.SendNext(HoldPose);
                stream.SendNext(RelativeHandDist);
            }
            else
            {
                Primary = (Force)stream.ReceiveNext();
                Secondary = (Force)stream.ReceiveNext();
                Spin = (bool)stream.ReceiveNext();
                RecallRight = (bool)stream.ReceiveNext();
                RecallLeft = (bool)stream.ReceiveNext();
                Reset = (bool)stream.ReceiveNext();
                Held = (bool)stream.ReceiveNext();
                HoldPose = (HandPose)stream.ReceiveNext();
                RelativeHandDist = (float)stream.ReceiveNext();
            }
        }

        #endregion
        
        // TODO seperate the state for photon
        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("Director").GetComponent<NewTracking>();
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            origins = GameObject.FindGameObjectWithTag("Director").GetComponent<CastOrigins>();
            multiAxis = GameObject.FindGameObjectWithTag("Director").GetComponent<MultiAxisController>();
            rigidbody = GetComponent<Rigidbody>();
            ball = GetComponent<Ball>();
        }

        void Update()
        {
            bool gravityCondition = !Held && ball.State == BallState.Active && Primary == Force.idle;
            
            RelativeHandDist = (origins.PalmsDist - MinDistance * ball.transform.localScale.x) / (HoldDistance - MinDistance * ball.transform.localScale.x);

            var ballDirection = transform.position - Camera.main.transform.position;
            var angleToBall = Vector3.Angle(Camera.main.transform.forward, ballDirection);
            Debug.Log("angleToBall: " + angleToBall);
            Debug.Log("distance to ball: " + ball.Distance);

            rLassoTimer += Time.deltaTime;
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
                        if (angleToBall < fieldOfControl)
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

                            if (multiAxis.PalmRightStaffForward > (90 + multiAxis.DeadZone) && ball.Distance > deadZone)
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

            Moving = Primary != Force.idle || RecallRight;
            
            #endregion

            #region Recall
            // Right Hand
            if (ball.IsNotQuiet && tracking.rightPose == HandPose.fist && tracking.rightPalmRel == Direction.palmOut)
            {
                if (!rightLasso)
                {
                    rLassoTimer = 0;
                    rightLasso = true;
                }
            }
            else
            {
                rightLasso = false;
            }

            if (ball.IsNotQuiet && rLassoTimer < 3 && tracking.rightPose == HandPose.flat && tracking.rightPalmRel == Direction.palmOut)
            {
                RecallRight = true;
                recallPunchTimer = 0;
            }

            if (tracking.rightPose != HandPose.flat)
            {
                RecallRight = false;
            }

            // Left Hand
            if (ball.IsNotQuiet && tracking.leftPose == HandPose.fist && tracking.leftPalmRel == Direction.palmOut)
            {
                if (!leftLasso)
                {
                    lLassoTimer = 0;
                    leftLasso = true;
                }
            }
            else
            {
                leftLasso = false;
            }

            if (ball.IsNotQuiet && lLassoTimer < 3 && tracking.leftPose == HandPose.flat && tracking.leftPalmRel == Direction.palmOut)
            {
                RecallLeft = true;
                recallPunchTimer = 0;
            }

            if (tracking.leftPose != HandPose.flat)
            {
                RecallLeft = false;
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
        }
    }

}

