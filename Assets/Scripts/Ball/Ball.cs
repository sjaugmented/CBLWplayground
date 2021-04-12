using System.Collections;
using UnityEngine;
using LW.Core;

namespace LW.Ball{
    public enum BallState { Active, Still };
    public enum Notes { rFore, rBack, rFist, rPointer, rPeace, lFore, lBack, lFist, lPointer, lPeace, none}

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
        [SerializeField] float bounce = 30;
        [SerializeField] float recallDistance = 0.3f;
        [SerializeField] float antiGrav = 0.5f;
        [SerializeField] float maxSpinY = 30;
        [SerializeField] float maxSpinZ = 20;
        

        public BallState State = BallState.Active;
        public Notes Note = Notes.none;
        public bool WithinRange { get; set; }
        public int TouchLevel { get; set; }
        public float Hue { get; set; }
        public bool CoreActive { get; set; }
        bool touched = false;
        public bool InteractingWithParticles { get; set; }
        public Color NoteColor { get; set; }
        public Vector3 LockPos { get; set; }
        public bool Manipulating { get; set; }
        public bool HasSpawned { get; set; }
        public bool Stasis { get; set; }

        float touchTimer = Mathf.Infinity;
        bool touchResponseLimiter;
        Vector3 lassoOrigin;

        BallDirector director;
        NewTracking tracking;
        CastOrigins origins;
        BallCaster caster;
        BallJedi jedi;
        BallOsc osc;
        Rigidbody rigidbody;
        MultiAxis multiAxis;

        private void Awake()
        {
            osc = GetComponent<BallOsc>();
        }

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            jedi = GetComponent<BallJedi>();
            rigidbody = GetComponent<Rigidbody>();
            multiAxis = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<MultiAxis>();

            TouchLevel = 0;
            Hue = 0;
            CoreActive = true;

            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            StartCoroutine("Spawning");
        }

        void Update()
        {
            State = director.Still ? BallState.Still : BallState.Active;

            touchTimer += Time.deltaTime;
            float distToOrigin = Vector3.Distance(transform.position, lassoOrigin);
            float distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
            WithinRange = distanceToPlayer < perimeter;
            GetComponent<Collider>().enabled = !InteractingWithParticles;
            containmentSphere.SetActive(State == BallState.Still);
            CoreActive = touched;
            InteractingWithParticles = jedi.ControlPose != HandPose.none;

            if (State == BallState.Still && jedi.Primary == Force.idle && !Manipulating && !jedi.Recall)
            {
                transform.position = LockPos;
            }
            else
            {
                LockPos = transform.position;
            }

            Quaternion handsRotation = Quaternion.Slerp(tracking.GetRtPalm.Rotation, tracking.GetLtPalm.Rotation, 0.5f);
            float totalPrimaryRange = 90 - multiAxis.DeadZone;
            float totalSecondaryRange = 90 - multiAxis.DeadZone / 2;

            if (jedi.Primary == Force.pull)
            {
                float pullCorrection = 90 + multiAxis.DeadZone;
                float forceFloat = Mathf.Clamp((multiAxis.StaffRight - pullCorrection) / totalPrimaryRange, 0, 1);
                transform.rotation = handsRotation * Quaternion.Euler(multiAxis.InOffset);
                rigidbody.AddForce(transform.forward * forceFloat * jedi.MasterForce);
            }
            
            if (jedi.Primary == Force.push)
            {
                float forceFloat = Mathf.Clamp(1 - (multiAxis.StaffRight / totalPrimaryRange), 0, 1);
                transform.rotation = handsRotation * Quaternion.Euler(multiAxis.OutOffset);
                rigidbody.AddForce(transform.forward * forceFloat * jedi.MasterForce);
            }

            if (jedi.Secondary == Force.right)
            {
                float rightCorrection = 90 + multiAxis.DeadZone / 2;
                float forceFloat = Mathf.Clamp((multiAxis.StaffForward - rightCorrection) / totalSecondaryRange, 0, 1);
                rigidbody.AddForce(transform.right * forceFloat * jedi.MasterForce);
            }

            if (jedi.Secondary == Force.left)
            {
                float forceFloat = Mathf.Clamp(1 - (multiAxis.StaffForward / totalSecondaryRange), 0, 1);
                rigidbody.AddForce(transform.right * -forceFloat * jedi.MasterForce);
            }

            if (jedi.Spin)
            {
                transform.Rotate(0, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0, 1)) * maxSpinY, tracking.StaffRight / 90 * maxSpinZ);
            }

            if (jedi.Recall)
            {
                lassoOrigin = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>().GetRtPalm.Position;
                transform.LookAt(lassoOrigin);

                if (distToOrigin > recallDistance)
                {
                    rigidbody.AddForce((transform.forward * jedi.RecallForce) + new Vector3(0, antiGrav, 0));
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (InteractingWithParticles) { return; }

            Vector3 dir = other.contacts[0].point - transform.position;
            dir = -dir.normalized;

            var force = other.impulse.magnitude >= 1 ? other.impulse.magnitude : 1;

            if (hasBounce && State == BallState.Active && (other.gameObject.CompareTag("RightHand") || other.gameObject.CompareTag("LeftHand")))
            {
                rigidbody.AddForce(dir * force * bounce);

                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(bounceFX);
                }
            }

            if (HasSpawned && other.gameObject.CompareTag("RightHand") || other.gameObject.CompareTag("LeftHand"))
            {
                if (!touchResponseLimiter)
                {
                    TouchLevel += 1;
                    DetermineTouchResponse(other);
                    if (HasSpawned)
                    {
                        osc.Send(Note.ToString(), TouchLevel);
                    }
                    touchResponseLimiter = true;
                }

                if (State == BallState.Active)
                {
                    touched = true;
                }
            }
        }

        private void DetermineTouchResponse(Collision other)
        {
            if (InteractingWithParticles) { return; }

            if (other.collider.CompareTag("RightHand"))
            {
                if (jedi.LevelUpTimer < 2 && tracking.rightPose == HandPose.fist)
                {
                    osc.Send("LevelUp!");
                    caster.WorldLevel = caster.WorldLevel == 1 ? 2 : 1;
                    StartCoroutine("DestroySelf");
                }

                if (tracking.rightPose == HandPose.fist)
                {
                    Note = Notes.rFist;
                    NoteColor = Color.HSVToRGB(0, 1, 0.8f); // red
                }
                else if (tracking.rightPose != HandPose.fist)
                {
                    if (tracking.rightPose == HandPose.pointer)
                    {
                        Note = Notes.rPointer;
                        NoteColor = Color.HSVToRGB(0.66f, 0.58f, 0.8f); // baby blue

                    }
                    else if (tracking.rightPose == HandPose.peace)
                    {
                        Note = Notes.rPeace;
                        NoteColor = Color.HSVToRGB(0.29f, 0.58f, 1f); // light green

                    }
                    else if (other.gameObject.name == "Backhand")
                    {
                        Note = Notes.rBack;
                        NoteColor = Color.HSVToRGB(0.29f, 1, 0.8f); // green
                    }
                    else
                    {
                        Note = Notes.rFore;
                        NoteColor = Color.HSVToRGB(0.66f, 1, 0.8f); // blue
                    }
                }
            }
            else
            {
                if (tracking.leftPose == HandPose.fist)
                {
                    Note = Notes.lFist;
                    NoteColor = Color.HSVToRGB(0.15f, 1, 0.8f); // yellow
                }
                else if (tracking.leftPose != HandPose.fist)
                {
                    if (tracking.leftPose == HandPose.pointer)
                    {
                        Note = Notes.lPointer;
                        NoteColor = Color.HSVToRGB(0, 0.58f, 1); // light red

                    }
                    else if (tracking.leftPose == HandPose.peace)
                    {
                        Note = Notes.lPeace;
                        NoteColor = Color.HSVToRGB(0.86f, 0.58f, 1); // pink

                    }
                    else if (other.gameObject.name == "Backhand")
                    {
                        Note = Notes.lBack;
                        NoteColor = Color.HSVToRGB(0.89f, 1, 0.8f); // magenta
                    }
                    else
                    {
                        Note = Notes.lFore;
                        NoteColor = Color.HSVToRGB(0.5f, 1, 0.8f); // cyan
                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (State == BallState.Active)
            {
                touched = false;
            }
            touchResponseLimiter = false;
        }

        public void KillBall(OscMessage message)
        {
            StartCoroutine("DestroySelf");
        }

        IEnumerator DestroySelf()
        {
            HasSpawned = false;

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

        IEnumerator Spawning()
        {
            HasSpawned = false;
            yield return new WaitForSeconds(1);
            HasSpawned = true;
        }

        public void IsGazedAt()
        {
            // something
        }

        public void PlayStillFx()
        {
            GetComponent<AudioSource>().PlayOneShot(stillFX);
        }

        public void PlayActionFx()
        {
            GetComponent<AudioSource>().PlayOneShot(activeFX);
        }
    }
}


