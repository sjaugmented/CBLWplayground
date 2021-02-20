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
        [SerializeField] float maxSlingShotPullDistance = 0.25f;
        [SerializeField] float maximumHandDistance = 0.35f;
        float minimumHandDistance = 0.2f;
        public Color LiveColor { get; set; }
        public Color PreviewColor { get; set; }

        public float hueFloat = 0;
        public float satFloat = 1;
        public float valFloat = 0;
        
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
            if (GameObject.FindGameObjectWithTag("Light").GetComponent<LightHolo>().Manipulated) { hslOrb.gameObject.SetActive(false); }
            
            hslOrb.transform.position = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);

            if (!GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>().HandPicker)
			{
                if (hands.rightPeace || hands.leftPeace) return;

                if (hands.palmsOpposed)
                {
                    hslOrb.gameObject.SetActive(true);

                    hueFloat = hands.GetStaffForCamUp / 180;
                    valFloat = hands.GetStaffForCamFor / 180;

                    float rawHandDist = Vector3.Distance(hands.rightPalm.Position, hands.leftPalm.Position);
                    satFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);

                    PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);

                    if (!hands.rightFist && !hands.leftFist)
                    {


                        LiveColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else
                {
                    hslOrb.gameObject.SetActive(false);
                }
            }
            else
			{
                // valFloat = Mathf.Clamp(Vector3.Distance(hands.leftPalm.Position, hands.rightPalm.Position) / maxSlingShotPullDistance, 0, 1);
                valFloat = 1;
                
                if (hands.leftPeace)
                {
                    hueFloat = hands.ltPalmForFloorUp / 90;
                    
                    if (hands.rightFist)
					{
                        satFloat = hands.rtPalmRtCamUp / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else if (hands.rightPeace)
                {
                    hueFloat = hands.rtPalmForFloorUp / 90;

                    if (hands.leftFist)
					{
                        satFloat = hands.ltPalmRtCamUp / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else return;
			}
        }
	}
}


