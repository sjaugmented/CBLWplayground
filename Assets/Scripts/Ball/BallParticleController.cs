using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallParticleController : MonoBehaviour
    {
        [SerializeField] float maxLifetime = 2;
        [SerializeField] float maxSize = 0.25f;
        [SerializeField] float maxSpeed = 1;
        [SerializeField] float maxParticles = 500;

        public float CoreSize { get; set; }
        public float CoreLifetime { get; set; }
        public float CoreSpeed { get; set; }
        public float CoreHue { get; set; }
        public float CoreSat { get; set; }
        public float CoreVal { get; set; }
        public float CoreEmission { get; set; }

        Ball ball;
        BallJedi jedi;
        NewTracking tracking;
        CastOrigins origins;
        ParticleSystem innerParticles;
        ParticleSystem forceParticles;
        ParticleSystem liftParticles;
        ParticleSystem spinParticles;
        Light light;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<CoreParticlesID>().transform.GetComponent<ParticleSystem>();
            forceParticles = GetComponentInChildren<ForceParticlesID>().transform.GetComponent<ParticleSystem>();
            liftParticles = GetComponentInChildren<LiftParticlesID>().transform.GetComponent<ParticleSystem>();
            spinParticles = GetComponentInChildren<SpinParticlesID>().transform.GetComponent<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            light = GetComponentInChildren<Light>();

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
            var innerEmission = innerParticles.emission;
            var forceEmission = forceParticles.emission;
            var liftEmission = liftParticles.emission;
            var spinEmission = spinParticles.emission;

            if (ball.WithinRange)
            {
                if (ball.State == BallState.Still)
                {
                    if (jedi.ControlPose == HandPose.pointer)
                    {
                        CoreHue = jedi.RelativeHandDist;
                    }
                    if (jedi.ControlPose == HandPose.fist)
                    {
                        CoreSize = 1 - jedi.RelativeHandDist;
                        CoreSpeed = jedi.RelativeHandDist;
                        CoreLifetime = jedi.RelativeHandDist;
                    }
                    //if (jedi.ControlPose == HandPose.thumbsUp)
                    //{
                    //    CoreEmission = jedi.RelativeHandDist;
                    //}
                }
                //else
                //{
                //    if (jedi.ControlPose == HandPose.pointer)
                //    {
                //        CoreHue = jedi.RelativeHandDist;
                //    }
                //    if (jedi.ControlPose == HandPose.fist)
                //    {
                //        CoreSat = jedi.RelativeHandDist;
                //        CoreLifetime = jedi.RelativeHandDist;
                //    }
                //    if (jedi.ControlPose == HandPose.thumbsUp)
                //    {
                //        CoreVal = jedi.RelativeHandDist;
                //        CoreSpeed = jedi.RelativeHandDist;
                //    }
                //}
            }

            //innerMain.simulationSpace = ball.State == BallState.Active ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;

            innerEmission.enabled = ball.State == BallState.Still || ball.CoreActive;
            innerEmission.rateOverTime = ball.State == BallState.Active ? maxParticles / 2 : CoreEmission * maxParticles;
            innerMain.startSize = ball.State == BallState.Active ? maxSize / 2 : CoreSize * maxSize;
            innerMain.startSpeed = ball.State == BallState.Active ? maxSpeed / 2 : CoreSpeed * maxSpeed;
            innerMain.startLifetime = ball.State == BallState.Active ? maxLifetime / 2 : CoreLifetime * maxLifetime;
            
            Color color = Color.HSVToRGB(CoreHue, CoreSat, CoreVal);
            innerMain.startColor = ball.State == BallState.Active ? ball.NoteColor : color;

            forceEmission.enabled = ball.State == BallState.Active && (jedi.Power == TheForce.push || jedi.Power == TheForce.pull);
            liftEmission.enabled = ball.State == BallState.Active && (jedi.Power == TheForce.lift || jedi.Power == TheForce.down);
            spinEmission.enabled = jedi.Power == TheForce.spin;

            light.enabled = ball.CoreActive; ;
            light.color = ball.NoteColor;
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

