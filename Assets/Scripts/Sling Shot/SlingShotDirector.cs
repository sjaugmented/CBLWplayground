using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.SlingShot
{
    public class SlingShotDirector : MonoBehaviour
    {
        [SerializeField] AudioClip handOn;
        [SerializeField] AudioClip handOff;
        [SerializeField] AudioClip buildOn;
        [SerializeField] AudioClip buildOff;

        [SerializeField] GameObject rightHandGroup, leftHandGroup, rightToggle, leftToggle;

        List<GameObject> rightHand = new List<GameObject>();
        List<GameObject> leftHand = new List<GameObject>();

        public bool HandPicker { get; set; }
        public bool BuildMode { get; set; }

        HandTracking handtracking;

        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();

            rightHand.Add(rightHandGroup);
            rightHand.Add(rightToggle);
            leftHand.Add(leftHandGroup);
            leftHand.Add(leftToggle);

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
            if (handtracking.rightHand)
			{
				SetRightHand(true);
			}
			else SetRightHand(false);

            if (handtracking.leftHand)
			{
				SetLeftHand(true);
			}
			else SetLeftHand(false);

            

            ///// DEV CONTROLS
            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleHandMode();
            }
        }

		public void ToggleHandMode()
		{
            if (HandPicker)
			{
                HandPicker = false;
                GetComponent<AudioSource>().PlayOneShot(handOff);
            }
            else
			{
                HandPicker = true;
                GetComponent<AudioSource>().PlayOneShot(handOn);
            }
        }

        public void ToggleBuildMode()
		{
            if (BuildMode)
			{
                BuildMode = false;
                GetComponent<AudioSource>().PlayOneShot(buildOff);
			}
            else
			{
                BuildMode = true;
                GetComponent<AudioSource>().PlayOneShot(buildOn);
			}
		}
	}
}

