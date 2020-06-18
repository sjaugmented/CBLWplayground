using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MagicController : MonoBehaviour
{
    #region Inspector Fields
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

    [Header("Caster transforms for streams")]
    [SerializeField] Transform rightHandCaster;
    [SerializeField] Transform leftHandCaster;

    [Header("Rates of Fire")]
    [SerializeField] [Range(1, 20)] float orbsPerSecond = 1f;

    

    [Header("OSC controller")]
    public List<String> elementOSC;
    public List<String> variantOSC;
    [SerializeField] string yOSCMessage = "/yOSCfloat/";
    [SerializeField] string zOSCMessage = "/zOSCfloat/";

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

    /*public enum LightVariants { light1, light2, light3, light4, none };
    public LightVariants currLight = LightVariants.none;
    int lightID = 4;

    public enum FireVariants { fire1, fire2, fire3, fire4, none };
    public FireVariants currFire = FireVariants.none;
    int fireID = 4;

    public enum WaterVariants { water1, water2, water3, water4, none };
    public WaterVariants currWater = WaterVariants.none;
    int waterID = 4;

    public enum IceVariants { ice1, ice2, ice3, ice4, none };
    public IceVariants currIce = IceVariants.none;
    int iceID = 4;*/

    // parameters for conjure floats
    float maxXAxisDist = 0.5f;
    float palmDistOffset = 0.15f;
    Vector3 palmMidpointOffset = new Vector3(0, 0.05f, 0);

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

    HandTracking handTracking;
    SpellBook spellBook;
    OSC osc;
    DMXcontroller dmx;
    DMXChannels dmxChan;
    SoundManager sound;
    AudioSource audio;

    int dimmerChan = 0;
    int kelvinChan = 1;
    int xOverChan = 3;
    int redChan = 4;
    int greenChan = 5;
    int blueChan = 6;
    int whiteChan = 7;

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
        handTracking = FindObjectOfType<HandTracking>();
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

    }

    void OnEnable()
    {
        // set global dimmer and color crossfade to max on skypanel
        dmx.SetAddress(dmxChan.SkyPanel1[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel1[xOverChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[dimmerChan], 255);
        dmx.SetAddress(dmxChan.SkyPanel2[xOverChan], 255);

        Color color = transparency.color;
        color.a = 0;
        transparency.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        ConvertElementToID();
        CalcHandPositions();

        if (handTracking.rockOnRight) EnableRightStreams();
        else DisableRightStreams();

        if (handTracking.rockOnLeft) EnableLeftStreams();
        else DisableLeftStreams();

        if (handTracking.twoHands)
        {   
            // two handed casting
            if (handTracking.palmsOut)
            {
                CastOrb();
                //sound.orbAmbienceFX.Pause();
                lightMasterOrb.SetActive(false);
                fireMasterOrb.SetActive(false);
                waterMasterOrb.SetActive(false);
                iceMasterOrb.SetActive(false);

                if (variantID == 0 || variantID == 1) LiveDMX();

            }

            // element menu
            if (handTracking.pullUps)
            {
                ElementSelector();
            }

            // variant menu
            else if (handTracking.fistsIn || manualElMenu)
            {
                VariantSelector();
                //if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();
            }

            // element scaler
            else if (handTracking.palmsIn)
            {
                LiveScaler();
                //if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();

                /*conjureValueOSC = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
                if (conjureValueOSC < 0) conjureValueOSC = 0;
                if (conjureValueOSC > 1) conjureValueOSC = 1;*/
                conjureValueOSC = elementScale;
                //Debug.Log(conjureValueOSC); // remove

                SendOSCMessage(elementOSC[elementID]+variantOSC[variantID], conjureValueOSC);
                if (variantID == 2 || variantID == 3) LiveDMX();

            }
            else
            {
                lightMasterOrb.SetActive(false);
                //sound.orbAmbienceFX.Pause();
            }
        }
        else
        {
            lightMasterOrb.SetActive(false);
            //sound.orbAmbienceFX.Pause();
        }
    }

    #region OSC/DMX
    private void SendOSCMessage(string address, float value)
    {
        OscMessage message = new OscMessage();
        message.address = address;
        message.values.Add(value);
        osc.Send(message);
        //Debug.Log("Sending OSC: " + address + " " + value); // todo remove
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

    private void CalcHandPositions()
    {
        palmDist = Vector3.Distance(handTracking.rightPalm.Position, handTracking.leftPalm.Position);
        indexMidDist = Vector3.Distance(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position);
        midpointIndexes = Vector3.Lerp(handTracking.rtIndexMid.Position, handTracking.ltIndexMid.Position, 0.5f);

        var midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
        rightStreamPos = Vector3.Lerp(handTracking.rtIndexTip.Position, handTracking.rtPinkyTip.Position, 0.5f);
        leftStreamPos = Vector3.Lerp(handTracking.ltIndexTip.Position, handTracking.ltPinkyTip.Position, 0.5f);
    }

    private void ElementSelector()
    {
        fromOrbScaler = false;

        if (!elementMenu.activeInHierarchy)
        {
            lightMasterOrb.SetActive(false);
            elementMenu.SetActive(true);
            ResetElementSelection();
            StartCoroutine("MenuTimeOut", 20);

            Vector3 midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
            elementMenu.transform.position = midpointPalms + elMenuOffset;
            elementMenu.transform.localRotation = Camera.main.transform.rotation;
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

    private void LiveScaler()
    {
        fromOrbScaler = true;

        elementMenu.SetActive(false);

        // determine scale
        if (palmDist >= palmDistOffset && palmDist <= maxXAxisDist) elementScale = 1 - (palmDist - palmDistOffset) / (maxXAxisDist - palmDistOffset);
        else if (palmDist > maxXAxisDist) elementScale = 0;
        else if (palmDist < palmDistOffset) elementScale = 1;

        if (!hoverSelectFromMenu)
        {
            if (elementScale < 0.2) hoverOrb = true;
            else hoverOrb = false;
        }
        else return;

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
    }

    private void CastOrb()
    {
        Quaternion palmsRotationMid = Quaternion.Slerp(handTracking.rightPalm.Rotation, handTracking.leftPalm.Rotation, 0.5f);
        Quaternion castRotation = palmsRotationMid * Quaternion.Euler(orbCastRotOffset);

        if (!hoverOrb)
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
        }
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

    IEnumerator ElementSelection()
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

    #region Hook Ups
    public void SetLight()
    {
        currEl = Element.light;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("ElementSelection");
    }

    public void SetFire()
    {
        currEl = Element.fire;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("ElementSelection");
    }

    public void SetWater()
    {
        currEl = Element.water;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("ElementSelection");
    }

    public void SetIce()
    {
        currEl = Element.ice;
        variantID = 0;
        StartCoroutine("MenuTimeOut", 1);
        StartCoroutine("ElementSelection");
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
