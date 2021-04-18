using LW.Core;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
	public class BallDirector : MonoBehaviour
	{
		[SerializeField] GameObject portalPrefab;
		[SerializeField] GameObject uniBall;
		[SerializeField] float portalSpawnDistance = 1;
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle;
		[SerializeField] GameObject handColliders;

		List<GameObject> rightHand = new List<GameObject>();
		List<GameObject> leftHand = new List<GameObject>();
		[SerializeField] int worldLvl = 0;

		public int WorldLevel
		{
			get { return worldLvl; }
			set { worldLvl = value; }
		}

		public bool Still { get; set; }

		public bool RightBallInPlay { get; set; }
		public bool LeftBallInPlay { get; set; }

		bool conjureReady, hasReset;
		float conjureTimer = Mathf.Infinity;

		public Vector3 SpawnOffset { get; set; }

		NewTracking tracking;
		GameObject rightBall;
		GameObject leftBall;


		void Start()
		{
			tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();

			rightHand.Add(rightPointer);
			rightHand.Add(rightToggle);
			leftHand.Add(leftPointer);
			leftHand.Add(leftToggle);

			SetRightHand(false);
			SetLeftHand(false);

			WorldLevel = 1;
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
				Portal = true;
				Transform player = Camera.main.transform;
				portal = Instantiate(portalPrefab, player.position + player.forward * portalSpawnDistance, player.rotation);
				//portal.transform.LookAt(player.position);
			}
			else
			{
				portal.GetComponent<PortalController>().SelfDestruct();
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