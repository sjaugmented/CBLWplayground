using LW.Core;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
	public class BallDirector : MonoBehaviour
	{
		[SerializeField] GameObject prefab1;
		[SerializeField] GameObject prefab3;
		[SerializeField] GameObject prefab4;
		[SerializeField] AudioClip nodeTap;
		[SerializeField] AudioClip gazeTap;

		[SerializeField] GameObject rightHandTouch, leftHandTouch, rightToggle, leftToggle;
		[SerializeField] GameObject handColliders;

		List<GameObject> rightHand = new List<GameObject>();
		List<GameObject> leftHand = new List<GameObject>();
		[SerializeField] int worldLevel = 1;
		[SerializeField] bool sendCoordinates = false;

		public bool SendCoordinates
        {
			get { return sendCoordinates; }
        }

		public bool Viewfinder { get; set; }

		public int WorldLevel
		{	
			get { return worldLevel; }
			set { worldLevel = value; }
		}

		public bool Still { get; set; }

		public bool RightBallInPlay { get; set; }
		public bool LeftBallInPlay { get; set; }

		bool spawnReady, hasReset;
		float spawnWindow = Mathf.Infinity;

		public Vector3 SpawnOffset { get; set; }

		NewTracking tracking;
		ThumbTrigger rThumbTrigger, lThumbTrigger;
		GameObject rightBall;
		GameObject leftBall;

		void Start()
		{
			tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
			rThumbTrigger = GameObject.FindGameObjectWithTag("Right Thumb").GetComponent<ThumbTrigger>();
			lThumbTrigger = GameObject.FindGameObjectWithTag("Left Thumb").GetComponent<ThumbTrigger>();

			if (PhotonNetwork.PrefabPool is DefaultPool pool)
            {
				if (prefab1 != null) pool.ResourceCache.Add(prefab1.name, prefab1);
				if (prefab3 != null) pool.ResourceCache.Add(prefab3.name, prefab3);
				if (prefab4 != null) pool.ResourceCache.Add(prefab4.name, prefab4);
            }

			rightHand.Add(rightHandTouch);
			rightHand.Add(rightToggle);
			leftHand.Add(leftHandTouch);
			leftHand.Add(leftToggle);

			SetRightHand(false);
			SetLeftHand(false);

			worldLevel = worldLevel <= 1 ? 1 : worldLevel;
		}

		

		void Update()
		{
			List<Ball> balls = new List<Ball>();
			
			//if (RightBallInPlay && rightBall.GetComponent<Ball>().State != BallState.Dead)
   //         {
			//	balls.Add(rightBall.GetComponent<Ball>());
   //         }
			//if (LeftBallInPlay && leftBall.GetComponent<Ball>().State != BallState.Dead)
   //         {
			//	balls.Add(leftBall.GetComponent<Ball>());
   //         }

			//if (RightBallInPlay && LeftBallInPlay)
   //         {
			//	if (balls[0].State == balls[1].State)
   //             {
			//		balls[0].GetComponent<BallJedi>().NoJedi = true;
			//		balls[1].GetComponent<BallJedi>().NoJedi = true;
			//	}
			//	else
   //             {
			//		balls[0].GetComponent<BallJedi>().NoJedi = balls[0].State == BallState.Still;
			//		balls[1].GetComponent<BallJedi>().NoJedi = balls[1].State == BallState.Still;
			//	}
   //         }
			
			Viewfinder = rThumbTrigger.Triggered && lThumbTrigger.Triggered;
			
			SetRightHand(tracking.FoundRightHand);
			SetLeftHand(tracking.FoundLeftHand);

			if (rightBall)
			{
				if (RightBallInPlay && rightBall.GetComponent<Ball>().State != BallState.Dead)
				{
					balls.Add(rightBall.GetComponent<Ball>());
				}

				handColliders.SetActive(!rightBall.GetComponent<Ball>().InteractingWithParticles);
				if (rightBall.GetComponent<Ball>().State == BallState.Dead)
                {
					RightBallInPlay = false;
                }
			}
			if (leftBall)
			{
				if (LeftBallInPlay && leftBall.GetComponent<Ball>().State != BallState.Dead)
				{
					balls.Add(leftBall.GetComponent<Ball>());
				}

				handColliders.SetActive(!leftBall.GetComponent<Ball>().InteractingWithParticles);
				if (leftBall.GetComponent<Ball>().State == BallState.Dead)
				{
					LeftBallInPlay = false;
				}
			}

			if (RightBallInPlay && LeftBallInPlay)
			{
				if (balls[0].State == balls[1].State)
				{
					balls[0].GetComponent<BallJedi>().NoJedi = true;
					balls[1].GetComponent<BallJedi>().NoJedi = true;
				}
				else
				{
					balls[0].GetComponent<BallJedi>().NoJedi = balls[0].State == BallState.Still;
					balls[1].GetComponent<BallJedi>().NoJedi = balls[1].State == BallState.Still;
				}
			}

			// Spawning
			SpawnOffset = new Vector3(0, 0.1f, 0) + Camera.main.transform.InverseTransformDirection(0, 0, 0.03f);
			spawnWindow += Time.deltaTime;

			if (tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.fist)
			{
				if (!spawnReady)
				{
					spawnWindow = 0;
					spawnReady = true;
				}
			}
			else
			{
				spawnReady = false;
			}

			if (spawnWindow < 1 && tracking.rightPalmAbs == Direction.up && tracking.rightPose == HandPose.flat)
			{
				if (!RightBallInPlay)
				{
					SpawnBall("right");
				}
			}

			if (tracking.leftPalmAbs == Direction.up && tracking.leftPose == HandPose.fist)
			{
				if (!spawnReady)
				{
					spawnWindow = 0;
					spawnReady = true;
				}
			}
			else
			{
				spawnReady = false;
			}

			if (spawnWindow < 1 && tracking.leftPalmAbs == Direction.up && tracking.leftPose == HandPose.flat)
			{
				if (!LeftBallInPlay)
				{
					SpawnBall("left");
				}
			}

        }

		private void SpawnBall(string side)
		{
			GameObject spawnPrefab;

			switch (worldLevel)
            {
				case 3:
					spawnPrefab = prefab3;
					break;
				case 4:
					spawnPrefab = prefab4;
					break;
				default:
					spawnPrefab = prefab1;
					break;
            }
			
			if (side == "right")
            {
				RightBallInPlay = true;
				rightBall = PhotonNetwork.Instantiate(spawnPrefab.name, tracking.GetRtPalm.Position + SpawnOffset, Camera.main.transform.rotation);
                rightBall.GetComponent<Ball>().Handedness = Hands.right;
            }
			else
            {
				LeftBallInPlay = true;
				leftBall = PhotonNetwork.Instantiate(spawnPrefab.name, tracking.GetLtPalm.Position + SpawnOffset, Camera.main.transform.rotation);
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

		public void SetGlobalStill(bool val)
        {
			List<Ball> balls = new List<Ball>();

			if (RightBallInPlay && LeftBallInPlay)
            {
				balls.Add(rightBall.GetComponent<Ball>());
				balls.Add(leftBall.GetComponent<Ball>());

				foreach(Ball ball in balls)
                {
					ball.Still = val;
                }
            }
			else if (RightBallInPlay && !LeftBallInPlay)
            {
				rightBall.GetComponent<Ball>().Still = val;
			}
			else if (!RightBallInPlay && LeftBallInPlay)
            {
				leftBall.GetComponent<Ball>().Still = val;
            }
			else
            {
				return;
            }
        }
	}
}