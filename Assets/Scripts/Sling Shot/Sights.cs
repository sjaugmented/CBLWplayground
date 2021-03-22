using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.SlingShot
{
    public class Sights : MonoBehaviour
    {
        NewTracking tracking;

        public Vector3 rightSight, leftSight;

        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
        }

        void Update()
        {
            rightSight = Vector3.Lerp(tracking.GetRtIndex.Position, tracking.GetRtMiddle.Position, 0.5f);
            leftSight = Vector3.Lerp(tracking.GetLtIndex.Position, tracking.GetLtMiddle.Position, 0.5f);
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
