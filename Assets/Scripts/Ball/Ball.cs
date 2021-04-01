using System.Collections;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
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
        [SerializeField] float recallDistance = 0.3f;

        public int TouchLevel { get; set; }
        public float Hue { get; set; }

        float touchTimer;
        bool touchToggled;
        Vector3 lassoOrigin;

        CastOrigins origins;
        BallCaster caster;
        BallJedi jedi;
        BallOsc osc;
        Rigidbody rigidbody;

        private void Awake()
        {
            osc = GetComponent<BallOsc>();
        }

        void Start()
        {
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            jedi = GetComponent<BallJedi>();
            rigidbody = GetComponent<Rigidbody>();

            TouchLevel = 0;
            Hue = 0;
            touchTimer = Mathf.Infinity;


            GetComponent<AudioSource>().PlayOneShot(conjureFX);
        }

        void Update()
        {
            touchTimer += Time.deltaTime;
            float distToOrigin = Vector3.Distance(transform.position, lassoOrigin);

            if (jedi.Power == TheForce.push)
            {
                transform.LookAt(2 * transform.position - Camera.main.transform.position);
                rigidbody.AddForce(transform.forward * (origins.PalmsDist / jedi.HoldDistance * jedi.PushForce));
            }

            if (jedi.Power == TheForce.pull)
            {
                transform.LookAt(Camera.main.transform.position);
                rigidbody.AddForce(transform.forward * (origins.PalmsDist / jedi.HoldDistance * jedi.PullForce));
            }

            if (jedi.Power == TheForce.lift)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                rigidbody.AddForce(transform.up * (origins.PalmsDist / jedi.HoldDistance * jedi.LiftForce));
            }

            if (jedi.Recall)
            {
                lassoOrigin = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>().GetRtPalm.Position;
                //transform.position = lassoOrigin;
                transform.LookAt(lassoOrigin);

                if (distToOrigin > recallDistance)
                {
                    rigidbody.AddForce((transform.forward * jedi.RecallForce));
                }
                jedi.Recall = false;
            }
        }

        private void OnCollisionEnter(Collision other) {
            if (jedi.Frozen) { return; }

            Vector3 dir = other.contacts[0].point - transform.position;
            dir = -dir.normalized;

            var force = other.impulse.magnitude >= 1 ? other.impulse.magnitude : 1;

            if (hasBounce)
            {
                rigidbody.AddForce(dir * force * bounce);

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
            else
            {
                osc.Send("bounced", TouchLevel);
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
            osc.Send("touched", TouchLevel);
        }

        public void KillBall(OscMessage message)
        {
            StartCoroutine("DestroySelf");
        }

        IEnumerator DestroySelf() 
        {
            if (GetComponentInChildren<DeathParticlesId>())
            {
                var deathParticles = GetComponentInChildren<DeathParticlesId>().GetComponent<ParticleSystem>();
                var deathMain = deathParticles.main;
                deathMain.startColor = Color.HSVToRGB(Hue, 1, 1);
                deathParticles.Play();

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
            osc.Send("iDead");
            Destroy(gameObject); 
        }

    }
}


