using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class StaffTester : MonoBehaviour
{
    [SerializeField] Transform wizardStaff;
    [SerializeField] TextMeshPro floorUpText;
    [SerializeField] TextMeshPro floorForText;
    [SerializeField] TextMeshPro camUpText;
    [SerializeField] TextMeshPro camForText;

    HandTracking handTracker;

    // Start is called before the first frame update
    void Start()
    {
        handTracker = FindObjectOfType<HandTracking>();
    }

    // Update is called once per frame
    void Update()
    {
        if (handTracker.twoHands)
        {
            wizardStaff.gameObject.SetActive(true);
            wizardStaff.position = Vector3.Lerp(handTracker.rightPalm.Position, handTracker.leftPalm.Position, 0.5f);
            float staffLength = Vector3.Distance(handTracker.rightPalm.Position, handTracker.leftPalm.Position);
            wizardStaff.localScale = new Vector3(0.004223645f, 0.004223645f, staffLength);
            Vector3 direction = handTracker.rightPalm.Position - handTracker.leftPalm.Position;
            wizardStaff.rotation = Quaternion.LookRotation(direction, Camera.main.transform.up);
            floorUpText.text = Mathf.RoundToInt(handTracker.GetFloorUp()).ToString();
            floorForText.text = Mathf.RoundToInt(handTracker.GetFloorForward()).ToString();
            camUpText.text = Mathf.RoundToInt(handTracker.GetCamUp()).ToString();
            camForText.text = Mathf.RoundToInt(handTracker.GetCamForward()).ToString();
        }
        else wizardStaff.gameObject.SetActive(false);
    }
}
