using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    HandTracking handTracker;
    SpellBook spellBook;

    float castThreshold; 

    // Start is called before the first frame update
    void Start()
    {
        handTracker = FindObjectOfType<HandTracking>();
        spellBook = FindObjectOfType<SpellBook>();
    }

    float previousDist;
    public float magnifier = 10;
    public float velocity;

    // Update is called once per frame
    void Update()
    {
        if (handTracker.rightThrower)
        {
            float currentDist = Vector3.Distance(Camera.main.transform.position, handTracker.rightPalm.Position);

            velocity = (previousDist - currentDist) / Time.deltaTime * magnifier;

            previousDist = Vector3.Distance(Camera.main.transform.position, handTracker.rightPalm.Position);


        }
    }
}
