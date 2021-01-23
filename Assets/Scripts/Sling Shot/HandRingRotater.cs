using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRingRotater : MonoBehaviour
{
    [SerializeField] bool leftHand = false;
    [SerializeField] float rateOfRotation = 1;

    void Update()
    {
        if (leftHand) transform.Rotate(0, rateOfRotation, 0);
        else transform.Rotate(0, -rateOfRotation, 0);
    }
}
