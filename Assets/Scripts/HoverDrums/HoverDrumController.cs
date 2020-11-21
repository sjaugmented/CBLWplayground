using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HoverDrums
{
    public class HoverDrumController : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] float hFloat = 0f;
        [SerializeField] string oscMessage = "shape";

        OSC osc;

        void Start()
        {
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            oscMessage = transform.GetChild(0).name;
        }

        void Update()
        {
            // for testing color prop - TODO remove
            GetComponentInChildren<Renderer>().material.color = Color.HSVToRGB(hFloat, 1, 1);
        }

        public void Touched()
        {
            SendOSCMessage(oscMessage, hFloat);
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
