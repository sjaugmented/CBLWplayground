using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

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

        HandTracking handtracking;
        SlingShotDirector director;
        Sights sights;

        // Start is called before the first frame update
        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
            sights = GetComponent<Sights>();
        }

        // Update is called once per frame
        void Update()
        {
            if (handtracking.leftPeace)
			{
				fingerDistance = Vector3.Distance(handtracking.ltIndexTip.Position, handtracking.ltMiddleTip.Position);
				ActivateSlingShot(handRingLeft, sights.GetLeftSight(), handtracking.ltMiddleTip.Position);

				if (handtracking.rightFist)
				{
					PullSlingShot(sights.GetLeftSight());
				}
			}
			else if (handtracking.rightPeace)
			{
                fingerDistance = Vector3.Distance(handtracking.rtIndexTip.Position, handtracking.rtMiddleTip.Position);
                ActivateSlingShot(handRingRight, sights.GetRightSight(), handtracking.rtMiddleTip.Position);

                if (handtracking.leftFist)
				{
                    PullSlingShot(sights.GetRightSight());
				}
			}
			else
            {
                //deactivate HUDs
                sightHUD.SetActive(false);
                pebbleHUD.SetActive(false);
                handRingRight.SetActive(false);
                handRingLeft.SetActive(false);
                rightReadyToFire = false;
            }

            WatchForRelease();
        }

		private void ActivateSlingShot(GameObject handRing, Vector3 sights, Vector3 sightsDirection)
		{
			// activate rangeFinder object
			sightHUD.SetActive(true);
			// position between finger tips
			sightHUD.transform.position = sights;
			// size to distance
			sightHUD.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
			// rotate to face middle finger
			sightHUD.transform.LookAt(sightsDirection);

            if (director.HandPicker) handRing.SetActive(true);
		}

        private void PullSlingShot(Vector3 sights)
        {
            pebbleHUD.SetActive(true);
            pebbleHUD.transform.position = handtracking.rtIndexMid.Position;
            pebbleHUD.transform.LookAt(sights);
            rightReadyToFire = true;
        }

        private void WatchForRelease()
        {
            if (rightReadyToFire)
            {
                if (!handtracking.rightFist) Fire();
            }

            if (leftReadyToFire)
			{
                if (!handtracking.leftFist) Fire();
			}
        }

        private void Fire()
        {
            pebbleHUD.SetActive(false);
            float force = Vector3.Distance(handtracking.rightPalm.Position, sightHUD.transform.position) * forceMultiplier;
            Debug.Log("Distance: " + Vector3.Distance(handtracking.rightPalm.Position, sightHUD.transform.position));
            Debug.Log("force: " + force); // TODO remove

            GameObject newArrow = Instantiate(pebblePrefab, handtracking.rightPalm.Position, pebbleHUD.transform.rotation);

            newArrow.GetComponent<PebbleController>().Force = force;

            rightReadyToFire = false;
        }
    }
}
