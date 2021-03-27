using System.Collections;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] string killCode;
        [SerializeField] string glitterCode;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] AudioClip bounceFX;
        [SerializeField] float explosionDelay = 0.5f;
        [SerializeField] float destroyDelay = 0.5f;
        [SerializeField] float bounceForce = 1;
        [SerializeField] float touchFrequency = 1;
        [SerializeField] float forceMult = 10000;
        [SerializeField] float killForce = 1000;
        [SerializeField] float bounce = 10;

        public float distanceToRtHand, distanceToLtHand;

        public int TouchLevel { get; set; }

        float touchTimer = Mathf.Infinity;
        bool touchToggled = false;
        float hueVal = Mathf.Epsilon;
        bool alive = true;
        bool frozenSent, deathSent;

        NewTracking tracking;
        OSC osc;
        BallCaster caster;
        ParticleSystem innerParticles;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            innerParticles = GetComponentInChildren<ParticleSystem>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, KillBall);
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(GlitterBall);

            SendOSC("iAM!");
            TouchLevel = 0;
        }

        void Update()
        {
            touchTimer += Time.deltaTime;
            
            distanceToRtHand = Vector3.Distance(transform.position, tracking.GetRtPalm.Position);
            distanceToLtHand = Vector3.Distance(transform.position, tracking.GetLtPalm.Position);
            
            // TODO move to particle controller script
            //ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            //Light light = GetComponentInChildren<Light>();
            //var main = particles.main;
            //var emission = particles.emission;
            //main.startLifetime = caster.Held ? 3 : 1.15f;
            //main.startSpeed = caster.Held ? 1f : 0.25f;
            //emission.enabled = !caster.Frozen;
            //light.enabled = !caster.Frozen;

            //if (alive)
            //{
            //    main.startColor = Color.HSVToRGB(hueVal, 1, 1);
            //    light.color = Color.HSVToRGB(hueVal, 1, 1);

            //}

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
            //float collisionForce = other.impulse.magnitude * forceMult / Time.fixedDeltaTime;

            if (caster.Frozen) { return; }

            Vector3 dir = other.contacts[0].point - transform.position;
            dir = -dir.normalized;

            var force = other.impulse.magnitude >= 1 ? other.impulse.magnitude : 1;
            // TODO move to bounce script
            //GetComponent<Rigidbody>().AddForce(dir * force * bounce);

            //if (!GetComponent<AudioSource>().isPlaying)
            //{
            //    GetComponent<AudioSource>().PlayOneShot(bounceFX);
            //}

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
            // TODO move to hue script
            //hueVal += 0.1388f; // 1/5 of 360
            //if (hueVal > 1)
            //{
            //    hueVal -= 1;
            //}

            TouchLevel += 1;
            SendOSC("touched", TouchLevel);
        }

        public void SendOSC(string address, float val = 1) {
            OscMessage message = new OscMessage();
            message.address = caster.WorldLevel + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }

        public bool Handled {get; set;}

        void KillBall(OscMessage message)
        {
            // TODO rethink this logic of having caster destroy Ball
            StartCoroutine("DestroySelf");
        }

        // TODO move to glitter script
        //void GlitterBall(OscMessage message)
        //{
        //    SendMessage("GlitterBall");
        //    GetComponentInChildren<FacePlayer>().GetComponent<ParticleSystem>().Play();
        //}

        IEnumerator DestroySelf() 
        {
            if (!deathSent)
            {
                SendOSC("iDead");
                deathSent = true;
            }
            
            alive = false;
            
            // TODO move to particle controller script
            //var emission = innerParticles.emission;
            //emission.enabled = false;
            //var deathParticles = GetComponentInChildren<DeathParticles>().GetComponent<ParticleSystem>();
            //var deathMain = deathParticles.main;
            //deathMain.startColor = Color.HSVToRGB(hueVal, 1, 1);
            //deathParticles.Play();
            
            // yield return new WaitForSeconds(explosionDelay);

            //MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
            //foreach(MeshRenderer mesh in meshes)
            //{
            //    mesh.enabled = false;
            //}

            if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().PlayOneShot(destroyFX);
            }

            yield return new WaitForSeconds(destroyDelay);
            
            caster.Ball = false;
            Destroy(gameObject);
        }

    }
}


