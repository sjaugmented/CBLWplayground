using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [SerializeField] GameObject topLevelMenu;
    [SerializeField] Vector3 menuOffset;
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

    GameObject masterOrb;    
    GameObject rightStreams;
    GameObject leftStreams;
    GameObject rightFloat;
    GameObject leftFloat;
    GameObject rightHUD;
    GameObject leftHUD;
    GameObject skyPanelBox;
    GameObject spotBox;
    GameObject clearBox;
    
    // Start is called before the first frame update
    void Start()
    {
        handTracking = FindObjectOfType<OrbFingerTracker>();
        magic = FindObjectOfType<SpellManager>();
        precisionPose = FindObjectOfType<PrecisionPoseTracker>();
        precision = FindObjectOfType<PrecisionController>();

        masterOrb = FindObjectOfType<MasterOrbRotater>().gameObject;
        rightStreams = FindObjectOfType<RightStreams>().gameObject;
        leftStreams = FindObjectOfType<LeftStreams>().gameObject;
        rightFloat = FindObjectOfType<RightFloat>().gameObject;
        leftFloat = FindObjectOfType<LeftFloat>().gameObject;
        rightHUD = FindObjectOfType<RightHUD>().gameObject;
        leftHUD = FindObjectOfType<LeftHUD>().gameObject;
        skyPanelBox = FindObjectOfType<SkyPanelBox>().gameObject;
        spotBox = FindObjectOfType<SpotBox>().gameObject;
        clearBox = FindObjectOfType<ClearBox>().gameObject;

    }

    

    // Update is called once per frame
    void Update()
    {
        ConvertIndex();
        SetSelectorMaterial();

        if (handTracking.pullUps)
        {
            topLevelMenu.SetActive(true);

            Vector3 midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
            topLevelMenu.transform.position = midpointPalms + menuOffset;
        }
        else topLevelMenu.SetActive(false);
        
    }

    private void ConvertIndex()
    {
        if (currentMode == Mode.Magic)
        {
            modeIndex = 0;
            SetGameObjects(true, false, false);
        }

        if (currentMode == Mode.RGB)
        {
            modeIndex = 1;
            SetGameObjects(false, true, false);
        }

        if (currentMode == Mode.Precision)
        {
            modeIndex = 2;
            SetGameObjects(false, false, true);
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

    private void SetGameObjects(bool mag, bool rgb, bool prec)
    {
        magic.enabled = mag;
        //rgb.enabled = rgb;
        precisionPose.enabled = prec;
        precision.enabled = prec;

        masterOrb.SetActive(mag);
        rightStreams.SetActive(mag);
        leftStreams.SetActive(mag);

        rightFloat.SetActive(prec);
        leftFloat.SetActive(prec);
        rightHUD.SetActive(prec);
        leftHUD.SetActive(prec);
        skyPanelBox.SetActive(prec);
        spotBox.SetActive(prec);
        clearBox.SetActive(prec);
    }

    #region GazeFunctions
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
    #endregion
}
