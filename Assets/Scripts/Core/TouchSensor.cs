using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class TouchSensor : MonoBehaviour
    {
        [SerializeField] float touchDistanceScale = 0.03f; // TODO refactor to be relative to object size

        float distanceRight, distanceLeft;

        public bool IsTouched { get; set; }
        public GameObject RightPointer { get; set; }
        public GameObject RightMiddle { get; set; }
        public GameObject LeftPointer { get; set; }
        public GameObject LeftMiddle { get; set; }


        private void Start()
        {
            IsTouched = false;
            RightPointer = GameObject.FindGameObjectWithTag("Right Pointer");
            RightMiddle = GameObject.FindGameObjectWithTag("Right Middle");
            LeftPointer = GameObject.FindGameObjectWithTag("Left Pointer");
            LeftMiddle = GameObject.FindGameObjectWithTag("Left Middle");
            distanceRight = Mathf.Infinity;
            distanceLeft = Mathf.Infinity;
        }

        void Update()
        {
            float touchThreshold = touchDistanceScale * transform.localScale.x;

            if (!RightPointer && !LeftPointer) return;

            if (RightPointer)
            {
                distanceRight = Vector3.Distance(transform.position, RightPointer.transform.position);
                //Physics.IgnoreCollision(RightPointer.GetComponent<Collider>(), GetComponent<Collider>());
                //Physics.IgnoreCollision(RightMiddle.GetComponent<Collider>(), GetComponent<Collider>());
            }

            if (LeftPointer)
            {
                distanceLeft = Vector3.Distance(transform.position, LeftPointer.transform.position);
                //Physics.IgnoreCollision(LeftPointer.GetComponent<Collider>(), GetComponent<Collider>());
                //Physics.IgnoreCollision(LeftMiddle.GetComponent<Collider>(), GetComponent<Collider>());
            }

            IsTouched = distanceRight < touchThreshold || distanceLeft < touchThreshold;

            //if (distanceRight < touchThreshold || distanceLeft < touchThreshold)
            //{
            //    if (!Touched)
            //    {
            //        Touched = true;
            //    }
            //}
            //else
            //{
            //    Touched = false;
            //}
        }
    }
}

