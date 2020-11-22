using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.HoverDrums
{
    public class HoverDrummer : MonoBehaviour
    {
        [Header("DevMode controls")]
        [SerializeField] bool devMode = false;
        [SerializeField] [Range(10, 75)] float force = 10f;

        [Header("Controller Settings")]
        [SerializeField] float castDelay = 3f;
        [SerializeField] float maxXAxisDist = 0.5f; //TODO hardcode

        [Header("Prefabs")]
        [SerializeField] List<GameObject> drumVariants;
        
        List<float> colorVariants = new List<float>()
        {
            0, 
            0.15f, 
            0.3f, 
            0.45f, 
            0.6f, 
            0.8f
        };

        // stores live drums, for dev purposes only TODO make private
        public List<HoverDrumController> liveDrums = new List<HoverDrumController>();

        int drumsLeftToCast;
        int drumShape = 0;
        int drumColor = 0;

        HandTracking handtracking;
        CastOrigins castOrigins;
        float timeSinceLastCast = Mathf.Infinity;


        private void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            castOrigins = FindObjectOfType<CastOrigins>();
            drumsLeftToCast = drumVariants.Count * colorVariants.Count;
        }

        private void Update()
        {
            timeSinceLastCast += Time.deltaTime;

            if (drumsLeftToCast == 0) return;

            if (handtracking.palmsOut && handtracking.rightOpen && handtracking.leftOpen)
            {
                CastOrb();
            }

            if (handtracking.palmsIn && handtracking.rightFist && handtracking.leftFist)
            {
                Reset();
            }

            if (devMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CastOrb();
                }

                force += Input.mouseScrollDelta.y;

                if (Input.GetMouseButtonDown(2))
                {
                    Reset();
                }
            }
        }

        private void CastOrb()
        {
            Vector3 castOrigin = Vector3.Lerp(handtracking.rtMiddleKnuckle.Position, handtracking.ltMiddleKnuckle.Position, 0.5f);
            Quaternion handRotation = Quaternion.Slerp(handtracking.rightPalm.Rotation, handtracking.leftPalm.Rotation, 0.5f);
            Quaternion castRotation = handRotation * Quaternion.Euler(60, 0, 0); // rotational offset - so casts go OUT instead of UP along the hand.Z axis

            if (timeSinceLastCast >= castDelay)
            {
                timeSinceLastCast = 0;
                GameObject drum;

                if (devMode)
                {
                    drum = Instantiate(drumVariants[drumShape], Camera.main.transform.position, Camera.main.transform.rotation);
                }
                else
                {
                    drum = Instantiate(drumVariants[drumShape], castOrigin, castRotation);
                }
                
                HoverDrumController currentDrum = drum.GetComponent<HoverDrumController>();

                float spellForce = (1 - (castOrigins.palmDist / maxXAxisDist)) * 75;
                Debug.Log("spellForce: " + spellForce); //REMOVE
                if (spellForce < 10) spellForce = 10;
                // set drum casting force and color
                if (devMode) currentDrum.force = force;
                else currentDrum.force = spellForce;
                currentDrum.SetDrumColor(colorVariants[drumColor]);
                // add drum to list of live drums
                liveDrums.Add(currentDrum);
                
                SetNextColorOrShape();
            }
        }

        private void SetNextColorOrShape()
        {
            if (drumColor < colorVariants.Count - 1)
            {
                drumColor++;
                drumsLeftToCast--;
            }
            else
            {
                drumColor = 0;
                if (drumShape < drumVariants.Count - 1)
                {
                    drumShape++;
                }
                else return;
            }
        }

        private void Reset()
        {
            // clear all drums
            for (int i = 0; i < liveDrums.Count; i++)
            {
                StartCoroutine("DropAndDestroy", liveDrums[i]);
                liveDrums.Remove(liveDrums[i]);
            }
            
            // reset shape and color ints
            drumShape = 0;
            drumColor = 0;
        }

        private IEnumerator DropAndDestroy(HoverDrumController drum)
        {
            drum.GetComponent<Rigidbody>().useGravity = true;
            yield return new WaitForSeconds(2);
            Destroy(drum.gameObject);
        }
    }
}

