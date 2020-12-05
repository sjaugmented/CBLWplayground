using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.HoverDrums
{
    public class DrumProximitySensor : MonoBehaviour
    {
        [SerializeField] float userProximitySet = 0.35f;
        [SerializeField] float minHandDistance = 0.15f;
        [SerializeField] float maxHandDistance = 0.3f;
        [SerializeField] float effectiveBubbleScale = 0.67f;
        [SerializeField] Transform proximityBubble;

        HoverDrumController drumController;
        HoverDrummer drummer;
        HandTracking handtracking;

        float bubbleScale = 0;

        void Start()
        {
            drumController = GetComponent<HoverDrumController>();
            drummer = GameObject.FindGameObjectWithTag("Drummer").GetComponent<HoverDrummer>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }

        void Update()
        {
            float distanceToUser = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (distanceToUser < userProximitySet)
            {
                drummer.SetAbleToCast(false);
                
                if (handtracking.twoHands && handtracking.rightOpen && handtracking.leftOpen)
                {
                    GrowProximityBubble();

                    if (bubbleScale > effectiveBubbleScale)
                    {
                        drumController.SendOSCMessage(drumController.address + "/proximity", 1 - handtracking.GetStaffForCamUp() / 180);
                    }
                }
            }
            else
            {
                drummer.SetAbleToCast(true);
            }
        }

        private void GrowProximityBubble()
        {
            float distanceToHand = Vector3.Distance(transform.position, handtracking.rightPalm.Position);

            bubbleScale = Mathf.Clamp(1 - ((distanceToHand - minHandDistance) / maxHandDistance), 0, 1);
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }
    }
}
