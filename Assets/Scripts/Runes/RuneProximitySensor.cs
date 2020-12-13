using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.Runic
{
    public class RuneProximitySensor : MonoBehaviour
    {
        [SerializeField] float userProximitySet = 0.35f;
        [SerializeField] float minHandDistance = 0.15f;
        [SerializeField] float maxHandDistance = 0.3f;
        [SerializeField] float effectiveBubbleScale = 0.67f;
        [SerializeField] Transform proximityBubble;

        RunicDirector director;
        RuneController runeController;
        HandTracking handtracking;

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
            if (director.currentMode == RunicDirector.Mode.Touch)
			{
                
            }

            float distanceToUser = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (distanceToUser < userProximitySet)
            {
                GameObject.FindGameObjectWithTag("Caster").GetComponent<RuneMaster>().SetAbleToCast(false);

                if (handtracking.twoHands)
                {
                    ActivateProximityBubble();

                    if (bubbleScale > effectiveBubbleScale) // if bubble is visible -ish
                    {
                        if (!handtracking.rightFist && handtracking.leftFist)
                        {
                            runeController.SendOSCMessage(runeController.address1 + "/proximityLeft", 1 - handtracking.GetStaffForCamUp() / 180);
                        }

                        if (handtracking.rightFist && !handtracking.leftFist)
                        {
                            runeController.SendOSCMessage(runeController.address1 + "/proximityRight", 1 - handtracking.GetStaffForCamUp() / 180);
                        }

                        if (handtracking.rightFist && handtracking.leftFist)
                        {
                            runeController.SendOSCMessage(runeController.address1 + "/proximity", 1 - handtracking.GetStaffForCamUp() / 180);
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
                GameObject.FindGameObjectWithTag("Caster").GetComponent<RuneMaster>().SetAbleToCast(true);
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
