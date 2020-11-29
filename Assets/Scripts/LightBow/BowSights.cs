using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.LightBow
{
    public class BowSights : MonoBehaviour
    {
        HandTracking handtracking;

        public Vector3 rightIndex, rightMiddle, leftIndex, leftMiddle;

        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();

            rightIndex = handtracking.rtIndexTip.Position;
            rightMiddle = handtracking.rtMiddleTip.Position;
            leftIndex = handtracking.ltIndexTip.Position;
            leftMiddle = handtracking.ltMiddleTip.Position;
        }

        void Update()
        {

        }

        public Vector3 GetRightSight()
        {
            return Vector3.Lerp(rightIndex, rightMiddle, 0.5f);
        }

        public Vector3 GetLeftSight()
        {
            return Vector3.Lerp(leftIndex, leftMiddle, 0.5f);
        }
    }
}
