using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball {
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] GameObject ballPrefab;
        [SerializeField] float destroyDelay = 1;
        public bool Ball {get; set;}

        bool conjureReady, destroyReady = false;
        float conjureTimer, destroyTimer = Mathf.Infinity;

        HandTracking hands;
        GameObject ballInstance;
        
        void Start()
        {
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();

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
                if (hands.rightFist && hands.rtPalmUpFloorUp > 140) {
                    if (!conjureReady) {
                        conjureTimer = 0;
                        conjureReady = true;
                        Debug.Log("conjureReady");
                    }
                }
                else {
                    conjureReady = false;
                }

                if (conjureTimer < 3 && hands.rightOpen) {
                    ConjureBall();
                }
            }
            else {
                if (hands.rightOpen && hands.rtPalmUpFloorUp < 40) {
                    if (!destroyReady) {
                        destroyTimer = 0;
                        destroyReady = true;
                        Debug.Log("destroyReady");
                    }
                    
                } else {
                    destroyReady = false;
                }

                if (destroyTimer < 3 && hands.rightFist) {
                    StartCoroutine("DestroyBall");
                }
            }
        }


        private void ConjureBall()
        {
            Ball = true;
            ballInstance = Instantiate(ballPrefab, hands.rightPalm.Position + new Vector3(0, 0.1f, 0), hands.rightPalm.Rotation);
        }

        IEnumerator DestroyBall()
        {
            ballInstance.GetComponent<Ball>().DestroySelf();
            yield return new WaitForSeconds(destroyDelay);
            Destroy(ballInstance);
            Ball = false;
        }
    }
}
