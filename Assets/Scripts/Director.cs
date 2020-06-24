using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Director : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject topLevelMenu;
    [SerializeField] float zOffset;
    [SerializeField] float yOffset;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] float menuTimer = 5f;
    [SerializeField] float menuSelectDelay = 0.5f;

    [Header("Mode Touch Toggles")]
    [SerializeField] GameObject leftHandToggle;

    public List<Renderer> selectorPlatonics = new List<Renderer>();

    public enum Mode { Magic, RGB, Precision, Staff };
    public Mode currentMode = Mode.Magic;
    int modeIndex = 1;
    public bool menuActive = false;

    List<Vector3> platonicStartPos = new List<Vector3>();
    
    HandTracking handTracking;

    MagicController magicController;
    //RGBController rgbController;
    PrecisionController precisionController;
    ThrowController throwController;
    StaffTester staffController;

    GameObject magicComponents;
    //GameObject rgbComponents;
    GameObject precisionComponents;
    GameObject staffComponents;

    bool chinUpsActive = false;

    public bool readGestures = true;
    [SerializeField] GameObject gestureToggleObj;
    [SerializeField] GameObject gesturesOffHUD;

    public void ToggleGestures()
    {
        readGestures = !readGestures;
    }

    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();

        magicController = FindObjectOfType<MagicController>();
        //rgbController = FindObjectOfType<RGBController>();
        precisionController = FindObjectOfType<PrecisionController>();
        throwController = FindObjectOfType<ThrowController>();
        staffController = FindObjectOfType<StaffTester>();


        magicComponents = FindObjectOfType<MagicID>().gameObject;
        //rgbComponents = FindObjectOfType<RGBID>().gameObject;
        precisionComponents = FindObjectOfType<PrecisionID>().gameObject;
        staffComponents = FindObjectOfType<StaffTester>().gameObject;

        magicController.enabled = false;
        //rgbController.enabled = false;
        precisionController.enabled = false;
        throwController.enabled = false;
        staffController.enabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        topLevelMenu.SetActive(false);
        gestureToggleObj.SetActive(false);

        foreach (Renderer selector in selectorPlatonics)
        {
            Vector3 startingPos = selector.transform.parent.localPosition;
            platonicStartPos.Add(startingPos);
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if (readGestures)
        {
            gesturesOffHUD.SetActive(false);
        }
        else
        {
            if (handTracking.rightHand) gesturesOffHUD.SetActive(true);
            else gesturesOffHUD.SetActive(false);
        }

        if (handTracking.rightHand) gestureToggleObj.SetActive(true);
        else gestureToggleObj.SetActive(false);
        if (handTracking.leftHand) leftHandToggle.SetActive(true);
        else leftHandToggle.SetActive(false);

        if (readGestures)
        {
            if (handTracking.palmsIn && handTracking.rightFist && handTracking.leftFist)
            {
                /*if (!chinUpsActive)
                {
                    if (!menuActive)
                    {
                        //ResetSelectorPositions();
                        topLevelMenu.SetActive(true);
                        //NoGravity();
                        StartCoroutine("MenuTimeOut", menuTimer);

                        Vector3 midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
                        var camZOffset = Camera.main.transform.forward * zOffset;
                        var camYOffset = Camera.main.transform.up * yOffset;
                        topLevelMenu.transform.position = midpointPalms + camZOffset + camYOffset;
                        topLevelMenu.transform.localRotation = Camera.main.transform.rotation;

                        if (magicController.elMenuActive == true)
                        {
                            magicController.ElMenuOverride();
                        }
                    }
                    else topLevelMenu.SetActive(false);

                    chinUpsActive = true;
                }*/

                ResetDMX();
            }
            //else chinUpsActive = false;
        }

        ConvertIndex();
        SetSelectorMaterial();

        if (topLevelMenu.activeInHierarchy)
        {
            menuActive = true;
        }
        else menuActive = false;

        
        
    }

    IEnumerator MenuTimeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        topLevelMenu.SetActive(false);
    }

    IEnumerator Gravity()
    {
        yield return new WaitForSeconds(0.1f);

        for(int i = 0; i < selectorPlatonics.Count; i++)
        {
            if (i != modeIndex)
            {
                Rigidbody rigidbody = selectorPlatonics[i].gameObject.GetComponentInParent<Rigidbody>();
                rigidbody.useGravity = true;
            }
        }
    }

    private void NoGravity()
    {
        foreach(Renderer selector in selectorPlatonics)
        {
            Rigidbody rigidbody = selector.gameObject.GetComponentInParent<Rigidbody>();
            rigidbody.useGravity = false;
        }
    }

    private void ResetSelectorPositions()
    {
        for(int i = 0; i < selectorPlatonics.Count; i++)
        {
            selectorPlatonics[i].transform.parent.localPosition = platonicStartPos[i];
        }
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

        if (currentMode == Mode.Staff)
        {
            modeIndex = 3;
            SetGameObjects(false, false, false, true);
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

    private void SetGameObjects(bool mag, bool rgb, bool prec, bool staff)
    {
        magicController.enabled = mag;
        //rgbController.enabled = rgb;
        precisionController.enabled = prec;
        staffController.enabled = staff;

        magicComponents.SetActive(mag);
        //rgbComponents.SetActive(rgb);
        precisionComponents.SetActive(prec);
        staffComponents.SetActive(staff);
        
    }

    #region Hook ups

    public void ToggleMode()
    {
        if (currentMode == Mode.Magic) currentMode = Mode.Precision;
        else currentMode = Mode.Magic;
        Debug.Log("touched");

    }
    public void MagicMode()
    {
        currentMode = Mode.Magic;
        //StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("DelayTouch");
        //StartCoroutine("Gravity");
    }

    public void RGBMode()
    {
        currentMode = Mode.RGB;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");

    }

    public void PrecisionMode()
    {
        currentMode = Mode.Precision;
        //StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("DelayTouch");
        //StartCoroutine("Gravity");

    }

    public void StaffMode()
    {
        currentMode = Mode.Staff;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");
    }

    private void ResetDMX()
    {
        Debug.Log("resetDMX");

        DMXcontroller dmx = FindObjectOfType<DMXcontroller>();
        DMXChannels dmxChan = FindObjectOfType<DMXChannels>();

        int dimmerChan = 0;
        int kelvinChan = 1;
        int xOverChan = 3;

        dmx.ResetDMX();

        if (currentMode == Mode.Magic)
        {
            dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
            dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
            dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
            dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);
        }
        /*else if (currentMode == Mode.Precision)
        {
            dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 0);
            dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 0);
            dmx.SetAddress(dmxChan.SkyPanel1[kelvinChan], 0);
            dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 0);
            dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 0);
            dmx.SetAddress(dmxChan.SkyPanel2[kelvinChan], 0);
        }*/

        //StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");
    }
    #endregion
}
