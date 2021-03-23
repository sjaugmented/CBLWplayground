using LW.Core;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using UnityEngine;

namespace LW.Ball
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(RadialView))]
    public class BallHandToggle : MonoBehaviour
    {
        [SerializeField] AudioClip singleTap;
        [SerializeField] AudioClip doubleTap;

        [SerializeField] bool leftHand = false;
        bool triggered = false;

        NewTracking tracking;
        BallDirector director;
        OSC osc;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (leftHand)
            {
                if (tracking.leftPose != HandPose.flat)
                {
                    if (collider.CompareTag("Right Pointer"))
                    {
                        SpawnPortal();
                    }
                }
                else
                {
                    if (collider.CompareTag("Right Pointer"))
                    {
                        if (tracking.rightPose != HandPose.peace)
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
            }

            else
            {
                if (tracking.rightPose != HandPose.flat)
                {
                    if (collider.CompareTag("Left Pointer"))
                    {
                        ToggleGaze();
                    }
                }
                else
                {
                    if (collider.CompareTag("Left Pointer"))
                    {
                        if (tracking.leftPose != HandPose.peace)
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
        }

        private void SendOSC(string messageToSend)
        {
            OscMessage message = new OscMessage();
            message.address = messageToSend;
            message.values.Add(1);
            osc.Send(message);
            Debug.Log(this.gameObject.name + " sending OSC:" + message); // todo remove		
        }

        private void SpawnPortal()
        {
            if (!triggered)
            {
                director.TogglePortal();
                triggered = true;
                StartCoroutine("ToggleDelay");

            }
        }

        private void ToggleGaze()
        {
            if (!triggered)
            {
                director.ToggleGaze();
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
