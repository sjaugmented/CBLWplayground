using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class MultiAxisController : MonoBehaviour
    {
        [SerializeField] float deadZone = 15;
        [Tooltip("Angle correction for palms out - typically only requires correction of the X vector axis. Recommend 60 degrees.")]
        [SerializeField] Vector3 outOffset = new Vector3(60, 0, 0);
        [Tooltip("Angle correction for palms in - typically only requires correction of the X vector axis. Recommend 120 degrees.")]
        [SerializeField] Vector3 inOffset = new Vector3(120, 0, 0);

        //float palmRightAgainstStaffForward, staffAgainstCamForward;

        /// <summary>
        /// Palm.right vs Staff.forward.
        /// We use this to compare palm angles with neutral. This is the main throttle, how we turn two hands into joysticks. 
        /// Neutral/center stick is 90 degrees. As palms rotate away we head toward 0; as they rotate toward, we head toward 180.
        /// </summary>
        public float PalmRightStaffForward { get; set; }
        /// <summary>
        /// Staff.forward vs Camera.forward.
        /// We use this for secondary Jedi - left and right push on the ball. This is like a steering wheel for a bus - a flat rotation. 
        /// Neutral is 90 degrees. As staff rotates left, we head toward 0; rotating right heads toward 180.
        /// </summary>
        public float StaffForwardCamForward { get; set; }
        public float DeadZone { get { return deadZone; } }
        public Vector3 OutOffset { get { return outOffset; } }
        public Vector3 InOffset { get { return inOffset; } }

        NewTracking tracking;

        private void Start()
        {
            tracking = GetComponent<NewTracking>();
        }

        void Update()
        {
            MixedRealityPose rightPalm = tracking.GetRtPalm;
            Vector3 rightPalmRight = rightPalm.Right.normalized;
            PalmRightStaffForward = Vector3.Angle(rightPalmRight, tracking.Staff);
            StaffForwardCamForward = Vector3.Angle(tracking.Staff, Camera.main.transform.forward);
        }
    }
}

