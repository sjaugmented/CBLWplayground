using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball {
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] GameObject ballPrefab;
        [SerializeField] float zOffset = 0.1f;
        public bool Ball {get; set;}

        bool conjureReady, destroyReady = false;
        float conjureTimer, destroyTimer = Mathf.Infinity;

        // HandTracking hands;
        NewTracking tracking;
        GameObject ballInstance;
        
        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();

            if (GameObject.FindGameObjectWithTag("Ball")) {
                Ball = true;
                ballInstance = GameObject.FindGameObjectWithTag("Ball");
            }
            else {
                Ball = false;
            }
        }

        void Update()
        {
            conjureTimer += Time.deltaTime;
            destroyTimer += Time.deltaTime;

            if (!Ball) {
                if (tracking.rightPose == LWPose.fist && tracking.rightPalm == Direction.up) {
                    if (!conjureReady) {
                        conjureTimer = 0;
                        conjureReady = true;
                        Debug.Log("conjureReady");
                    }
                }
                else {
                    conjureReady = false;
                }

                if (conjureTimer < 3 && tracking.rightPose == LWPose.flat) {
                    ConjureBall();
                }
            }
            else {
                if (tracking.rightPose == LWPose.flat && tracking.rightPalm == Direction.palmIn) {
                    if (!destroyReady) {
                        destroyTimer = 0;
                        destroyReady = true;
                        Debug.Log("destroyReady");
                    }
                    
                } else {
                    destroyReady = false;
                }

                if (destroyTimer < 1 && tracking.rightPose == LWPose.fist) {
                    DestroyBall();
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
            ballInstance.GetComponent<Ball>().StartCoroutine("DestroySelf");
        }
    }
}
