using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    [Header("Hand Rings")]
    [SerializeField] GameObject rightRingParent;
    [SerializeField] GameObject leftRingParent;
    [SerializeField] Renderer rightRing;
    [SerializeField] Renderer leftRing;
    [SerializeField] Material redMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material blueMat;
    [SerializeField] Material lightMat;
    [SerializeField] Material fireMat;
    [SerializeField] Material waterMat;
    [SerializeField] Material iceMat;


    [Header("Menus")]
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
    public bool menuActive = false;

    List<Vector3> platonicStartPos = new List<Vector3>();
    
    HandTracking handTracking;

    MagicController magicController;
    RGBController rgbController;
    PrecisionController precisionController;
    ThrowController throwController;

    GameObject magicComponents;
    GameObject rgbComponents;
    GameObject precisionComponents;

    bool chinUpsActive = false;

    void Awake()
    {
        handTracking = FindObjectOfType<HandTracking>();

        magicController = FindObjectOfType<MagicController>();
        rgbController = FindObjectOfType<RGBController>();
        precisionController = FindObjectOfType<PrecisionController>();
        throwController = FindObjectOfType<ThrowController>();

        magicComponents = FindObjectOfType<MagicID>().gameObject;
        rgbComponents = FindObjectOfType<RGBID>().gameObject;
        precisionComponents = FindObjectOfType<PrecisionID>().gameObject;

        magicController.enabled = false;
        rgbController.enabled = false;
        precisionController.enabled = false;
        throwController.enabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        topLevelMenu.SetActive(false);

        foreach (Renderer selector in selectorPlatonics)
        {
            Vector3 startingPos = selector.transform.parent.localPosition;
            platonicStartPos.Add(startingPos);
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        ConvertIndex();
        SetSelectorMaterial();
        RingControl();

        if (topLevelMenu.activeInHierarchy)
        {
            menuActive = true;
        }
        else menuActive = false;

        if (handTracking.chinUps)
        {
            if (!chinUpsActive)
            {
                if (!menuActive)
                {
                    //ResetSelectorPositions();
                    topLevelMenu.SetActive(true);
                    //NoGravity();
                    StartCoroutine("MenuTimeOut", menuTimer);

                    Vector3 midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
                    topLevelMenu.transform.position = midpointPalms + menuOffset;
                    topLevelMenu.transform.localRotation = Camera.main.transform.rotation;
                }
                else topLevelMenu.SetActive(false);

                chinUpsActive = true;
            }
        }
        else chinUpsActive = false;
        
    }

    private void RingControl()
    {
        // right ring
        if (handTracking.rightHand && currentMode == Mode.Magic)
        {
            rightRingParent.SetActive(true);
            if (magicController.elementID == 0)
            {
                rightRing.material = lightMat;
            }
            if (magicController.elementID == 1)
            {
                rightRing.material = fireMat;
            }
            if (magicController.elementID == 2)
            {
                rightRing.material = waterMat;
            }
            if (magicController.elementID == 3)
            {
                rightRing.material = iceMat;
            }
        }
        else if (handTracking.rightHand && currentMode == Mode.RGB)
        {
            rightRingParent.SetActive(true);
            if (handTracking.rightHand && rgbController.currentMode == RGBController.RGB.red)
            {
                rightRing.material = redMat;
            }
            if (handTracking.rightHand && rgbController.currentMode == RGBController.RGB.green)
            {
                rightRing.material = greenMat;
            }
            if (handTracking.rightHand && rgbController.currentMode == RGBController.RGB.blue)
            {
                rightRing.material = blueMat;
            }

        }
        else rightRingParent.SetActive(false);

        // left ring
        if (handTracking.leftHand && currentMode == Mode.Magic)
        {
            leftRingParent.SetActive(true);
            if (magicController.elementID == 0)
            {
                leftRing.material = lightMat;
            }
            if (magicController.elementID == 1)
            {
                leftRing.material = fireMat;
            }
            if (magicController.elementID == 2)
            {
                leftRing.material = waterMat;
            }
            if (magicController.elementID == 3)
            {
                leftRing.material = iceMat;
            }

        }
        else if (handTracking.leftHand && currentMode == Mode.RGB)
        {
            leftRingParent.SetActive(true);
            if (handTracking.leftHand && rgbController.currentMode == RGBController.RGB.red)
            {
                leftRing.material = redMat;
            }
            if (handTracking.leftHand && rgbController.currentMode == RGBController.RGB.green)
            {
                leftRing.material = greenMat;
            }
            if (handTracking.leftHand && rgbController.currentMode == RGBController.RGB.blue)
            {
                leftRing.material = blueMat;
            }

        }
        else leftRingParent.SetActive(false);
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

    #region Hook ups
    public void MagicMode()
    {
        currentMode = Mode.Magic;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
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
        StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");

    }

    public void ThrowMode()
    {
        currentMode = Mode.Throw;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");
    }

    public void ResetDMX()
    {
        DMXcontroller dmx = FindObjectOfType<DMXcontroller>();
        dmx.ResetDMX();
        currentMode = Mode.ResetDMX;
        StartCoroutine("MenuTimeOut", menuSelectDelay);
        //StartCoroutine("Gravity");
    }
    #endregion
}
