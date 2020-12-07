using LW.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HoverDrums
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
    public class HoverDrummer : MonoBehaviour
    {
        [Header("DevMode controls")]
        [SerializeField] bool devMode = false;
        [SerializeField] [Range(7.5f, 75)] float force = 10f;
        public bool ableToCast = true;

        [Header("Controller Settings")]
        [SerializeField] float castDelay = 3f;
        [SerializeField] float maxXAxisDist = 0.5f; //TODO hardcode

        [Header("Hook Ups")]
        [SerializeField] AudioClip resetFX;
        [SerializeField] List<GameObject> drumVariants;

        List<HSV> colorVariants = new List<HSV>();

        float timeSinceLastCast = Mathf.Infinity;
        int totalDrums;
        int drumId = 0;
        int drumShape = 0;
        int drumColor = 0;
        
        // stores live drums, for dev purposes only TODO make private
        public List<HoverDrumController> liveDrums = new List<HoverDrumController>();

        HandTracking handtracking;
        CastOrigins castOrigins;
        AudioSource audio;

        private void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            castOrigins = FindObjectOfType<CastOrigins>();
            audio = GetComponent<AudioSource>();

            colorVariants.Add(new HSV(0, 0, 1));
            colorVariants.Add(new HSV(0, 1, 1));
            colorVariants.Add(new HSV(0.15f, 1, 1));
            colorVariants.Add(new HSV(0.37f, 1, 1));
            colorVariants.Add(new HSV(0.5f, 1, 1));
            colorVariants.Add(new HSV(0.6f, 1, 1));
            colorVariants.Add(new HSV(0.8f, 1, 1));
            colorVariants.Add(new HSV(0, 0, 0));

            totalDrums = drumVariants.Count * colorVariants.Count;
        }

        private void Update()
        {
            timeSinceLastCast += Time.deltaTime;

            if (devMode)
            {
				if (Input.GetKeyDown(KeyCode.R))
				{
					Reset();
				}

				if (Input.GetKeyDown(KeyCode.G))
				{
                    GatherDrums();
				}

                force += Input.mouseScrollDelta.y;
                if (drumId >= totalDrums) return;

                if (Input.GetMouseButtonDown(0) && ableToCast)
                {
                    CastDrum();
                }
            }

            if (handtracking.palmsIn && handtracking.rightFist && handtracking.leftFist)
            {
                Reset();
            }

            if (drumId >= totalDrums) return;

            if (handtracking.palmsOut && handtracking.rightOpen && handtracking.leftOpen && ableToCast)
            {
                CastDrum();
            }
        }

        private void CastDrum()
        {
            Vector3 castOrigin = Vector3.Lerp(handtracking.rtMiddleKnuckle.Position, handtracking.ltMiddleKnuckle.Position, 0.5f);
            Quaternion handRotation = Quaternion.Slerp(handtracking.rightPalm.Rotation, handtracking.leftPalm.Rotation, 0.5f);
            Quaternion castRotation = handRotation * Quaternion.Euler(60, 0, 0); // rotational offset - so casts go OUT instead of UP along the hand.Z axis

            if (timeSinceLastCast >= castDelay)
            {
                timeSinceLastCast = 0;
                drumId++;
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
                currentDrum.SetDrumAddress(drumId);
                currentDrum.SetDrumColor(colorVariants[drumColor]);

                float spellForce = (castOrigins.palmDist / maxXAxisDist) * 75;
                if (spellForce < 7.5f) spellForce = 7.5f;
                // set drum casting force and color
                if (devMode) currentDrum.force = force;
                else currentDrum.force = spellForce;
                // add drum to list of live drums
                liveDrums.Add(currentDrum);

                //add to DrumContainer parent
                currentDrum.transform.SetParent(FindObjectOfType<DrumParent>().transform);

                SetNextDrum();
            }
        }

        private void SetNextDrum()
        {
            if (drumColor < colorVariants.Count - 1)
            {
                drumColor++;
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

        private void GatherDrums()
		{
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

            // clear all drums
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

        private IEnumerator DropAndDestroy(HoverDrumController drum)
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

