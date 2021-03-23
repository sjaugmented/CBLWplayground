using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class PortalController : MonoBehaviour
    {
        [SerializeField] AudioClip breachFX;
        [SerializeField] AudioClip openFX;

        BallCaster caster;

        private void Start()
        {
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().PlayOneShot(openFX);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                Debug.Log("Hello " + other.name);
                caster.DestroyBall();
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(breachFX);
                }
            }
            if (other.name == "Ball(Clone)")
            {
                caster.WorldLevel = 2;
            }
            else
            {
                caster.WorldLevel = 1;
            }
            Debug.Log("WorldLevel: " + caster.WorldLevel);
        }

        public void DestroySelf()
        {
            Debug.Log("goodbye");
            Destroy(gameObject);
        }
    }
}


