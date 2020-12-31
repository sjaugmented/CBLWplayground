using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

public class ThreeFingerTest : MonoBehaviour
{
    HandTracking hands;

    void Start()
    {
        hands = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
    }

    void Update()
    {
        if (hands.rightThree || hands.leftThree) GetComponent<MeshRenderer>().enabled = true;
		else GetComponent<MeshRenderer>().enabled = false;
	}
}
