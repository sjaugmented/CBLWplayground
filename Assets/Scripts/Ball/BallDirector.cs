using LW.Core;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
	public class BallDirector : MonoBehaviourPunCallbacks, IPunObservable
	{
		[SerializeField] bool sharedExperience = false;
		[SerializeField] bool multiBall = false;
		[SerializeField] bool killJedi = false;
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

		public bool SharedExperience
        {
			get { return sharedExperience; }
        }

		public bool SendCoordinates
        {
			get { return sendCoordinates; }
        }
		public bool KillJedi
        {
			get { return killJedi; }
			set { killJedi = value; }
        }

		public bool Viewfinder { get; set; }

		public int WorldLevel
		{	
			get { return worldLevel; }
			set { worldLevel = value; }
		}

		public bool Still { get; set; }

		public bool rightInPlay, leftInPlay;

		public bool RightBallInPlay { get { return rightInPlay; } set { rightInPlay = value; } }
		public bool LeftBallInPlay { get { return leftInPlay; } set { leftInPlay = value; } }

		bool spawnReady, hasReset;
		float spawnWindow = Mathf.Infinity;

		public Vector3 SpawnOffset { get; set; }

        NewTracking tracking;
        ThumbTrigger rThumbTrigger, lThumbTrigger;
		GameObject rightBall;
		GameObject leftBall;

		public List<Ball> currentBalls = new List<Ball>();

		//PhotonRoom room;

		#region Photon

		public static GameObject LocalPlayerInstance;

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
            {
				// This is us - send data
				//stream.SendNext(transform.localPosition);
				//stream.SendNext(transform.localRotation);
			}
			else
            {
				// This is networked user - receive data
            }
		}

		#endregion

		private void Awake()
        {
			//if (photonView.IsMine)
   //         {
			//	BallDirector.LocalPlayerInstance = this.gameObject;
   //         }

			//DontDestroyOnLoad(this.gameObject);

			if (!photonView.IsMine)
            {
				return;
            }

            tracking = GetComponent<NewTracking>();

   //         if (sharedExperience)
			//{
			//	if (PhotonNetwork.PrefabPool is DefaultPool pool)
			//	{
			//		if (prefab1 != null) pool.ResourceCache.Add(prefab1.name, prefab1);
			//		if (prefab3 != null) pool.ResourceCache.Add(prefab3.name, prefab3);
			//		if (prefab4 != null) pool.ResourceCache.Add(prefab4.name, prefab4);
			//	}
			//}
		}

		void Start()
		{
			if (!photonView.IsMine)
			{
				return;
			}
			rThumbTrigger = GameObject.FindGameObjectWithTag("Right Thumb").GetComponent<ThumbTrigger>();
			lThumbTrigger = GameObject.FindGameObjectWithTag("Left Thumb").GetComponent<ThumbTrigger>();

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
			if (photonView.IsMine)
            {
				SetRightHand(tracking.FoundRightHand);
				SetLeftHand(tracking.FoundLeftHand);

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
			
				if (rightBall)
				{
					//if (RightBallInPlay && rightBall.GetComponent<Ball>().State != BallState.Dead)
					//{
					//	balls.Add(rightBall.GetComponent<Ball>());
					//}

					handColliders.SetActive(!rightBall.GetComponent<Ball>().InteractingWithParticles);
					if (rightBall.GetComponent<Ball>().State == BallState.Dead)
					{
						RightBallInPlay = false;
					}
				}
				if (leftBall)
				{
					//if (LeftBallInPlay && leftBall.GetComponent<Ball>().State != BallState.Dead)
					//{
					//	balls.Add(leftBall.GetComponent<Ball>());
					//}

					handColliders.SetActive(!leftBall.GetComponent<Ball>().InteractingWithParticles);
					if (leftBall.GetComponent<Ball>().State == BallState.Dead)
					{
						LeftBallInPlay = false;
					}
				}

				if (currentBalls.Count > 1)
				{
					if (currentBalls[0].State == currentBalls[1].State)
					{
						currentBalls[0].GetComponent<BallJedi>().NoJedi = true;
						currentBalls[1].GetComponent<BallJedi>().NoJedi = true;
					}
					else
					{
						currentBalls[0].GetComponent<BallJedi>().NoJedi = currentBalls[0].State == BallState.Still;
						currentBalls[1].GetComponent<BallJedi>().NoJedi = currentBalls[1].State == BallState.Still;
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
					if (!LeftBallInPlay && multiBall && worldLevel > 3)
					{
						//SpawnBall("left");
					}
				}
            }

        }

		private void SpawnBall(string side)
		{
			GameObject spawnPrefab = prefab1;

			//switch (worldLevel)
   //         {
   //             case 3:
   //                 spawnPrefab = prefab3;
   //                 break;
   //             case 4:
   //                 spawnPrefab = prefab4;
   //                 break;
   //             default:
			//		spawnPrefab = prefab1;
			//		break;
   //         }
			
			if (side == "right")
            {
				Debug.Log("spawning");

				RightBallInPlay = true;
				rightBall = PhotonNetwork.Instantiate(spawnPrefab.name, tracking.GetRtPalm.Position + SpawnOffset, Camera.main.transform.rotation);

				Debug.Log("spawned");

				rightBall.GetComponent<Ball>().Handedness = Hands.right;

				Debug.Log("set handedness");
				
				currentBalls.Add(rightBall.GetComponent<Ball>());

				Debug.Log("added to Balls");
            }
			//else
   //         {
			//	LeftBallInPlay = true;
			//	leftBall = Instantiate(spawnPrefab, tracking.GetRtPalm.Position + SpawnOffset, Camera.main.transform.rotation);
			//	leftBall.GetComponent<Ball>().Handedness = Hands.left;

			//	currentBalls.Add(leftBall.GetComponent<Ball>());
			//}
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
			if (!photonView.IsMine)
			{
				return;
			}
			Debug.Log("NEXT WORLD");
			worldLevel = worldLevel < 4 ? worldLevel + 1 : 1;
		}

		public void DisableHandColliders()
		{
			if (!photonView.IsMine)
			{
				return;
			}
			handColliders.SetActive(false);
		}

		public void EnableHandColliders()
		{
			if (!photonView.IsMine)
			{
				return;
			}
			handColliders.SetActive(true);
		}

		public void RemoveBall(Hands handedness)
        {
			if (!photonView.IsMine)
			{
				return;
			}
			int index = 0;

			for(int i = 0; i < currentBalls.Count; i++)
            {
				if (currentBalls[i].Handedness == handedness)
                {
					index = i;
                }
            }

			Debug.Log("Removing ball at index " + index);
			currentBalls.RemoveAt(index);
        }

		public void SetGlobalStill(bool val)
        {
			if (!photonView.IsMine)
			{
				return;
			}
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