using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallGazeCollider : MonoBehaviour
    {
        SphereCollider collider;
        Ball ball;
        // Start is called before the first frame update
        void Start()
        {
            collider = GetComponent<SphereCollider>();
            ball = GetComponentInParent<Ball>();
        }

        // Update is called once per frame
        void Update()
        {
            collider.radius = ball.Distance <= 1f ? 0.05f : 0.05f * ball.Distance * 2;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.layer);
            Physics.IgnoreCollision(collider, collision.gameObject.GetComponent<Collider>());
        }
    }

}
