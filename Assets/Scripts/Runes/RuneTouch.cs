using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RuneTouch : MonoBehaviour
    {
        [SerializeField] float touchDistance = 0.03f;

        public float distanceRight = Mathf.Infinity;
        public float distanceLeft = Mathf.Infinity;
        
        bool touched = false;

        void Start()
		{
            
		}

        void Update()
		{
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

            if (distanceRight < touchDistance || distanceLeft < touchDistance)
			{
                if (!touched)
				{
                    GetComponentInParent<RuneController>().Touched();
                    touched = true;
				}
			}
            else
			{
                GetComponentInParent<RuneController>().NotTouched();
                touched = false;
			}
		}
    }
}
