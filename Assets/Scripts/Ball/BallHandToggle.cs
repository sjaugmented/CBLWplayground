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
        [SerializeField] AudioClip activeFX;
        [SerializeField] AudioClip stillFX;

        [SerializeField] bool leftHand = false;
        bool triggered, inactive;

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
            if (inactive) { return; }

            if (leftHand)
            {
                if (tracking.leftPose != HandPose.flat)
                {
                    if (collider.CompareTag("Right Index"))
                    {
                        director.SetGlobalStill(true);
                        //SendOSC("still/");
                        GetComponent<AudioSource>().PlayOneShot(stillFX);
                    }
                }
                else
                {
                    if (collider.CompareTag("Right Index"))
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
                    if (collider.CompareTag("Left Index"))
                    {
                        director.SetGlobalStill(false);
                        //SendOSC("active/");
                        GetComponent<AudioSource>().PlayOneShot(activeFX);
                    }
                }
                else
                {
                    if (collider.CompareTag("Left Index"))
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
        }


        IEnumerator ToggleDelay()
        {
            yield return new WaitForSeconds(1);
            triggered = false;
        }
    }
}
