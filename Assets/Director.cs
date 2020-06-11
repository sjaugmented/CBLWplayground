using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [SerializeField] Material defaultSelector;
    [SerializeField] Material selectedSelector;
    public List<GameObject> selectorPlatonics = new List<GameObject>();

    public enum Mode { Magic, RGB, Precision};
    public Mode currentMode = Mode.Magic;
    public int modeIndex = 1;
    
    OrbFingerTracker handTracking;
    SpellManager magic;
    PrecisionPoseTracker precisionPose;
    PrecisionController precision;
    
    // Start is called before the first frame update
    void Start()
    {
        handTracking = FindObjectOfType<OrbFingerTracker>();
        magic = FindObjectOfType<SpellManager>();
        precisionPose = FindObjectOfType<PrecisionPoseTracker>();
        precision = FindObjectOfType<PrecisionController>();

        MagicMode();
    }

    private void ConvertIndex()
    {
        if (currentMode == Mode.Magic)
        {
            modeIndex = 1;
        }

        if (currentMode == Mode.RGB)
        {
            modeIndex = 2;
        }

        if (currentMode == Mode.Precision)
        {
            modeIndex = 3;
        }
    }

    public void MagicMode()
    {
        currentMode = Mode.Magic;
    }

    public void RGBMode()
    {
        currentMode = Mode.RGB;

    }

    public void PrecisionMode()
    {
        currentMode = Mode.Precision;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
