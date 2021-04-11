using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Core
{
    public class MultiAxis : MonoBehaviour
    {
        public Vector3 average;
        public float forwardStick, upStick, rightAgainstStaffForward, crossAngle;

        public float ZStick { get { return forwardStick; } }
        public float YStick { get { return upStick; } }
        public float StaffAngle { get { return rightAgainstStaffForward; } }
        public float CrossAngle { get { return crossAngle; } }

        NewTracking tracking;
        Transform floor;
        void Start()
        {
            tracking = GameObject.FindGameObjectWithTag("HandTracking").GetComponent<NewTracking>();
            floor = GameObject.FindGameObjectWithTag("Floor").GetComponent<LevelObject>().transform;
        }

        void Update()
        {
            // base all axis off hands together; together == 0

            // average palms.down
            var rightPalm = tracking.GetRtPalm;
            var leftPalm = tracking.GetLtPalm;
            var rightDown = (rightPalm.Up * -1).normalized;
            var rightRight = rightPalm.Right.normalized;
            var leftDown = (leftPalm.Up * -1).normalized;
            average = (rightDown + leftDown).normalized;

            // compare against cam.forward, cam.up
            forwardStick = Vector3.Angle(rightDown, Camera.main.transform.forward);
            upStick = Vector3.Angle(rightDown, floor.up);
            rightAgainstStaffForward = Vector3.Angle(rightRight, tracking.Staff);
            Vector3 crossStaff = Vector3.Cross(tracking.Staff, Vector3.up).normalized;
            crossAngle = Vector3.Angle(rightDown, crossStaff);



            // FORWARD
            // if average < 45 degrees
            // push force = average / 45 (possibly the inverse since you're heading toward zero (forward == 0))

            // if average > 45 degrees
            // pull force = (average - 90) / 45 ?

            // UP
            // if average < 45 degrees
            // lift force = average / 45 (possibly the inverse since you're heading toward zero (forward == 0))

            // if average > 45 degrees
            // down force = (average - 90) / 45 ?
        }
    }
}

