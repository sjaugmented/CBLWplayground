using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.HoverDrums
{
    public class DrumProximitySensor : MonoBehaviour
    {
        [SerializeField] float userProximitySet = 0.5f;
        [SerializeField] float minHandDistance = 0.15f;
        [SerializeField] float maxHandDistance = 0.4f;
        [SerializeField] float effectiveBubbleScale = 0.67f;
        [SerializeField] Transform proximityBubble;

        HoverDrumController drumController;
        Transform user;
        HoverDrummer drummer;
        HandTracking handtracking;

        public float bubbleScale = 0;

        void Start()
        {
            drumController = GetComponent<HoverDrumController>();
            user = Camera.main.transform;
            drummer = GameObject.FindGameObjectWithTag("Drummer").GetComponent<HoverDrummer>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }

        void Update()
        {
            float distanceToUser = Vector3.Distance(transform.position, user.position);

            if (distanceToUser < userProximitySet)
            {
                drummer.SetCast(false);
                

                if (handtracking.twoHands && handtracking.rightOpen && handtracking.leftOpen)
                {
                    float distanceToHand = Vector3.Distance(transform.position, handtracking.rightPalm.Position);

                    bubbleScale = Mathf.Clamp(1 - ((distanceToHand - minHandDistance) / maxHandDistance), 0, 1);
                    proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);

                    if (bubbleScale > effectiveBubbleScale)
                    {
                        drumController.SendOSCMessage(drumController.address + "/proximity", 1 - handtracking.GetStaffForCamUp() / 180);
                    }
                }
            }
            else
            {
                drummer.SetCast(true);
            }
        }
    }
}
