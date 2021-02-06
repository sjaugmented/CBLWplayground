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

        public delegate void ModeDelegate();
        public ModeDelegate handToggle;
        public ModeDelegate buildToggle;

        SlingShotDirector director;
        HandTracking hands;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
            handToggle = director.ToggleHandMode;
            buildToggle = director.ToggleBuildMode;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (GameObject.FindGameObjectWithTag("HSLOrb")) { return; }
            if (hands.rightPeace || hands.leftPeace) { return; }
            
            if (leftHand)
            {
                if (collider.CompareTag("Right Pointer"))
                {
                    ToggleMode(handToggle);
                }
            }

            else
            {
                if (collider.CompareTag("Left Pointer"))
                {
                    ToggleMode(buildToggle);
                }
            }
        }

        void ToggleMode(ModeDelegate method)
		{
            if (!triggered)
            {
                method();
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

