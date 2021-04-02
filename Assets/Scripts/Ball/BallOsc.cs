using LW.Ball;
using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool holdOSC, frozenOSC, leftFisted, rightFisted, dualFisted, withinRange;

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
        if (jedi.Held)
        {
            if (!holdOSC)
            {
                Send("holdingOn");
                holdOSC = true;
            }
        }
        else
        {
            if (holdOSC)
            {
                Send("holdingOff");
            }
            holdOSC = false;
        }

        if (jedi.Frozen)
        {
            if (!frozenOSC)
            {
                Send("frozenOn");
                frozenOSC = true;
            }
        } 
        else
        {
            if (frozenOSC)
            {
                Send("frozenOff");
            }
            frozenOSC = false;
        }

        if (jedi.ControlPose == HandPose.pointer)
        {
            if (!leftFisted)
            {
                Send("pointerControlOn");
                leftFisted = true;
            }
            if (ball.WithinRange)
            {
                Send("pointerControlAngle", 1 - tracking.StaffUp / 180);
                Send("pointerControlDistance", origins.PalmsDist / jedi.HoldDistance);
            }
        }
        else
        {
            if (leftFisted)
            {
                Send("pointerControlOff");
            }
            leftFisted = false;
        }

        if (jedi.ControlPose == HandPose.peace)
        {
            if (!rightFisted)
            {
                Send("peaceControlOn");
                rightFisted = true;
            }
            if (ball.WithinRange)
            {
                Send("peaceControlAngle", 1 - tracking.StaffUp / 180);
                Send("peaceControlDistance", origins.PalmsDist / jedi.HoldDistance);
            }
        } 
        else
        {
            if (rightFisted)
            {
                Send("peaceControlOff");
            }
            rightFisted = false;
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
                Send("fistControlDistance", origins.PalmsDist / jedi.HoldDistance);
            }
        } 
        else
        {
            if (dualFisted)
            {
                Send("fistControlOff");
            }
            dualFisted = false;
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
    }

    public void Send(string address = "default", float val = 1)
    {
        OscMessage message = new OscMessage();
        message.address = caster.WorldLevel + "/" + address + "/";
        message.values.Add(val);
        osc.Send(message);
    }
}
