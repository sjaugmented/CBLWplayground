using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] bool manualElMenu = false;
    [SerializeField] bool floatPassthru = true;
    
    [SerializeField] GameObject masterOrb;
    public Vector3 orbCastRotOffset = new Vector3(0, 0, 0); // todo hardcode
    public Vector3 particleCastOffset = new Vector3(0, 0, 0.1f); // todo hardcode
    [SerializeField] Vector3 elMenuOffset = new Vector3(0, 0, 0); // todo hardcode


    [Header("Caster transforms for particles/streams")]
    [SerializeField] Transform rightHandCaster;
    [SerializeField] Transform leftHandCaster;

    [Header("Rates of Fire")]
    [SerializeField] [Range(1, 20)] float orbsPerSecond = 1f;
    [SerializeField] [Range(1, 20)] float particlesPerSecond = 20f;

    [Header("Palm Menus")]
    [SerializeField] GameObject elementMenu;
    [Tooltip("Parent object of the palm menu visuals")]
    [SerializeField] GameObject leftPalmMenuVisuals;
    [SerializeField] GameObject rightPalmMenuVisuals;
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

    [Header("DMX controllers")]
    public List<int> lightChannels;
    public List<int> lightValues;
    public List<int> fireChannels;
    public List<int> fireValues;
    public List<int> waterChannels;
    public List<int> waterValues;
    public List<int> iceChannels;
    public List<int> iceValues;

    float conjureValueOSC = 0;

    public enum Element { light, fire, water, ice };
    public Element currEl = Element.light;
    int elementID = 0;

    // coordinates for conjuring
    Vector3 masterOrbPos;
    Vector3 rightStreamPos;
    Vector3 leftStreamPos;
    float palmDist;
    float elementScale;

    // used to create rate of fire for spells
    bool ableToCast = true;
    public bool fromOrbScaler = false;
    public bool hoverOrb = false;
    bool hoverSelectFromMenu = false;
    bool activeLightHover = false;   //
    bool activeFireHover = false;    // todo
    bool activeWaterHover = false;   //  make private
    bool activeIceHover = false;     //

    OrbFingerTracker handTracking;
    SpellBook spellBook;
    OSC osc;
    DMXcontroller dmx;
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
        dmx = FindObjectOfType<DMXcontroller>();
        sound = FindObjectOfType<SoundManager>();
        audio = FindObjectOfType<SoundManager>().GetComponent<AudioSource>();

        masterOrb.SetActive(false);
        elementMenu.SetActive(false);
        DisableRightStreams();
        DisableLeftStreams();

    }

    // Update is called once per frame
    void Update()
    {
        ConvertElementToID();
        CalcHandPositions();

        /*if (handTracking.fingerGunRight || handTracking.fingerGunLeft)
            {
                CastParticle();
                DisableStreams();
            }*/
        if (handTracking.rockOnRight) EnableRightStreams();
        else DisableRightStreams();

        if (handTracking.rockOnLeft) EnableLeftStreams();
        else DisableLeftStreams();

        if (handTracking.twoPalms)
        {
            // two handed casting
            if (handTracking.palmsOut)
            {
                CastOrb();
                sound.orbAmbienceFX.Pause();
                masterOrb.SetActive(false);
                elementMenu.SetActive(false);

            }
            // element menu
            else if (handTracking.palmsIn && handTracking.twoFists || manualElMenu)
            {
                ElementSelector();
                if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();
            }
            // element scaler
            else if (handTracking.palmsIn && !handTracking.twoFists)
            {
                ElementScaler();
                if (!sound.orbAmbienceFX.isPlaying) sound.orbAmbienceFX.Play();

                /*conjureValueOSC = 1 - (palmDist - palmDistOffset) / (maxPalmDistance - palmDistOffset);
                if (conjureValueOSC < 0) conjureValueOSC = 0;
                if (conjureValueOSC > 1) conjureValueOSC = 1;*/
                conjureValueOSC = elementScale;
                //Debug.Log(conjureValueOSC); // remove

                SendOSCMessage(conjureOSCMessages[elementID], conjureValueOSC);
                LiveDMX();
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

    private void LiveDMX()
    {
        if (currEl == Element.light)
        {
            if (lightChannels.Count == 0) return;

            if (lightChannels.Count == lightValues.Count)
            {
                // calc float per channel value
                List<int> adjustedValues = new List<int>();

                foreach (int value in lightValues)
                {
                    int adjVal = Mathf.RoundToInt(value * elementScale);
                    adjustedValues.Add(adjVal);
                }

                // sendDMX
                for (int i = 0; i < lightChannels.Count; i++)
                {
                    dmx.SetAddress(lightChannels[i], adjustedValues[i]);
                }

                Debug.Log(adjustedValues[0]); // remove
            }
            else Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }

        if (currEl == Element.fire)
        {
            if (fireChannels.Count == 0) return;

            if (fireChannels.Count == fireValues.Count)
            {
                // calc float per channel value
                List<int> adjustedValues = new List<int>();

                foreach (int value in fireValues)
                {
                    int adjVal = Mathf.RoundToInt(value * elementScale);
                    adjustedValues.Add(adjVal);
                }

                // sendDMX
                for (int i = 0; i < fireChannels.Count; i++)
                {
                    dmx.SetAddress(fireChannels[i], adjustedValues[i]);
                }
            }
            else Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }

        if (currEl == Element.water)
        {
            if (waterChannels.Count == 0) return;

            if (waterChannels.Count == waterValues.Count)
            {
                // calc float per channel value
                List<int> adjustedValues = new List<int>();

                foreach (int value in waterValues)
                {
                    int adjVal = Mathf.RoundToInt(value * elementScale);
                    adjustedValues.Add(adjVal);
                }

                // sendDMX
                for (int i = 0; i < waterChannels.Count; i++)
                {
                    dmx.SetAddress(waterChannels[i], adjustedValues[i]);
                }
            }
            else Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }

        if (currEl == Element.ice)
        {
            if (iceChannels.Count == 0) return;

            if (iceChannels.Count == iceValues.Count)
            {
                // calc float per channel value
                List<int> adjustedValues = new List<int>();

                foreach (int value in iceValues)
                {
                    int adjVal = Mathf.RoundToInt(value * elementScale);
                    adjustedValues.Add(adjVal);
                }

                // sendDMX
                for (int i = 0; i < iceChannels.Count; i++)
                {
                    dmx.SetAddress(iceChannels[i], adjustedValues[i]);
                }
            }
            else Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }
    }

    private void CalcHandPositions()
    {
        palmDist = handTracking.palmDist;

        var midpointPalms = Vector3.Lerp(handTracking.rightPalm.Position, handTracking.leftPalm.Position, 0.5f);
        masterOrbPos = midpointPalms + palmMidpointOffset;
        rightStreamPos = Vector3.Lerp(handTracking.rtIndexTip.Position, handTracking.rtPinkyTip.Position, 0.5f);
        leftStreamPos = Vector3.Lerp(handTracking.ltIndexTip.Position, handTracking.ltPinkyTip.Position, 0.5f);
    }

    private void ElementSelector()
    {
        DisableRightStreams();
        DisableLeftStreams();

        fromOrbScaler = false;
        hoverSelectFromMenu = false;

        masterOrb.SetActive(true);
        elementMenu.SetActive(true);
        masterOrb.transform.position = masterOrbPos;

        for (int i = 0; i < spellBook.masterOrbElements.Count; i++)
        {
            if (i == elementID) spellBook.masterOrbElements[i].SetActive(true);
            else spellBook.masterOrbElements[i].SetActive(false);
        }
    }

    private void ElementScaler()
    {
        fromOrbScaler = true;
        DisableRightStreams();
        DisableLeftStreams();

        

        masterOrb.SetActive(true);
        elementMenu.SetActive(false);
        masterOrb.transform.position = masterOrbPos;
        masterOrb.GetComponent<MasterOrbRotater>().xRotation = 100 * elementScale;

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

        if (!hoverSelectFromMenu)
        {
            if (elementScale < 0.2) hoverOrb = true;
            else hoverOrb = false;
        }
        else return;

        // apply scale based on orb element
        spellBook.masterOrbElements[elementID].transform.localScale = new Vector3(elementScale, elementScale, elementScale);
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
                    Debug.Log(elementScale); // remove
                    OrbCastController spellController = spellOrb.GetComponent<OrbCastController>();
                    spellController.valueOSC = elementScale;

                    float spellForceRange = 1 - (palmDist / maxPalmDistance);

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
        else
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
        }
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

    private void CastParticle()
    {
        if (ableToCast)
        {
            if (handTracking.fingerGunRight)
            {
                GameObject spellParticle = Instantiate(spellBook.particleSpells[elementID], handTracking.rtIndexTip.Position + particleCastOffset, Camera.main.transform.rotation);
                StartCoroutine("CastDelay", particlesPerSecond);
            }

            if (handTracking.fingerGunLeft)
            {
                GameObject spellParticle = Instantiate(spellBook.particleSpells[elementID], handTracking.ltIndexTip.Position + particleCastOffset, Camera.main.transform.rotation);
                StartCoroutine("CastDelay", particlesPerSecond);
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

        sound.fireStreamFX.Pause();
        sound.waterIceStreamFX.Pause();
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

        sound.fireStreamFX.Pause();
        sound.waterIceStreamFX.Pause();
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

    public int GetElementID()
    {
        return elementID;
    }

    #region UI Hook Ups
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
    #endregion
}
