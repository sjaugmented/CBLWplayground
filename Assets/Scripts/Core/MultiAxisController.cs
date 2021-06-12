using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class MultiAxisController : MonoBehaviour
    {
        public static MultiAxisController Instance;
        
        [SerializeField] float deadZone = 40;
        [SerializeField] Vector3 outOffset;
        [SerializeField] Vector3 inOffset;

        public float palmRightAgainstStaffForward, staffAgainstCamForward;

        public float StaffRight { get { return palmRightAgainstStaffForward; } }
        public float StaffForward { get { return staffAgainstCamForward; } }
        public float DeadZone { get { return deadZone; } }
        public Vector3 OutOffset { get { return outOffset; } }
        public Vector3 InOffset { get { return inOffset; } }

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            var rightPalm = NewTracking.Instance.GetRtPalm;
            var rightPalmRight = rightPalm.Right.normalized;

            palmRightAgainstStaffForward = Vector3.Angle(rightPalmRight, NewTracking.Instance.Staff);
            staffAgainstCamForward = Vector3.Angle(NewTracking.Instance.Staff, Camera.main.transform.forward);
        }
    }
}

