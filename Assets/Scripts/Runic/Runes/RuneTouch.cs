using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RuneTouch : MonoBehaviour
    {
        [SerializeField] float touchDistanceScale = 0.03f; // TODO refactor to be relative to object size

        float distanceRight = Mathf.Infinity;
        float distanceLeft = Mathf.Infinity;
        
        bool touched = false;

        void Update()
		{
            float touchThreshold = touchDistanceScale * transform.localScale.x;
            
            if (!GameObject.FindGameObjectWithTag("Right Pointer") && !GameObject.FindGameObjectWithTag("Left Pointer")) return;

            if (GameObject.FindGameObjectWithTag("Right Pointer"))
			{
                distanceRight = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Right Pointer").transform.position);
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Right Pointer").GetComponent<Collider>(), GetComponent<Collider>());
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Right Middle").GetComponent<Collider>(), GetComponent<Collider>());
            }

			if (GameObject.FindGameObjectWithTag("Left Pointer"))
			{
                distanceLeft = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Left Pointer").transform.position);
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Left Pointer").GetComponent<Collider>(), GetComponent<Collider>());
                Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Left Middle").GetComponent<Collider>(), GetComponent<Collider>());
            }

            if (distanceRight < touchThreshold || distanceLeft < touchThreshold)
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
