using System.Collections;
using UnityEngine;
using LW.Core;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using Photon.Realtime;

namespace LW.Ball{
    public enum BallState { Active, Still, Dead };
    public enum Notes { rFore, rBack, rFist, rPointer, rPeace, rThumbsUp, lFore, lBack, lFist, lPointer, lPeace, lThumbsUp, none}

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] bool toggleContainmentSphere = true;
        [SerializeField] GameObject containmentSphere; // on off based on frozen
        [SerializeField] float perimeter = 0.5f;
        [SerializeField] AudioClip conjureFX;
        [SerializeField] AudioClip resetFX;
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
        [SerializeField] float masterThrottle = 0.5f;
        [SerializeField] float unClampedLerp = 2f;
        [SerializeField] float modeToggleSpacer = 1;

        public Hands Handedness = Hands.none;
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
        public bool IsNotQuiet { get; set; }
        public bool Still { get; set; }
        public bool ModeToggled { get; set; }
        public bool BallCollision { get; set; }

        //float touchTimer = Mathf.Infinity;
        bool touchResponseLimiter, hasReset;
        Vector3 lassoOrigin;
        public MixedRealityPose DominantHand { get; set; }
        public HandPose DominantPose { get; set; }
        public Direction DominantDir { get; set; }
        public float Distance { get; set; }
        public float ModeToggleTimer { get; set; }

        BallDirector director;
        NewTracking tracking;
        CastOrigins origins;
        BallJedi jedi;
        BallOsc osc;
        Rigidbody rigidbody;
        MultiAxisController multiAxis;
        NotePlayer notePlayer;
        IEnumerator quietBall, destroySelf;

        private void Awake()
        {
            osc = GetComponent<BallOsc>();
        }

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            jedi = GetComponent<BallJedi>();
            rigidbody = GetComponent<Rigidbody>();
            multiAxis = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<MultiAxisController>();
            notePlayer = GetComponent<NotePlayer>();

            TouchLevel = 0;
            Hue = 0;

            GetComponent<AudioSource>().PlayOneShot(conjureFX);
            quietBall = QuietBall();
            destroySelf = DestroySelf();
            StartCoroutine(quietBall);

            ModeToggleTimer = Mathf.Infinity;
        }

        void Update()
        {
            State = Still ? BallState.Still : BallState.Active;
            Distance = Vector3.Distance(transform.position, Camera.main.transform.position);

            ModeToggleTimer += Time.deltaTime;

            DominantHand = Handedness == Hands.right ? tracking.GetRtPalm : tracking.GetLtPalm;
            DominantPose = Handedness == Hands.right ? tracking.rightPose : tracking.leftPose;
            DominantDir = Handedness == Hands.right ? tracking.rightPalmAbs : tracking.leftPalmAbs;

            //touchTimer += Time.deltaTime;

            float distToOrigin = Vector3.Distance(transform.position, lassoOrigin);
            float distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
            WithinRange = distanceToPlayer < perimeter;

            GetComponent<SphereCollider>().enabled = !InteractingWithParticles;
            containmentSphere.SetActive(!toggleContainmentSphere || State == BallState.Still);
            CoreActive = touched;
            //InteractingWithParticles = jedi.HoldPose != HandPose.none;

            if (State == BallState.Still && jedi.Primary == Force.idle && !Manipulating && !jedi.Recall && !jedi.Reset)
            {
                transform.position = LockPos;
            }
            else
            {
                LockPos = transform.position;
            }

            if (jedi.Held && State == BallState.Active)
            {
                rigidbody.velocity += new Vector3(0, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0.001f, 1)) * jedi.GingerLift);
            }

            Quaternion handsRotation = Quaternion.Slerp(tracking.GetRtPalm.Rotation, tracking.GetLtPalm.Rotation, 0.5f);
            float totalPrimaryRange = 90 - multiAxis.DeadZone;
            float totalSecondaryRange = 90 - multiAxis.DeadZone / 2;
            float palmDistThrottle = (1 - Mathf.Clamp(jedi.RelativeHandDist, 0.001f, 1)) * masterThrottle;

            if (jedi.Primary == Force.pull)
            {
                float pullCorrection = 90 + multiAxis.DeadZone;
                float palmForce = Mathf.Clamp((multiAxis.PalmRightStaffForward - pullCorrection) / totalPrimaryRange, 0.001f, 1);
                transform.rotation = handsRotation * Quaternion.Euler(multiAxis.InOffset);
                rigidbody.AddForce(transform.forward * (palmForce * palmDistThrottle * jedi.MasterForce));
            }
            
            if (jedi.Primary == Force.push)
            {
                float palmForce = Mathf.Clamp(1 - (multiAxis.PalmRightStaffForward / totalPrimaryRange), 0.001f, 1);
                transform.rotation = handsRotation * Quaternion.Euler(multiAxis.OutOffset);
                rigidbody.AddForce(transform.forward * (palmForce * palmDistThrottle * jedi.MasterForce));
            }

            if (jedi.Secondary == Force.right)
            {
                float rightCorrection = 90 + multiAxis.DeadZone / 2;
                float palmForce = Mathf.Clamp((multiAxis.StaffForwardCamForward - rightCorrection) / totalSecondaryRange, 0.001f, 1);
                //rigidbody.AddForce(transform.right * palmForce * jedi.MasterForce);
                rigidbody.velocity += new Vector3(palmForce * -jedi.MasterForce * 0.01f, 0);
            }

            if (jedi.Secondary == Force.left)
            {
                float palmForce = Mathf.Clamp(1 - (multiAxis.StaffForwardCamForward / totalSecondaryRange), 0.001f, 1);
                //rigidbody.AddForce(transform.right * -palmForce * jedi.MasterForce);
                rigidbody.velocity += new Vector3(palmForce * jedi.MasterForce * 0.01f, 0);
            }

            if (jedi.Spin)
            {
                var shell = GetComponentInChildren<ShellId>().transform;
                Debug.Log("spinning");
                
                if (jedi.HoldPose == HandPose.flat)
                {
                    shell.Rotate(tracking.StaffRight / 90 * maxSpinZ, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0.001f, 1)) * maxSpinY, 0);
                }
                else if (jedi.HoldPose == HandPose.pointer)
                {
                    shell.Rotate(0, tracking.StaffRight / 90 * maxSpinZ, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0.001f, 1)) * maxSpinY);
                }
                else if (jedi.HoldPose == HandPose.thumbsUp)
                {
                    shell.Rotate((1 - Mathf.Clamp(jedi.RelativeHandDist, 0.001f, 1)) * maxSpinY, tracking.StaffRight / 90 * maxSpinZ, 0);
                }
                
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

            if (jedi.Reset)
            {
                ResetBall();
            }
            else
            {
                hasReset = false;
            }
        }

        private void ResetBall()
        {
            if (!hasReset)
            {
                transform.position = DominantHand.Position + director.SpawnOffset;
                transform.rotation = Camera.main.transform.rotation;
                
                if (IsNotQuiet)
                {
                    GetComponent<BallOsc>().Send("reset");
                }

                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(resetFX);
                }

                StartCoroutine(quietBall);
                hasReset = true;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (InteractingWithParticles) { return; }

            Vector3 dir = other.contacts[0].point - transform.position;
            dir = -dir.normalized;

            var force = other.impulse.magnitude >= 1 ? other.impulse.magnitude : 1;

            if (IsNotQuiet)
            {
                if (other.gameObject.CompareTag("RightHand") || other.gameObject.CompareTag("LeftHand"))
                {
                    touched = true;

                    if (!touchResponseLimiter)
                    {
                        //touched = true;
                        TouchLevel += 1;
                        DetermineTouchResponse(other);
                        osc.Send(Note.ToString(), TouchLevel);
                        touchResponseLimiter = true;
                    }

                    if (hasBounce && State == BallState.Active)
                    {
                        rigidbody.AddForce(dir * force * bounce);
                    }
                }

                if (other.gameObject.CompareTag("Ball"))
                {
                    BallCollision = true;
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
                    director.NextWorldLevel();
                    StartCoroutine(destroySelf);
                }

                if (tracking.rightPose == HandPose.fist)
                {
                    Note = Notes.rFist;
                    NoteColor = Color.HSVToRGB(0, 1, 0.8f); // red
                    notePlayer.PlayNote(0);
                }
                else if (tracking.rightPose != HandPose.fist)
                {
                    if (tracking.rightPose == HandPose.pointer)
                    {
                        Note = Notes.rPointer;
                        NoteColor = Color.HSVToRGB(0.66f, 0.58f, 0.8f); // baby blue
                        notePlayer.PlayNote(1);

                    }
                    else if (tracking.rightPose == HandPose.peace)
                    {
                        Note = Notes.rPeace;
                        NoteColor = Color.HSVToRGB(0.29f, 0.58f, 0.5f); // light green
                        notePlayer.PlayNote(2);

                    }
                    else if (tracking.rightPose == HandPose.thumbsUp)
                    {
                        if (ModeToggleTimer > modeToggleSpacer)
                        {
                            Still = !Still;
                            ModeToggleTimer = 0;
                            ModeToggled = true;
                        }
                    }
                    else if (other.gameObject.name == "Backhand")
                    {
                        Note = Notes.rBack;
                        NoteColor = Color.HSVToRGB(0.29f, 1, 0.8f); // green
                        notePlayer.PlayNote(3);

                    }
                    else
                    {
                        Note = Notes.rFore;
                        NoteColor = Color.HSVToRGB(0.66f, 1, 0.8f); // blue
                        notePlayer.PlayNote(4);

                    }
                }
            }
            else
            {
                //if (jedi.LevelUpTimer < 2 && tracking.leftPose == HandPose.fist)
                //{
                //    osc.Send("LevelUp!");
                //    director.NextWorldLevel();
                //    StartCoroutine(destroySelf);
                //}

                if (tracking.leftPose == HandPose.fist)
                {
                    Note = Notes.lFist;
                    NoteColor = Color.HSVToRGB(0.15f, 1, 0.8f); // yellow
                    notePlayer.PlayNote(5);

                }
                else if (tracking.leftPose != HandPose.fist)
                {
                    if (tracking.leftPose == HandPose.pointer)
                    {
                        Note = Notes.lPointer;
                        NoteColor = Color.HSVToRGB(0, 0.58f, 1); // light red
                        notePlayer.PlayNote(6);

                    }
                    else if (tracking.leftPose == HandPose.peace)
                    {
                        Note = Notes.lPeace;
                        NoteColor = Color.HSVToRGB(0.86f, 0.58f, 1); // pink
                        notePlayer.PlayNote(7);

                    }
                    else if (tracking.leftPose == HandPose.thumbsUp)
                    {
                        if (ModeToggleTimer > modeToggleSpacer)
                        {
                            Still = !Still;
                            ModeToggleTimer = 0;
                            ModeToggled = true;
                        }
                    }
                    else if (other.gameObject.name == "Backhand")
                    {
                        Note = Notes.lBack;
                        NoteColor = Color.HSVToRGB(0.89f, 1, 0.8f); // magenta
                        notePlayer.PlayNote(8);

                    }
                    else
                    {
                        Note = Notes.lFore;
                        NoteColor = Color.HSVToRGB(0.5f, 1, 0.8f); // cyan
                        notePlayer.PlayNote(9);

                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            touched = false;
            touchResponseLimiter = false;
            BallCollision = false;
        }

        public void KillBall(OscMessage message)
        {
            StartCoroutine(destroySelf);
        }

        IEnumerator DestroySelf()
        {
            State = BallState.Dead;
            IsNotQuiet = false;

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

            director.RemoveBall(Handedness);

            if (director.SharedExperience && GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("Destroying with Photon--");
                PhotonNetwork.Destroy(gameObject);
            }
            else 
            {
                Destroy(gameObject);
            }
        }

        IEnumerator QuietBall()
        {
            IsNotQuiet = false;
            yield return new WaitForSeconds(1);
            IsNotQuiet = true;
        }

        public void IsGazedAt()
        {
            if (director.Viewfinder && ModeToggleTimer > modeToggleSpacer)
            {
                Still = !Still;
                ModeToggleTimer = 0;
                ModeToggled = true;
            }
        }

        public void NotGazedAt()
        {
            //ModeToggled = false;
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


