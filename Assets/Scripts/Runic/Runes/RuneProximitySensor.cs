using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RuneProximitySensor : MonoBehaviour
    {
        [SerializeField] float userProximitySet = 0.7f;
        [SerializeField] float minHandDistance = 0.15f;
        [SerializeField] float maxHandDistance = 0.3f;
        [SerializeField] float effectiveBubbleScale = 0.67f;
        [SerializeField] Transform proximityBubble;

        RunicDirector director;
        RuneController runeController;
        HandTracking handtracking;

        bool leftFisted = false;
        bool rightFisted = false;
        bool dualFisted = false;

        float bubbleScale = 0;

        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
            runeController = GetComponent<RuneController>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }

        void Update()
        {
            float distanceToUser = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (distanceToUser < userProximitySet)
            {
                GameObject.FindGameObjectWithTag("Caster").GetComponent<RuneCaster>().TriggerProximitySensor();

                if (handtracking.twoHands)
                {
                    ActivateProximityBubble();

                    if (bubbleScale > effectiveBubbleScale) // if bubble is visible -ish
                    {
                        if (!handtracking.rightFist && handtracking.leftFist)
                        {
                            if (!leftFisted)
							{
                                runeController.SendOSCMessage("leftFistOn");
                                leftFisted = true;
                            }
							runeController.SendOSCMessage(runeController.addressBasic1 + "/leftProximityAngle", 1 - handtracking.GetStaffForCamUp() / 180);
							runeController.SendOSCMessage(runeController.addressBasic1 + "/leftProximityDistance", bubbleScale.Remap(1, 0.67f, 1, 0));
                        }
                        else
						{
                            if (leftFisted)
							{
                                runeController.SendOSCMessage("leftFistOff");
                                leftFisted = false;
                            }
						}

                        if (handtracking.rightFist && !handtracking.leftFist)
                        {
                            if (!rightFisted)
                            {
                                runeController.SendOSCMessage("rightFistOn");
                                rightFisted = true;
                            }
							runeController.SendOSCMessage(runeController.addressBasic1 + "/rightProximityAngle", 1 - handtracking.GetStaffForCamUp() / 180);
							runeController.SendOSCMessage(runeController.addressBasic1 + "/rightProximityDistance", bubbleScale.Remap(1, 0.67f, 1, 0));
                        }
                        else
                        {
                            if (rightFisted)
                            {
                                runeController.SendOSCMessage("rightFistOff");
                                rightFisted = false;
                            }
                        }

                        if (handtracking.rightFist && handtracking.leftFist)
                        {
                            if (!dualFisted)
							{
                                runeController.SendOSCMessage("dualFistOn");
                                dualFisted = true;
                            }
							runeController.SendOSCMessage(runeController.addressBasic1 + "/proximityAngle", 1 - handtracking.GetStaffForCamUp() / 180);
							runeController.SendOSCMessage(runeController.addressBasic1 + "/proximityDistance", bubbleScale.Remap(1, 0.67f, 1, 0));
                        }
                        else
						{
                            if (dualFisted)
							{
                                runeController.SendOSCMessage("dualFistOff");
                                dualFisted = false;
                            }
						}
                    }
                }
                else
                {
                    proximityBubble.localScale = new Vector3(0, 0, 0);
                }
            }
            else
            {
                proximityBubble.localScale = new Vector3(0, 0, 0);
            }
        }

        private void ActivateProximityBubble()
        {
            float distanceToHand = Vector3.Distance(transform.position, handtracking.rightPalm.Position);

            bubbleScale = Mathf.Clamp(1 - ((distanceToHand - minHandDistance) / maxHandDistance), 0, 1);
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }
    }
}
