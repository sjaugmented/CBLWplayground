using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;

namespace LW.Core
{
    public class CastOrigins : MonoBehaviourPunCallbacks
    {
        public float PalmsDist { get; set; }
        public Quaternion CastRotation { get; set; }
        public Vector3 PalmsMidpoint { get; set; }
        public Vector3 RightStreamOrigin { get; set; }
        public Vector3 LeftStreamOrigin { get; set; }

        NewTracking tracking;

        private void Start()
        {
            tracking = GetComponent<NewTracking>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                PalmsDist = Vector3.Distance(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position);
                CastRotation = Quaternion.Slerp(tracking.GetRtPalm.Rotation, tracking.GetLtPalm.Rotation, 0.5f) * Quaternion.Euler(60, 0, 0);
                PalmsMidpoint = Vector3.Lerp(tracking.GetRtPalm.Position, tracking.GetLtPalm.Position, 0.5f);
                RightStreamOrigin = Vector3.Lerp(tracking.GetRtIndex.Position, tracking.GetRtPinky.Position, 0.5f);
                LeftStreamOrigin = Vector3.Lerp(tracking.GetLtIndex.Position, tracking.GetLtPinky.Position, 0.5f);
            }
        }
    }
}
