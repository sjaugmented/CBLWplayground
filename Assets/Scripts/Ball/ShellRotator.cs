using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ShellRotator : MonoBehaviourPunCallbacks
    {
        [SerializeField] float xRotation = 2;
        [SerializeField] float yRotation = 2;
        [SerializeField] float zRotation = 2;
        [SerializeField] float activeMultiplier = 5;

        Ball ball;

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            ball = GetComponentInParent<Ball>();
        }
        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (ball.State == BallState.Still)
            {
                transform.Rotate(xRotation, yRotation, zRotation);
            }
            else
            {
                transform.Rotate(xRotation * activeMultiplier, yRotation * activeMultiplier, zRotation * activeMultiplier);
            }
        }
    }
}

