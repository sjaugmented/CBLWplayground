using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Ball
{
    public enum TheForce { push, pull, lift, idle}
    public class BallJedi : MonoBehaviour
    {
        //[SerializeField] AudioSource forceFX;
        [SerializeField] AudioClip freezeFX;
        [SerializeField] float pushMultiplier = 5;
        [SerializeField] float pullMultiplier = 5;
        [SerializeField] float liftMultiplier = 3;
        [SerializeField] float recallMultiplier = 10;
        [SerializeField] float holdDistance = 0.5f;
        public float HoldDistance
        {
            get { return holdDistance; }
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
        public bool Held { get; set; }
        public bool Frozen { get; set; }
        public bool Recall { get; set; }
        public Hands Fists = Hands.none;
        public TheForce Power = TheForce.idle;

        bool frozenTriggered, lassoReady;
        float lassoTimer = Mathf.Infinity;

        NewTracking tracking;
        CastOrigins origins;
        Rigidbody rigidbody;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            lassoTimer += Time.deltaTime;

            GetComponent<ConstantForce>().enabled = !Held && !Frozen;
            rigidbody.useGravity = !Held && !Frozen;

            #region HOLDING & FISTS
            if (tracking.palms == Formation.together && origins.PalmsDist < holdDistance)
            {
                Held = true;

                if (tracking.rightPose != HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    Fists = Hands.left;
                }

                if (tracking.rightPose == HandPose.fist && tracking.leftPose != HandPose.fist)
                {
                    Fists = Hands.right;
                }

                if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    Fists = Hands.both;
                }

                if (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
                {
                    if (!frozenTriggered)
                    {
                        Frozen = !Frozen;

                        if (!Frozen && !GetComponent<AudioSource>().isPlaying)
                        {
                            GetComponent<AudioSource>().PlayOneShot(freezeFX);
                        }

                        frozenTriggered = true;
                    }
                }
                if (tracking.rightPose != HandPose.thumbsUp && tracking.leftPose != HandPose.thumbsUp)
                {
                    frozenTriggered = false;
                }
            }
            else
            {
                Held = false;
            }
            #endregion

            #region RETRIEVE
            if (tracking.rightPose == HandPose.fist && tracking.rightPalm == Direction.palmOut)
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

            if (lassoTimer < 3 && tracking.rightPose == HandPose.flat && tracking.rightPalm == Direction.palmOut)
            {
                Recall = true;
            }
            //else
            //{
            //    Recall = false;
            //}
            if (tracking.rightPose != HandPose.fist)
            {
                Recall = false;
            }
            #endregion

            #region FORCES
            if (tracking.palms == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                Power = TheForce.push;
                //forceFX.Play();
            }
            else if (tracking.palms == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                Power = TheForce.pull;
                //forceFX.Play();
            }
            else if (tracking.palms == Formation.palmsUp && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            {
                Power = TheForce.lift;
                //forceFX.Play();
            }
            else
            {
                Power = TheForce.idle;
                //forceFX.Stop();
            }
            #endregion
        }
    }

}

