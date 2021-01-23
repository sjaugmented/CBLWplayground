using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.SlingShot
{
    public class SlingShotDirector : MonoBehaviour
    {
        [SerializeField] AudioClip orbModeOn;
        [SerializeField] AudioClip orbModeOff;

        [SerializeField] GameObject rightHandGroup, leftHandGroup, rightToggle, leftToggle, rightDorsal, leftDorsal;

        List<GameObject> rightHand = new List<GameObject>();
        List<GameObject> leftHand = new List<GameObject>();

        public bool OrbMode { get; set; }

        HandTracking handtracking;

        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();

            rightHand.Add(rightHandGroup);
            rightHand.Add(rightToggle);
            rightHand.Add(rightDorsal);
            leftHand.Add(leftHandGroup);
            leftHand.Add(leftToggle);
            leftHand.Add(leftDorsal);

            SetRightHand(false);
            SetLeftHand(false);
        }

        private void SetRightHand(bool set)
        {
            foreach (GameObject asset in rightHand)
            {
                asset.SetActive(set);
            }
        }

        private void SetLeftHand(bool set)
        {
            foreach (GameObject asset in leftHand)
            {
                asset.SetActive(set);
            }
        }

        void Update()
        {
            if (handtracking.rightHand) SetRightHand(true);
            else SetRightHand(false);

            if (handtracking.leftHand) SetLeftHand(true);
            else SetLeftHand(false);

            ///// DEV CONTROLS
            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleOrbMode();
            }
        }

		public void ToggleOrbMode()
		{
            if (OrbMode)
			{
                OrbMode = false;
                GetComponent<AudioSource>().PlayOneShot(orbModeOff);
            }
            else
			{
                OrbMode = true;
                GetComponent<AudioSource>().PlayOneShot(orbModeOn);
            }
        }
	}
}

