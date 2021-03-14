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
        // can catch in forcefield between hands and manipulate floats
        // TODO
        // every cast levels up the world integer 

        [SerializeField] string killCode;
        [SerializeField] string glitterCode;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] float destroyDelay = 0.5f;
        [SerializeField] float magnetRange = 0.1f;
        // [SerializeField] float stopRange = 0.06f;
        [SerializeField] float magneticForce = 2;
        [SerializeField] float bounceForce = 1;
        [SerializeField] float touchFrequency = 1;
        float touchTimer = Mathf.Infinity;
        bool touchToggled = false;
        [SerializeField] float forceMult = 10000;
        [SerializeField] float killForce = 1000;

        public float distanceToRtHand, distanceToLtHand;

        int worldLevel = 0;
        int touchValue = 0;
        public int WorldLevel 
        { 
            get { return worldLevel;  }
            set { worldLevel = value; } 
        }
        public int TouchLevel
        {
            get { return touchValue; }
            set { touchValue = value; }
        }

        float hueVal = Mathf.Epsilon;
        //bool gravity;

        NewTracking tracking;
        //CastOrigins castOrigins;
        OSC osc;
        BallCaster caster;
        //ForceField forceField;

        void Start()
        {
            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            //castOrigins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();

            //gravity = GetComponent<Rigidbody>().useGravity; // TODO do we need this?
            //forceField = GameObject.FindGameObjectWithTag("ForceField").GetComponent<ForceField>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, KillBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);

            SendOSC("iAM!");
        }

        void Update()
        {
            touchTimer += Time.deltaTime;
            
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
            float collisionForce = other.impulse.magnitude * forceMult / Time.fixedDeltaTime;
            Debug.Log("FORCE>>>>>>>>>>>>>>>>>>>>>" + collisionForce);

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
                        hueVal += 0.1388f; // 1/5 of 360
                        if (hueVal > 1)
                        {
                            hueVal -= 1;
                        }

                        var ballMaterial = GetComponentInChildren<Renderer>().material;
                        ballMaterial.color = Color.HSVToRGB(hueVal, 1, 1);

                        touchValue += 1;
                        SendOSC("touched", touchValue);
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

        public void SendOSC(string address, float val = 1) {
            OscMessage message = new OscMessage();
            message.address = worldLevel + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }

        private void Magnetism(Vector3 attraction)
        {
            transform.LookAt(attraction);
            GetComponent<Rigidbody>().AddForce(transform.forward * magneticForce);
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
            GetComponentInChildren<MeshExploder>().Explode();
            if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().PlayOneShot(destroyFX);
            }
            GetComponentInChildren<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(destroyDelay);
            caster.Ball = false;
            Destroy(gameObject);
        }

    }
}


