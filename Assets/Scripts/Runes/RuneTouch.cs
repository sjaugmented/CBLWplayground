using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RuneTouch : MonoBehaviour
    {
        [SerializeField] float touchDistance = 0.03f;

        public float distance = Mathf.Infinity;
        
        Transform rightPointer;
        bool touched = false;

        void Start()
		{
            //rightPointer = GameObject.FindGameObjectWithTag("Right Pointer").transform;
		}

        void Update()
		{
            if (!GameObject.FindGameObjectWithTag("Right Pointer")) return;

            distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Right Pointer").transform.position);

            if (distance < touchDistance)
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
