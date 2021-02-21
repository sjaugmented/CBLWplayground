using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball {
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] GameObject ballPrefab;
        public bool Ball {get; set;}

        float conjureReady, destroyReady = Mathf.Infinity;

        HandTracking hands;
        
        void Start()
        {
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
        }

        void Update()
        {
            conjureReady += Time.deltaTime;
            destroyReady += Time.deltaTime;

            if (!Ball) {
                if (hands.rightFist && hands.rtPalmUpFloorUp < 10) {
                    conjureReady = 0;
                }

                if (conjureReady < 3 && hands.rightOpen) {
                    ConjureBall();
                }
            }
            else {
                if (hands.rightOpen && hands.rtPalmUpFloorUp > 170) {
                    destroyReady = 0;
                }

                if (destroyReady < 3 && hands.rightFist) {
                    StartCoroutine("DestroyBall");
                }
            }


        }

        GameObject ballInstance;
        [SerializeField] float destroyDelay = 1;

        private void ConjureBall()
        {
            Ball = true;
            ballInstance = Instantiate(ballPrefab, hands.rightPalm.Position, hands.rightPalm.Rotation);
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
