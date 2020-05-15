using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] GameObject masterOrb;
    public bool fistScaler = false;
    public Vector3 orbCastRotOffset = new Vector3(0, 0, 0);
    public Vector3 particleCastOffset = new Vector3(0, 0, 0.1f);

    [Header("Caster transforms for particles/streams")]
    [SerializeField] Transform orbCaster; //remove
    [SerializeField] Transform particleCasterRight;
    [SerializeField] Transform particleCasterLeft;
    [SerializeField] Transform streamCasterRight;
    [SerializeField] Transform streamCasterLeft;

    [Header("Rates of Fire")]
    [SerializeField] [Range(1, 20)] float orbsPerSecond = 1f;
    [SerializeField] [Range(1, 20)] float particlesPerSecond = 20f;

    [Header("Palm Menus")]
    [Tooltip("Parent object of the palm menu visuals")]
    [SerializeField] GameObject palmMenuVisuals1;
    [SerializeField] GameObject palmMenuVisuals2;
    [Tooltip("Rate of fire GameObject sliders in hierarchy")]
    [SerializeField] PinchSlider orbROFSlider;
    [SerializeField] TextMeshPro orbROFText;
    [Tooltip("Rate of fire GameObject sliders in hierarchy")]
    [SerializeField] PinchSlider particleROFSlider;
    [SerializeField] TextMeshPro particleROFText;
    [SerializeField] PinchSlider palmOffsetSlider;
    [SerializeField] TextMeshPro palmOffsetText;
    [SerializeField] PinchSlider maxPalmDistSlider;
    [SerializeField] TextMeshPro maxPalmDistText;

    [Header("Palm Conjure")]
    [Tooltip("Max distance between palms for conjuring")]
    [SerializeField] [Range( 0.2f, 0.6f)] float maxPalmDistance = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] float palmDistOffset = 0.05f;
    [SerializeField] Vector3 palmMidpointOffset;

    [Header("OSC controller")]
    public List<String> conjureOSCMessages;    
    
    float conjureValueOSC = 0;

    public enum Element { light, fire, water, ice };
    public enum Form { particle, orb, stream };
    public Element currEl = Element.light;
    public Form currForm = Form.orb; // in case we reintroduce different forms - ie, particles, streams
    int elementID = 0;

    // coordinates for conjuring
    Vector3 masterOrbPos;
    Vector3 midpointRtIndexPinky;
    Vector3 midpointLtIndexPinky;
    Vector3 palm1Pos;
    Vector3 palm2Pos;
    float palmDist;
    Vector3 rtIndexTipPos;
    Vector3 rtPinkyPos;
    Vector3 ltIndexTipPos;
    Vector3 ltPinkyPos;

    bool twoPalms;
    bool touchDown;
    bool palmsForward;
    bool palmsIn;
    bool palmsParallel;
    bool fists;
    bool rockOnRight;
    bool fingerGunRight;
    bool rockOnLeft;
    bool fingerGunLeft;


    float elementScale;

    // used to create rate of fire for spells
    bool ableToCast = true;
    bool fromElSelector = false;
    bool fromOrbScaler = false;

    OrbFingerTracker handTracking;
    SpellBook spellBook;
    OSC osc;
    SoundManager sound;
    AudioSource audio;

    private void ConvertElementToID() // allows for quick selection in inspector for testing various elements and forms
    {
        if (currEl == Element.light) elementID = 0;
        if (currEl == Element.fire) elementID = 1;
        if (currEl == Element.water) elementID = 2;
        if (currEl == Element.ice) elementID = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        handTracking = FindObjectOfType<OrbFingerTracker>();
        spellBook = GetComponent<SpellBook>();
        osc = FindObjectOfType<OSC>();
        sound = FindObjectOfType<SoundManager>();
        audio = FindObjectOfType<SoundManager>().GetComponent<AudioSource>();

        masterOrb.SetActive(false);
        DisableStreams();
    }

    // Update is called once per frame
    void Update()
    {
        ConvertElementToID();

        twoPalms = handTracking.GetTwoPalms();
        touchDown = handTracking.GetTouchdown();
        palmsForward = handTracking.GetPalmsForward();
        palmsIn = handTracking.GetPalmsIn();
        palmsParallel = handTracking.GetPalmsParallel();
        fists = handTracking.GetFists();
        rockOnRight = handTracking.GetRockOnRight();
        fingerGunRight = handTracking.GetFingerGunRight();
        rockOnLeft = handTracking.GetRockOnLeft();
        fingerGunLeft = handTracking.GetFingerGunLeft();

        CalcHandPositions();

        if (twoPalms)
        {
            if (palmsForward)
            {
                CastOrb();
                sound.orbAmbienceFX.Pause();
                masterOrb.SetActive(false);
            }
            else if (palmsParallel && !fists)
            {
                if (fistScaler)
                {
                    masterOrb.SetActive(true);
                    masterOrb.transform.position = masterOrbPos;
                    ElementSelector();
                    if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();
                }
                else
                {
                    masterOrb.SetActive(true);
                    masterOrb.transform.position = masterOrbPos;
                    ElementScaler();
                    if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();

                    /*if (palmDist - palmDistOffset > 0 && palmDist - palmDistOffset < maxPalmDistance - palmDistOffset)
                    {
                        conjureValueOSC = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
                    }
                    else if (palmDist - palmDistOffset >= maxPalmDistance - palmDistOffset) conjureValueOSC = 0;
                    else if (palmDist - palmDistOffset <= 0) conjureValueOSC = 1;*/

                    if (palmDist >= palmDistOffset && palmDist <= maxPalmDistance) conjureValueOSC = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
                    else if (palmDist > maxPalmDistance) conjureValueOSC = 0;
                    else if (palmDist < palmDistOffset) conjureValueOSC = 1;

                    SendOSCMessage(conjureOSCMessages[elementID], conjureValueOSC);
                    Debug.Log(conjureValueOSC); // remove

                }
            }
            else if (palmsParallel && fists)
            {
                if (fistScaler)
                {
                    masterOrb.SetActive(true);
                    masterOrb.transform.position = masterOrbPos;
                    ElementScaler();
                    if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();

                    if (palmDist >= palmDistOffset && palmDist <= maxPalmDistance) conjureValueOSC = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
                    else if (palmDist > maxPalmDistance) conjureValueOSC = 0;
                    else if (palmDist < palmDistOffset) conjureValueOSC = 1;

                    SendOSCMessage(conjureOSCMessages[elementID], conjureValueOSC);
                }
                else
                {
                    masterOrb.SetActive(true);
                    masterOrb.transform.position = masterOrbPos;
                    ElementSelector();
                    if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();
                }
            }
            else
            {
                masterOrb.SetActive(false);
                sound.orbAmbienceFX.Pause();
            }
        }
        else
        {
            masterOrb.SetActive(false);
            sound.orbAmbienceFX.Pause();

            if (fingerGunRight || fingerGunLeft)
            {
                CastParticle();
                DisableStreams();
            }
            else if (rockOnRight || rockOnLeft)
            {
                EnableStream();
            }
            else DisableStreams();
        }

        
    }

    private void SendOSCMessage(string address, float value)
    {
        OscMessage message = new OscMessage();
        message.address = address;
        message.values.Add(value);
        osc.Send(message);
        //Debug.Log("Sending OSC: " + address + " " + value); // todo remove
    }

    private void CalcHandPositions()
    {
        palmDist = handTracking.GetPalmDist();
        palm1Pos = handTracking.GetRtPalmPos();
        palm2Pos = handTracking.GetLtPalmPos();
        rtIndexTipPos = handTracking.GetRtIndexTipPos();
        rtPinkyPos = handTracking.GetRtPinkyTipPos();
        ltIndexTipPos = handTracking.GetLtIndexTipPos();
        ltPinkyPos = handTracking.GetLtPinkyTipPos();

        var midpointPalms = Vector3.Lerp(palm1Pos, palm2Pos, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
        midpointRtIndexPinky = Vector3.Lerp(rtIndexTipPos, rtPinkyPos, 0.5f);
        midpointLtIndexPinky = Vector3.Lerp(ltIndexTipPos, ltPinkyPos, 0.5f);
    }

    private void ElementSelector()
    {
        fromOrbScaler = false;

        float elSlotSize = (maxPalmDistance - palmDistOffset) / spellBook.masterOrbElements.Count;

        // select element based on distance between palms
        if ((palmDist > 0 && palmDist <= palmDistOffset) || (palmDist > palmDistOffset && palmDist <= maxPalmDistance - (elSlotSize * 3)))
        {
            currEl = Element.light;

            // play soundfx as you leave the zone
            if (palmDist == maxPalmDistance - (elSlotSize * 3))
            {
                sound.elementSwitchFX.Play();
                Debug.Log("switch!");
            }
        }
        else if (palmDist > maxPalmDistance - (elSlotSize * 3) && palmDist <= maxPalmDistance - (elSlotSize * 2))
        {
            currEl = Element.fire;
            // activate corresponding element
            for (int i = 0; i < spellBook.masterOrbElements.Count; i++)

            // play soundfx as you leave the zone
            if (palmDist == maxPalmDistance - (elSlotSize * 2)) sound.elementSwitchFX.Play();

        }
        else if (palmDist > maxPalmDistance - (elSlotSize * 2) && palmDist <= maxPalmDistance - elSlotSize)
        {
            currEl = Element.water;

            // play soundfx as you leave the zone
            if (palmDist == maxPalmDistance - elSlotSize) sound.elementSwitchFX.Play();

        }
        else if (palmDist > maxPalmDistance - elSlotSize && palmDist <= maxPalmDistance)
        {
            currEl = Element.ice;
        }

        // activate corresponding element
        for (int i = 0; i < spellBook.masterOrbElements.Count; i++)
        {
            if (i == elementID) spellBook.masterOrbElements[i].SetActive(true);
            else spellBook.masterOrbElements[i].SetActive(false);
        }

        // keep orb element scaled at 0.5 for best visibility
        spellBook.masterOrbElements[elementID].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

    }

    private void ElementScaler()
    {
        fromOrbScaler = true;

        // activate correct orb element
        for (int i = 0; i < spellBook.masterOrbElements.Count; i++)
        {
            if (i == elementID) spellBook.masterOrbElements[i].SetActive(true);
            else spellBook.masterOrbElements[i].SetActive(false);
        }

        // determine scale
        if (palmDist >= palmDistOffset && palmDist <= maxPalmDistance) elementScale = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
        else if (palmDist > maxPalmDistance) elementScale = 0;
        else if (palmDist < palmDistOffset) elementScale = 1;

        // apply scale based on orb element
        spellBook.masterOrbElements[elementID].transform.localScale = new Vector3(elementScale, elementScale, elementScale);
    }

    private void CastOrb()
    {
        Quaternion rtPalmRot = handTracking.GetRtPalmRot();
        Quaternion ltPalmRot = handTracking.GetLtPalmRot();
        
        if (ableToCast)
        {
            orbCaster.position = masterOrbPos; // remove
            Quaternion palmsRotationMid = Quaternion.Slerp(rtPalmRot, ltPalmRot, 0.5f);
            Quaternion castRotation = palmsRotationMid * Quaternion.Euler(orbCastRotOffset);

            GameObject spellOrb = Instantiate(spellBook.orbSpells[elementID], masterOrbPos, castRotation); // todo check rotation
            spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
            
            ElementParent elParent = spellOrb.GetComponentInChildren<ElementParent>();

            if (currEl == Element.water)
            {
                var liquidController = elParent.GetComponentInChildren<LiquidVolumeAnimator>();
                liquidController.level = elementScale;
            }
            else
            {
                foreach (Transform child in elParent.transform)
                {
                    if (child.CompareTag("Spell"))
                    {
                        child.localScale = new Vector3(elementScale, elementScale, elementScale);
                    }
                }
            }

            StartCoroutine("CastDelay", orbsPerSecond);
        }
    }

    private void CastParticle()
    {
        Quaternion rtIndexRot = handTracking.GetRtIndRot();
        Quaternion ltIndexRot = handTracking.GetLtIndRot();

        if (ableToCast)
        {
            if (fingerGunRight)
            {
                GameObject spellParticle = Instantiate(spellBook.particleSpells[elementID], rtIndexTipPos + particleCastOffset, particleCasterRight.rotation);
                StartCoroutine("CastDelay", particlesPerSecond);
            }

            if (fingerGunLeft)
            {
                GameObject spellParticle = Instantiate(spellBook.particleSpells[elementID], ltIndexTipPos + particleCastOffset, particleCasterLeft.rotation);
                StartCoroutine("CastDelay", particlesPerSecond);
            }
            
        }
    }

    public void DisableStreams()
    {
        foreach (ParticleSystem stream in spellBook.streamSpells)
        {
            var emission = stream.emission;
            emission.enabled = false;
            foreach (Transform child in stream.transform)
            {
                var childEmission = child.GetComponent<ParticleSystem>().emission;
                childEmission.enabled = false;
            }
        }

        sound.fireStreamFX.Pause();
        sound.waterIceStreamFX.Pause();
    }

    private void EnableStream()
    {
        for(int i = 0; i < spellBook.streamSpells.Count; i++)
        {
            if (i == elementID)
            {
                var emission = spellBook.streamSpells[i].emission;
                emission.enabled = true;
                Transform streamParent = spellBook.streamSpells[elementID].transform.parent;
                if (rockOnRight)
                {
                    streamCasterRight.position = midpointRtIndexPinky;
                    streamParent.position = midpointRtIndexPinky;
                    streamParent.rotation = streamCasterRight.rotation;
                    // todo check rotation
                }
                if (rockOnLeft)
                {
                    streamCasterLeft.position = midpointLtIndexPinky;
                    streamParent.position = midpointLtIndexPinky;
                    streamParent.rotation = streamCasterLeft.rotation;
                    // todo check rotation
                }

                foreach (Transform child in spellBook.streamSpells[elementID].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = true;
                }
            }
            else
            {
                var emission = spellBook.streamSpells[i].emission;
                emission.enabled = false;
                Transform streamParent = spellBook.streamSpells[i].transform.parent;
                streamParent.position = midpointRtIndexPinky;

                foreach (Transform child in spellBook.streamSpells[i].transform)
                {
                    var childEmission = child.GetComponent<ParticleSystem>().emission;
                    childEmission.enabled = false;
                }
            }
        }


        if (currEl == Element.fire)
        {
            sound.fireStreamFX.Play();
            sound.waterIceStreamFX.Pause();
        }
        else
        {
            sound.waterIceStreamFX.Play();
            sound.fireStreamFX.Pause();
        }
    }

    IEnumerator CastDelay(float delay)
    {
        ableToCast = false;
        yield return new WaitForSeconds(1 / delay);
        ableToCast = true;
    }

    public void SetLight()
    {
        currEl = Element.light;
    }

    public void SetFire()
    {
        currEl = Element.fire;
    }

    public void SetWater()
    {
        currEl = Element.water;
    }

    public void SetIce()
    {
        currEl = Element.ice;
    }

    public void SetOrbRateOfFire()
    {
        float sliderVal = orbROFSlider.SliderValue;
        // rate of fire cannot go below 1
        if (sliderVal < 0.05f) sliderVal = 0.05f;
        orbsPerSecond = sliderVal * 20;

        orbROFText.text = orbsPerSecond.ToString();
    }

    public void SetParticleRateOfFire()
    {
        float sliderVal = particleROFSlider.SliderValue;
        // rate of fire cannot go below 1
        if (sliderVal < 0.05f) sliderVal = 0.05f;
        particlesPerSecond = sliderVal * 20;

        particleROFText.text = particlesPerSecond.ToString();
    }

    public void SetPalmDistanceOffset()
    {
        float sliderVal = palmOffsetSlider.SliderValue;

        palmDistOffset = sliderVal * 0.2f;

        palmOffsetText.text = palmDistOffset.ToString();
    }

    public void SetMaxPalmDist()
    {
        float sliderVal = maxPalmDistSlider.SliderValue;
        maxPalmDistance = sliderVal * 0.6f;
        if (maxPalmDistance < 0.2f) maxPalmDistance = 0.2f;
        maxPalmDistText.text = maxPalmDistance.ToString();
    }

    public void SetFistScaler()
    {
        fistScaler = !fistScaler;
    }
}
