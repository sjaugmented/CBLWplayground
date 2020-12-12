using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class RunicHandToggle : MonoBehaviour
    {
        [SerializeField] bool leftHand = false;
        bool triggered = false;

        RunicDirector director;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
        }

        private void OnTriggerEnter(Collider other)
		{
            Debug.Log("collision");
            if (leftHand)
			{
                Debug.Log("left hand");
                if (other.CompareTag("Right Pointer"))
                {
                    Debug.Log("rightPointer");
                    if (!triggered)
                    {
                        Debug.Log("HandToggle: toggled");
                        director.ToggleMode();
                        triggered = true;
                        StartCoroutine("ToggleDelay");
                    }
                }
            }

            else
			{
                Debug.Log("right hand");
                if (other.CompareTag("Left Pointer"))
                {
                    Debug.Log("leftPointer");
                    if (!triggered)
                    {
                        Debug.Log("HandToggle: toggled");
                        director.ToggleMode();
                        triggered = true;
                        StartCoroutine("ToggleDelay");
                    }

                }
            }            
            
        }

        IEnumerator ToggleDelay()
        {
            yield return new WaitForSeconds(0.2f);
            triggered = !triggered;
        }
    }

}