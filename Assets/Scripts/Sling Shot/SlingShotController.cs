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

        HandTracking handtracking;
        Sights bowSights;

        // Start is called before the first frame update
        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
            bowSights = GetComponent<Sights>();
        }

        // Update is called once per frame
        void Update()
        {
            if (handtracking.leftPeace)
			{
				float fingerDistance = Vector3.Distance(handtracking.ltIndexTip.Position, handtracking.ltMiddleTip.Position);
				ActivateSlingShot(fingerDistance);

				if (handtracking.rightFist)
				{
					PullSlingShot(bowSights.GetLeftSight());
				}
			}
			else if (handtracking.rightPeace)
			{
                float fingerDistance = Vector3.Distance(handtracking.rtIndexTip.Position, handtracking.rtMiddleTip.Position);
                ActivateSlingShot(fingerDistance);

                if (handtracking.leftFist)
				{
                    PullSlingShot(bowSights.GetRightSight());
				}
			}
			else
            {
                //deactivate sight and arrow Huds
                sightHUD.SetActive(false);
                pebbleHUD.SetActive(false);
                rightReadyToFire = false;
            }

            WatchForRelease();
        }

		private void PullSlingShot(Vector3 sights)
		{
			pebbleHUD.SetActive(true);
			pebbleHUD.transform.position = handtracking.rtIndexMid.Position;
			pebbleHUD.transform.LookAt(sights);
			rightReadyToFire = true;
		}

		private void ActivateSlingShot(float fingerDistance)
		{
			// activate rangeFinder object
			sightHUD.SetActive(true);
			// position between finger tips
			sightHUD.transform.position = bowSights.GetLeftSight();
			// size to distance
			sightHUD.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
			// rotate to face middle finger
			sightHUD.transform.LookAt(handtracking.ltMiddleTip.Position);

            //if ()
            if (handtracking.leftPeace)
            {
                handRingLeft.SetActive(true);
            }
            else handRingLeft.SetActive(false);

            if (handtracking.rightPeace)
            {
                handRingRight.SetActive(true);
            }
            else handRingRight.SetActive(false);
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
