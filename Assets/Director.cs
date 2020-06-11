using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [SerializeField] GameObject topLevelMenu;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    public List<Renderer> selectorPlatonics = new List<Renderer>();

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

    }

    

    // Update is called once per frame
    void Update()
    {
        ConvertIndex();
        SetSelectorMaterial();

        if (handTracking.pullUps) topLevelMenu.SetActive(true);
        else topLevelMenu.SetActive(false);
        
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

    private void SetSelectorMaterial()
    {
        for (int i = 0; i < selectorPlatonics.Count; i++)
        {
            if (i == modeIndex)
            {
                selectorPlatonics[i].material = selectedMaterial;
            }
            else
            {
                selectorPlatonics[i].material = defaultMaterial;
            }
        }
    }

    #region GazeFunctions
    public void MagicMode()
    {
        currentMode = Mode.Magic;

        magic.enabled = true;
        //rgb.enabled = false;
        precisionPose.enabled = false;
        precision.enabled = false;
    }

    public void RGBMode()
    {
        currentMode = Mode.RGB;

    }

    public void PrecisionMode()
    {
        currentMode = Mode.Precision;

        magic.enabled = false;
        //rgb.enabled = false;
        precisionPose.enabled = true;
        precision.enabled = true;
    }
    #endregion
}
