using System.Collections;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] string killCode;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] AudioClip bounceFX;
        [SerializeField] float explosionDelay = 0.5f;
        [SerializeField] float destroyDelay = 0.5f;
        [SerializeField] float bounceForce = 1;
        [SerializeField] float touchFrequency = 1;
        [SerializeField] float forceMult = 10000;
        [SerializeField] float killForce = 1000;
        [SerializeField] bool hasBounce = true;
        [SerializeField] float bounce = 10;

        public bool IsAlive { get; set; }
        public int TouchLevel { get; set; }
        public float Hue { get; set; }
        public bool IsHeld { get; set; }
        public bool IsFrozen { get; set; }
        //public bool Handled { get; set; }

        float distanceToRtHand, distanceToLtHand, touchTimer;
        bool frozenSent, deathSent, touchToggled;

        NewTracking tracking;
        OSC osc;
        BallCaster caster;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, KillBall);

            TouchLevel = 0;
            IsAlive = true;
            Hue = 0;
            touchTimer = Mathf.Infinity;

            SendOSC("iAM!");
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
        }

        void Update()
        {
            touchTimer += Time.deltaTime;

            distanceToRtHand = Vector3.Distance(transform.position, tracking.GetRtPalm.Position);
            distanceToLtHand = Vector3.Distance(transform.position, tracking.GetLtPalm.Position);

            IsHeld = caster.Held;
            IsFrozen = caster.Frozen;
            
            if (caster.Frozen)
            {
                if (!frozenSent)
                {
                    SendOSC("frozen");
                    frozenSent = true;
                }
            } else
            {
                frozenSent = false;
            }
        }

        private void OnCollisionEnter(Collision other) {
            if (caster.Frozen) { return; }

            Vector3 dir = other.contacts[0].point - transform.position;
            dir = -dir.normalized;

            var force = other.impulse.magnitude >= 1 ? other.impulse.magnitude : 1;

            if (hasBounce)
            {
                GetComponent<Rigidbody>().AddForce(dir * force * bounce);

                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(bounceFX);
                }
            }

            if (other.gameObject.CompareTag("Player"))
            {
                if (other.gameObject.CompareTag("Player")) {
                    if (!touchToggled)
                    {
                        touchTimer = 0;
                        touchToggled = true;
                    }

                    if (touchTimer > touchFrequency)
                    {
                        TouchResponse();
                        touchToggled = false;
                    }
                }
            }
        }

        private void TouchResponse()
        {
            Hue += 0.1388f; // 1/5 of 360
            if (Hue > 1)
            {
                Hue -= 1;
            }

            TouchLevel += 1;
            SendOSC("touched", TouchLevel);
        }

        public void SendOSC(string address, float val = 1) {
            OscMessage message = new OscMessage();
            message.address = caster.WorldLevel + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }

        void KillBall(OscMessage message)
        {
            // TODO rethink this logic of having caster destroy Ball
            StartCoroutine("DestroySelf");
        }

        IEnumerator DestroySelf() 
        {
            if (!deathSent)
            {
                SendOSC("iDead");
                deathSent = true;
            }
            
            IsAlive = false;

            if (GetComponentInChildren<DeathParticlesId>())
            {
                var deathParticles = GetComponentInChildren<DeathParticlesId>().GetComponent<ParticleSystem>();
                var deathMain = deathParticles.main;
                deathMain.startColor = Color.HSVToRGB(Hue, 1, 1);
                deathParticles.Play();

                yield return new WaitForSeconds(explosionDelay);

                MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mesh in meshes)
                {
                    mesh.enabled = false;
                }
            }

            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().PlayOneShot(destroyFX);
            }
            
            yield return new WaitForSeconds(destroyDelay);
            
            caster.BallInPlay = false;
            Destroy(gameObject); 
        }

    }
}


