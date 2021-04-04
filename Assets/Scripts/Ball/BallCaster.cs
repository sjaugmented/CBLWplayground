using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball
{
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] AudioClip resetFX;
        [SerializeField] GameObject handColliders;
        [SerializeField] GameObject uniBall;
        [SerializeField] GameObject multiBall;
        [SerializeField] float zOffset = 0.1f;
        [SerializeField] int worldLvl = 0;
        public int WorldLevel {
            get { return worldLvl; }
            set { worldLvl = value; }
        }
        public bool BallInPlay { get; set; }

        public bool hasReset = false;
        bool conjureReady, destroyReady; /*hasReset = false;*/
        float conjureTimer, destroyTimer = Mathf.Infinity;

        NewTracking tracking;
        GameObject ballInstance;
        
        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();

            if (GameObject.FindGameObjectWithTag("Ball"))
            {
                BallInPlay = true;
                ballInstance = GameObject.FindGameObjectWithTag("Ball");
            }
            else
            {
                BallInPlay = false;
            }

            Debug.Log("Ball in Play: " + BallInPlay);

            WorldLevel = 1;
        }

        void Update()
        {
            conjureTimer += Time.deltaTime;
            destroyTimer += Time.deltaTime;

            if (tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.fist)
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

            if (conjureTimer < 1 && tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.flat)
            {
                ConjureBall();
            }
            else
            {
                hasReset = false;
            }

            //if (!BallInPlay)
            //{
            //    //if (tracking.rightPose == HandPose.fist && (tracking.rightPalmRel == Direction.up || tracking.rightPalmRel == Direction.up))
            //    //{
            //    //    if (!conjureReady)
            //    //    {
            //    //        conjureTimer = 0;
            //    //        conjureReady = true;
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    conjureReady = false;
            //    //}

            //    //if (conjureTimer < 3 && tracking.rightPose == HandPose.flat)
            //    //{
            //    //    ConjureBall();
            //    //}
            //}
            if (BallInPlay)
            {
                if (!ballInstance) { return; }

                if (tracking.rightPose == HandPose.rockOn && tracking.rightPalmRel == Direction.palmIn)
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

                if (destroyTimer < 1 && conjureTimer > 3 && tracking.rightPose == HandPose.fist)
                {
                    DestroyBall();
                }
            }

            if (ballInstance)
            {
                handColliders.SetActive(!ballInstance.GetComponent<Ball>().InteractingWithParticles);
            }
        }

        private void ConjureBall()
        {
            Debug.Log("Conjure/Reset");
            Vector3 offset = Camera.main.transform.InverseTransformDirection(0, 0, zOffset);
            
            if (!BallInPlay)
            {
                BallInPlay = true;
                ballInstance = Instantiate(uniBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            }
            else
            {
                Debug.Log("Ball is in play");
                if (!hasReset)
                {
                    Debug.Log("RESET");
                    ballInstance.transform.position = tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset;
                    ballInstance.transform.rotation = tracking.GetRtPalm.Rotation;
                    if (!GetComponent<AudioSource>().isPlaying)
                    {
                        GetComponent<AudioSource>().PlayOneShot(resetFX);
                    }
                    hasReset = true;
                }
            }
            

            //if (WorldLevel == 1)
            //{
            //    ballInstance = Instantiate(uniBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            //}
            //else
            //{
            //    ballInstance = Instantiate(multiBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            //}
        }

        public void DestroyBall()
        {
            if (ballInstance)
            {
                ballInstance.GetComponent<Ball>().StartCoroutine("DestroySelf");
            }
        }

        public void DisableHandColliders()
        {
            handColliders.SetActive(false);
        }

        public void EnableHandColliders()
        {
            handColliders.SetActive(true);
        }
    }
}
