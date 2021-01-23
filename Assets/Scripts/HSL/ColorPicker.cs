using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;
using System;

namespace LW.HSL
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] float maximumHandDistance = 0.3f;
        float minimumHandDistance = 0.15f;
        public Color ChosenColor { get; set; }

        public float handAngle = 0;
        public float handDistance = 0;
        
        HandTracking hands;
        HSLOrbController hslOrb;

        void Start()
        {
            hands = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
            hslOrb = GameObject.FindGameObjectWithTag("HSLOrb").GetComponent<HSLOrbController>();

            ChosenColor = Color.white;
        }

        void Update()
        {
            hslOrb.transform.position = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);

            if (hands.rightPeace || hands.leftPeace) return;

            if (hands.palmsOpposed)
            {
                hslOrb.gameObject.SetActive(true);

                if (hands.rightOpen && hands.leftOpen)
                {
                    handAngle = hands.GetStaffForCamUp() / 180;

                    float rawHandDist = Vector3.Distance(hands.rightPalm.Position, hands.leftPalm.Position);
                    handDistance = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);
                }
            }
            else
            {
                hslOrb.gameObject.SetActive(false);
            }

            SetHueAndSat();
        }

		private void PickColor()
		{
			throw new NotImplementedException();
		}

        private void SetHueAndSat()
		{
            ChosenColor = Color.HSVToRGB(handAngle, handDistance, 0.5f);
		}
	}
}


