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
        public bool Ball { get; set; }
        public bool Held { get; set; }

        bool conjureReady, destroyReady = false;
        float conjureTimer, destroyTimer = Mathf.Infinity;

        // HandTracking hands;
        NewTracking tracking;
        CastOrigins origins;
        GameObject ballInstance;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();

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
                    //// TODO
                    //// remote test
                    //forceField.SetActive(true);
                    //forceField.transform.position = origins.PalmsMidpoint;
                    //forceField.GetComponent<CapsuleCollider>().height = origins.PalmsDist;

                    //if (tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                    //{
                    //    // neutral
                    //}

                    //if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                    //{
                    //    // manipulate
                    //}

                    ballInstance.GetComponent<Rigidbody>().useGravity = false;
                    ballInstance.GetComponent<ConstantForce>().enabled = false;
                }
                else {
                    //forceField.SetActive(false); 

                    ballInstance.GetComponent<Rigidbody>().useGravity = true;
                    ballInstance.GetComponent<ConstantForce>().enabled = true;
                }

                //if (forceField.GetComponent<ForceField>().Caught && tracking.palms == Formation.palmsOut)
                //{
                //    // throw

                //    // TODO
                //    // remote test
                //    forceField.GetComponent<ForceField>().Caught = false;
                //    Quaternion castRotation = Quaternion.Slerp(tracking.GetRtPalm.Rotation, tracking.GetLtPalm.Rotation, 0.5f) * Quaternion.Euler(60, 0, 0);
                //    float force = (1 - (origins.PalmsDist / holdDistance)) * 50;
                    
                //    //ballInstance.transform.LookAt(2 * transform.position - Camera.main.transform.position);
                //    ballInstance.transform.rotation = castRotation;
                //    ballInstance.GetComponent<Rigidbody>().AddForce(transform.forward * force);
                //}
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
            ballInstance.GetComponent<Ball>().StartCoroutine("DestroySelf");
        }
    }
}
