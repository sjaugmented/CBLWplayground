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
        Transform floor;
        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<LevelObject>().transform;
        }

        void Update()
        {
            var rightPalm = tracking.GetRtPalm;
            var rightRight = rightPalm.Right.normalized;

            palmRightAgainstStaffForward = Vector3.Angle(rightRight, tracking.Staff);
            staffAgainstCamForward = Vector3.Angle(tracking.Staff, Camera.main.transform.forward);
        }
    }
}

