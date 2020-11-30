using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.LightBow
{
    public class LightBowController : MonoBehaviour
    {
        [SerializeField] GameObject sightHud;
        [SerializeField] GameObject arrowHud;
        [SerializeField] GameObject arrowPrefab;
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
                sightHud.SetActive(true);
                // position between finger tips
                sightHud.transform.position = bowSights.GetLeftSight();
                // size to distance
                sightHud.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
                // rotate to face middle finger
                sightHud.transform.LookAt(handtracking.ltMiddleTip.Position);

                if (handtracking.rightFist)
                {
                    arrowHud.SetActive(true);
                    arrowHud.transform.position = handtracking.rtIndexMid.Position;
                    arrowHud.transform.LookAt(bowSights.GetLeftSight());
                    readyToFire = true;
                }
            }
            else
            {
                //deactivate sight and arrow Huds
                sightHud.SetActive(false);
                arrowHud.SetActive(false);
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
            arrowHud.SetActive(false);
            float bowForce = Vector3.Distance(handtracking.rightPalm.Position, sightHud.transform.position) * forceMultiplier;
            Debug.Log("Distance: " + Vector3.Distance(handtracking.rightPalm.Position, sightHud.transform.position));
            Debug.Log("force: " + bowForce); // TODO remove

            GameObject newArrow = Instantiate(arrowPrefab, handtracking.rightPalm.Position, arrowHud.transform.rotation);

            newArrow.GetComponent<ArrowController>().force = bowForce;

            readyToFire = false;
        }
    }
}
