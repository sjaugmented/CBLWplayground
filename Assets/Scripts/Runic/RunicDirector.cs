using LW.Core;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
	public class RunicDirector : MonoBehaviour
    {
		[SerializeField] GameObject gazeHud;
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle, rightDorsal, leftDorsal;

        List<GameObject> rightHand = new List<GameObject>();
        List<GameObject> leftHand = new List<GameObject>();

		//public enum Mode { Touch, Node };
		//public Mode currentMode = Mode.Touch;

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

			if (Gaze) gazeHud.SetActive(true);
			else gazeHud.SetActive(false);

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
			if (Node && Gaze)
			{
				Node = false;
				Gaze = false;
				GetComponent<AudioSource>().PlayOneShot(nodeTap);
			}
			else if (Node && !Gaze)
			{
				Node = false;
				GetComponent<AudioSource>().PlayOneShot(nodeTap);
			}
			else
			{
				Node = true;
				GetComponent<AudioSource>().PlayOneShot(nodeTap);
			}
		}

		public void ToggleGaze()
		{

			if (Gaze && Node)
			{
				Gaze = false;
				GetComponent<AudioSource>().PlayOneShot(gazeTap);
			}
			else if (!Gaze && Node)
			{
				Gaze = true;
				GetComponent<AudioSource>().PlayOneShot(gazeTap);
			}
			else if (!Gaze && !Node)
			{
				Gaze = true;
				Node = true;
				GetComponent<AudioSource>().PlayOneShot(nodeTap);
			}
		}

		private void SetGazeProvider()
		{
			//GazeProvider gazeProvider = Camera.main.GetComponent<GazeProvider>();

			//if (Gaze) gazeProvider.enabled = true;
			//else gazeProvider.enabled = false;
		}
    }
}
