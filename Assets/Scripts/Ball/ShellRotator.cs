using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ShellRotator : MonoBehaviour
    {
        [SerializeField] float xRotation = 2;
        [SerializeField] float yRotation = 2;
        [SerializeField] float zRotation = 2;
        [SerializeField] float activeMultiplier = 5;

        Ball ball;

        private void Start()
        {
            ball = GetComponentInParent<Ball>();
        }
        void Update()
        {
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

