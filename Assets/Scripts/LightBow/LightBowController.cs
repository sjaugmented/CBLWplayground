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
        [SerializeField] GameObject arrow;

        bool readyToFire = false;

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
                    arrow.SetActive(true);
                    arrow.transform.LookAt(bowSights.GetLeftSight());
                    readyToFire = true;
                    WatchForRelease();
                }
                else
                {
                    arrow.SetActive(false);
                }
            }
            else if (handtracking.rightPeace)
            {
                float fingerDistance = Vector3.Distance(handtracking.rtIndexTip.Position, handtracking.rtMiddleTip.Position);
                // activate rangeFinder object
                sightHud.SetActive(true);
                // position between finger tips
                sightHud.transform.position = bowSights.GetRightSight();
                // size to distance
                sightHud.transform.localScale = new Vector3(fingerDistance, fingerDistance, fingerDistance);
                // rotate to face middle finger
                sightHud.transform.LookAt(handtracking.rtMiddleTip.Position);
            }
            else
            {
                //deactivate sight
                sightHud.SetActive(false);
                arrow.SetActive(false);
            }
        }

        private void WatchForRelease()
        {
            if (readyToFire && !handtracking.rightFist)
            {

            }
        }
    }
}
