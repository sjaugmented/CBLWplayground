using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallParticleController : MonoBehaviour
    {
        Ball ball;
        ParticleSystem innerParticles;
        BallJedi jedi;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
        }

        void Update()
        {
            var main = innerParticles.main;
            var emission = innerParticles.emission;
            
            main.startLifetime = jedi.Held ? 3 : 1.15f;
            main.startSpeed = jedi.Held ? 1f : 0.25f;
            emission.enabled = !jedi.Frozen && ball.CoreActive;
            
            Light light = GetComponentInChildren<Light>();
            light.enabled = !jedi.Frozen && ball.CoreActive;

            main.startColor = Color.HSVToRGB(ball.Hue, 1, 1);
            light.color = Color.HSVToRGB(ball.Hue, 1, 1);
        }

        public void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

