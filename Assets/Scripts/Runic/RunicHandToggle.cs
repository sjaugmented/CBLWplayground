using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(RadialView))]
    public class RunicHandToggle : MonoBehaviour
    {
        [SerializeField] AudioClip singleTap;
        [SerializeField] AudioClip doubleTap;
        
        [SerializeField] bool leftHand = false;
        bool triggered = false;

        RunicDirector director;
        OSC osc;
        HandTracking hands;

        void Start()
        {
			director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            hands = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
		}

        private void OnTriggerEnter(Collider collider)
        {
            if (leftHand)
			{
                if (collider.CompareTag("Right Pointer") || collider.CompareTag("Right Middle"))
                {
                    if (hands.rightOpen)
                    {
                        ToggleNode();
                    }
                }

                if (collider.CompareTag("Right Pointer"))
                {
					if (!hands.rightPeace)
					{
						SendOSC("leftTap1/");
						GetComponent<AudioSource>().PlayOneShot(singleTap);
					}
					else
					{
						SendOSC("leftTap/peace/");
						GetComponent<AudioSource>().PlayOneShot(doubleTap);
					}
				}

                if (collider.CompareTag("Right Middle"))
				{
                    SendOSC("leftTap2/");
                    GetComponent<AudioSource>().PlayOneShot(singleTap);
                }
            }

            else
			{
                if (collider.CompareTag("Left Pointer") || collider.CompareTag("Left Middle"))
				{
                    if (hands.leftOpen)
					{
                        ToggleGaze();
                    }
				}

                if (collider.CompareTag("Left Pointer"))
                {
                    if (!hands.leftPeace)
                    {
                        SendOSC("rightTap1/");
                        GetComponent<AudioSource>().PlayOneShot(singleTap);
                    }
                    else
                    {
                        SendOSC("rightTap/peace/");
                        GetComponent<AudioSource>().PlayOneShot(doubleTap);
                    }
                }

                if (collider.CompareTag("Left Middle"))
				{
                    SendOSC("rightTap2/");
                    GetComponent<AudioSource>().PlayOneShot(singleTap);
                }
            }            
            
        }

        private void SendOSC(string messageToSend)
		{
            OscMessage message = new OscMessage();
            message.address = messageToSend;
            message.values.Add(1);
            osc.Send(message);
            Debug.Log(this.gameObject.name + " sending OSC:" + message); // todo remove		
        }

        private void ToggleNode()
		{
			director.ToggleNode();
			triggered = true;
			StartCoroutine("ToggleDelay");
		}

        private void ToggleGaze()
		{
            director.ToggleGaze();
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