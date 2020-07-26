using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class MagicController : MonoBehaviour
{
    #region Inspector Fields
    [Header("Master Orbs Appearance")]
    [SerializeField] Material clearTrans;
    [SerializeField] Material cyanTrans;
    [SerializeField] Material magentaTrans;
    [SerializeField] Material yellowTrans;
    [SerializeField] Material greenTrans;


    [Header("Hand Clouds")]
    [SerializeField] GameObject rightCloudParent;
    [SerializeField] GameObject leftCloudParent;
    public List<GameObject> rightClouds;
    public List<GameObject> leftClouds;

    [SerializeField] bool manualElMenu = false;
    [SerializeField] bool floatPassthru = true;
    [SerializeField] Material transparency;
    
    [SerializeField] GameObject elementMenu;
    [SerializeField] GameObject lightMasterOrb;
    [SerializeField] GameObject fireMasterOrb;
    [SerializeField] GameObject waterMasterOrb;
    [SerializeField] GameObject iceMasterOrb;
    public Vector3 orbCastRotOffset = new Vector3(0, 0, 0); // todo hardcode
    [SerializeField] Vector3 elMenuOffset = new Vector3(0, 0, 0); // todo hardcode
    [SerializeField] float zOffset;
    [SerializeField] float yOffset;

    [Header("Caster transforms for streams")]
    [SerializeField] Transform rightHandCaster;
    [SerializeField] Transform leftHandCaster;

    [Header("Rates of Fire")]
    [SerializeField] [Range(1, 20)] float orbsPerSecond = 1f;

    

    [Header("OSC controller")]
    public List<String> elementOSC;
    public List<String> variantOSC;
    public List<String> staffOSC;
    /*[SerializeField] string oscMessage0 = "/0degreeOSC/";
    [SerializeField] string oscMessage45 = "/45degreeOSC/";
    [SerializeField] string oscMessage90 = "/90degreeOSC/";
    [SerializeField] string oscMessage135 = "/135degreeOSC/";
    [SerializeField] string oscMessage180 = "/180degreeOSC/";*/

    [Header("DMX values")]
    public List<int> lightChannels;
    public List<int> lightValues;
    public List<int> fireChannels;
    public List<int> fireValues;
    public List<int> waterChannels;
    public List<int> waterValues;
    public List<int> iceChannels;
    public List<int> iceValues;

    #endregion

    #region public vars
    float conjureValueOSC = 0;

    public enum Element { light, fire, water, ice };
    public Element currEl = Element.light;
    public int elementID = 0;
    public int variantID = 0;
    public int channelID = 0;

    public enum Scaler { deg00, deg45, deg90, deg135, deg180 }
    public Scaler currScaler = Scaler.deg90;
    public int staffID = 0;


    // parameters for conjure floats
    float maxXAxisDist = 0.5f;
    float palmDistOffset = 0.15f;
    Vector3 palmMidpointOffset = new Vector3(0, 0.05f, 0);

    public bool orbActive = false;

    // coordinates for conjuring
    Vector3 masterOrbPos;
    Vector3 rightStreamPos;
    Vector3 leftStreamPos;
    float palmDist;
    float indexMidDist;
    private Vector3 midpointIndexes;
    float elementScale;

    // used to create rate of fire for spells
    bool ableToCast = true;
    public bool fromOrbScaler = false;

    // hover orbs
    public bool hoverOrb = false;
    bool hoverSelectFromMenu = false;
    bool activeLightHover = false;   
    bool activeFireHover = false;    
    bool activeWaterHover = false;   
    bool activeIceHover = false;
    #endregion

    Director director;
    HandTracking hands;
    SpellBook spellBook;
    OSC osc;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    SoundManager sound;
    AudioSource audio;
    Renderer orbRender;
    Transform staffIndicator;

    int dimmerChan = 0;
    int kelvinChan = 1;
    int xOverChan = 3;
    int redChan = 4;
    int greenChan = 5;
    int blueChan = 6;
    int whiteChan = 7;

    public bool elMenuActive = false;

    List<List<List<int>>> masterElements = new List<List<List<int>>>();
    List<List<List<int>>> masterValues = new List<List<List<int>>>();


    List<GameObject> masterOrbs = new List<GameObject>();

    List<List<int>> lightVariants = new List<List<int>>();
    List<List<int>> lightVals = new List<List<int>>();

    List<List<int>> fireVariants = new List<List<int>>();
    List<List<int>> fireVals = new List<List<int>>();

    List<List<int>> waterVariants = new List<List<int>>();
    List<List<int>> waterVals = new List<List<int>>();

    List<List<int>> iceVariants = new List<List<int>>();
    List<List<int>> iceVals = new List<List<int>>();

    void Awake()
    {
        director = FindObjectOfType<Director>();
        hands = FindObjectOfType<HandTracking>();
        spellBook = GetComponent<SpellBook>();
        osc = FindObjectOfType<OSC>();
        dmx = FindObjectOfType<DMXcontroller>();
        dmxChan = FindObjectOfType<DMXChannels>();
        //sound = FindObjectOfType<SoundManager>();
        //audio = FindObjectOfType<SoundManager>().GetComponent<AudioSource>();

        lightMasterOrb.SetActive(false);
        fireMasterOrb.SetActive(false);
        waterMasterOrb.SetActive(false);
        iceMasterOrb.SetActive(false);
        elementMenu.SetActive(false);
        DisableRightStreams();
        DisableLeftStreams();
        //DisableClouds();
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Magic lists
        //create lists for magic
        masterOrbs.Add(lightMasterOrb);
        masterOrbs.Add(fireMasterOrb);
        masterOrbs.Add(waterMasterOrb);
        masterOrbs.Add(iceMasterOrb);

        lightVariants.Add(spellBook.light1);
        lightVariants.Add(spellBook.light2);
        lightVariants.Add(spellBook.light3);
        lightVariants.Add(spellBook.light4);

        lightVals.Add(spellBook.light1vals);
        lightVals.Add(spellBook.light2vals);
        lightVals.Add(spellBook.light3vals);
        lightVals.Add(spellBook.light4vals);

        fireVariants.Add(spellBook.fire1);
        fireVariants.Add(spellBook.fire2);
        fireVariants.Add(spellBook.fire3);
        fireVariants.Add(spellBook.fire4);

        fireVals.Add(spellBook.fire1vals);
        fireVals.Add(spellBook.fire2vals);
        fireVals.Add(spellBook.fire3vals);
        fireVals.Add(spellBook.fire4vals);

        waterVariants.Add(spellBook.water1);
        waterVariants.Add(spellBook.water2);
        waterVariants.Add(spellBook.water3);
        waterVariants.Add(spellBook.water4);

        waterVals.Add(spellBook.water1vals);
        waterVals.Add(spellBook.water2vals);
        waterVals.Add(spellBook.water3vals);
        waterVals.Add(spellBook.water4vals);

        iceVariants.Add(spellBook.ice1);
        iceVariants.Add(spellBook.ice2);
        iceVariants.Add(spellBook.ice3);
        iceVariants.Add(spellBook.ice4);

        iceVals.Add(spellBook.ice1vals);
        iceVals.Add(spellBook.ice2vals);
        iceVals.Add(spellBook.ice3vals);
        iceVals.Add(spellBook.ice4vals);

        masterElements.Add(lightVariants);
        masterElements.Add(fireVariants);
        masterElements.Add(waterVariants);
        masterElements.Add(iceVariants);

        masterValues.Add(lightVals);
        masterValues.Add(fireVals);
        masterValues.Add(waterVals);
        masterValues.Add(iceVals);
        #endregion

        staffIndicator = FindObjectOfType<StaffID>().transform;
    }

    void OnEnable()
    {
        // set global dimmer and color crossfade to max on skypanel
        SetSkyPanelXOvers();

        Color color = transparency.color;
        color.a = 0;
        transparency.color = color;
    }

    void OnDisable()
    {
        //DisableClouds();
    }

    private void DisableClouds()
    {
        rightCloudParent.SetActive(false);
        leftCloudParent.SetActive(false);
    }

    private void SetSkyPanelXOvers()
    {
        dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);
    }

    // Update is called once per frame
    void Update()
    {
        ConvertElementToID();
        ConvertStaffToID();
        CalcHandPositions();
        ProcessHandClouds();

        if (director.readGestures)
        {
            if (hands.rightRockOn) EnableRightStreams();
            else DisableRightStreams();

            if (hands.leftRockOn) EnableLeftStreams();
            else DisableLeftStreams();

            if (hands.twoHands)
            {
                ///////// PALMS OUT
                // two handed casting
                if (hands.palmsOut /*&& handTracking.staffCamUp90*/ && hands.rightOpen && hands.leftOpen)
                {
                    TurnOffMasterOrbs();
                    CastOrb();
                    //sound.orbAmbienceFX.Pause();

                    /*lightMasterOrb.SetActive(false);
                    fireMasterOrb.SetActive(false);
                    waterMasterOrb.SetActive(false);
                    iceMasterOrb.SetActive(false);*/

                    if (variantID == 0 || variantID == 1) LiveDMX();

                }

                ///////// PALMS OPPOSED
                // element menu
                else if (hands.palmsOpposed && hands.rightFist && hands.leftFist)
                {
                    ElementVariantSelector();
                }

                else if (hands.palmsOpposed && hands.rightOpen && hands.leftOpen)
                {
                    VariantScaler();
                }

                else if (hands.palmsOpposed && hands.rightThumbsUp && hands.leftThumbsUp)
                {
                    return;
                }

                
                /*// floats
                // 0
                else if (hands.palmsOpposed && hands.staffCamUp00)
                {
                    currScaler = Scaler.deg00;

                    if (hands.rightFist && hands.leftFist || hands.rightOpen && hands.leftOpen)
                    {
                        VariantScaler();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = yellowTrans;
                    }
                }

                // 45
                else if (hands.palmsOpposed && hands.staffCamUp45)
                {
                    currScaler = Scaler.deg45;

                    if (hands.rightFist && hands.leftFist || hands.rightOpen && hands.leftOpen)
                    {
                        VariantScaler();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = cyanTrans;
                    }
                }

                // 90
                else if (hands.palmsOpposed && hands.staffCamUp90)
                {
                    currScaler = Scaler.deg90;

                    if (hands.rightFist && hands.leftFist || hands.rightOpen && hands.leftOpen)
                    {
                        VariantScaler();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = clearTrans;
                    }

                    if (hands.rightThumbsUp && hands.leftThumbsUp)
                    {
                        VariantSelector();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = clearTrans;
                    }
                }

                // 135
                else if (hands.palmsOpposed && hands.staffCamUp135)
                {
                    currScaler = Scaler.deg135;

                    if (hands.rightFist && hands.leftFist || hands.rightOpen && hands.leftOpen)
                    {
                        VariantScaler();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = magentaTrans;
                    }
                }

                // 180
                else if (hands.palmsOpposed && hands.staffCamUp180)
                {
                    currScaler = Scaler.deg180;

                    if (hands.rightFist && hands.leftFist || hands.rightOpen && hands.leftOpen)
                    {
                        VariantScaler();
                        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
                        orbRender.material = greenTrans;
                    }
                }*/

                else
                {
                    TurnOffMasterOrbs();
                    //lightMasterOrb.SetActive(false);
                    //sound.orbAmbienceFX.Pause();
                }
            }
            else
            {
                TurnOffMasterOrbs();
                //lightMasterOrb.SetActive(false);
                //sound.orbAmbienceFX.Pause();
            }
        }
        else return;
        
    }

    private void ProcessHandClouds()
    {
        if (!director.readGestures || orbActive == true)
        {
            rightCloudParent.SetActive(false);
            leftCloudParent.SetActive(false);
        }
        else
        {
            // right cloud
            if (hands.rightHand)
            {
                rightCloudParent.SetActive(true);
                for (int i = 0; i < rightClouds.Count; i++)
                {
                    if (i == elementID)
                    {
                        rightClouds[i].SetActive(true);
                    }
                    else rightClouds[i].SetActive(false);
                }
            }
            else rightCloudParent.SetActive(false);


            // left cloud
            if (hands.leftHand)
            {
                leftCloudParent.SetActive(true);

                for (int i = 0; i < leftClouds.Count; i++)
                {
                    if (i == elementID)
                    {
                        leftClouds[i].SetActive(true);
                    }
                    else leftClouds[i].SetActive(false);
                }
            }
            else leftCloudParent.SetActive(false);
        }

        
    }

    private void TurnOffMasterOrbs()
    {
        foreach (GameObject orb in masterOrbs)
        {
            orb.SetActive(false);
        }

        orbActive = false;
    }

    #region OSC/DMX
    private void SendOSCMessage(string address, float value)
    {
        OscMessage message = new OscMessage();
        message.address = address;
        message.values.Add(value);
        osc.Send(message);
        Debug.Log(this.gameObject.name + " sending OSC:" + message); // todo remove
    }

    private void LiveDMX()
    {
        for (int channel = 0; channel < masterElements[elementID][variantID].Count; channel++)
        {
            List<int> adjustedValues = new List<int>();

            foreach (int value in masterValues[elementID][variantID])
            {
                int adjVal = Mathf.RoundToInt(value * elementScale);
                adjustedValues.Add(adjVal);
            }

            dmx.SetAddress(dmxChan.SkyPanel1[masterElements[elementID][variantID][channel]], adjustedValues[channel]);
            dmx.SetAddress(dmxChan.SkyPanel2[masterElements[elementID][variantID][channel]], adjustedValues[channel]);
        }
    }
    #endregion


    private void ConvertElementToID() // allows for quick selection in inspector for testing various elements and forms
    {
        if (currEl == Element.light) elementID = 0;
        if (currEl == Element.fire) elementID = 1;
        if (currEl == Element.water) elementID = 2;
        if (currEl == Element.ice) elementID = 3;
    }

    private void ConvertStaffToID()
    {
        if (currScaler == Scaler.deg00) staffID = 0;
        if (currScaler == Scaler.deg45) staffID = 1;
        if (currScaler == Scaler.deg90) staffID = 2;
        if (currScaler == Scaler.deg135) staffID = 3;
        if (currScaler == Scaler.deg180) staffID = 4;

    }

    private void CalcHandPositions()
    {
        palmDist = Vector3.Distance(hands.rightPalm.Position, hands.leftPalm.Position);
        indexMidDist = Vector3.Distance(hands.rtIndexMid.Position, hands.ltIndexMid.Position);
        midpointIndexes = Vector3.Lerp(hands.rtIndexMid.Position, hands.ltIndexMid.Position, 0.5f);

        var midpointHands = Vector3.Lerp(hands.rtMiddleKnuckle.Position, hands.ltMiddleKnuckle.Position, 0.5f);
        masterOrbPos = midpointHands;
        rightStreamPos = Vector3.Lerp(hands.rtIndexTip.Position, hands.rtPinkyTip.Position, 0.5f);
        leftStreamPos = Vector3.Lerp(hands.ltIndexTip.Position, hands.ltPinkyTip.Position, 0.5f);
    }

    bool menuTimerActive = false;

    bool switch00Sent = false;
    bool switch45Sent = false;
    bool switch90Sent = false;
    bool switch135Sent = false;
    bool switch180Sent = false;


    private void ElementVariantSelector()
    {
        fromOrbScaler = false;
        orbActive = false;
        ShowStaffAngle(clearTrans);

        /*if (!elementMenu.activeInHierarchy && director.menuActive == false)
        {
            TurnOffMasterOrbs();
            //lightMasterOrb.SetActive(false);
            elementMenu.SetActive(true);
            ResetElementSelection();
            if (!menuTimerActive)
            {
                StartCoroutine("MenuTimeOut", 5);
                menuTimerActive = true;
            }

            Vector3 midpointPalms = Vector3.Lerp(hands.rightPalm.Position, hands.leftPalm.Position, 0.5f);
            var camZOffset = Camera.main.transform.forward * zOffset;
            var camYOffset = Camera.main.transform.up * yOffset;
            elementMenu.transform.position = midpointPalms + camYOffset + camZOffset;
            elementMenu.transform.localRotation = Camera.main.transform.rotation;

            elMenuActive = true;
        }*/


        if (hands.staffCamUp00 || hands.staffCamUp45)
        {
            currEl = Element.fire;
            VariantSelector();
        }

        else if (hands.staffCamUp90)
        {
            currEl = Element.light;
            VariantSelector();
        }

        else if (hands.staffCamUp135 || hands.staffCamUp180)
        {
            currEl = Element.water;
            VariantSelector();
        }

        // activate element orb
        for (int i = 0; i < masterOrbs.Count; i++)
        {
            if (i == elementID)
            {
                masterOrbs[i].SetActive(true);
                masterOrbs[i].transform.position = masterOrbPos;
                masterOrbs[i].GetComponent<MasterOrbRotater>().xRotation = 1;
            }
            else masterOrbs[i].SetActive(false);

            List<Transform> variants = new List<Transform>();

            foreach (Transform child in masterOrbs[i].transform)
            {
                variants.Add(child);
            }

            for (int n = 0; n < variants.Count; n++)
            {
                if (n == variantID)
                {
                    variants[n].gameObject.SetActive(true);
                    variants[n].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else variants[n].gameObject.SetActive(false);
            }
        }



    }

    private void VariantSelector()
    {
        fromOrbScaler = false;

        elementMenu.SetActive(false);

        float elSlotSize = (maxXAxisDist - palmDistOffset) / spellBook.lightMasterOrb.Count;

        // select variant based on distance between palms
        if ((palmDist > 0 && palmDist <= palmDistOffset) || (palmDist > palmDistOffset && palmDist <= maxXAxisDist - (elSlotSize * 3)))
        {
            variantID = 0;
        }
        else if (palmDist > maxXAxisDist - (elSlotSize * 3) && palmDist <= maxXAxisDist - (elSlotSize * 2))
        {
            variantID = 1;
        }
        else if (palmDist > maxXAxisDist - (elSlotSize * 2) && palmDist <= maxXAxisDist - elSlotSize)
        {
            variantID = 2;
        }
        else if (palmDist > maxXAxisDist - elSlotSize && palmDist <= maxXAxisDist)
        {
            variantID = 3;
        }

        // activate element orb
        for (int i = 0; i < masterOrbs.Count; i++)
        {
            if (i == elementID)
            {
                masterOrbs[i].SetActive(true);
                masterOrbs[i].transform.position = masterOrbPos;
                masterOrbs[i].GetComponent<MasterOrbRotater>().xRotation = 1;
            }
            else masterOrbs[i].SetActive(false);

            List<Transform> variants = new List<Transform>();

            foreach(Transform child in masterOrbs[i].transform)
            {
                variants.Add(child);
            }

            for (int n = 0; n < variants.Count; n++)
            {
                if (n == variantID)
                {
                    variants[n].gameObject.SetActive(true);
                    variants[n].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else variants[n].gameObject.SetActive(false);
            }
        }
    }

    private void ShowStaffAngle(Material colorMat)
    {
        orbRender = masterOrbs[elementID].GetComponent<Renderer>();
        orbRender.material = colorMat;
    }

    private void VariantScaler()
    {
        fromOrbScaler = true;
        orbActive = true;

        elementMenu.SetActive(false);

        // determine scale
        if (palmDist >= palmDistOffset && palmDist <= maxXAxisDist) elementScale = 1 - (palmDist - palmDistOffset) / (maxXAxisDist - palmDistOffset);
        else if (palmDist > maxXAxisDist) elementScale = 0;
        else if (palmDist < palmDistOffset) elementScale = 1;
        GetStaffAngle();

        // activate current element and variant
        for (int i = 0; i < masterOrbs.Count; i++)
        {
            if (i == elementID)
            {
                masterOrbs[i].SetActive(true);
                masterOrbs[i].transform.position = masterOrbPos;
                masterOrbs[i].GetComponent<MasterOrbRotater>().xRotation = 100 * elementScale;
            }
            else masterOrbs[i].SetActive(false);

            List<Transform> variants = new List<Transform>();

            foreach (Transform child in masterOrbs[i].transform)
            {
                variants.Add(child);
            }

            for (int n = 0; n < variants.Count; n++)
            {
                if (n == variantID)
                {
                    variants[n].gameObject.SetActive(true);

                    // apply scale based on orb element
                    variants[n].localScale = new Vector3(elementScale, elementScale, elementScale);
                }
                else variants[n].gameObject.SetActive(false);
            }
        }

        if (hands.rightOpen && hands.leftOpen)
        {
            SendOSCMessage(elementOSC[elementID] + variantOSC[variantID] + staffOSC[staffID], elementScale);
            LiveDMX();

            if (currScaler == Scaler.deg00)
            {
                switch45Sent = false;
                switch90Sent = false;
                switch135Sent = false;
                switch180Sent = false;
                
                if (!switch00Sent)
                {
                    SendOSCMessage("/switch00/" + elementOSC[elementID] + variantOSC[variantID], 1);
                    switch00Sent = true;
                }
            }

            if (currScaler == Scaler.deg45)
            {
                switch00Sent = false;
                switch90Sent = false;
                switch135Sent = false;
                switch180Sent = false; 
                
                if (!switch45Sent)
                {
                    SendOSCMessage("/switch45/" + elementOSC[elementID] + variantOSC[variantID], 1);
                    switch45Sent = true;
                }
            }

            if (currScaler == Scaler.deg90)
            {
                switch45Sent = false;
                switch45Sent = false;
                switch135Sent = false;
                switch180Sent = false; 
                
                if (!switch90Sent)
                {
                    SendOSCMessage("/switch90/" + elementOSC[elementID] + variantOSC[variantID], 1);
                    switch90Sent = true;
                }
            }

            if (currScaler == Scaler.deg135)
            {
                switch00Sent = false;
                switch45Sent = false;
                switch90Sent = false;
                switch180Sent = false; 
                
                if (!switch135Sent)
                {
                    SendOSCMessage("/switch135/" + elementOSC[elementID] + variantOSC[variantID], 1);
                    switch135Sent = true;
                }
            }

            if (currScaler == Scaler.deg180)
            {
                switch00Sent = false;
                switch45Sent = false;
                switch90Sent = false;
                switch135Sent = false;
                
                if (!switch180Sent)
                {
                    SendOSCMessage("/switch180/" + elementOSC[elementID] + variantOSC[variantID], 1);
                    switch180Sent = true;
                }
            }
        }
    }

    private void GetStaffAngle()
    {
        // set staff angle
        if (hands.staffCamUp00)
        {
            currScaler = Scaler.deg00;

            if (currEl == Element.light) ShowStaffAngle(yellowTrans);

        }

        // 45
        else if (hands.staffCamUp45)
        {
            currScaler = Scaler.deg45;

            if (currEl == Element.light) ShowStaffAngle(magentaTrans);
        }

        // 90
        else if (hands.staffCamUp90)
        {
            currScaler = Scaler.deg90;

            if (currEl == Element.light) ShowStaffAngle(clearTrans);
        }

        // 135
        else if (hands.staffCamUp135)
        {
            currScaler = Scaler.deg135;

            if (currEl == Element.light) ShowStaffAngle(cyanTrans);
        }

        // 180
        else if (hands.palmsOpposed && hands.staffCamUp180)
        {
            currScaler = Scaler.deg180;

            if (currEl == Element.light) ShowStaffAngle(greenTrans);
        }
    }

    private void CastOrb()
    {
        Quaternion palmsRotationMid = Quaternion.Slerp(hands.rightPalm.Rotation, hands.leftPalm.Rotation, 0.5f);
        Quaternion castRotation = palmsRotationMid * Quaternion.Euler(orbCastRotOffset);

        if (ableToCast)
        {
            GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
            StartCoroutine("CastDelay", orbsPerSecond);
            spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);

            ElementParent elParent = spellOrb.GetComponentInChildren<ElementParent>();
            OrbCastController spellController = spellOrb.GetComponent<OrbCastController>();

            if (floatPassthru)
            {
                spellController.valueOSC = elementScale;
            }
            else
            {
                spellController.valueOSC = 1;
            }
            
            float spellForceRange = 1 - (palmDist / maxXAxisDist);

            float spellForce = spellForceRange * 50;
            if (spellForce < 1) spellForce = 2;
            spellController.force = spellForce;

            GetStaffAngle();
            SendOSCMessage(spellController.GetMessageOSC() + variantOSC[variantID] + staffOSC[staffID], spellController.valueOSC);

            float particleScale = elementScale * 1.167388f;

            foreach (Transform child in elParent.transform)
            {
                if (child.CompareTag("Spell"))
                {
                    child.localScale = new Vector3(particleScale, particleScale, particleScale);
                }
            }
        }

        /*if (!hoverOrb)
        {
            if (ableToCast)
            {
                GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                StartCoroutine("CastDelay", orbsPerSecond);
                spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);

                ElementParent elParent = spellOrb.GetComponentInChildren<ElementParent>();

                if (fromOrbScaler && floatPassthru)
                {
                    OrbCastController spellController = spellOrb.GetComponent<OrbCastController>();
                    spellController.valueOSC = elementScale;

                    float spellForceRange = 1 - (palmDist / maxXAxisDist);

                    float spellForce = spellForceRange * 50;
                    if (spellForce < 1) spellForce = 2;
                    spellController.force = spellForce;

                    Debug.Log("GetMessageOSC: " + spellController.GetMessageOSC());
                    SendOSCMessage(spellController.GetMessageOSC() + variantOSC[variantID] + staffOSC[staffID], elementScale);


                    float particleScale = elementScale * 1.167388f;

                    foreach (Transform child in elParent.transform)
                    {
                        if (child.CompareTag("Spell"))
                        {
                            child.localScale = new Vector3(particleScale, particleScale, particleScale);
                        }
                    }
                }
                else return;
            }
            else return;
        }*/
        /*else
        {
            if (currEl == Element.light && !activeLightHover)
            {
                GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                activeLightHover = true;
                spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
            }

            if (currEl == Element.fire && !activeFireHover)
            {
                GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                activeFireHover = true;
                spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
            }

            if (currEl == Element.water && !activeWaterHover)
            {
                GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                activeWaterHover = true;
                spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
            }

            if (currEl == Element.ice && !activeIceHover)
            {
                GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation);
                activeIceHover = true;
                spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
            }
        }*/
    }

    public void DestroyHoverOrb()
    {
        OrbHoverController[] gazeOrbs = FindObjectsOfType<OrbHoverController>();

        if (gazeOrbs == null) return;
        else
        {
            foreach (OrbHoverController orb in gazeOrbs)
            {
                if (currEl == Element.light && orb.CompareTag("Light"))
                {
                    Destroy(orb.gameObject);
                    activeLightHover = false;
                }

                if (currEl == Element.fire && orb.CompareTag("Fire"))
                {
                    Destroy(orb.gameObject);
                    activeFireHover = false;
                }

                if (currEl == Element.water && orb.CompareTag("Water"))
                {
                    Destroy(orb.gameObject);
                    activeWaterHover = false;
                }

                if (currEl == Element.ice && orb.CompareTag("Ice"))
                {
                    Destroy(orb.gameObject);
                    activeIceHover = false;
                }
            }
        }        
    }

    
    public void DisableRightStreams()
    {
        foreach (ParticleSystem stream in spellBook.rightStreams)
        {
            var emission = stream.emission;
            emission.enabled = false;
            foreach (Transform child in stream.transform)
            {
                var childEmission = child.GetComponent<ParticleSystem>().emission;
                childEmission.enabled = false;
            }
        }
    }

    private void EnableRightStreams()
    {
        for(int i = 0; i < spellBook.rightStreams.Count; i++)
        {
            if (i == elementID)
            {
                var emission = spellBook.rightStreams[i].emission;
                emission.enabled = true;
                Transform streamParent = spellBook.rightStreams[elementID].transform.parent;
                
                rightHandCaster.position = rightStreamPos;
                streamParent.position = rightStreamPos;
                streamParent.rotation = rightHandCaster.rotation;

                foreach (Transform child in spellBook.rightStreams[elementID].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = true;
                }
            }
            else
            {
                var emission = spellBook.rightStreams[i].emission;
                emission.enabled = false;
                Transform streamParent = spellBook.rightStreams[i].transform.parent;
                streamParent.position = rightStreamPos;

                foreach (Transform child in spellBook.rightStreams[i].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = false;
                }
            }
        }

        // todo fix sound not firing
        if (currEl == Element.fire)
        {
            //sound.fireStreamFX.Play();
            //sound.waterIceStreamFX.Pause();
        }
        else
        {
            //sound.waterIceStreamFX.Play();
            //sound.fireStreamFX.Pause();
        }
    }

    public void DisableLeftStreams()
    {
        foreach (ParticleSystem stream in spellBook.leftStreams)
        {
            var emission = stream.emission;
            emission.enabled = false;
            foreach (Transform child in stream.transform)
            {
                var childEmission = child.GetComponent<ParticleSystem>().emission;
                childEmission.enabled = false;
            }
        }
    }

    private void EnableLeftStreams()
    {
        for (int i = 0; i < spellBook.leftStreams.Count; i++)
        {
            if (i == elementID)
            {
                var emission = spellBook.leftStreams[i].emission;
                emission.enabled = true;
                Transform streamParent = spellBook.leftStreams[elementID].transform.parent;
                
                leftHandCaster.position = leftStreamPos;
                streamParent.position = leftStreamPos;
                streamParent.rotation = leftHandCaster.rotation;

                foreach (Transform child in spellBook.leftStreams[elementID].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = true;
                }
            }
            else
            {
                var emission = spellBook.leftStreams[i].emission;
                emission.enabled = false;
                Transform streamParent = spellBook.leftStreams[i].transform.parent;
                streamParent.position = rightStreamPos;

                foreach (Transform child in spellBook.leftStreams[i].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = false;
                }
            }
        }


        if (currEl == Element.fire)
        {
            //sound.fireStreamFX.Play();
            //sound.waterIceStreamFX.Pause();
        }
        else
        {
            //sound.waterIceStreamFX.Play();
            //sound.fireStreamFX.Pause();
        }
    }

    IEnumerator CastDelay(float delay)
    {
        ableToCast = false;
        yield return new WaitForSeconds(1 / delay);
        ableToCast = true;
    }

    IEnumerator MenuTimeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        elementMenu.SetActive(false);
    }

    IEnumerator OnElementSelection()
    {
        List<Transform> elements = new List<Transform>();

        foreach (Transform child in elementMenu.transform)
        {
            elements.Add(child);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < elements.Count; i++)
        {
            if (i != elementID)
            {
                elements[i].gameObject.SetActive(false);
            }
        }

        elMenuActive = false;
        menuTimerActive = false;

        Color color = transparency.color;
        color.a = 0;
        transparency.color = color;

    }

    private void ResetElementSelection()
    {
        foreach (Transform child in elementMenu.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public int GetElementID()
    {
        return elementID;
    }

    public void ElMenuOverride()
    {
        elementMenu.SetActive(false);
        elMenuActive = false;
    }

    #region Hook Ups
    public void SetLight()
    {
        currEl = Element.light;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("OnElementSelection");
    }

    public void SetFire()
    {
        currEl = Element.fire;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("OnElementSelection");
    }

    public void SetWater()
    {
        currEl = Element.water;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("OnElementSelection");
    }

    public void SetIce()
    {
        currEl = Element.ice;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("OnElementSelection");
    }

    public void HoverOrbYes()
    {
        hoverOrb = true;
        hoverSelectFromMenu = true;
    }

    public void HoverOrbNo()
    {
        hoverOrb = false;
        hoverSelectFromMenu = true;
    }
    #endregion
}
