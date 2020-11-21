using TMPro;
using UnityEngine;
using LW.Core;

public class StaffTester : MonoBehaviour
{
    [SerializeField] Transform wizardStaff;
    [SerializeField] TextMeshPro floorUpText;
    [SerializeField] TextMeshPro camRightText;
    [SerializeField] TextMeshPro camUpText;
    [SerializeField] TextMeshPro camForText;

    HandTracking handTracker;

    void Awake()
    {
        handTracker = FindObjectOfType<HandTracking>();
    }

    // Start is called before the first frame update
    void Start()
    {

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
            camRightText.text = Mathf.RoundToInt(handTracker.GetCamRight()).ToString();
            camUpText.text = Mathf.RoundToInt(handTracker.GetCamUp()).ToString();
            camForText.text = Mathf.RoundToInt(handTracker.GetCamForward()).ToString();
        }
        else wizardStaff.gameObject.SetActive(false);
    }
}
