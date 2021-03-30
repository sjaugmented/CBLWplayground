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
        public bool Held { get; set; }
        public bool Frozen { get; set; }
        public Hands fists = Hands.none;
        public TheForce power = TheForce.idle;

        bool frozenTriggered;

        NewTracking tracking;
        CastOrigins origins;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
        }

        void Update()
        {
            GetComponent<Rigidbody>().useGravity = !Held && !Frozen;
            GetComponent<ConstantForce>().enabled = !Held && !Frozen;

            if (tracking.palms == Formation.together && origins.PalmsDist < holdDistance)
            {
                Held = true;

                if (tracking.rightPose != HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    fists = Hands.left;
                }

                if (tracking.rightPose == HandPose.fist && tracking.leftPose != HandPose.fist)
                {
                    fists = Hands.right;
                }

                if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    fists = Hands.both;
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


            if (tracking.palms == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                power = TheForce.push;
                //forceFX.Play();
            }
            else if (tracking.palms == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
            {
                power = TheForce.pull;
                //forceFX.Play();
            }
            else if (tracking.palms == Formation.palmsUp && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
            {
                power = TheForce.lift;
                //forceFX.Play();
            }
            else
            {
                power = TheForce.idle;
                //forceFX.Stop();
            }
        }
    }

}

