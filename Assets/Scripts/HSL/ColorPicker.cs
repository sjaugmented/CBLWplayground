using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using LW.SlingShot;
using System;
using TMPro;

namespace LW.HSL
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] float maxSlingShotPullDistance = 0.25f;
        [SerializeField] float maximumHandDistance = 0.35f;
        [SerializeField] TextMeshPro hueHud;
        [SerializeField] TextMeshPro satHud;
        [SerializeField] TextMeshPro valHud;
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
            if (GameObject.FindGameObjectWithTag("Light").GetComponent<LightHolo>().Manipulated) { 
                hslOrb.gameObject.SetActive(false); 
                return;
            }
            
            hslOrb.transform.position = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);

            if (GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>().HandPicker)
			{
                valFloat = Mathf.Clamp(Vector3.Distance(hands.leftPalm.Position, hands.rightPalm.Position) / maxSlingShotPullDistance, 0, 1);
                satFloat = 1;
                
                if (hands.leftPeace)
                {
                    // hueFloat = hands.ltPalmForFloorUp / 90;
                    
                    if (hands.rightFist)
					{
                        hueFloat = hands.rtPalmRtCamUp / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else if (hands.rightPeace)
                {
                    // hueFloat = hands.rtPalmForFloorUp / 90;

                    if (hands.leftFist)
					{
                        hueFloat = hands.ltPalmRtCamUp / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else return;
                
            }
            else
			{
                if (hands.rightPeace || hands.leftPeace) return;

                if (hands.palmsOpposed)
                {
                    hslOrb.gameObject.SetActive(true);

                    // TODO remap Hue to 27-230
                    if (hands.GetStaffForCamUp > 20 && hands.GetStaffForCamUp < 162) {
                        var adjustedAngle = hands.GetStaffForCamUp - 20;
                        hueFloat = adjustedAngle / 142;
                    }

                    // TODO remap val to 69-188
                    if (hands.GetStaffForCamFor > 48 && hands.GetStaffForCamFor < 132) {
                        var adjustedAngle = hands.GetStaffForCamFor - 48;
                        valFloat = adjustedAngle / 84;
                    }

                    float rawHandDist = Vector3.Distance(hands.rightPalm.Position, hands.leftPalm.Position);
                    satFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);

                    PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);

                    if (!hands.rightFist && !hands.leftFist)
                    {
                        LiveColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }

                    // TODO 
                    // remove
                    hueHud.text = "Hue: " + Math.Round(hueFloat * 255).ToString();
                    satHud.text = "Sat: " + Math.Round(satFloat * 255).ToString();
                    valHud.text = "Val: " + Math.Round(valFloat * 255).ToString();
                    // rethink staff angles - consider extra guard rails
                }
                else
                {
                    hslOrb.gameObject.SetActive(false);
                }
			}
        }
	}
}


