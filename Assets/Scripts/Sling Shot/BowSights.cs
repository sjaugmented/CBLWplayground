using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.SlingShot
{
    public class BowSights : MonoBehaviour
    {
        HandTracking handtracking;

        public Vector3 rightSight, leftSight;

        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
        }

        void Update()
        {
            rightSight = Vector3.Lerp(handtracking.rtIndexTip.Position, handtracking.rtMiddleTip.Position, 0.5f);
            leftSight = Vector3.Lerp(handtracking.ltIndexTip.Position, handtracking.ltMiddleTip.Position, 0.5f);
        }

        public Vector3 GetRightSight()
        {
            return rightSight;
        }

        public Vector3 GetLeftSight()
        {
            return leftSight;
        }
    }
}
