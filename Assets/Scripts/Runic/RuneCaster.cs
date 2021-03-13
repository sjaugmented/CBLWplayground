using LW.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    // TODO
    // OSC receive visuals...
    // sparkle & spin
    public class RuneCaster : MonoBehaviour
    {
        [Header("DevMode controls")]
        [SerializeField] bool devMode = false; // TODO remove
        [Range(7.5f, 50)] float force = 10f; // TODO make private

        [Header("Controller Settings")]
        [SerializeField] float castDelay = 0.5f; //TODO hardcode
        [SerializeField] float maxPalmDist = 0.5f; //TODO hardcode
        
        [Header("OSC")]
        [SerializeField] float defaultOSCValue = 127;

        public float DefaultOSCValue
		{
            get { return defaultOSCValue; }
            set { defaultOSCValue = value; }
		}

        [Header("Hook Ups")]
        [SerializeField] GameObject masterRune;
        [SerializeField] AudioClip resetFX;
        [SerializeField] AudioClip gatherFX;

        public RuneType runeType; // TODO private; easy shape switching in inspector
        int runeTypeIndex = 0; // automates rune selection
        
        [SerializeField] List<Material> runeMaterials = new List<Material>();
        int runeMaterialIndex = 0;
        
        // stores live drums, for dev purposes only TODO make private
        List<RuneController> liveRunes = new List<RuneController>();

        float proximitySensor = Mathf.Infinity;
        float timeSinceLastCast = Mathf.Infinity;
        float resetTimer = 5;
        bool readyToGather = false;

        bool manipulating = false;

        public bool Manipulating
        {
            get { return manipulating; }
            set { manipulating = value; }
        }

        NewTracking tracking;
        CastOrigins castOrigins;
        RunicDirector director;
        RuneBelt runeBelt;
        AudioSource audioSource;

        private void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            castOrigins = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<CastOrigins>();
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
            runeBelt = GetComponent<RuneBelt>();
            audioSource = GetComponent<AudioSource>();

            runeBelt.ResetAllRuneAmmo(runeMaterials.Count);
            masterRune.SetActive(false);
        }

        private void Update()
        {
            timeSinceLastCast += Time.deltaTime;
            resetTimer += Time.deltaTime;
            runeType = (RuneType)runeTypeIndex;
            proximitySensor += Time.deltaTime;

            #region DEV MODE
            if (Input.GetKeyDown(KeyCode.B)) devMode = !devMode;

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

                if (Input.GetMouseButtonDown(0) && proximitySensor > 0)
                {
                    CastRune();
                }

                if (Input.GetKeyDown(KeyCode.Period)) runeTypeIndex++;
                if (Input.GetKeyDown(KeyCode.Comma)) runeTypeIndex--;

            }
            #endregion

            if (proximitySensor > 0.1f && !Manipulating)
			{
                ////// Set Rune Type
                if (tracking.palms == Formation.together && tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                {
                    masterRune.SetActive(true);
                    SelectRuneType();
                }
                else masterRune.SetActive(false);

                ////// Casting
                if (tracking.palms == Formation.palmsOut && tracking.rightPose == HandPose.flat && tracking.leftPose == HandPose.flat)
                {
                    CastRune();
                }
            }
            else
			{
                masterRune.SetActive(false);
			}


            #region Gather & Reset - activates Build Mode (!!Build mode deprecated for now!!)
            ////// Gather Runes
            // prime the gather runes method
            if ((tracking.handedness == Hands.right || tracking.handedness == Hands.both) && tracking.rightPose == HandPose.rockOn)
            {
                if (!readyToGather)
                {
                    resetTimer = 0;
                    readyToGather = true;
                }
            }
            else { readyToGather = false; }

            // trigger the gather runes method
            if (resetTimer < 2 && (tracking.handedness == Hands.right || tracking.handedness == Hands.both) && tracking.rightPose == HandPose.fist)
            {
                GatherRunes();
                //director.currentMode = RunicDirector.Mode.Build;
                resetTimer = Mathf.Infinity;
            }

            ////// Reset Interface
            if (tracking.palms == Formation.palmsIn && tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
            {
                Reset();
                //director.currentMode = RunicDirector.Mode.Build;
            }
			#endregion

		}

		private void SelectRuneType()
		{
            float slotSize = 180 / runeBelt.GetRuneSlots(); // size of selectable area based on number of Rune Types

            for (int i = 0; i < runeBelt.GetRuneSlots(); i++)
			{
                if (tracking.StaffUp < (180 - slotSize * i) && tracking.StaffUp > (180 - slotSize * (i+1)))
				{
                    runeTypeIndex = i;
				}
			}

            foreach (Transform child in masterRune.transform)
			{
                child.gameObject.SetActive(false);
			}
            masterRune.transform.GetChild(runeTypeIndex).gameObject.SetActive(true);

            masterRune.transform.position = castOrigins.PalmsMidpoint;
            masterRune.transform.LookAt(Camera.main.transform);

		}

		private void CastRune()
        {
            if (timeSinceLastCast >= castDelay && runeBelt.GetCurrentRuneAmmo(runeType) > 0)
            {
                timeSinceLastCast = 0;
                GameObject rune;

                GameObject runePrefab = runeBelt.GetRunePrefab(runeType);

                if (devMode)
                {
                    rune = Instantiate(runePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                }
                else
                {
                    rune = Instantiate(runePrefab, castOrigins.PalmsMidpoint, castOrigins.CastRotation);
                }

                runeMaterialIndex = runeMaterials.Count - runeBelt.GetCurrentRuneAmmo(runeType);
                runeBelt.ReduceCurrentRuneAmmo(runeType);
                int runeID = runeMaterials.Count - runeBelt.GetCurrentRuneAmmo(runeType);
                RuneController currentRune = rune.GetComponent<RuneController>();
                currentRune.SetRuneAddressAndMaterial(runeID, runeMaterials[runeMaterialIndex]);

                float spellForce = (1 - (castOrigins.PalmsDist / maxPalmDist)) * 50;
                if (spellForce < 7.5f) spellForce = 7.5f;
                if (devMode) currentRune.force = force;
                else currentRune.force = spellForce;

                liveRunes.Add(currentRune);

                currentRune.transform.SetParent(FindObjectOfType<RuneGrid>().transform);
            }
        }

        private void GatherRunes()
		{
            if (!audioSource.isPlaying) audioSource.PlayOneShot(gatherFX);
            RuneGrid grid = FindObjectOfType<RuneGrid>();
            grid.UpdateCollection();
            grid.PositionGrid();
		}

        private void Reset()
        {
            if (liveRunes.Count == 0) return;
            
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(resetFX);
            }

            // clear all runes
            for (int i = 0; i < liveRunes.Count; i++)
            {
                StartCoroutine("DropAndDestroy", liveRunes[i]);
                liveRunes.Remove(liveRunes[i]);
            }

            // reset ammo counts, id, shape, color
            runeBelt.ResetAllRuneAmmo(runeMaterials.Count);
            runeMaterialIndex = 0;
        }

        private IEnumerator DropAndDestroy(RuneController drum)
        {
            if (!drum) yield break;
            drum.GetComponent<Rigidbody>().useGravity = true;
            yield return new WaitForSeconds(2);
            Destroy(drum.gameObject);
        }

        public void TriggerProximitySensor()
		{
            proximitySensor = 0;
		}

        public int GetRuneColorCount()
		{
            return runeMaterials.Count;
		}
    }
}

