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

        NewTracking tracking;
        SlingShotDirector director;
        HSLOrbController hslOrb;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            hslOrb = GameObject.FindGameObjectWithTag("HSLOrb").GetComponent<HSLOrbController>();

            //ChosenColor = Color.white;
        }


        void Update()
        {
            if (GameObject.FindGameObjectWithTag("Light").GetComponent<LightHolo>().Manipulated) { 
                hslOrb.gameObject.SetActive(false); 
                return;
            }
            
            hslOrb.transform.position = Vector3.Lerp(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position, 0.5f);

            if (GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>().SlingShot)
			{
                valFloat = Mathf.Clamp(Vector3.Distance(tracking.GetLtPalm.Position, tracking.GetRtPalm.Position) / maxSlingShotPullDistance, 0, 1);
                satFloat = 1;
                
                if (tracking.leftPose == HandPose.peace)
                {
                    // hueFloat = hands.ltPalmForFloorUp / 90;
                    
                    if (tracking.rightPose == HandPose.fist)
					{
                        hueFloat = tracking.RtLauncher / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else if (tracking.rightPose == HandPose.peace)
                {
                    // hueFloat = hands.rtPalmForFloorUp / 90;

                    if (tracking.leftPose == HandPose.fist)
					{
                        hueFloat = tracking.LtLauncher / 180;
                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    }
                }
                else return;
            }
            else
			{
                if (tracking.rightPose == HandPose.peace || tracking.leftPose == HandPose.peace) 
                {
                    hslOrb.gameObject.SetActive(false);
                    return; 
                }

                if (tracking.palmsRel == Formation.together)
                {
                    hslOrb.gameObject.SetActive(true);

                    #region 3 axis 1.0
                    //if (tracking.StaffUp > 20 && tracking.StaffUp < 162) {
                    //    var adjustedAngle = tracking.StaffUp - 20;
                    //    hueFloat = adjustedAngle / 142;
                    //}

                    //if (tracking.StaffForward > 48 && tracking.StaffForward < 132) {
                    //    var adjustedAngle = tracking.StaffForward - 48;
                    //    valFloat = adjustedAngle / 84;
                    //}

                    //float rawHandDist = Vector3.Distance(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position);
                    //satFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);

                    //PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);

                    //if (tracking.rightPose != HandPose.fist && tracking.leftPose != HandPose.fist)
                    //{
                    //    LiveColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                    //}
                    #endregion

                    if (tracking.rightPose != HandPose.pointer && tracking.leftPose != HandPose.pointer)
                    {
                        if (tracking.rightPose == HandPose.fist && tracking.leftPose != HandPose.fist)
                        {
                            hueFloat = tracking.StaffUp / 180;
                        }

                        if (tracking.rightPose != HandPose.fist && tracking.leftPose == HandPose.fist)
                        {
                            satFloat = tracking.StaffUp / 180;
                        }

                        if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                        {
                            float rawHandDist = Vector3.Distance(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position);
                            valFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);
                        }


                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);

                        if (tracking.rightPose == HandPose.fist && tracking.leftPose == HandPose.fist)
                        {
                            LiveColor = PreviewColor;
                        }
                    }
                    else
                    {
                        if (tracking.rightPose == HandPose.pointer && (tracking.leftPose != HandPose.fist || tracking.leftPose != HandPose.pointer))
                        {
                            hueFloat = tracking.StaffUp / 180;
                        }

                        if ((tracking.rightPose != HandPose.fist || tracking.rightPose != HandPose.pointer) && tracking.leftPose == HandPose.pointer)
                        {
                            satFloat = tracking.StaffUp / 180;
                        }

                        float rawHandDist = Vector3.Distance(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position);
                        valFloat = Mathf.Clamp(1 - (rawHandDist - minimumHandDistance) / maximumHandDistance, 0, 1);

                        PreviewColor = Color.HSVToRGB(hueFloat, satFloat, valFloat);
                        LiveColor = PreviewColor;
                    }

                    
                    // TODO remove
                    hueHud.text = "Hue: " + Math.Round(hueFloat * 255).ToString();
                    satHud.text = "Sat: " + Math.Round(satFloat * 255).ToString();
                    valHud.text = "Val: " + Math.Round(valFloat * 255).ToString();
                    
                    // TODO color cube selector
                }
                else
                {
                    hslOrb.gameObject.SetActive(false);
                }
			}
        }
	}
}


