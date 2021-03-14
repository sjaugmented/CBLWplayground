using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball
{
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] GameObject forceField;
        [SerializeField] GameObject ballPrefab;
        [SerializeField] float zOffset = 0.1f;
        [SerializeField] float holdDistance = 0.3f;
        [SerializeField] float forceMultiplier = 100;
        public bool Ball { get; set; }
        public bool Held { get; set; }

        bool conjureReady, destroyReady = false;
        float conjureTimer, destroyTimer = Mathf.Infinity;

        NewTracking tracking;
        CastOrigins origins;
        GameObject ballInstance;
        OSC osc;
        private bool leftFisted;
        private bool rightFisted;
        private bool dualFisted;
        bool hasBoosted = false;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();

            if (GameObject.FindGameObjectWithTag("Ball"))
            {
                Ball = true;
                ballInstance = GameObject.FindGameObjectWithTag("Ball");
            }
            else
            {
                Ball = false;
            }
        }

        void Update()
        {
            conjureTimer += Time.deltaTime;
            destroyTimer += Time.deltaTime;

            if (!Ball)
            {
                if (tracking.rightPose == HandPose.fist && tracking.rightPalm == Direction.up)
                {
                    if (!conjureReady)
                    {
                        conjureTimer = 0;
                        conjureReady = true;
                        Debug.Log("conjureReady");
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

                if (tracking.rightPose == HandPose.flat && tracking.rightPalm == Direction.palmIn)
                {
                    if (!destroyReady)
                    {
                        destroyTimer = 0;
                        destroyReady = true;
                        Debug.Log("destroyReady");
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
                    ballInstance.GetComponent<Rigidbody>().useGravity = false;
                    ballInstance.GetComponent<ConstantForce>().enabled = false;

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
                }
                else {
                    ballInstance.GetComponent<Rigidbody>().useGravity = true;
                    ballInstance.GetComponent<ConstantForce>().enabled = true;
                }

                // boosting
                if (tracking.palms == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    if (!hasBoosted)
                    {
                        ballInstance.GetComponent<Ball>().WorldLevel++;
                        ballInstance.GetComponent<Ball>().TouchLevel = 0;
                        ballInstance.GetComponent<Ball>().SendOSC("boosting");
                        ballInstance.transform.LookAt(2 * ballInstance.transform.position - Camera.main.transform.position);
                        ballInstance.GetComponent<Rigidbody>().AddForce(ballInstance.transform.forward * forceMultiplier);
                        hasBoosted = true;
                    }
                }
                else { hasBoosted = false; }

                if (tracking.palms == Formation.palmsIn && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    ballInstance.transform.LookAt(Camera.main.transform.position);
                    ballInstance.GetComponent<Rigidbody>().AddForce(ballInstance.transform.forward * (forceMultiplier / 5));
                }
            }
        }

        private void ConjureBall()
        {
            Ball = true;
            Vector3 offset = Camera.main.transform.InverseTransformDirection(0, 0, zOffset);
            ballInstance = Instantiate(ballPrefab, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
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
