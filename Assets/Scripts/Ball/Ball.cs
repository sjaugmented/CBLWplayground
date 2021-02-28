using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        // TODO
        // tennis ball
        // each hit changes color and sends OSC
        // can catch in forcefield between hands and manipulate floats
        // receive OSC to explode orb on downbeat


        [SerializeField] string oscAddress;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] float magnetRange = 0.1f;
        // [SerializeField] float stopRange = 0.06f;
        [SerializeField] float magneticForce = 2;

        public float distanceToRtHand, distanceToLtHand;

        float hueVal = Mathf.Epsilon;
        int oscVal = 0;

        // HandTracking hands;
        NewTracking tracking;
        OSC osc;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        }

        void Update()
        {
            distanceToRtHand = Vector3.Distance(transform.position, tracking.GetRtPalm.Position);
            distanceToLtHand = Vector3.Distance(transform.position, tracking.GetLtPalm.Position);

            if (distanceToRtHand < magnetRange)
            {
                Magnetism(tracking.GetRtPalm.Position);
            }
            if (distanceToLtHand < magnetRange) {
                Magnetism(tracking.GetLtPalm.Position);
            }
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Player")) {
                hueVal += 0.1388f; // 1/5 of 360
                if (hueVal > 1) {
                    hueVal -= 1;
                }
                // TODO particles not changing color
                var particleColor = GetComponentInChildren<ParticleSystem>().colorOverLifetime;
                Debug.Log("old color: " + particleColor);
                particleColor.color = Color.HSVToRGB(hueVal, 1, 1);
                Debug.Log("new color: " + particleColor);
                
                oscVal += 1;
                SendOSC(oscAddress, oscVal);
            }
        }

        private void SendOSC(string address, float val) {
            OscMessage message = new OscMessage();
            message.address = address;
            message.values.Add(val);
            osc.Send(message);
        }

        private void Magnetism(Vector3 attraction)
        {
            transform.LookAt(attraction);
            GetComponent<Rigidbody>().AddForce(transform.forward * magneticForce);
        }

        public bool Handled {get; set;}

        public void DestroySelf() {
            GetComponentInChildren<MeshExploder>().Explode();
            if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().PlayOneShot(destroyFX);
            }
            GetComponentInChildren<MeshRenderer>().enabled = false;
            var particles = GetComponentInChildren<ParticleSystem>().emission;
            particles.enabled = false;
        }
    }
}


