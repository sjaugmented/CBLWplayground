using LW.Core;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace LW.Ball
{
    public class BallParticleController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] float maxLifetime = 2;
        [SerializeField] float maxSize = 0.25f;
        [SerializeField] float maxSpeed = 1;
        [SerializeField] float maxParticles = 500;
        [SerializeField] float maxSpinParticlesSpeed = 10;

        public float CoreSize { get; set; }
        public float CoreLifetime { get; set; }
        public float CoreSpeed { get; set; }
        public float CoreHue { get; set; }
        public float CoreSat { get; set; }
        public float CoreVal { get; set; }
        public float CoreEmission { get; set; }

        Ball ball;
        BallJedi jedi;
        ParticleSystem innerParticles;
        ParticleSystem forceParticles;
        ParticleSystem spinParticles;
        ParticleSystem collisionParticles;

        #region Photon

        bool core, spin, force, col;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CoreHue);
                stream.SendNext(CoreSat);
                stream.SendNext(CoreVal);
                stream.SendNext(core);
                stream.SendNext(spin);
                stream.SendNext(force);
                stream.SendNext(col);
            }
            else
            {
                CoreHue = (float)stream.ReceiveNext();
                CoreSat = (float)stream.ReceiveNext();
                CoreVal = (float)stream.ReceiveNext();
                core = (bool)stream.ReceiveNext();
                spin = (bool)stream.ReceiveNext();
                force = (bool)stream.ReceiveNext();
                col = (bool)stream.ReceiveNext();
            }
        }

        #endregion
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<CoreParticlesID>().transform.GetComponent<ParticleSystem>();
            forceParticles = GetComponentInChildren<ForceParticlesID>().transform.GetComponent<ParticleSystem>();
            spinParticles = GetComponentInChildren<SpinParticlesID>().transform.GetComponent<ParticleSystem>();
            collisionParticles = GetComponentInChildren<CollisionParticlesID>().transform.GetComponent<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();

            CoreSize = 0.9f;
            CoreLifetime = 0.1f;
            CoreSpeed = 0.1f;
            CoreHue = 1f;
            CoreSat = 1f;
            CoreVal = 1;
            CoreEmission = 1f;
        }

        void Update()
        {
            var innerMain = innerParticles.main;
            var coreEmission = innerParticles.emission;
            var forceEmission = forceParticles.emission;
            var spinEmission = spinParticles.emission;
            var spinMain = spinParticles.main;
            var colEmission = collisionParticles.emission;

            if (photonView.IsMine)
            {
                // Process particle emission states
                core = ball.State == BallState.Active && ball.CoreActive;
                force = jedi.Primary == Force.push || jedi.Primary == Force.pull;
                spin = jedi.Spin;
                col = ball.State == BallState.Active && ball.BallCollision;
            }
            
            // Assign emission states
            coreEmission.enabled = core;
            forceEmission.enabled = force;
            spinEmission.enabled = spin;
            colEmission.enabled = col;
            coreEmission.rateOverTime = maxParticles;
            spinMain.startSpeed = new ParticleSystem.MinMaxCurve(2f, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0, 1)) * maxSpinParticlesSpeed);

            Color color = Color.HSVToRGB(CoreHue, CoreSat, CoreVal);
            innerMain.startColor = ball.State == BallState.Active ? ball.NoteColor : color;
            innerMain.startSize = maxSize;
            innerMain.startSpeed = maxSpeed;
            innerMain.startLifetime = maxLifetime;

            #region // Alts (deprecated for now)
            //innerEmission.rateOverTime = ball.State == BallState.Active ? maxParticles / 2 : CoreEmission * maxParticles;
            //innerMain.startSize = ball.State == BallState.Active ? maxSize / 2 : CoreSize * maxSize;
            //innerMain.startSpeed = ball.State == BallState.Active ? maxSpeed / 2 : CoreSpeed * maxSpeed;
            //innerMain.startLifetime = ball.State == BallState.Active ? maxLifetime / 2 : CoreLifetime * maxLifetime;
            //forceEmission.enabled = ball.State == BallState.Active && (jedi.Primary == Force.push || jedi.Primary == Force.pull);
            //liftEmission.enabled = ball.State == BallState.Active && (jedi.Power == TheForce.lift || jedi.Power == TheForce.down);
            #endregion
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

