using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbCastController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Time before self-destruct")] 
    [SerializeField] float lifeSpan = 5f;
    public float force = 50;
    
    [Header("DMX/OSC")]
    [Tooltip("DMX channels to control")] 
    [SerializeField] int[] DMXchannels;
    [Tooltip("Brightness value for corresponding channel - ORDER MUST MATCH DMX CHANNEL ORDER!")] [Range(0, 255)] 
    [SerializeField] int[] DMXvalues;

    [Tooltip("Dim light(s) over time or leave at set values?")] 
    [SerializeField] bool dimOverTime = true;
    [Tooltip("Percent of dimming per second")] [Range(0, 100)] 
    [SerializeField] int rateOfDim = 20;

    [Tooltip("OSC message to receive - triggers destruction/explosion of spell orb/particle")]
    [SerializeField] string OSCtoReceive = "message to receive here";
    [Tooltip("OSC message to send, either on cast or collision")]
    [SerializeField] string messageOSC;
    public float valueOSC = 1f;
    [Tooltip("If Dim Over Time is true and you want OSC value to change with DMX. Converts the highest DMX value to an OSC float.")] 
    [SerializeField] bool dimOSCwithDMX = false;

    [Header("Misc Controls")]
    [SerializeField] bool timedBlackout = true;
    [SerializeField] float blackoutDelay = 1;

    public bool isBullet = true;

    Rigidbody rigidBody;
    DMXcontroller dmx;
    OSC osc;
    Collider collider;
    EyeTrackingTarget eyeTracking;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        dmx = FindObjectOfType<DMXcontroller>();
        osc = FindObjectOfType<OSC>();
        collider = GetComponent<SphereCollider>();
        eyeTracking = GetComponent<EyeTrackingTarget>();
        eyeTracking.enabled = false;
        
        /*SendDMX();
        SendOSCMessage(); //todo refactor for lockOSCvalueToDMX

        osc.SetAddressHandler(OSCtoReceive, OnReceiveOSC);

        if (timedBlackout)
        {
            StartCoroutine("TimedLight");
        }*/

        StartCoroutine("SelfDestruct");
    }

    // Update is called once per frame
    void Update()
    {
        /*if (dimOverTime)
        {
            DimLight();
        }*/
    }

    private void SendDMX()
    {
        if (DMXchannels.Length == 0) return;

        if(DMXchannels.Length == DMXvalues.Length)
        {
            for (int i = 0; i < DMXchannels.Length; i++)
            {
                dmx.SetAddress(DMXchannels[i], DMXvalues[i]);
            }
        }
        else
        {
            Debug.LogError("Mismatch between channels and values arrays - check inspector fields.");
        }  
    }
    private void SendOSCMessage()
    {
        OscMessage message = new OscMessage();
        message.address = messageOSC;
        message.values.Add(valueOSC);
        osc.Send(message);
        Debug.Log("sending OSC: " + message); //todo remove
    }

    void OnReceiveOSC(OscMessage message)
    {
        // todo - play particle explosion
        Destroy(gameObject);
    }

    private void DimLight()
    {
        var dimPercentage = rateOfDim / 100;

        if (DMXchannels.Length == 0) return; 

        for (int i = 0; i < DMXchannels.Length; i++)
        {
            if (DMXvalues[i] > 0)
            {
                DMXvalues[i] -= Mathf.RoundToInt(dimPercentage * Time.deltaTime);
                if (DMXvalues[i] < 0) DMXvalues[i] = 0;
                dmx.SetAddress(DMXchannels[i], DMXvalues[i]);
            }
            else return;
        }

        /*int sum = 0;

        foreach (int val in DMXvalues)
        {
            sum += val;
        }

        if (dimOSCwithDMX)
        {
            if (sum > 0)
            {
                
                SendOSCMessage(valueOSC - dimPercentage * Time.deltaTime);
            }
            else
            {
                SendOSCMessage(0);
            }
            
        }
        else return;*/
    }

    IEnumerator TimedLight()
    {
        yield return new WaitForSeconds(blackoutDelay);
        dmx.ResetDMX();
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

    

    private void OnCollisionEnter(Collision other)
    {
        if (!isBullet)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        rigidBody.AddForce(transform.forward * force);
    }

    
}
