using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HoverDrums
{
    public class HoverDrumController : MonoBehaviour
    {
        public float force = 1;
        public string drumShape;
        public float colorFloat;

        OSC osc;
        Rigidbody rigidBody;
        

        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            drumShape = transform.GetChild(0).name;
        }

        void Update()
        {
            
        }

        public void SetDrumColor(float color)
        {
            colorFloat = color;
            GetComponentInChildren<Renderer>().material.color = Color.HSVToRGB(colorFloat, 1, 1);
        }

        void FixedUpdate()
        {
            rigidBody.AddForce(transform.forward * force);
        }

        public void Touched()
        {
            SendOSCMessage(drumShape, colorFloat);
        }

        private void SendOSCMessage(string address, float value)
        {
            OscMessage message = new OscMessage();
            message.address = address;
            message.values.Add(value);
            osc.Send(message);
            Debug.Log(this.gameObject.name + " sending OSC:" + message); // todo remove
        }
    }

}
