using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class PortalController : MonoBehaviour
    {
        [SerializeField] AudioClip breachFX;
        [SerializeField] AudioClip openFX;

        BallDirector director;
        BallCaster caster;

        private void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().PlayOneShot(openFX);
            }

            //director.Portal = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                caster.DestroyBall();
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(breachFX);
                }
            }
            if (other.name == "Ball(Clone)")
            {
                caster.WorldLevel += 1;
                director.TogglePortalBool();
                SelfDestruct();
            }
            //else
            //{
            //    caster.WorldLevel -= 1;
            //}
        }

        public void SelfDestruct()
        {
            Destroy(gameObject);
        }
    }
}


