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

        //NewTracking tracking;
        //CastOrigins origins;
        OSC osc;
        //BallDirector director;
        Ball ball;
        BallJedi jedi;
        BallParticleController particles;

        bool holdToggle, activeToggle, stillToggle, pointerToggle, rightFisted, dualFisted, thumbsUpped, withinRange, pushed, pulled, lifted, lowered, spun, recalled;

        private void Awake()
        {
            osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        }

        void Start()
        {
            //tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            //origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            //director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
            ball = GetComponent<Ball>();
            jedi = GetComponent<BallJedi>();
            particles = GetComponent<BallParticleController>();

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(killCode, ball.KillBall);
            //GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(glitterCode, GlitterBall);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(particles.GlitterBall);

            //Send("iAM!");
        }

        void Update()
        {
            string ballId = ball.DominantHand == NewTracking.Instance.GetRtPalm ? "rightBall" : "leftBall";
            if (BallDirector.Instance.SendCoordinates && jedi.Moving)
            {
                SendClean(ballId + "/WorldSpace/X/", transform.position.x);
                SendClean(ballId + "/WorldSpace/Y/", transform.position.y);
                SendClean(ballId + "/WorldSpace/Z/", transform.position.z);
            }

            if (!ball.IsNotQuiet) { return; }

            if (jedi.Held)
            {
                Send("HoldDist/" + jedi.HoldPose, 1 - Mathf.Clamp(jedi.RelativeHandDist, 0, 1));
                Send("HoldAng/" + jedi.HoldPose, NewTracking.Instance.StaffRight / 90);
                //if (!holdToggle)
                //{
                //    Send("holdingOn");
                //    holdToggle = true;
                //}
            }
            else
            {
                //if (holdToggle)
                //{
                //    Send("holdingOff");
                //    holdToggle = false;
                //}
            }

            if (jedi.Moving)
            {
                Send("moving", ball.Distance);
            }

            if (ball.State == BallState.Active) // ACTIVE
            {
                //if (!activeToggle)
                //{
                //    Send("activeMode");
                //    activeToggle = true;
                //}

                if (ball.BallCollision)
                {
                    Send("ballsCollided");
                }
            }
            else // STILL
            {
                //if (activeToggle)
                //{
                //    Send("stillMode");
                //    activeToggle = false;
                //}

                //if (jedi.ControlPose == HandPose.pointer)
                //{
                //    if (!pointerToggle)
                //    {
                //        Send("pointerControlOn");
                //        pointerToggle = true;
                //    }
                //    if (ball.WithinRange)
                //    {
                //        //Send("pointerControlAngle", 1 - tracking.StaffUp / 180);
                //        Send("pointerControlDistance", jedi.RelativeHandDist);
                //    }
                //}
                //else
                //{
                //    if (pointerToggle)
                //    {
                //        Send("pointerControlOff");
                //        pointerToggle = false;
                //    }
                //}

                //if (jedi.ControlPose == HandPose.fist)
                //{
                //    if (!dualFisted)
                //    {
                //        Send("fistControlOn");
                //        dualFisted = true;
                //    }
                //    if (ball.WithinRange)
                //    {
                //        //Send("fistControlAngle", 1 - tracking.StaffUp / 180);
                //        Send("fistControlDistance", jedi.RelativeHandDist);
                //    }
                //}
                //else
                //{
                //    if (dualFisted)
                //    {
                //        Send("fistControlOff");
                //        dualFisted = false;
                //    }
                //}

                //if (jedi.ControlPose == HandPose.thumbsUp)
                //{
                //    if (!thumbsUpped)
                //    {
                //        Send("thumbsUpControlOn");
                //        thumbsUpped = true;
                //    }
                //    if (ball.WithinRange)
                //    {
                //        //Send("thumbsUpControlAngle", 1 - tracking.StaffUp / 180);
                //        Send("thumbsUpControlDistance", jedi.RelativeHandDist);
                //    }
                //}
                //else
                //{
                //    if (thumbsUpped)
                //    {
                //        Send("fistControlOff");
                //        thumbsUpped = false;
                //    }
                //}
            }
            

            //if (jedi.Primary == Force.push)
            //{
            //    if (!pushed)
            //    {
            //        Send("pushing");
            //        pushed = true;
            //    }
            //}
            //else
            //{
            //    pushed = false;
            //}
            

            //if (jedi.Primary == Force.pull)
            //{
            //    if (!pulled)
            //    {
            //        Send("pulling");
            //        pulled = true;
            //    }
            //}
            //else
            //{
            //    pulled = false;
            //}

            //if (jedi.Power == TheForce.lift)
            //{
            //    if (!lifted)
            //    {
            //        Send("lifting");
            //        lifted = true;
            //    }
            //}
            //else
            //{
            //    lifted = false;
            //}

            if (jedi.Spin)
            {
                if (!spun)
                {
                    Send("spinningDist", 1 - jedi.RelativeHandDist);
                    Send("spinningAngle", NewTracking.Instance.StaffRight / 90);
                    spun = true;
                }
            }
            else
            {
                spun = false;
            }

            if (jedi.Recall)
            {
                if (!recalled)
                {
                    Send("recalled");
                    recalled = true;
                }
            }
            else
            {
                recalled = false;
            }

            if (ball.State == BallState.Dead)
            {
                //Send("iDead");
            }

        }

        public void Send(string address = "", float val = 1)
        {
            OscMessage message = new OscMessage();
            message.address = BallDirector.Instance.WorldLevel + "/" + ball.Handedness + "/" + address + "/";
            message.values.Add(val);
            osc.Send(message);
        }
        
        public void SendClean(string address = "", float val = 1)
        {
            OscMessage message = new OscMessage();
            message.address = address + "/";
            message.values.Add(val);
            osc.Send(message);
        }
    }
}

