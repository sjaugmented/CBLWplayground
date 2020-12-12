using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(RadialView))]
    public class RunicHandToggle : MonoBehaviour
    {
        [SerializeField] bool leftHand = false;
        bool triggered = false;

        RunicDirector director;

        void Start()
        {
			director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
		}

        private void OnTriggerEnter(Collider collider)
        {
            
            if (leftHand)
			{
                if (collider.CompareTag("Right Pointer"))
                {
                    if (!triggered)
					{
						ToggleMode();
					}
				}
            }

            else
			{
                if (collider.CompareTag("Left Pointer"))
                {
                    if (!triggered)
                    {
                        ToggleMode();
                    }

                }
            }            
            
        }

		private void ToggleMode()
		{
			director.ToggleMode();
			triggered = true;
			StartCoroutine("ToggleDelay");
		}

		IEnumerator ToggleDelay()
        {
            yield return new WaitForSeconds(1);
            triggered = !triggered;
        }
    }

}