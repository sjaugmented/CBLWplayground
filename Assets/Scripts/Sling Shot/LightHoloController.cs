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
		[Range(10, 200)] float force;
		
		HandTracking hands;
		public bool holoOut = false;
		public bool lassoPrimed = false;
		public bool recall = false;
		//private Vector3 currentPos;

		void Start()
		{
			hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
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
			    Vector3 lassoOrigin = Camera.main.transform.position - new Vector3(0, 0.3f, 0);
			    transform.LookAt(lassoOrigin);
			    if (transform.position != lassoOrigin)
			    {
					GetComponent<Rigidbody>().AddForce((transform.forward * 5));
			    }
			    else
			    {
				    recall = false;
				    Component[] childRends = GetComponentsInChildren<Renderer>();
				    foreach (Renderer rend in childRends)
				    {
					    rend.enabled = false;
				    }
			    }
		    }

		    #region DEV CONTROLS
		    if (devMode)
		    {
			    force += Input.mouseScrollDelta.y;
			    if (Input.GetKeyDown(KeyCode.Z))
			    {
					ThrowHolo();
			    }

			    if (Input.GetKeyDown(KeyCode.G))
			    {
				    recall = true;
			    }

		    }
		    

		    #endregion
		}

	    private void ThrowHolo()
	    {
			Component[] childRends = GetComponentsInChildren<Renderer>();
			foreach (Renderer rend in childRends)
			{
				rend.enabled = true;
			}

			Vector3 castOrigin = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);
		    Quaternion castRotation = Quaternion.Slerp(hands.rightPalm.Rotation, hands.leftPalm.Rotation, 0.5f) *
		                              Quaternion.Euler(60, 0, 0);

			GetComponent<Rigidbody>().AddForce(transform.forward * force);

			holoOut = true;
	    }
	}
	
}
