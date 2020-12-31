using LW.Core;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
	public class RunicDirector : MonoBehaviour
    {
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle, rightDorsal, leftDorsal;

        List<GameObject> rightHand = new List<GameObject>();
        List<GameObject> leftHand = new List<GameObject>();

		public enum Mode { Touch, Node };
		public Mode currentMode = Mode.Touch;

		public bool node = false;
		public bool Node
		{
			get { return node; }
			set { node = value; }
		}

		public bool gaze = false;
		public bool Gaze
		{
			get { return gaze; }
			set { gaze = value; }
		}

		HandTracking handtracking;
		AudioSource audio;
        
        void Start()
		{
			handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
			
			rightHand.Add(rightPointer);
			rightHand.Add(rightToggle);
			rightHand.Add(rightDorsal);
			leftHand.Add(leftPointer);
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
			SetGazeProvider();
			
			if (handtracking.rightHand) SetRightHand(true);
			else SetRightHand(false);

			if (handtracking.leftHand) SetLeftHand(true);
			else SetLeftHand(false);

			///// DEV CONTROLS
			if (Input.GetKeyDown(KeyCode.N))
			{
				ToggleNode();
			}

			if (Input.GetKeyDown(KeyCode.M))
			{
				ToggleGaze();
			}

        }

        public void ToggleNode()
		{
			if (currentMode == Mode.Touch) currentMode = Mode.Node;
			else currentMode = Mode.Touch;

			GetComponent<AudioSource>().PlayOneShot(nodeTap);
			Node = !Node;
		}

		public void ToggleGaze()
		{
			GetComponent<AudioSource>().PlayOneShot(gazeTap);
			Gaze = !Gaze;
		}

		private void SetGazeProvider()
		{
			GazeProvider gazeProvider = Camera.main.GetComponent<GazeProvider>();

			if (Gaze) gazeProvider.enabled = true;
			else gazeProvider.enabled = false;
		}
    }
}
