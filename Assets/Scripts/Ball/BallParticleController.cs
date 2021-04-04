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
        
        public float CoreSize { get; set; }
        public float CoreLifetime { get; set; }
        public float CoreSpeed { get; set; }
        public float CoreHue { get; set; }
        public float CoreSat { get; set; }
        public float CoreVal { get; set; }

        Ball ball;
        ParticleSystem innerParticles;
        BallJedi jedi;
        NewTracking tracking;
        CastOrigins origins;
        Light light;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            light = GetComponentInChildren<Light>();

            CoreSize = 0.5f;
            CoreLifetime = 0.5f;
            CoreSpeed = 0.5f;
            CoreHue = 1f;
            CoreSat = 1f;
            CoreVal = 1;
        }

        void Update()
        {
            var main = innerParticles.main;
            var emission = innerParticles.emission;

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
                        CoreSize = jedi.RelativeHandDist;
                    }
                    if (jedi.ControlPose == HandPose.thumbsUp)
                    {
                        emission.rateOverTime = jedi.RelativeHandDist * 500;
                    }
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

            emission.enabled = ball.State == BallState.Still || ball.CoreActive;
            light.enabled = ball.State == BallState.Active;

            main.simulationSpace = ball.State == BallState.Active ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;

            main.startSize = ball.State == BallState.Active ? 0.1f : CoreSize * maxSize;
            main.startSpeed = ball.State == BallState.Still ? 0.26f : CoreSpeed * maxSpeed;
            main.startLifetime = ball.State == BallState.Still ? 1.5f : CoreLifetime * maxLifetime;

            Color color = Color.HSVToRGB(CoreHue, CoreSat, CoreVal);
            main.startColor = ball.State == BallState.Active ? ball.NoteColor : color;
            light.color = ball.State == BallState.Active ? ball.NoteColor : color;
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

