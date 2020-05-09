using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackerTest : MonoBehaviour
{
    [SerializeField] GameObject vectors;
    [SerializeField] GameObject readOuts;
    [SerializeField] TextMeshPro rpuLPU;
    [SerializeField] TextMeshPro rpuCF;
    [SerializeField] TextMeshPro rpfCF;
    [SerializeField] TextMeshPro rifRPF;
    [SerializeField] TextMeshPro rmfRPF;
    [SerializeField] TextMeshPro rtfRPF;
    [SerializeField] TextMeshPro rpifRPF;
    [SerializeField] TextMeshPro lprCF;
    [SerializeField] TextMeshPro lprRPR;
    [SerializeField] TextMeshPro lprCR;

    public bool vectorsOn = false;

    MixedRealityPose rightPalm, leftPalm, rightIndex, leftIndex, rightMiddle, leftMiddle, rightPinky, leftPinky, rightThumb, leftThumb;

    void Start()
    {
        DeactivateVectors();
    }

    void Update()
    {
        ProcessVectors(); 
    }

    private void ProcessVectors()
    {
        Transform cam = Camera.main.transform;

        // if both palms + right index, middle, pinky & thumb
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rightIndex) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rightThumb) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rightMiddle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rightPinky))
        {

            rpuLPU.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Up, leftPalm.Up)).ToString();
            rpuCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Up, cam.forward)).ToString();
            rpfCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Forward, cam.forward)).ToString();
            rifRPF.text = Mathf.RoundToInt(Vector3.Angle(rightIndex.Forward, rightPalm.Forward)).ToString();
            rmfRPF.text = Mathf.RoundToInt(Vector3.Angle(rightMiddle.Forward, rightPalm.Forward)).ToString();
            rtfRPF.text = Mathf.RoundToInt(Vector3.Angle(rightThumb.Forward, rightPalm.Forward)).ToString();
            rpifRPF.text = Mathf.RoundToInt(Vector3.Angle(rightPinky.Forward, rightPalm.Forward)).ToString();
            lprCF.text = Mathf.RoundToInt(Vector3.Angle(leftPalm.Right, cam.forward)).ToString();
            lprRPR.text = Mathf.RoundToInt(Vector3.Angle(leftPalm.Right, rightPalm.Right)).ToString();
            lprCR.text = Mathf.RoundToInt(Vector3.Angle(leftPalm.Right, cam.right)).ToString();
        }

        // if only right hand
        else if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rightIndex) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rightThumb) && HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rightMiddle) && HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rightPinky))
        {
            rpuCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Up, cam.forward)).ToString();
            rpfCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Forward, cam.forward)).ToString();
            rifRPF.text = Mathf.RoundToInt(Vector3.Angle(rightIndex.Forward, rightPalm.Forward)).ToString();
            rmfRPF.text = Mathf.RoundToInt(Vector3.Angle(rightMiddle.Forward, rightPalm.Forward)).ToString();
            rtfRPF.text = Mathf.RoundToInt(Vector3.Angle(rightThumb.Forward, rightPalm.Forward)).ToString();
            rpifRPF.text = Mathf.RoundToInt(Vector3.Angle(rightPinky.Forward, rightPalm.Forward)).ToString();
        }

        // if only right palm, index & thumb
        else if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rightIndex) && HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rightThumb))
        {
            rpuCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Up, cam.forward)).ToString();
            rpfCF.text = Mathf.RoundToInt(Vector3.Angle(rightPalm.Forward, cam.forward)).ToString();
            rifRPF.text = Mathf.RoundToInt(Vector3.Angle(rightIndex.Forward, rightPalm.Forward)).ToString();
            rtfRPF.text = Mathf.RoundToInt(Vector3.Angle(rightThumb.Forward, rightPalm.Forward)).ToString();
        }

        // if only left palm
        else if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            lprCF.text = Mathf.RoundToInt(Vector3.Angle(leftPalm.Right, cam.forward)).ToString();
            lprCR.text = Mathf.RoundToInt(Vector3.Angle(leftPalm.Right, cam.right)).ToString();
        }
        else return;

        // if joints unavailable, mark text field as "NA"
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out rightPalm))
        {
            rpuLPU.text = "RPNA";
            rpuCF.text = "RPNA";
            rpfCF.text = "RPNA";
            rifRPF.text = "RPNA";
            rmfRPF.text = "RPNA";
            rtfRPF.text = "RPNA";
            rpifRPF.text = "RPNA";
            lprRPR.text = "RPNA";
        }
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out leftPalm))
        {
            rpuLPU.text = "LPNA";
            lprCF.text = "LPNA";
            lprRPR.text = "LPNA";
            lprCR.text = "LPNA";
        }
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out rightIndex))
        {
            rifRPF.text = "RINA";
        }
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out rightMiddle))
        {
            rmfRPF.text = "RMNA";
        }
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out rightThumb))
        {
            rtfRPF.text = "RTNA";
        }
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out rightPinky))
        {
            rpifRPF.text = "RPiNA";
        }
    }

    public void ToggleVectors()
    {
        if (!vectorsOn)
        {
            ActivateVectors();
        }
        else
        {
            DeactivateVectors();
        }

    }

    private void ActivateVectors()
    {
        vectorsOn = true;

        vectors.SetActive(true);
        readOuts.SetActive(true);
    }


    private void DeactivateVectors()
    {
        vectorsOn = false;

        vectors.SetActive(false);
        readOuts.SetActive(false);

    }
}
