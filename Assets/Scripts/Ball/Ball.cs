using System.Collections;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    public enum BallState { Active, Still };

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] GameObject containmentSphere; // on off based on frozen
        [SerializeField] float perimeter = 0.5f;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip destroyFX;
        [SerializeField] AudioClip bounceFX;
        [SerializeField] AudioClip stillFX;
        [SerializeField] AudioClip activeFX;
        [SerializeField] float explosionDelay = 0.5f;
        [SerializeField] float destroyDelay = 0.5f;
        [SerializeField] float bounceForce = 1;
        [SerializeField] float touchFrequency = 1;
        [SerializeField] float forceMult = 10000;
        [SerializeField] float killForce = 1000;
        [SerializeField] bool hasBounce = true;
        [SerializeField] float bounce = 10;
        [SerializeField] float recallDistance = 0.3f;
        [SerializeField] float antiGrav = 0.7f;

        public BallState State = BallState.Active;
        public bool WithinRange { get; set; }
        public int TouchLevel { get; set; }
        public float Hue { get; set; }
        public bool CoreActive { get; set; }
        bool touched = false;
        public bool InteractingWithParticles { get; set; }

        float touchTimer = Mathf.Infinity;
        bool touchToggle;
        Vector3 lassoOrigin;

        NewTracking tracking;
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
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            jedi = GetComponent<BallJedi>();
            rigidbody = GetComponent<Rigidbody>();

            TouchLevel = 0;
            Hue = 0;
            CoreActive = true;

            GetComponent<AudioSource>().PlayOneShot(conjureFX);
        }

        void Update()
        {
            touchTimer += Time.deltaTime;
            float distToOrigin = Vector3.Distance(transform.position, lassoOrigin);
            float distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
            WithinRange = distanceToPlayer < perimeter;
            GetComponent<Collider>().enabled = !InteractingWithParticles;
            containmentSphere.SetActive(State == BallState.Still);
            CoreActive = touched || jedi.Power != TheForce.idle || jedi.Recall == true;
            InteractingWithParticles = jedi.ControlPose != HandPose.none;

            if (jedi.Power == TheForce.push)
            {
                transform.LookAt(2 * transform.position - Camera.main.transform.position);
                rigidbody.AddForce(transform.forward * (origins.PalmsDist / jedi.HoldDistance * jedi.PushForce) + new Vector3(0, antiGrav, 0));
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

            if (jedi.Power == TheForce.down)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                rigidbody.AddForce(transform.up * -(origins.PalmsDist / jedi.HoldDistance * jedi.LiftForce));
            }

            if (jedi.Recall)
            {
                lassoOrigin = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>().GetRtPalm.Position;
                transform.LookAt(lassoOrigin);

                if (distToOrigin > recallDistance)
                {
                    rigidbody.AddForce((transform.forward * jedi.RecallForce));
                }
            }
        }

        private void OnCollisionEnter(Collision other) {
            if (InteractingWithParticles) { return; }

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
                DetermineTouchResponse(other);
            }
            else
            {
                osc.Send("bounced", TouchLevel);
                Debug.Log("bounced");
            }
        }

        private void DetermineTouchResponse(Collision other)
        {
            if (jedi.PunchTimer < 2)
            {
                if (tracking.rightPose == HandPose.fist)
                {
                    osc.Send("LevelUp!");
                    caster.WorldLevel += 1;
                    DestroySelf();
                }
            }
            
            if (tracking.rightPose == HandPose.fist || tracking.leftPose == HandPose.fist)
            {
                if (State != BallState.Still)
                {
                    GetComponent<AudioSource>().PlayOneShot(stillFX);
                }
                State = BallState.Still;
            }

            if (tracking.rightPose == HandPose.flat || tracking.leftPose == HandPose.flat)
            {
                if (State != BallState.Active)
                {
                    GetComponent<AudioSource>().PlayOneShot(activeFX);
                }
                State = BallState.Active;
            }

            if (State == BallState.Active)
            {
                touched = true;
            }

            if (!touchToggle)
            {
                //touchTimer = 0;
                //touchToggled = true;
                TouchLevel += 1;
                TouchOSC(other);
                touchToggle = true;
            }

            //if (touchTimer > touchFrequency)
            //{
            //TouchLevel += 1;
            //TouchOSC();
            //touchToggled = false;
            //}
        }

        private void OnCollisionExit(Collision collision)
        {
            if (State == BallState.Active)
            {
                touched = false;
            }
            touchToggle = false;
        }

        private void TouchOSC(Collision other)
        {
            if (tracking.rightPose == HandPose.fist)
            {
                osc.Send("rightFist", TouchLevel);
            }
            else
            {
                if (other.gameObject.name == "Backhand")
                {
                    osc.Send("rightOpen/backhand", TouchLevel);
                }
                else
                {
                    Debug.Log(other.gameObject.name);
                    osc.Send("rightOpen/forehand", TouchLevel);
                }
            }

            if (tracking.leftPose == HandPose.fist)
            {
                osc.Send("leftFist", TouchLevel);
            }
            else
            {
                if (other.gameObject.name == "Backhand")
                {
                    osc.Send("leftOpen/backhand", TouchLevel);
                }
                else
                {
                    osc.Send("leftOpen/forehand", TouchLevel);
                }
            }

            //Hue += 0.1388f; // five colors: 1/5 of 360
            //if (Hue > 1)
            //{
            //    Hue -= 1;
            //}
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

        public void IsGazedAt()
        {
            // something
        }

        // NOTES
        // Action Mode
        // no floats
        // each touch has a unique color
        // forehand backhand and fist
        // more bounce
        // play with recall more - punch it on recall and the shell explodes
        // 
        // STILL
        // new transitional gesture into still
        // pointer X or dorsal tap or peace smash
        // Two floats - distance only - CoreHue and CoreDensity
        // Ten touches - adding pointer and peace back 
        // immovable except by jedi
        // touch responses - ring colors (default back to no color in action mode)

    }
}


