using LW.Core;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //NewTracking tracking;
        //CastOrigins origins;
        ParticleSystem innerParticles;
        ParticleSystem forceParticles;
        ParticleSystem spinParticles;
        ParticleSystem collisionParticles;

        #region Photon

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this ball so send the others our data
                //stream.SendNext(CoreSize);
                //stream.SendNext(CoreLifetime);
                //stream.SendNext(CoreSpeed);
                stream.SendNext(CoreHue);
                stream.SendNext(CoreSat);
                stream.SendNext(CoreVal);
                //stream.SendNext(CoreEmission);
            }
            else
            {
                // Network ball, receive data
                CoreHue = (float)stream.ReceiveNext();
                CoreSat = (float)stream.ReceiveNext();
                CoreVal = (float)stream.ReceiveNext();
            }
        }

        #endregion
        void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<CoreParticlesID>().transform.GetComponent<ParticleSystem>();
            forceParticles = GetComponentInChildren<ForceParticlesID>().transform.GetComponent<ParticleSystem>();
            spinParticles = GetComponentInChildren<SpinParticlesID>().transform.GetComponent<ParticleSystem>();
            collisionParticles = GetComponentInChildren<CollisionParticlesID>().transform.GetComponent<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
            //tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            //origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();

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
            if (!photonView.IsMine)
            {
                return;
            }

            var innerMain = innerParticles.main;
            var coreEmission = innerParticles.emission;
            var forceEmission = forceParticles.emission;
            var spinEmission = spinParticles.emission;
            var spinMain = spinParticles.main;
            var colEmission = collisionParticles.emission;

            colEmission.enabled = ball.State == BallState.Active && ball.BallCollision;


            //if (ball.WithinRange)
            //{
            //    if (ball.State == BallState.Still)
            //    {
            //        if (jedi.ControlPose == HandPose.pointer)
            //        {
            //            CoreHue = Mathf.Clamp(jedi.RelativeHandDist, 0, 0.9f);
            //        }
            //        if (jedi.ControlPose == HandPose.fist)
            //        {
            //            CoreSize = Mathf.Clamp(1 - jedi.RelativeHandDist, 0.1f, 1);
            //            CoreSpeed = Mathf.Clamp(jedi.RelativeHandDist, 0.1f, 1);
            //            CoreLifetime = Mathf.Clamp(jedi.RelativeHandDist, 0.1f, 1);
            //        }
            //    }
            //}

            coreEmission.enabled = ball.State == BallState.Active && ball.CoreActive;
            //innerEmission.rateOverTime = ball.State == BallState.Active ? maxParticles / 2 : CoreEmission * maxParticles;
            coreEmission.rateOverTime = maxParticles;
            //innerMain.startSize = ball.State == BallState.Active ? maxSize / 2 : CoreSize * maxSize;
            innerMain.startSize = maxSize;
            //innerMain.startSpeed = ball.State == BallState.Active ? maxSpeed / 2 : CoreSpeed * maxSpeed;
            innerMain.startSpeed = maxSpeed;
            //innerMain.startLifetime = ball.State == BallState.Active ? maxLifetime / 2 : CoreLifetime * maxLifetime;
            innerMain.startLifetime = maxLifetime;
            
            Color color = Color.HSVToRGB(CoreHue, CoreSat, CoreVal);
            innerMain.startColor = ball.State == BallState.Active ? ball.NoteColor : color;

            //forceEmission.enabled = ball.State == BallState.Active && (jedi.Primary == Force.push || jedi.Primary == Force.pull);
            forceEmission.enabled = jedi.Primary == Force.push || jedi.Primary == Force.pull;
            //liftEmission.enabled = ball.State == BallState.Active && (jedi.Power == TheForce.lift || jedi.Power == TheForce.down);
            spinEmission.enabled = jedi.Spin;
            spinMain.startSpeed = new ParticleSystem.MinMaxCurve(2f, (1 - Mathf.Clamp(jedi.RelativeHandDist, 0, 1)) * maxSpinParticlesSpeed);
        }

        public void GlitterBall(OscMessage message)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

