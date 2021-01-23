using UnityEngine;

namespace LW.Core
{
    public class CastOrigins : MonoBehaviour
    {
        public float palmDist;
        public Vector3 midpointhandtracking, rightStreamPos, leftStreamPos;

        HandTracking handtracking;

        private void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<HandTracking>();
        }

        private void Update()
        {
            CalcHandPositions();
        }

        private void CalcHandPositions()
        {
            palmDist = Vector3.Distance(handtracking.rightPalm.Position, handtracking.leftPalm.Position);

            midpointhandtracking = Vector3.Lerp(handtracking.rightPalm.Position, handtracking.leftPalm.Position, 0.5f);
            rightStreamPos = Vector3.Lerp(handtracking.rtIndexTip.Position, handtracking.rtPinkyTip.Position, 0.5f);
            leftStreamPos = Vector3.Lerp(handtracking.ltIndexTip.Position, handtracking.ltPinkyTip.Position, 0.5f);
        }
    }
}
