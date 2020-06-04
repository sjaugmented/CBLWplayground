using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastVsHoverController : MonoBehaviour
{
    SpellManager spellManager;
    OrbCastController castController;
    OrbHoverController hoverController;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<SpellManager>();
        castController = GetComponent<OrbCastController>();
        hoverController = GetComponent<OrbHoverController>();

        if (spellManager.hoverOrb)
        {
            castController.enabled = false;
            hoverController.enabled = true;
        }
        else
        {
            castController.enabled = true;
            hoverController.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
