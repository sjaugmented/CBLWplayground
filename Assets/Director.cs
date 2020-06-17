using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [SerializeField] GameObject topLevelMenu;
    [SerializeField] Vector3 menuOffset;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] float menuTimer = 5f;
    [SerializeField] float menuSelectDelay = 0.5f;
    public List<Renderer> selectorPlatonics = new List<Renderer>();

    public enum Mode { Magic, RGB, Precision, Throw, ResetDMX };
    public Mode currentMode = Mode.Magic;
    int modeIndex = 1;
    bool menuActive = false;
    
    HandTracking handTracking;

    MagicController magicController;
    RGBController rgbController;
    PrecisionController precisionController;
    ThrowController throwController;

    GameObject magicComponents;
    GameObject rgbComponents;
    GameObject precisionComponents;
    
    // Start is called before the first frame update
    void Start()
    {
        handTracking = FindObjectOfType<HandTracking>();

        magicController = FindObjectOfType<MagicController>();
        rgbController = FindObjectOfType<RGBController>();
        precisionController = FindObjectOfType<PrecisionController>();
        throwController = FindObjectOfType<ThrowController>();

        magicComponents = FindObjectOfType<MagicID>().gameObject;
        rgbComponents = FindObjectOfType<RGBID>().gameObject;
        precisionComponents = FindObjectOfType<PrecisionID>().gameObject;


    }

    

    // Update is called once per frame
    void Update()
    {
        ConvertIndex();
        SetSelectorMaterial();

        if (handTracking.pullUps)
        {
            if (!menuActive)
            {
                menuActive = true;
                topLevelMenu.SetActive(true);
                StartCoroutine("MenuTimeOut", menuTimer);

                Vector3 midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
                topLevelMenu.transform.position = midpointPalms + Camera.main.transform.forward;
                topLevelMenu.transform.localRotation = Camera.main.transform.rotation;
            }
            else return;
            
        }
        else topLevelMenu.SetActive(false);
        
    }

    IEnumerator MenuTimeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        topLevelMenu.SetActive(false);
    }

    private void ConvertIndex()
    {
        if (currentMode == Mode.Magic)
        {
            modeIndex = 0;
            SetGameObjects(true, false, false, false);
        }

        if (currentMode == Mode.RGB)
        {
            modeIndex = 1;
            SetGameObjects(false, true, false, false);
        }

        if (currentMode == Mode.Precision)
        {
            modeIndex = 2;
            SetGameObjects(false, false, true, false);
        }

        if (currentMode == Mode.Throw)
        {
            modeIndex = 3;
            SetGameObjects(false, false, false, true);
        }

        if (currentMode == Mode.ResetDMX)
        {
            modeIndex = 4;
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

    private void SetGameObjects(bool mag, bool rgb, bool prec, bool thrower)
    {
        magicController.enabled = mag;
        rgbController.enabled = rgb;
        precisionController.enabled = prec;
        throwController.enabled = thrower;

        magicComponents.SetActive(mag);
        rgbComponents.SetActive(rgb);
        precisionComponents.SetActive(prec);
        
    }

    #region GazeFunctions
    public void MagicMode()
    {
        currentMode = Mode.Magic;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
    }

    public void RGBMode()
    {
        currentMode = Mode.RGB;
        StartCoroutine("MenuTimeOut", menuSelectDelay);

    }

    public void PrecisionMode()
    {
        currentMode = Mode.Precision;
        StartCoroutine("MenuTimeOut", menuSelectDelay);

    }

    public void ThrowMode()
    {
        currentMode = Mode.Throw;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
    }

    public void ResetDMX()
    {
        DMXcontroller dmx = FindObjectOfType<DMXcontroller>();
        dmx.ResetDMX();
        currentMode = Mode.ResetDMX;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
    }
    #endregion
}
