using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HoverDrums
{
    public class DrumProximitySensor : MonoBehaviour
    {
        [SerializeField] float proximitySet = 0.3f;
        [SerializeField] GameObject proximityBubble;
        
        Transform user;
        HoverDrummer drummer;

        bool withinRange;

        void Start()
        {
            user = Camera.main.transform;
            drummer = GameObject.FindGameObjectWithTag("Drummer").GetComponent<HoverDrummer>();
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, user.position);
            if (distance < proximitySet)
            {
                withinRange = true;
                proximityBubble.SetActive(true);
                drummer.SetCast(false);
            }
            else
            {
                withinRange = false;
                proximityBubble.SetActive(false);
                drummer.SetCast(true);
            }
        }

        void CheckDistanceToUser()
        {
            
        }
    }
}
