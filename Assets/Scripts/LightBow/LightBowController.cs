using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.LightBow
{
    public class LightBowController : MonoBehaviour
    {
        
        
        HandTracking handtracking;

        // Start is called before the first frame update
        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
