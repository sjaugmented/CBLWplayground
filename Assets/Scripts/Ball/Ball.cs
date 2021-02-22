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

        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] float magnetRange = 0.1f;
        [SerializeField] float stopRange = 0.06f;
        [SerializeField] float magneticForce = 2;

        public float distanceToRtHand, distanceToLtHand;

        HandTracking hands;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
        }

        void Update()
        {
            distanceToRtHand = Vector3.Distance(transform.position, hands.rightPalm.Position);
            distanceToLtHand = Vector3.Distance(transform.position, hands.leftPalm.Position);

            if (distanceToRtHand < magnetRange && distanceToRtHand > stopRange)
            {
                Magnetism(hands.rightPalm.Position);
            }
            if (distanceToLtHand < magnetRange && distanceToLtHand > stopRange) {
                Magnetism(hands.leftPalm.Position);
            }
        }

        private void Magnetism(Vector3 attraction)
        {
            transform.LookAt(attraction);
            GetComponent<Rigidbody>().AddForce(transform.forward * magneticForce);
        }

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


