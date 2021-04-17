using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.Ball
{
    public class BallCaster : MonoBehaviour
    {
        [SerializeField] GameObject handColliders;
        [SerializeField] GameObject uniBall;
        [SerializeField] GameObject multiBall;
        [SerializeField] int worldLvl = 0;
        public int WorldLevel
        {
            get { return worldLvl; }
            set { worldLvl = value; }
        }
        public bool BallInPlay { get; set; }

        public bool hasReset = false;
        bool conjureReady, destroyReady; /*hasReset = false;*/
        float conjureTimer, destroyTimer = Mathf.Infinity;

        public Vector3 SpawnOffset { get; set; }

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

            WorldLevel = 1;
        }

        void Update()
        {
            SpawnOffset = new Vector3(0, 0.1f, 0) + Camera.main.transform.InverseTransformDirection(0, 0, 0.03f);
            conjureTimer += Time.deltaTime;
            destroyTimer += Time.deltaTime;

            if (ballInstance)
            {
                handColliders.SetActive(!ballInstance.GetComponent<Ball>().InteractingWithParticles);
            }

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
                if (!BallInPlay)
                {
                    ConjureBall();
                }
            }

            //if (BallInPlay)
            //{
            //    if (!ballInstance) { return; }

            //    if (ballInstance.GetComponent<Ball>().State == BallState.Still)
            //    {
            //        if (tracking.rightPose == HandPose.rockOn && tracking.rightPalmRel == Direction.palmIn)
            //        {
            //            if (!destroyReady)
            //            {
            //                destroyTimer = 0;
            //                destroyReady = true;
            //            }
            //        }
            //        else
            //        {
            //            destroyReady = false;
            //        }

            //        if (destroyTimer < 1 && conjureTimer > 3 && tracking.rightPose == HandPose.fist)
            //        {
            //            DestroyBall();
            //        }
            //    }
            //}
        }

        private void ConjureBall()
        {
            BallInPlay = true;
            ballInstance = Instantiate(uniBall, tracking.GetRtPalm.Position + SpawnOffset, tracking.GetRtPalm.Rotation);

            //if (WorldLevel == 1)
            //{
            //    ballInstance = Instantiate(uniBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            //}
            //else
            //{
            //    ballInstance = Instantiate(multiBall, tracking.GetRtPalm.Position + new Vector3(0, 0.1f, 0) + offset, tracking.GetRtPalm.Rotation);
            //}
        }

        //private void ResetBall()
        //{
        //    if (!hasReset)
        //    {
        //        ballInstance.transform.position = tracking.GetRtPalm.Position + SpawnOffset;
        //        ballInstance.transform.rotation = tracking.GetRtPalm.Rotation;
        //        if (ballInstance.GetComponent<Ball>().HasSpawned)
        //        {
        //            ballInstance.GetComponent<BallOsc>().Send("reset");
        //        }

        //        if (!GetComponent<AudioSource>().isPlaying)
        //        {
        //            GetComponent<AudioSource>().PlayOneShot(resetFX);
        //        }

        //        hasReset = true;
        //    }
        //}

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
