using LW.Core;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
	public class BallDirector : MonoBehaviour
	{
		[SerializeField] GameObject portalPrefab;
		[SerializeField] float portalSpawnDistance = 1;
		[SerializeField] GameObject gazeHud;
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle, rightDorsal, leftDorsal;

		List<GameObject> rightHand = new List<GameObject>();
		List<GameObject> leftHand = new List<GameObject>();

		public bool Portal { get; set; }
		public bool Gaze { get; set; }

		GameObject portal;

		NewTracking tracking;

		void Start()
		{
			tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();

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
			if (tracking.FoundRightHand) SetRightHand(true);
			else SetRightHand(false);

			if (tracking.FoundLeftHand) SetLeftHand(true);
			else SetLeftHand(false);

			if (Gaze) gazeHud.SetActive(true);
			else gazeHud.SetActive(false);

			///// DEV CONTROLS
			if (Input.GetKeyDown(KeyCode.N))
			{
				TogglePortal();
			}

			if (Input.GetKeyDown(KeyCode.M))
			{
				ToggleGaze();
			}

		}

		public void TogglePortal()
		{
			if (!Portal)
            {
				Transform player = Camera.main.transform;
				portal = Instantiate(portalPrefab, player.position + player.forward * portalSpawnDistance, player.rotation);
				portal.transform.LookAt(player.position);
				//portal.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				Portal = true;
            }
			else
            {
				Destroy(portal);
				Portal = false;
            }
		}

		public void ToggleGaze()
		{
			//if (Gaze && Node)
			//{
			//	Gaze = false;
			//	GetComponent<AudioSource>().PlayOneShot(gazeTap);
			//}
			//else if (!Gaze && Node)
			//{
			//	Gaze = true;
			//	GetComponent<AudioSource>().PlayOneShot(gazeTap);
			//}
			//else if (!Gaze && !Node)
			//{
			//	Gaze = true;
			//	Node = true;
			//	GetComponent<AudioSource>().PlayOneShot(nodeTap);
			//}
		}
	}
}