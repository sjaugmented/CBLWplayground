using System.Collections;
using System.Collections.Generic;
using LW.Core;
using UnityEngine;

namespace LW.SlingShot
{
	public class LightHoloController : MonoBehaviour
	{
		[SerializeField] float maxHandDist = 0.5f;
		[SerializeField] bool devMode = false;
		[Range(100, 500)] public float force;
		
		HandTracking hands;
		private CastOrigins castOrigins;
		public bool holoOut = false;
		public bool lassoPrimed = false;
		public bool recall = false;
		private Vector3 lassoOrigin;

		void Start()
		{
			hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
			castOrigins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
		}

	    void Update()
	    {
		    //currentPos = transform.position;
		    
		    if (hands.palmsOut && hands.rightOpen && hands.leftOpen && !holoOut)
		    {
			    ThrowHolo();
		    }

		    if (hands.palmsIn && !hands.rightFist && !hands.leftFist)
		    {
			    lassoPrimed = true;
		    }
		    else
		    {
			    lassoPrimed = false;
		    }

		    if (lassoPrimed && hands.rightFist && hands.leftFist)
		    {
			    recall = true;
		    }

		    if (recall && holoOut)
		    {
			    lassoOrigin = Camera.main.transform.position - new Vector3(0, 0.3f, 0);
			    transform.LookAt(lassoOrigin);

			    float distToOrigin = Vector3.Distance(transform.position, lassoOrigin);

			    if (distToOrigin > 0.2)
			    {
					GetComponent<Rigidbody>().AddForce((transform.forward * 10));
			    }
			    else
			    {
				    Component[] childRends = GetComponentsInChildren<Renderer>();
				    foreach (Renderer rend in childRends)
				    {
					    rend.enabled = false;
				    }
				    transform.position = lassoOrigin;
				    
				    recall = false;
				    holoOut = false;
			    }
		    }

		    #region DEV CONTROLS
		    if (devMode)
		    {
			    force += Input.mouseScrollDelta.y;
			    if (Input.GetKeyDown(KeyCode.Z))
			    {
				   if( !holoOut) ThrowHolo();
			    }

			    if (Input.GetKeyDown(KeyCode.G))
			    {
				    if (holoOut)
				    {
						Debug.Log("lasso-ing");
					    recall = true;

				    }
			    }

		    }
		    #endregion
		}

	    private void ThrowHolo()
	    {
			Vector3 castOrigin = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);
		    Quaternion castRotation = Quaternion.Slerp(hands.rightPalm.Rotation, hands.leftPalm.Rotation, 0.5f) *
		                              Quaternion.Euler(60, 0, 0);
		    transform.position = castOrigin;
		    transform.rotation = castRotation;
		    
		    Component[] childRends = GetComponentsInChildren<Renderer>();
			foreach (Renderer rend in childRends)
			{
				rend.enabled = true;
			}

			if (devMode)
			{
				transform.position = lassoOrigin;
				transform.rotation = Camera.main.transform.rotation;
			}
			else
			{
				transform.position = castOrigin;
				transform.rotation = castRotation;
				force = (1 - (castOrigins.palmDist / maxHandDist)) * 500;
			}

			GetComponent<Rigidbody>().AddForce(transform.forward * Mathf.Clamp(force, 100, 500));

			holoOut = true;
	    }
	}
	
}
