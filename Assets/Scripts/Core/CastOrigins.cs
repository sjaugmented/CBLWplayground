using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LW.Core
{
    public class CastOrigins : MonoBehaviour
    {
        public static CastOrigins Instance;
        public float PalmsDist { get; set; }
        public Quaternion CastRotation { get; set; }
        public Vector3 PalmsMidpoint { get; set; }
        public Vector3 RightStreamOrigin { get; set; }
        public Vector3 LeftStreamOrigin { get; set; }

        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
            PalmsDist = Vector3.Distance(NewTracking.Instance.GetRtPalm.Position, NewTracking.Instance.GetLtPalm.Position);
            CastRotation = Quaternion.Slerp(NewTracking.Instance.GetRtPalm.Rotation, NewTracking.Instance.GetLtPalm.Rotation, 0.5f) * Quaternion.Euler(60, 0, 0);
            PalmsMidpoint = Vector3.Lerp(NewTracking.Instance.GetRtPalm.Position, NewTracking.Instance.GetLtPalm.Position, 0.5f);
            RightStreamOrigin = Vector3.Lerp(NewTracking.Instance.GetRtIndex.Position, NewTracking.Instance.GetRtPinky.Position, 0.5f);
            LeftStreamOrigin = Vector3.Lerp(NewTracking.Instance.GetLtIndex.Position, NewTracking.Instance.GetLtPinky.Position, 0.5f);
        }
    }
}
