using LW.Ball;
using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOsc : MonoBehaviour
{
    NewTracking tracking;
    CastOrigins origins;
    OSC osc;
    BallCaster caster;
    BallJedi jedi;

    bool holdOSC, frozenOSC, summonOSC, hasPushed, hasPulled, hasLifted, liftOSC, leftFisted, rightFisted, dualFisted, deadOSC;

    void Start()
    {
        tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
        origins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
        osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
        jedi = GetComponent<BallJedi>();
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
            if (!hasPushed)
            {
                Send("pushing");
                hasPushed = true;
            }
        } 
        else
        {
            hasPushed = false;
        }

        if (jedi.power == TheForce.pull)
        {
            if (!hasPulled)
            {
                Send("pulling");
                hasPushed = true;
            }
        }
        else
        {
            hasPulled = false;
        }

        if (jedi.power == TheForce.lift)
        {
            if (!hasLifted)
            {
                Send("lifting");
                hasLifted = true;
            }
        }
        else
        {
            hasLifted = false;
        }

    }

    public void Send(string address, float val = 1)
    {
        OscMessage message = new OscMessage();
        message.address = caster.WorldLevel + "/" + address + "/";
        message.values.Add(val);
        osc.Send(message);
    }
}
