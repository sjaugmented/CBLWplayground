using LW.Core;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
	public class BallDirector : MonoBehaviour
	{
		[SerializeField] GameObject ballPrefab;
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightPointer, leftPointer, rightToggle, leftToggle;
		[SerializeField] GameObject handColliders;

		List<GameObject> rightHand = new List<GameObject>();
		List<GameObject> leftHand = new List<GameObject>();
		[SerializeField] int worldLevel = 1;

		public int WorldLevel
		{
			get { return worldLevel; }
			set { worldLevel = value; }
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

			worldLevel = worldLevel <= 1 ? 1 : worldLevel;
		}

		

		void Update()
		{
			SetRightHand(tracking.FoundRightHand);
			SetLeftHand(tracking.FoundLeftHand);

			if (rightBall)
			{
				handColliders.SetActive(!rightBall.GetComponent<Ball>().InteractingWithParticles);
				if (rightBall.GetComponent<Ball>().State == BallState.Dead)
                {
					RightBallInPlay = false;
                }
			}
			if (leftBall)
			{
				handColliders.SetActive(!leftBall.GetComponent<Ball>().InteractingWithParticles);
				if (leftBall.GetComponent<Ball>().State == BallState.Dead)
				{
					LeftBallInPlay = false;
				}
			}

			SpawnOffset = new Vector3(0, 0.1f, 0) + Camera.main.transform.InverseTransformDirection(0, 0, 0.03f);
			conjureTimer += Time.deltaTime;

			// Spawning
			if (tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.fist)
			{
				if (!conjureReady)
				{
					conjureTimer = 0;
					conjureReady = true;
				}
			}
			else
			{
				conjureReady = false;
			}

			if (conjureTimer < 1 && tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.flat)
			{
				if (!RightBallInPlay)
				{
					SpawnBall("right");
				}
			}

			if (tracking.leftPalmAbs == Direction.up && tracking.leftPose == HandPose.fist)
			{
				if (!conjureReady)
				{
					conjureTimer = 0;
					conjureReady = true;
				}
			}
			else
			{
				conjureReady = false;
			}

			if (conjureTimer < 1 && tracking.leftPalmAbs == Direction.up && tracking.leftPose == HandPose.flat)
			{
				if (!LeftBallInPlay)
				{
					SpawnBall("left");
				}
			}

        }

		private void SpawnBall(string side)
		{
			if (side == "right")
            {
				RightBallInPlay = true;
				rightBall = Instantiate(ballPrefab, tracking.GetRtPalm.Position + SpawnOffset, tracking.GetRtPalm.Rotation);
				rightBall.GetComponent<Ball>().Handedness = Hands.right;
			}
			else
            {
				LeftBallInPlay = true;
				leftBall = Instantiate(ballPrefab, tracking.GetLtPalm.Position + SpawnOffset, tracking.GetLtPalm.Rotation);
				leftBall.GetComponent<Ball>().Handedness = Hands.left;
			}
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

		public void NextWorldLevel()
		{
			Debug.Log("NEXT WORLD");
			worldLevel = worldLevel < 4 ? worldLevel + 1 : 1;
		}

		public void DisableHandColliders()
		{
			handColliders.SetActive(false);
		}

		public void EnableHandColliders()
		{
			handColliders.SetActive(true);
		}
	}
}