using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball
{
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] AudioSource forceFX;
        [SerializeField] AudioClip freezeFX;
        [SerializeField] GameObject uniBall;
        [SerializeField] GameObject multiBall;
        [SerializeField] float zOffset = 0.1f;
        [SerializeField] float holdDistance = 0.5f;
        [SerializeField] float boostMultiplier = 5;
        [SerializeField] float summonMultiplier = 5;
        [SerializeField] float liftMultiplier = 3;
        [SerializeField] int worldLvl = 0;
        public int WorldLevel {
            get { return worldLvl; }
            set { worldLvl = value; }
        }
        public bool BallInPlay { get; set; }
        public bool Held { get; set; }
        public bool Frozen { get; set; }

        bool conjureReady, destroyReady = false;
        float conjureTimer, destroyTimer = Mathf.Infinity;

        NewTracking tracking;
        CastOrigins origins;
        GameObject ballInstance;
        private bool leftFisted;
        private bool rightFisted;
        private bool dualFisted;
        bool hasBoosted = false;
        bool frozenTriggered = false;

        bool holdOSC, summonOSC, liftOSC, deadOSC;


        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();

            if (GameObject.FindGameObjectWithTag("Ball"))
            {
                BallInPlay = true;
                ballInstance = GameObject.FindGameObjectWithTag("Ball");
            }
            else
            {
                BallInPlay = false;
            }

            WorldLevel = 1;
        }


        void Update()
        {
            conjureTimer += Time.deltaTime;
            destroyTimer += Time.deltaTime;

            if (!BallInPlay)
            {
                if (tracking.rightPose == HandPose.fist && tracking.rightPalm == Direction.up)
                {
                    if (!conjureReady)
                    {
                        conjureTimer = 0;
                        conjureReady = true;
                    }
                }
                else
                {
                    conjureReady = false;
                }

                if (conjureTimer < 3 && tracking.rightPose == HandPose.flat)
                {
                    ConjureBall();
                }
            }
            else
            {
                if (!ballInstance) { return; }

                ballInstance.GetComponent<Rigidbody>().useGravity = !Held && !Frozen;
                ballInstance.GetComponent<ConstantForce>().enabled = !Held && !Frozen;
                
                if (tracking.rightPose == HandPose.flat && tracking.rightPalm == Direction.palmIn)
                {
                    if (!destroyReady)
                    {
                        destroyTimer = 0;
                        destroyReady = true;
                    }

                }
                else
                {
                    destroyReady = false;
                }

                if (destroyTimer < 1 && tracking.rightPose == HandPose.fist)
                {
                    DestroyBall();
                }

                // catch, manipulate, throw
                if (tracking.palms == Formation.together && origins.PalmsDist < holdDistance)
                {
                    Held = true;
                    if (!holdOSC)
                    {
                        ballInstance.GetComponent<Ball>().SendOSC("holding");
                        holdOSC = true;
                    }

                    // prox floats
                    if (tracking.rightPose != HandPose.fist && tracking.leftPose == HandPose.fist)
                    {
                        if (!leftFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("leftFistOn");
                            leftFisted = true;
                        }

                        ballInstance.GetComponent<Ball>().SendOSC("leftProximityAngle", 1 - tracking.StaffUp / 180);
                        ballInstance.GetComponent<Ball>().SendOSC("leftProximityDistance", origins.PalmsDist / holdDistance);
                    }
                    else
                    {
                        if (leftFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("leftFistOff");
                            leftFisted = false;
                        }
                    }

                    if (tracking.rightPose == HandPose.fist && tracking.leftPose != HandPose.fist)
                    {
                        if (!rightFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("rightFistOn");
                            rightFisted = true;
                        }

                        ballInstance.GetComponent<Ball>().SendOSC("rightProximityAngle", 1 - tracking.StaffUp / 180);
                        ballInstance.GetComponent<Ball>().SendOSC("rightProximityDistance", origins.PalmsDist / holdDistance);
                    }
                    else
                    {
                        if (rightFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("rightFistOff");
                            rightFisted = false;
                        }
                    }

                    if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                    {
                        if (!dualFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("dualFistOn");
                            dualFisted = true;
                        }

                        ballInstance.GetComponent<Ball>().SendOSC("proximityAngle", 1 - tracking.StaffUp / 180);
                        ballInstance.GetComponent<Ball>().SendOSC("proximityDistance", origins.PalmsDist / holdDistance);
                    }
                    else
                    {
                        if (dualFisted)
                        {
                            ballInstance.GetComponent<Ball>().SendOSC("dualFistOff");
                            dualFisted = false;
                        }
                    }

                    if (tracking.rightPose == HandPose.thumbsUp && tracking.leftPose == HandPose.thumbsUp)
                    {
                        if (!frozenTriggered)
                        {
                            Frozen = !Frozen;

                            if(!Frozen && !GetComponent<AudioSource>().isPlaying)
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
                else {
                    Held = false;
                    holdOSC = false;
                }

                // boosting
                if (tracking.palms == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    if (!hasBoosted)
                    {
                        ballInstance.GetComponent<Ball>().SendOSC("boosting");
                        hasBoosted = true;
                    }
                    ballInstance.transform.LookAt(2 * ballInstance.transform.position - Camera.main.transform.position);
                    ballInstance.GetComponent<Rigidbody>().AddForce(ballInstance.transform.forward * (origins.PalmsDist / holdDistance * boostMultiplier));

                    forceFX.Play();
                }
                else { 
                    hasBoosted = false;
                    forceFX.Stop();
                }

                // summoning
                if (tracking.palms == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    if (!hasBoosted)
                    {
                        ballInstance.GetComponent<Ball>().SendOSC("summoning");
                        hasBoosted = true;
                    }

                    ballInstance.transform.LookAt(Camera.main.transform.position);
                    ballInstance.GetComponent<Rigidbody>().AddForce(ballInstance.transform.forward * (origins.PalmsDist / holdDistance * summonMultiplier));

                    forceFX.Play();
                }
                else {
                    hasBoosted = false;
                    forceFX.Stop();
                }

                // lift
                if (tracking.palms == Formation.palmsUp && (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat))
                {
                    if (!hasBoosted)
                    {
                        ballInstance.GetComponent<Ball>().SendOSC("lifting");
                        hasBoosted = true;
                    }

                    ballInstance.transform.rotation = new Quaternion(0, 0, 0, 0);
                    ballInstance.GetComponent<Rigidbody>().AddForce(ballInstance.transform.up * (origins.PalmsDist / holdDistance * liftMultiplier));

                    forceFX.Play();
                }
                else
                {
                    hasBoosted = false;
                    forceFX.Stop();
                }
            }
        }

        private void ConjureBall()
        {
            Frozen = false;
            Held = false;
            BallInPlay = true;
            Vector3 offset = Camera.main.transform.InverseTransformDirection(0, 0, zOffset);
            
            if (WorldLevel == 1)
            {
                ballInstance = Instantiate(uniBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            }
            else
            {
                ballInstance = Instantiate(multiBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            }
        }

        public void DestroyBall()
        {
            if (ballInstance)
            {
                ballInstance.GetComponent<Ball>().StartCoroutine("DestroySelf");
            }
        }
    }
}
