using LW.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class HSV
    {
        float hue;
        float sat;
        float val;

        public HSV(float hue, float sat, float val)
        {
            this.hue = hue;
            this.sat = sat;
            this.val = val;
        }

        public float Hue
        {
            get { return hue; }
            set { hue = value; }
        }
        public float Sat
        {
            get { return sat; }
            set { sat = value; }
        }
        public float Val
        {
            get { return val; }
            set { val = value; }
        }
    }
    public class RuneMaster : MonoBehaviour
    {
        [Header("DevMode controls")]
        [SerializeField] bool devMode = false; // TODO remove
        [SerializeField] [Range(7.5f, 75)] float force = 10f; // TODO make private
        public bool ableToCast = true; // TODO make private

        [Header("Controller Settings")]
        [SerializeField] float castDelay = 3f; //TODO hardcode
        [SerializeField] float maxPalmDist = 0.5f; //TODO hardcode
        [SerializeField] float minimumDist = 0.2f; //TODO hardcode
        [SerializeField] float resetWindow = 2; //TODO hardcode

        public float resetTimer = 5; // TODO make private
        public bool readyToGather = false;

        [Header("Hook Ups")]
        [SerializeField] AudioClip resetFX;

        public RuneType runeType;
        List<HSV> colorVariants = new List<HSV>();

        float timeSinceLastCast = Mathf.Infinity;
        int totalDrums;
        int drumId = 0;
        int drumShape = 0;
        int drumColor = 0;

        public int runeTypeIndex = 0;
        
        // stores live drums, for dev purposes only TODO make private
        public List<RuneController> liveDrums = new List<RuneController>();

        HandTracking handtracking;
        CastOrigins castOrigins;
        RuneBelt runeBelt;
        AudioSource audio;

        private void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            castOrigins = FindObjectOfType<CastOrigins>();
            runeBelt = GetComponent<RuneBelt>();
            audio = GetComponent<AudioSource>();

            colorVariants.Add(new HSV(0, 0, 0.7f)); // white
            colorVariants.Add(new HSV(0, 1, 1)); // red
            colorVariants.Add(new HSV(0.15f, 1, 1)); // yellow
            colorVariants.Add(new HSV(0.37f, 1, 1)); // green
            colorVariants.Add(new HSV(0.5f, 1, 1)); // cyan
            colorVariants.Add(new HSV(0.6f, 1, 1)); // blue
            colorVariants.Add(new HSV(0.8f, 1, 1)); // magenta
            colorVariants.Add(new HSV(0, 0, 0)); // black

            //totalDrums = runeTypes.Count * colorVariants.Count;
        }

        private void Update()
        {
            timeSinceLastCast += Time.deltaTime;
            resetTimer += Time.deltaTime;

			#region DEV CONTROLS
			if (devMode)
            {
				if (Input.GetKeyDown(KeyCode.R))
				{
					Reset();
				}

				if (Input.GetKeyDown(KeyCode.G))
				{
                    GatherRunes();
				}

                force += Input.mouseScrollDelta.y;
                if (drumId >= totalDrums) return;

                if (Input.GetMouseButtonDown(0) && ableToCast)
                {
                    CastDrum();
                }
            }
			#endregion

			if (handtracking.palmsIn && handtracking.rightFist && handtracking.leftFist)
            {
                Reset();
            }

			#region Gather Runes
            // prime the gather runes method
			if (!handtracking.twoHands && handtracking.rightRockOn)
			{
                if (!readyToGather)
				{
                    resetTimer = 0;
                    readyToGather = true;
				}
			}
            else { readyToGather = false; }

            // trigger the gather runes method
            if (resetTimer < resetWindow && !handtracking.twoHands && handtracking.rightFist)
            {
                GatherRunes();
                resetTimer = Mathf.Infinity;
            }
			#endregion

			//if (drumId >= totalDrums) return;

            // Set Rune Type
            if (handtracking.palmsOpposed && handtracking.rightFist && handtracking.leftFist)
			{
                SelectRuneType();
			}

            // Casting
            if (handtracking.palmsOut && handtracking.rightOpen && handtracking.leftOpen && ableToCast)
            {
                CastDrum();
            }
        }

		private void SelectRuneType()
		{
            float palmDist = castOrigins.palmDist;
            float totalUsableDist = maxPalmDist - minimumDist;
            int totalRunes = runeBelt.GetRuneSlots();
            float slotSize = totalUsableDist / totalRunes; // size of selectable area based on number of Rune Types

            for (int i = 0; i < runeBelt.GetRuneSlots(); i++)
			{
                if (palmDist < minimumDist)
				{
                    runeTypeIndex = totalRunes - 1;
				}

                else if (palmDist > maxPalmDist)
				{
                    runeTypeIndex = 0;
				}

                else if (palmDist > minimumDist && palmDist < (maxPalmDist - slotSize * i) && palmDist > (maxPalmDist - slotSize * (i+1)))
				{
                    Debug.Log("PalmDist ==== " + palmDist);
                    Debug.Log("TypeSlot ==== " + slotSize);
                    Debug.Log("Window ==== " + (maxPalmDist - slotSize * i));
                    runeTypeIndex = i;
				}
			}

		}

		private void CastDrum()
        {
            Vector3 castOrigin = Vector3.Lerp(handtracking.rtMiddleKnuckle.Position, handtracking.ltMiddleKnuckle.Position, 0.5f);
            Quaternion handRotation = Quaternion.Slerp(handtracking.rightPalm.Rotation, handtracking.leftPalm.Rotation, 0.5f);
            Quaternion castRotation = handRotation * Quaternion.Euler(60, 0, 0); // rotational offset - so casts go OUT instead of UP along the hand.Z axis

            //if (timeSinceLastCast >= castDelay)
            //{
            //    timeSinceLastCast = 0;
            //    drumId++;
            //    GameObject drum;

            //    if (devMode)
            //    {
            //        drum = Instantiate(runeTypes[drumShape], Camera.main.transform.position, Camera.main.transform.rotation);
            //    }
            //    else
            //    {
            //        drum = Instantiate(runeTypes[drumShape], castOrigin, castRotation);
            //    }

            //    RuneController currentDrum = drum.GetComponent<RuneController>();
            //    currentDrum.SetDrumAddress(drumId);
            //    currentDrum.SetDrumColor(colorVariants[drumColor]);

            //    float spellForce = (castOrigins.palmDist / maxPalmDist) * 75;
            //    if (spellForce < 7.5f) spellForce = 7.5f;
            //    // set drum casting force and color
            //    if (devMode) currentDrum.force = force;
            //    else currentDrum.force = spellForce;
            //    // add drum to list of live drums
            //    liveDrums.Add(currentDrum);

            //    //add to DrumContainer parent
            //    currentDrum.transform.SetParent(FindObjectOfType<DrumParent>().transform);

            //    SetNextRune();
            //}
        }

        private void SetNextRune()
        {
            if (drumColor < colorVariants.Count - 1)
            {
                drumColor++;
            }
            else
            {
                drumColor = 0;
                //if (drumShape < runeTypes.Count - 1)
                //{
                //    drumShape++;
                //}
                //else return;
            }
        }

        private void GatherRunes()
		{
            Debug.Log("Gathering"); // REMOVE
            DrumParent grid = FindObjectOfType<DrumParent>();
            grid.UpdateCollection();
            grid.PositionGrid();
		}

        private void Reset()
        {
            if (liveDrums.Count == 0) return;
            
            if (!audio.isPlaying)
            {
                audio.PlayOneShot(resetFX);
            }

            // clear all runes
            for (int i = 0; i < liveDrums.Count; i++)
            {
                StartCoroutine("DropAndDestroy", liveDrums[i]);
                liveDrums.Remove(liveDrums[i]);
            }

            // reset id, shape, color
            drumId = 0;
            drumShape = 0;
            drumColor = 0;
        }

        private IEnumerator DropAndDestroy(RuneController drum)
        {
            if (!drum) yield break;
            drum.GetComponent<Rigidbody>().useGravity = true;
            yield return new WaitForSeconds(2);
            Destroy(drum.gameObject);
        }

        public void SetAbleToCast(bool canCast)
        {
            ableToCast = canCast;
        }
    }
}

