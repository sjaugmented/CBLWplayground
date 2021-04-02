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
        public float CoreHue { get; set; }
        public float CoreSat { get; set; }

        Ball ball;
        ParticleSystem innerParticles;
        BallJedi jedi;
        CastOrigins origins;
        Light light;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            light = GetComponentInChildren<Light>();

            CoreDensity = 1;
            CoreHue = 0;
            CoreSat = 1;
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
                }
                if (jedi.ControlPose == HandPose.peace)
                {
                    CoreSat = origins.PalmsDist / jedi.HoldDistance;
                }
                if (jedi.ControlPose == HandPose.fist)
                {
                    CoreDensity = origins.PalmsDist / jedi.HoldDistance;
                }
            }
            

            emission.enabled = !jedi.Frozen && ball.CoreActive;
            light.enabled = !jedi.Frozen && ball.CoreActive;
            
            main.startSpeed = CoreDensity * maxSpeed;
            main.startSize = CoreDensity * maxSize;
            main.startLifetime = CoreDensity * maxLifetime;
            main.startColor = Color.HSVToRGB(CoreHue, CoreSat, 1);
            light.color = Color.HSVToRGB(CoreHue, CoreSat, 1);
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

