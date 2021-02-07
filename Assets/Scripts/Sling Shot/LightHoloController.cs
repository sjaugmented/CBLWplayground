using System.Collections;
using System.Collections.Generic;
using LW.Core;
using UnityEngine;

namespace LW.SlingShot
{
	public class LightHoloController : MonoBehaviour
	{
		private HandTracking hands;
		private bool lassoPrimed;

		void Start()
		{
			hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
		}

	    void Update()
	    {
		    if (hands.palmsOut && hands.rightOpen && hands.leftOpen)
		    {
			    ThrowHolo();
		    }

		    if (hands.palmsIn && !hands.rightFist && !hands.leftFist)
		    {
			    lassoPrimed = true;

			    if (hands.rightFist && hands.leftFist)
			    {
				    LassoHolo();
			    }
		    }
		    else
		    {
			    lassoPrimed = false;
		    }
	    }

	    private void ThrowHolo()
	    {
		    Vector3 castOrigin = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);
		    Quaternion castRotation = Quaternion.Slerp(hands.rightPalm.Rotation, hands.leftPalm.Rotation, 0.5f) *
		                              Quaternion.Euler(60, 0, 0);


	    }

	    private void LassoHolo()
	    {
		    throw new System.NotImplementedException();
	    }
	}
	
}
