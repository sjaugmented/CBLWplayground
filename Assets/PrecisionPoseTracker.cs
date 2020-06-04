using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionPoseTracker : MonoBehaviour
{
    // right hand joints
    public MixedRealityPose rightPalm, rtIndexTip, rtIndexMid, rtIndexKnuckle, rtMiddleTip, rtMiddleKnuckle, rtPinkyTip, rtThumbTip;
    // left hand joints
    public MixedRealityPose leftPalm, ltIndexTip, ltIndexMid, ltIndexKnuckle, ltMiddleTip, ltMiddleKnuckle, ltPinkyTip, ltThumbTip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform floor = FindObjectOfType<LevelObject>().transform;
        Transform cam = Camera.main.transform;

        #region Palm Ref Angles
        // right palm
        float rtPalmUpCamFor = Vector3.Angle(rightPalm.Up, cam.forward);
        float rtPalmForCamFor = Vector3.Angle(rightPalm.Forward, cam.forward);
        float rtPalmRtCamFor = Vector3.Angle(rightPalm.Right, cam.forward);
        float rtPalmRtCamRt = Vector3.Angle(rightPalm.Right, cam.right);
        float rtPalmRtCamUp = Vector3.Angle(rightPalm.Right, cam.up);
        float rtPalmForFloorFor = Vector3.Angle(rightPalm.Forward, floor.forward);
        float rtPalmRtFloorFor = Vector3.Angle(rightPalm.Right, floor.forward);
        float rtPalmUpCamRt = Vector3.Angle(rightPalm.Up, cam.right);
        float rtPalmForCamRt = Vector3.Angle(rightPalm.Forward, cam.right);

        // left palm
        float ltPalmUpCamFor = Vector3.Angle(leftPalm.Up, cam.forward);
        float ltPalmForCamFor = Vector3.Angle(leftPalm.Forward, cam.forward);
        float ltPalmRtCamFor = Vector3.Angle(leftPalm.Right, cam.forward);
        float ltPalmRtCamRt = Vector3.Angle(leftPalm.Right, cam.right);
        float ltPalmRtCamUp = Vector3.Angle(leftPalm.Right, cam.up);
        float ltPalmForFloorFor = Vector3.Angle(leftPalm.Forward, floor.forward);
        float ltPalmRtFloorFor = Vector3.Angle(leftPalm.Right, floor.forward);
        float ltPalmUpCamRt = Vector3.Angle(leftPalm.Up, cam.right);
        float ltPalmForCamRt = Vector3.Angle(leftPalm.Forward, cam.right);
        #endregion

        #region Finger Ref Angles
        // get right finger angles
        float rtIndForPalmFor = Vector3.Angle(rtIndexTip.Forward, rightPalm.Forward);
        float rtIndForCamFor = Vector3.Angle(rtIndexTip.Forward, cam.forward);
        float rtMidForPalmFor = Vector3.Angle(rtMiddleTip.Forward, rightPalm.Forward);
        float rtPinkForPalmFor = Vector3.Angle(rtPinkyTip.Forward, rightPalm.Forward);
        float rtThumbForCamFor = Vector3.Angle(rtThumbTip.Forward, cam.forward);
        float rtThumbForPalmFor = Vector3.Angle(rtThumbTip.Forward, rightPalm.Forward);
        float rtIndMidForPalmFor = Vector3.Angle(rtIndexMid.Forward, rightPalm.Forward);
        float rtIndKnuckForPalmFor = Vector3.Angle(rtIndexKnuckle.Forward, rightPalm.Forward);

        // get left finger angles
        float ltIndForPalmFor = Vector3.Angle(ltIndexTip.Forward, leftPalm.Forward);
        float ltIndForCamFor = Vector3.Angle(ltIndexTip.Forward, cam.forward);
        float ltMidForPalmFor = Vector3.Angle(ltMiddleTip.Forward, leftPalm.Forward);
        float ltPinkForPalmFor = Vector3.Angle(ltPinkyTip.Forward, leftPalm.Forward);
        float ltThumbForCamFor = Vector3.Angle(ltThumbTip.Forward, cam.forward);
        float ltThumbForPalmFor = Vector3.Angle(ltThumbTip.Forward, leftPalm.Forward);
        float ltIndMidForPalmFor = Vector3.Angle(ltIndexMid.Forward, leftPalm.Forward);
        float ltIndKnuckForPalmFor = Vector3.Angle(ltIndexKnuckle.Forward, leftPalm.Forward);
        #endregion


    }
}
