using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class MultiAxisController : MonoBehaviour
    {
        [SerializeField] float deadZone = 40;
        [SerializeField] Vector3 outOffset;
        [SerializeField] Vector3 inOffset;

        public float palmRightAgainstStaffForward, staffAgainstCamForward;

        public float StaffRight { get { return palmRightAgainstStaffForward; } }
        public float StaffForward { get { return staffAgainstCamForward; } }
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
            var rightPalm = tracking.GetRtPalm;
            var rightPalmRight = rightPalm.Right.normalized;

            palmRightAgainstStaffForward = Vector3.Angle(rightPalmRight, tracking.Staff);
            staffAgainstCamForward = Vector3.Angle(tracking.Staff, Camera.main.transform.forward);
        }
    }
}

