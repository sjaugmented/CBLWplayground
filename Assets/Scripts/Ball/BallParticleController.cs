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
        BallJedi jedi;
        void Start()
        {
            ball = GetComponent<Ball>();
            innerParticles = GetComponentInChildren<ParticleSystem>();
            jedi = GetComponentInParent<BallJedi>();
            
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(GlitterBall);
        }

        void Update()
        {
            var main = innerParticles.main;
            var emission = innerParticles.emission;
            
            main.startLifetime = jedi.Held ? 3 : 1.15f;
            main.startSpeed = jedi.Held ? 1f : 0.25f;
            emission.enabled = !jedi.Frozen;
            
            Light light = GetComponentInChildren<Light>();
            light.enabled = !jedi.Frozen;

            main.startColor = Color.HSVToRGB(ball.Hue, 1, 1);
            light.color = Color.HSVToRGB(ball.Hue, 1, 1);
        }

        void GlitterBall(OscMessage message)
        {
            SendMessage("GlitterBall");
            GetComponentInChildren<GlitterParticlesId>().GetComponent<ParticleSystem>().Play();
        }
    }
}

