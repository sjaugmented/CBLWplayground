using UnityEngine;

public class CastVsHoverController : MonoBehaviour
{
    MagicController spellManager;
    OrbCastController castController;
    OrbHoverController hoverController;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<MagicController>();
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
