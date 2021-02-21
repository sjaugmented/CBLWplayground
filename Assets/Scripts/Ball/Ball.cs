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
        [SerializeField] float magnetism = 2;

        HandTracking hands;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
        }

        void Update()
        {
            float distanceToHand = Vector3.Distance(transform.position, hands.rightPalm.Position);

            if (distanceToHand < 0.05f) {
                // gentle push toward hand
                transform.LookAt(hands.rightPalm.Position);
                GetComponent<Rigidbody>().AddForce(transform.forward * magnetism);
            }
        }

        public void DestroySelf() {
            GetComponent<MeshExploder>().Explode();
            GetComponent<AudioSource>().PlayOneShot(destroyFX);
            GetComponentInChildren<MeshRenderer>().enabled = false;
            var particles = GetComponentInChildren<ParticleSystem>().emission;
            particles.enabled = false;
        }
    }
}


