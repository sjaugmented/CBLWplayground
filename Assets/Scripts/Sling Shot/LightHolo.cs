using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHolo : MonoBehaviour
{
    DMXcontroller dmx;
    DMXChannels channels;

    void Start()
    {
        dmx = GameObject.FindGameObjectWithTag("DMX").GetComponent<DMXcontroller>();
        channels = GameObject.FindGameObjectWithTag("DMX").GetComponent < DMXChannels>();
    }

    public void ChangeLight(float hue, float sat, float val)
	{
        dmx.SetAddress(channels.hsiHue, Mathf.RoundToInt(hue * 255));
        dmx.SetAddress(channels.hsiSat, Mathf.RoundToInt(sat * 255));
        dmx.SetAddress(channels.hsiDimmer, Mathf.RoundToInt(val * 255));
	}
}
