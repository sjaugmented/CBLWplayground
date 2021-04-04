using LW.Core;
using UnityEngine;

namespace LW.Ball
{
    public enum TheForce { push, pull, lift, down, idle}
    public class BallJedi : MonoBehaviour
    {
        [SerializeField] float pushMultiplier = 5;
        [SerializeField] float pullMultiplier = 5;
        [SerializeField] float liftMultiplier = 3;
        [SerializeField] float recallMultiplier = 10;
        [SerializeField] float holdDistance = 0.5f;
        [SerializeField] float minDistance = 0.2f;
        public float HoldDistance
        {
            get { return holdDistance; }
        }
        public float MinDistance
        {
            get { return minDistance; }
        }
        public float PushForce
        {
            get { return pushMultiplier; }
        }
        public float PullForce
        {
            get { return pullMultiplier; }
        }
        public float LiftForce
        {
            get { return liftMultiplier; }
        }
        public float RecallForce
        {
            get { return recallMultiplier; }
        }
        public float RelativeHandDist { get; set; }
        public bool Held { get; set; }
        public bool Recall { get; set; }
        public float PunchTimer
        {
            get { return recallPunchTimer; }
        }
        public HandPose ControlPose = HandPose.none;
        public TheForce Power = TheForce.idle;

        bool lassoReady;
        float lassoTimer, recallPunchTimer = Mathf.Infinity;

        NewTracking tracking;
        CastOrigins origins;
        Rigidbody rigidbody;
        Ball ball;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            rigidbody = GetComponent<Rigidbody>();
            ball = GetComponent<Ball>();
        }

        void Update()
        {
            RelativeHandDist = Mathf.Clamp((origins.PalmsDist - MinDistance * ball.transform.localScale.x) / (HoldDistance - MinDistance * ball.transform.localScale.x), 0, 1);

            lassoTimer += Time.deltaTime;
            recallPunchTimer += Time.deltaTime;

            rigidbody.useGravity = !Held && ball.State == BallState.Active && Power == TheForce.idle;
            GetComponent<ConstantForce>().enabled = !Held && ball.State == BallState.Active && Power == TheForce.idle;
            
            #region Held / ControlPoses
            if (tracking.palmsRel == Formation.together)
            {
                Held = true;

                if (tracking.rightPose == HandPose.pointer && tracking.leftPose == HandPose.pointer)
                {
                    ControlPose = HandPose.pointer;
                }
                else if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    ControlPose = HandPose.fist;
                }
                else if (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
                {
                    ControlPose = HandPose.thumbsUp;
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

            #region RECALL
            if (tracking.rightPose == HandPose.fist && tracking.rightPalmRel == Direction.palmOut)
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

            if (lassoTimer < 3 && tracking.rightPose == HandPose.flat && tracking.rightPalmRel == Direction.palmOut)
            {
                Recall = true;
                recallPunchTimer = 0;
            }

            if (tracking.rightPose != HandPose.flat)
            {
                Recall = false;
            }
            #endregion

            #region FORCES
            if (tracking.palmsRel == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                Power = TheForce.push;
            }
            else if (tracking.palmsAbs == Formation.palmsUp && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            {
                Power = TheForce.lift;
            }
            else if (tracking.palmsRel == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                Power = TheForce.pull;
            }
            else if (tracking.palmsAbs == Formation.palmsDown && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            {
                Power = TheForce.down;
            }
            else
            {
                Power = TheForce.idle;
            }
            #endregion
        }
    }

}

