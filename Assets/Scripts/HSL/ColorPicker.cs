using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using LW.SlingShot;
using System;

namespace LW.HSL
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] float maximumHandDistance = 0.35f;
        float minimumHandDistance = 0.2f;
        public Color ChosenColor { get; set; }

        public float hueFloat = 0;
        public float satFloat = 1;
        
        HandTracking hands;
        SlingShotDirector director;
        HSLOrbController hslOrb;

        void Start()
        {
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
            hslOrb = GameObject.FindGameObjectWithTag("HSLOrb").GetComponent<HSLOrbController>();

            //ChosenColor = Color.white;
        }

        void Update()
        {
            hslOrb.transform.position = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);

            if (!GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>().HandPicker)
			{
                if (hands.rightPeace || hands.leftPeace) return;

                if (hands.palmsOpposed)
                {
                    hslOrb.gameObject.SetActive(true);

                    if (!hands.rightFist && !hands.leftFist)
                    {
                        hueFloat = hands.GetStaffForCamUp() / 180;

                        float rawHandDist = Vector3.Distance(hands.rightPalm.Position, hands.leftPalm.Position);
                        satFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);
                    }
                }
                else
                {
                    hslOrb.gameObject.SetActive(false);
                }
            }
            else
			{
                if (hands.leftPeace)
                {
                    hueFloat = hands.ltPalmForFloorUp / 90;

                    if (hands.rightFist)
					{
                        satFloat = hands.rtPalmRtCamUp / 180;
					}
                }
                else if (hands.rightPeace)
                {
                    hueFloat = hands.rtPalmForFloorUp / 90;

                    if (hands.leftFist)
					{
                        satFloat = hands.ltPalmRtCamUp / 180;
					}
                }
                else return;
			}

            SetHueAndSat();
        }

        private void SetHueAndSat()
		{
            ChosenColor = Color.HSVToRGB(hueFloat, satFloat, 0.5f);
		}
	}
}


