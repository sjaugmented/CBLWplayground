using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Ball
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField] string oscAddress = "/multi/";
        [SerializeField] AudioClip touchFX;
        [SerializeField] float panelTimeOut = 10;

        float touchTimer = Mathf.Infinity;
        Material originalMaterial;
        public bool Touched { get; set; }
        
        NewTracking tracking;
        OSC osc;
        Renderer myRenderer;
        TouchSensor touchSensor;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            touchSensor = GetComponent<TouchSensor>();
            myRenderer = GetComponent<Renderer>();
            originalMaterial = myRenderer.material;
        }

        void Update()
        {
            touchTimer += Time.deltaTime;

            if (touchSensor.IsTouched)
            {
                //Debug.Log("touched " + name);
                HasBeenTouched();
            }

            if (Touched) 
            {
                myRenderer.material.color = Color.HSVToRGB(0, 0, 1f);
                //myRenderer.enabled = false;
            }

            //if (touchTimer < panelTimeOut)
            //{
            //    myRenderer.material.color = Color.HSVToRGB(0, 0, 1f);
            //}
            //else
            //{
            //    myRenderer.material = originalMaterial;
            //}
        }

        public void HasBeenTouched()
        {
            if (tracking.rightPose == HandPose.pointer)
            {
                TouchResponse("rightPointer");
            }
            if (tracking.leftPose == HandPose.pointer)
            {
                TouchResponse("leftPointer");
            }
            if (tracking.leftPose == HandPose.peace)
            {
                TouchResponse("leftPeace");
            }
            if (tracking.leftPose == HandPose.peace)
            {
                TouchResponse("leftPeace");
            }

        }

        private void TouchResponse(string finger)
        {
            Touched = true;
            touchTimer = 0;
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().PlayOneShot(touchFX);
            }
            SendOSCMessage(finger);

        }

        public void SendOSCMessage(string address, float value = 1)
        {
            OscMessage message = new OscMessage();
            message.address = address + oscAddress;
            message.values.Add(value);
            osc.Send(message);
        }
    }
}

