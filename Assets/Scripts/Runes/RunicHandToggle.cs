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
                if (collider.CompareTag("Right Pointer"))
                {
                    if (!triggered)
					{
                        if (!hands.rightPeace)
						{
                            SendOSC("leftTap/");
						}
                        else
						{
                            //ToggleMode();
                            SendOSC("leftTap/peace/");
						}
					}
				}
            }

            else
			{
                if (collider.CompareTag("Left Pointer"))
                {
                    if (!triggered)
                    {
                        if (!hands.leftPeace)
                        {
                            SendOSC("rightTap/");
                        }
                        else
                        {
                            //ToggleMode();
                            SendOSC("rightTap/peace/");
                        }
                    }

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