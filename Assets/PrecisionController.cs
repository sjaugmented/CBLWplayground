using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionController : MonoBehaviour
{
    [SerializeField] GameObject tetherOn;
    [SerializeField] GameObject tetherOff;
    [SerializeField] GameObject brightness;
    [SerializeField] GameObject kelvin;

    PrecisionPoseTracker poseTracker;

    // Start is called before the first frame update
    void Start()
    {
        poseTracker = GetComponent<PrecisionPoseTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
