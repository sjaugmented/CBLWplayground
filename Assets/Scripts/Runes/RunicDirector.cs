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
        }

        public void ToggleMode()
		{
            Debug.Log("Director: toggled");
		}
    }
}
