using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core
{
    public class CastOrigins : MonoBehaviour
    {
        public float palmDist;
        public Vector3 midpointhandtracking, rightStreamPos, leftStreamPos;

        // HandTracking handtracking;
        NewTracking tracking;

        private void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
        }

        private void Update()
        {
            CalcHandPositions();
        }

        private void CalcHandPositions()
        {
            palmDist = Vector3.Distance(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position);

            midpointhandtracking = Vector3.Lerp(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position, 0.5f);
            rightStreamPos = Vector3.Lerp(tracking.GetRtIndex.Position, tracking.GetRtPinky.Position, 0.5f);
            leftStreamPos = Vector3.Lerp(tracking.GetLtIndex.Position, tracking.GetLtPinky.Position, 0.5f);
        }
    }
}
