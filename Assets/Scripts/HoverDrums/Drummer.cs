using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.HoverDrums
{
    public class Drummer : MonoBehaviour
    {
        [SerializeField] float castDelay = 3f;
        [SerializeField] List<GameObject> drumVariants;
        [SerializeField] List<float> colorVariants;

        public Vector3 orbCastRotOffset = new Vector3(0, 0, 0); // rotational offset of casting, else casting is straight up - TODO hardcode

        HandTracking handtracking;
        bool ableToCast = true;

        private void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
        }

        private void Update()
        {
            if (handtracking.palmsOut && handtracking.rightOpen && handtracking.leftOpen)
            {

            }
        }

        private void CastOrb()
        {
            Vector3 castOrigin = Vector3.Lerp(handtracking.rtMiddleKnuckle.Position, handtracking.ltMiddleKnuckle.Position, 0.5f);

            Quaternion palmsRotationMid = Quaternion.Slerp(handtracking.rightPalm.Rotation, handtracking.leftPalm.Rotation, 0.5f);
            Quaternion castRotation = palmsRotationMid * Quaternion.Euler(orbCastRotOffset);

            if (ableToCast)
            {
                GameObject drum = Instantiate(spellBook.orbSpells[elIndex], castOrigin, castRotation);

                float spellForceRange = 1 - (palmDist / maxXAxisDist);

                float spellForce = spellForceRange * 50;
                if (spellForce < 1) spellForce = 2;
                spellController.force = spellForce;

                //GetStaffAngle();
                SendOSCMessage(spellController.GetMessageOSC() + variantOSC[varIndex] + staffOSC[staffIndex], spellController.valueOSC);

                float particleScale = elementScale * 1.167388f;

                foreach (Transform child in elParent.transform)
                {
                    if (child.CompareTag("Spell"))
                    {
                        child.localScale = new Vector3(particleScale, particleScale, particleScale);
                    }
                }
            }

            /*if (!hoverOrb)
            {
                if (ableToCast)
                {
                    GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                    StartCoroutine("CastDelay", orbsPerSecond);
                    spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);

                    ElementParent elParent = spellOrb.GetComponentInChildren<ElementParent>();

                    if (fromOrbScaler && floatPassthru)
                    {
                        OrbCastController spellController = spellOrb.GetComponent<OrbCastController>();
                        spellController.valueOSC = elementScale;

                        float spellForceRange = 1 - (palmDist / maxXAxisDist);

                        float spellForce = spellForceRange * 50;
                        if (spellForce < 1) spellForce = 2;
                        spellController.force = spellForce;

                        Debug.Log("GetMessageOSC: " + spellController.GetMessageOSC());
                        SendOSCMessage(spellController.GetMessageOSC() + variantOSC[variantID] + staffOSC[staffID], elementScale);


                        float particleScale = elementScale * 1.167388f;

                        foreach (Transform child in elParent.transform)
                        {
                            if (child.CompareTag("Spell"))
                            {
                                child.localScale = new Vector3(particleScale, particleScale, particleScale);
                            }
                        }
                    }
                    else return;
                }
                else return;
            }*/
            /*else
            {
                if (currEl == Element.light && !activeLightHover)
                {
                    GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                    activeLightHover = true;
                    spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
                }

                if (currEl == Element.fire && !activeFireHover)
                {
                    GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                    activeFireHover = true;
                    spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
                }

                if (currEl == Element.water && !activeWaterHover)
                {
                    GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                    activeWaterHover = true;
                    spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
                }

                if (currEl == Element.ice && !activeIceHover)
                {
                    GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                    activeIceHover = true;
                    spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
                }
            }*/
        }

    }
}

