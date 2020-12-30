using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RunicDirector : MonoBehaviour
    {
        [SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle, rightDorsal, leftDorsal;

        List<GameObject> rightHand = new List<GameObject>();
        List<GameObject> leftHand = new List<GameObject>();

		public enum Mode { Touch, Node };
		public Mode currentMode = Mode.Touch;

		public bool node = false;
		public bool gaze = false;

		HandTracking handtracking;
        
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
			if (handtracking.rightHand) SetRightHand(true);
			else SetRightHand(false);

			if (handtracking.leftHand) SetLeftHand(true);
			else SetLeftHand(false);

			///// DEV CONTROLS
			if (Input.GetKeyDown(KeyCode.N))
			{
				ToggleNode();
			}

			if (Input.GetKeyDown(KeyCode.G))
			{
				ToggleGaze();
			}
        }

        public void ToggleNode()
		{
			if (currentMode == Mode.Touch) currentMode = Mode.Node;
			else currentMode = Mode.Touch;

			node = !node;
		}

		public void ToggleGaze()
		{
			gaze = !gaze;
		}
    }
}
