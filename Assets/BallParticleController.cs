using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallParticleController : MonoBehaviour
    {
        [SerializeField] string glitterCode;

        Ball ball;
        ParticleSystem innerParticles;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<ParticleSystem>();
            
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(GlitterBall);
        }

        void Update()
        {
            var main = innerParticles.main;
            var emission = innerParticles.emission;
            
            main.startLifetime = ball.IsHeld ? 3 : 1.15f;
            main.startSpeed = ball.IsHeld ? 1f : 0.25f;
            emission.enabled = ball.IsAlive || ball.IsFrozen;
            
            Light light = GetComponentInChildren<Light>();
            light.enabled = !ball.IsFrozen;

            if (ball.IsAlive)
            {
                main.startColor = Color.HSVToRGB(ball.Hue, 1, 1);
                light.color = Color.HSVToRGB(ball.Hue, 1, 1);
            } 
        }

        void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

