using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.SlingShot
{
    public class LightBowController : MonoBehaviour
    {
        [SerializeField] GameObject sightHUD;
        [SerializeField] GameObject pebbleHUD;
        [SerializeField] GameObject pebblePrefab;
        [SerializeField] float forceMultiplier = 100;

        public bool readyToFire = false;

        HandTracking handtracking;
        BowSights bowSights;

        // Start is called before the first frame update
        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            bowSights = GetComponent<BowSights>();
        }

        // Update is called once per frame
        void Update()
        {
            if (handtracking.leftPeace)
            {
                float fingerDistance = Vector3.Distance(handtracking.ltIndexTip.Position, handtracking.ltMiddleTip.Position);
                // activate rangeFinder object
                sightHUD.SetActive(true);
                // position between finger tips
                sightHUD.transform.position = bowSights.GetLeftSight();
                // size to distance
                sightHUD.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
                // rotate to face middle finger
                sightHUD.transform.LookAt(handtracking.ltMiddleTip.Position);

                if (handtracking.rightFist)
                {
                    pebbleHUD.SetActive(true);
                    pebbleHUD.transform.position = handtracking.rtIndexMid.Position;
                    pebbleHUD.transform.LookAt(bowSights.GetLeftSight());
                    readyToFire = true;
                }
            }
            else
            {
                //deactivate sight and arrow Huds
                sightHUD.SetActive(false);
                pebbleHUD.SetActive(false);
                readyToFire = false;
            }

            WatchForRelease();
        }

        private void WatchForRelease()
        {
            if (readyToFire)
            {
                if (!handtracking.rightFist) FireArrow();
            }
        }

        private void FireArrow()
        {
            pebbleHUD.SetActive(false);
            float bowForce = Vector3.Distance(handtracking.rightPalm.Position, sightHUD.transform.position) * forceMultiplier;
            Debug.Log("Distance: " + Vector3.Distance(handtracking.rightPalm.Position, sightHUD.transform.position));
            Debug.Log("force: " + bowForce); // TODO remove

            GameObject newArrow = Instantiate(pebblePrefab, handtracking.rightPalm.Position, pebbleHUD.transform.rotation);

            newArrow.GetComponent<ArrowController>().force = bowForce;

            readyToFire = false;
        }
    }
}
