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

        RuneController drumController;
        RuneMaster drummer;
        HandTracking handtracking;

        float bubbleScale = 0;

        void Start()
        {
            drumController = GetComponent<RuneController>();
            drummer = GameObject.FindGameObjectWithTag("Drummer").GetComponent<RuneMaster>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
        }

        void Update()
        {
            float distanceToUser = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (distanceToUser < userProximitySet)
            {
                drummer.SetAbleToCast(false);

                if (handtracking.twoHands)
                {
                    ActivateProximityBubble();

                    if (bubbleScale > effectiveBubbleScale && handtracking.rightFist && handtracking.leftFist)
                    {
                        drumController.SendOSCMessage(drumController.address1 + "/proximity", 1 - handtracking.GetStaffForCamUp() / 180);
                    }
                }
                else proximityBubble.localScale = new Vector3(0, 0, 0);


            }
            else
            {
                drummer.SetAbleToCast(true);
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
