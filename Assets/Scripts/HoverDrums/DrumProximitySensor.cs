using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.HoverDrums
{
    public class DrumProximitySensor : MonoBehaviour
    {
        [SerializeField] float userProximitySet = 0.5f;
        [SerializeField] float handProximitySet = 0.2f;
        [SerializeField] Transform proximityBubble;
        
        Transform user;
        HoverDrummer drummer;
        HandTracking handtracking;

        public float bubbleScale = 0;

        void Start()
        {
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
                

                if (handtracking.twoHands)
                {
                    float distanceToHand = Vector3.Distance(transform.position, handtracking.rightPalm.Position);
                    float handRecognitionRange = userProximitySet - handProximitySet;

                    if (distanceToHand > handProximitySet && distanceToHand <= handRecognitionRange) bubbleScale = 1 - (distanceToHand - handProximitySet) / (handRecognitionRange);
                    else if (distanceToHand > handRecognitionRange) bubbleScale = 0;
                    else if (distanceToHand <= handProximitySet) bubbleScale = 1;

                    proximityBubble.localScale = new Vector3(bubbleScale, bubbleScale, bubbleScale);
                }
            }
            else
            {
                drummer.SetCast(true);
            }
        }

        void CheckDistanceToUser()
        {
            
        }
    }
}
