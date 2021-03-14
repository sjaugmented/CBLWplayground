using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace LW.SlingShot
{
    public class SlingShotController : MonoBehaviour
    {
        [SerializeField] GameObject sightHUD;
        [SerializeField] GameObject pebbleHUD;
        [SerializeField] GameObject pebblePrefab;
        [SerializeField] GameObject handRingRight;
        [SerializeField] GameObject handRingLeft;
        [SerializeField] float forceMultiplier = 100;

        public bool rightReadyToFire = false;
        public bool leftReadyToFire = false;

        float fingerDistance;

        NewTracking tracking;
        SlingShotDirector director;
        Sights sights;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
            sights = GetComponent<Sights>();
        }

        void Update()
        {
            MixedRealityPose rtMiddleKnuckle, ltMiddleKnuckle;

            HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleKnuckle, Handedness.Right, out rtMiddleKnuckle);
            HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleKnuckle, Handedness.Left, out ltMiddleKnuckle);

            if (director.BuildMode) { return; }

            if (tracking.leftPose == HandPose.peace)
			{
				fingerDistance = Vector3.Distance(tracking.GetLtIndex.Position, tracking.GetLtMiddle.Position);
				ActivateSlingShot(handRingLeft, sights.GetLeftSight(), tracking.GetLtMiddle.Position);

				if (tracking.rightPose == HandPose.fist)
				{
					PullSlingShot(sights.GetLeftSight(), rtMiddleKnuckle.Position);
				}
			}
			else if (tracking.rightPose == HandPose.peace)
			{
                fingerDistance = Vector3.Distance(tracking.GetRtIndex.Position, tracking.GetRtMiddle.Position);
                ActivateSlingShot(handRingRight, sights.GetRightSight(), tracking.GetRtMiddle.Position);

                if (tracking.leftPose == HandPose.fist)
				{
                    PullSlingShot(sights.GetRightSight(), ltMiddleKnuckle.Position);
				}
			}
			else
            {
                sightHUD.SetActive(false);
                pebbleHUD.SetActive(false);
                handRingRight.SetActive(false);
                handRingLeft.SetActive(false);
                rightReadyToFire = false;
            }

            WatchForRelease();
        }

		private void ActivateSlingShot(GameObject handRing, Vector3 sights, Vector3 middleFinger)
		{
			sightHUD.SetActive(true);
			sightHUD.transform.position = sights;
			sightHUD.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
			sightHUD.transform.LookAt(middleFinger);

            if (director.SlingShot) handRing.SetActive(true);
		}

        private void PullSlingShot(Vector3 sights, Vector3 launchPosition)
        {
            pebbleHUD.SetActive(true);
            pebbleHUD.transform.position = launchPosition;
            pebbleHUD.transform.LookAt(sights);
            rightReadyToFire = true;
        }

        private void WatchForRelease()
        {
            // TODO can you combine these toggles?
            if (rightReadyToFire)
            {
                if (tracking.rightPose != HandPose.fist) Fire();
            }

            if (leftReadyToFire)
			{
                if (tracking.leftPose != HandPose.fist) Fire();
			}
        }

        private void Fire()
        {
            pebbleHUD.SetActive(false);
            float force = Vector3.Distance(tracking.GetRtPalm.Position, sightHUD.transform.position) * forceMultiplier;

            GameObject newPebble = Instantiate(pebblePrefab, tracking.GetRtPalm.Position, pebbleHUD.transform.rotation);

            newPebble.GetComponent<PebbleController>().Force = force;

            rightReadyToFire = false;
            leftReadyToFire = false;
        }
    }
}
