using System.Collections;
using UnityEngine;
using LW.Core;
using System.Collections.Generic;

namespace LW.Ball{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] string killCode;
        [SerializeField] string glitterCode;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] float destroyDelay = 0.5f;
        // [SerializeField] float stopRange = 0.06f;
        [SerializeField] float bounceForce = 1;
        [SerializeField] float touchFrequency = 1;
        [SerializeField] float forceMult = 10000;
        [SerializeField] float killForce = 1000;

        public float distanceToRtHand, distanceToLtHand;

        public int WorldLevel { get; set; }
        public int TouchLevel { get; set; }

        float touchTimer = Mathf.Infinity;
        bool touchToggled = false;
        
        float hueVal = Mathf.Epsilon;
        bool alive = true;

        NewTracking tracking;
        OSC osc;
        BallCaster caster;
        ParticleSystem particles;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            particles = GetComponentInChildren<ParticleSystem>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, KillBall);
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(GlitterBall);

            SendOSC("iAM!");

            WorldLevel = 1;
            TouchLevel = 0;
        }

        void Update()
        {
            touchTimer += Time.deltaTime;
            
            distanceToRtHand = Vector3.Distance(transform.position, tracking.GetRtPalm.Position);
            distanceToLtHand = Vector3.Distance(transform.position, tracking.GetLtPalm.Position);

            if (alive)
            {
                ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
                var main = particles.main;
                main.startColor = Color.HSVToRGB(hueVal, 1, 1);

                Light light = GetComponentInChildren<Light>();
                light.color = Color.HSVToRGB(hueVal, 1, 1);

            }
        }

        private void OnCollisionEnter(Collision other) {
            float collisionForce = other.impulse.magnitude * forceMult / Time.fixedDeltaTime;
            Debug.Log("FORCE>>>>" + collisionForce);

            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("ForceField"))
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
                if (collisionForce >= killForce)
                {
                    //caster.StartCoroutine("DestroyBall");
                    Debug.Log(other.collider.gameObject.layer);
                }
            }
            
        }

        private void TouchResponse()
        {
            hueVal += 0.1388f; // 1/5 of 360
            if (hueVal > 1)
            {
                hueVal -= 1;
            }

            TouchLevel += 1;
            SendOSC("touched", TouchLevel);
        }

        public void SendOSC(string address, float val = 1) {
            OscMessage message = new OscMessage();
            message.address = WorldLevel + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }

        public bool Handled {get; set;}

        void KillBall(OscMessage message)
        {
            // TODO rethink this logic of having caster destroy Ball
            StartCoroutine("DestroySelf");
        }

        void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<MeshExploder>().Explode();
        }

        IEnumerator DestroySelf() 
        {
            SendOSC("iDead");
            alive = false;
            var emission = particles.emission;
            emission.enabled = false;
            GetComponentInChildren<MeshExploder>().Explode();

            if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().PlayOneShot(destroyFX);
            }

            MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mesh in meshes)
            {
                mesh.enabled = false;
            }

            yield return new WaitForSeconds(destroyDelay);
            
            caster.Ball = false;
            Destroy(gameObject);
        }

    }
}


