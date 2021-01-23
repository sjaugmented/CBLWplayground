using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.SlingShot
{
    public class SlingShotHandToggle : MonoBehaviour
    {
        [SerializeField] bool leftHand = false;
        bool triggered = false;

        SlingShotDirector director;
        HandTracking hands;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
        }

        private void OnTriggerEnter(Collider collider)
        {


            if (leftHand)
            {
                if (collider.CompareTag("Right Pointer"))
                {
                    ToggleHandMode();
                }
            }

            else
            {
                if (collider.CompareTag("Left Pointer"))
                {
                    ToggleHandMode();
                }
            }
        }

        private void ToggleHandMode()
        {
            if (!triggered)
			{
                director.ToggleHandMode();
                triggered = true;
                StartCoroutine("ToggleDelay");
			}
        }

        IEnumerator ToggleDelay()
        {
            yield return new WaitForSeconds(1);
            triggered = !triggered;
        }
    }
}

