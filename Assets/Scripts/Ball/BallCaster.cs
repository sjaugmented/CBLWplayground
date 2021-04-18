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
        public bool RightBallInPlay { get; set; }

        bool conjureReady, hasReset;
        float conjureTimer = Mathf.Infinity;

        public Vector3 SpawnOffset { get; set; }

        NewTracking tracking;
        GameObject rightBall;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();

            if (GameObject.FindGameObjectWithTag("Ball"))
            {
                RightBallInPlay = true;
                rightBall = GameObject.FindGameObjectWithTag("Ball");
            }
            else
            {
                RightBallInPlay = false;
            }

            WorldLevel = 1;
        }

        void Update()
        {
            SpawnOffset = new Vector3(0, 0.1f, 0) + Camera.main.transform.InverseTransformDirection(0, 0, 0.03f);
            conjureTimer += Time.deltaTime;

            if (rightBall)
            {
                handColliders.SetActive(!rightBall.GetComponent<Ball>().InteractingWithParticles);
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
                if (!RightBallInPlay)
                {
                    ConjureBall();
                }
            }
        }

        private void ConjureBall()
        {
            RightBallInPlay = true;
            rightBall = Instantiate(uniBall, tracking.GetRtPalm.Position + SpawnOffset, tracking.GetRtPalm.Rotation);
        }

        public void NextWorldLevel()
        {
            worldLvl = worldLvl < 4 ? worldLvl + 1 : 1;
        }

        public void DestroyBall()
        {
            if (rightBall)
            {
                rightBall.GetComponent<Ball>().StartCoroutine("DestroySelf");
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
