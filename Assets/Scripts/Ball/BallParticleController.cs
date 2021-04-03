using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallParticleController : MonoBehaviour
    {
        [SerializeField] float maxLifetime = 3;
        [SerializeField] float maxSize = 0.5f;
        [SerializeField] float maxSpeed = 1;

        public float CoreDensity { get; set; }
        public float CoreIntensity { get; set; }
        public float CoreHue { get; set; }
        public float CoreSat { get; set; }

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

            CoreDensity = 0.5f;
            CoreIntensity = 0.5f;
            CoreHue = 0.5f;
            CoreSat = 0.5f;
        }

        void Update()
        {
            var main = innerParticles.main;
            var emission = innerParticles.emission;

            if (ball.WithinRange)
            {
                
                if (jedi.ControlPose == HandPose.pointer)
                {
                    CoreHue = origins.PalmsDist / jedi.HoldDistance;
                    CoreSat = tracking.StaffUp / 180;
                }
                if (jedi.ControlPose == HandPose.fist)
                {
                    CoreDensity = origins.PalmsDist / jedi.HoldDistance;
                    CoreIntensity = tracking.StaffUp / 180;
                }
            }

            emission.enabled = ball.State == BallState.Still || ball.CoreActive;
            light.enabled = ball.State == BallState.Active;

            main.simulationSpace = ball.State == BallState.Active ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;

            main.startSize = CoreDensity * maxSize;

            main.startSpeed = ball.State == BallState.Still ? 0.26f : CoreIntensity * maxSpeed;
            main.startLifetime = ball.State == BallState.Still ? 1.5f : CoreIntensity * maxLifetime;
            main.startColor = Color.HSVToRGB(CoreHue, CoreSat, 1);
            light.color = Color.HSVToRGB(CoreHue, 1, CoreSat);
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

