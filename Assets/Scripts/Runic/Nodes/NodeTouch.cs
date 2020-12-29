using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class NodeTouch : MonoBehaviour
    {
        [SerializeField] float touchDistanceScale = 1f; // TODO refactor to be relative to object size
        public float touchThreshold;

        public float distanceRight = Mathf.Infinity;
        public float distanceLeft = Mathf.Infinity;

        bool touched = false;

        void Update()
        {
            touchThreshold = touchDistanceScale * transform.localScale.x;

            if (!GameObject.FindGameObjectWithTag("Right Pointer") && !GameObject.FindGameObjectWithTag("Left Pointer")) return;

            if (GameObject.FindGameObjectWithTag("Right Pointer"))
            {
                distanceRight = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Right Pointer").transform.position);
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Right Pointer").GetComponent<Collider>(), GetComponent<Collider>());
            }

            if (GameObject.FindGameObjectWithTag("Left Pointer"))
            {
                distanceLeft = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Left Pointer").transform.position);
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Left Pointer").GetComponent<Collider>(), GetComponent<Collider>());
            }

            if (distanceRight < touchThreshold || distanceLeft < touchThreshold)
            {
                if (!touched)
                {
                    GetComponentInParent<NodeController>().Touched();
                    touched = true;
                }
            }
            else
            {
                GetComponentInParent<NodeController>().NotTouched();
                touched = false;
            }
        }
    }
}
