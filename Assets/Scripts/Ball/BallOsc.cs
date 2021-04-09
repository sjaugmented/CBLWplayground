using LW.Ball;
using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallOsc : MonoBehaviour
    {
        [SerializeField] string killCode;
        [SerializeField] string glitterCode;

        NewTracking tracking;
        CastOrigins origins;
        OSC osc;
        BallCaster caster;
        BallJedi jedi;
        Ball ball;
        BallParticleController particles;

        bool holdToggle, activeToggle, stillToggle, pointerToggle, rightFisted, dualFisted, thumbsUpped, withinRange;

        private void Awake()
        {
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        }

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
            jedi = GetComponent<BallJedi>();
            ball = GetComponent<Ball>();
            particles = GetComponent<BallParticleController>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, ball.KillBall);
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(particles.GlitterBall);

            Send("iAM!");
        }

        void Update()
        {
            if (!ball.HasSpawned) { return; }

            if (jedi.Held)
            {
                if (!holdToggle)
                {
                    Send("holdingOn");
                    holdToggle = true;
                }
            }
            else
            {
                if (holdToggle)
                {
                    Send("holdingOff");
                    holdToggle = false;
                }
            }

            if (ball.State == BallState.Active)
            {
                if (!activeToggle)
                {
                    Send("activeMode");
                    activeToggle = true;
                }
            }
            else
            {
                if (activeToggle)
                {
                    Send("stillMode");
                    activeToggle = false;
                }
            }

            if (jedi.ControlPose == HandPose.pointer)
            {
                if (!pointerToggle)
                {
                    Send("pointerControlOn");
                    pointerToggle = true;
                }
                if (ball.WithinRange)
                {
                    Send("pointerControlAngle", 1 - tracking.StaffUp / 180);
                    Send("pointerControlDistance", jedi.RelativeHandDist);
                }
            }
            else
            {
                if (pointerToggle)
                {
                    Send("pointerControlOff");
                    pointerToggle = false;
                }
            }

            if (jedi.ControlPose == HandPose.fist)
            {
                if (!dualFisted)
                {
                    Send("fistControlOn");
                    dualFisted = true;
                }
                if (ball.WithinRange)
                {
                    Send("fistControlAngle", 1 - tracking.StaffUp / 180);
                    Send("fistControlDistance", jedi.RelativeHandDist);
                }
            }
            else
            {
                if (dualFisted)
                {
                    Send("fistControlOff");
                    dualFisted = false;
                }
            }

            if (jedi.ControlPose == HandPose.thumbsUp)
            {
                if (!thumbsUpped)
                {
                    Send("thumbsUpControlOn");
                    thumbsUpped = true;
                }
                if (ball.WithinRange)
                {
                    Send("thumbsUpControlAngle", 1 - tracking.StaffUp / 180);
                    Send("thumbsUpControlDistance", jedi.RelativeHandDist);
                }
            }
            else
            {
                if (thumbsUpped)
                {
                    Send("fistControlOff");
                    thumbsUpped = false;
                }
            }

            if (jedi.Power == TheForce.push)
            {
                Send("pushing");
            }

            if (jedi.Power == TheForce.pull)
            {
                Send("pulling");
            }

            if (jedi.Power == TheForce.lift)
            {
                Send("lifting");
            }

            if (jedi.Power == TheForce.spin)
            {
                Send("spinning");
            }
        }

        public void Send(string address = "", float val = 1)
        {
            OscMessage message = new OscMessage();
            message.address = caster.WorldLevel + "/" + ball.State + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }
    }
}

