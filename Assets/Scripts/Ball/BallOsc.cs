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

    bool holdOSC, frozenOSC, leftFisted, rightFisted, dualFisted;

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

        if (jedi.fists == Hands.left)
        {
            if (!leftFisted)
            {
                Send("leftFistOn");
                leftFisted = true;
            }

            Send("leftProximityAngle", 1 - tracking.StaffUp / 180);
            Send("leftProximityDistance", origins.PalmsDist / jedi.HoldDistance);
        }
        else
        {
            if (leftFisted)
            {
                Send("leftFistOff");
            }
            leftFisted = false;
        }

        if (jedi.fists == Hands.right)
        {
            if (!rightFisted)
            {
                Send("rightFistOn");
                rightFisted = true;
            }

            Send("rightProximityAngle", 1 - tracking.StaffUp / 180);
            Send("rightProximityDistance", origins.PalmsDist / jedi.HoldDistance);
        } 
        else
        {
            if (rightFisted)
            {
                Send("rightFistOff");
            }
            rightFisted = false;
        }

        if (jedi.fists == Hands.both)
        {
            if (!dualFisted)
            {
                Send("dualFistOn");
                dualFisted = true;
            }

            Send("proximityAngle", 1 - tracking.StaffUp / 180);
            Send("proximityDistance", origins.PalmsDist / jedi.HoldDistance);
        } 
        else
        {
            if (dualFisted)
            {
                Send("dualFistOff");
            }
            dualFisted = false;
        }

        if (jedi.power == TheForce.push)
        {
            Send("pushing");
        } 

        if (jedi.power == TheForce.pull)
        {
            Send("pulling");
        }

        if (jedi.power == TheForce.lift)
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
