using LW.HSL;
using LW.SlingShot;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EyeTrackingTarget))]
public class LightHolo : MonoBehaviour
{
    [SerializeField] GameObject liveHUD;
    
    DMXcontroller dmx;
    DMXChannels channels;
    OSC osc;
    SlingShotDirector director;
    ColorPicker colorPicker;

    public bool Live { get; set; }

    private EyeTrackingTarget eyeTracking;

    void Start()
    {
        dmx = GameObject.FindGameObjectWithTag("DMX").GetComponent<DMXcontroller>();
        osc = GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>();
        channels = GameObject.FindGameObjectWithTag("DMX").GetComponent < DMXChannels>();
        director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
        colorPicker = GameObject.FindGameObjectWithTag("ColorPicker").GetComponent<ColorPicker>();
    }

    void Update()
	{
        if (Live)
		{
            liveHUD.SetActive(true);
		} else
		{
            liveHUD.SetActive(false);
		}
        
        if (Live && !director.HandPicker)
		{
            Material holoMat = GetComponentInChildren<LightHoloRendererID>().gameObject.GetComponent<Renderer>().material;
            holoMat.color = colorPicker.LiveColor;
            holoMat.SetColor("_EmissionColor", colorPicker.LiveColor);
            float hue;
            float sat;
            float dim;
            Color.RGBToHSV(colorPicker.LiveColor, out hue, out sat, out dim);

            ChangeDMXLight(hue, sat, dim);
            ChangeOSCLight(hue, sat, dim);
		}

        if (Input.GetKeyDown(KeyCode.X))
		{
            TargetSelected();
		}
	}

	public void ChangeDMXLight(float hue, float sat, float dim)
	{
        dmx.SetAddress(channels.hsiHue, Mathf.RoundToInt(hue * 255));
        dmx.SetAddress(channels.hsiSat, Mathf.RoundToInt(sat * 255));
        dmx.SetAddress(channels.hsiDimmer, Mathf.RoundToInt(dim * 255));
	}

	private void ChangeOSCLight(float hue, float sat, float dim)
	{
		List<float> vals = new List<float>
		{
			hue,
			sat,
			dim
		};

		for (int i = 0; i < vals.Count; i++)
		{
            string address;

            if (i == 0) {
                address = "hue";
			}
            else if (i == 1) {
                address = "sat";
			}
            else
			{
                address = "dim";
			}
            
            OscMessage message = new OscMessage();
            message.address = address + "Val/";
            message.values.Add(vals[i]);
            osc.Send(message);
		}
	}

	public void TargetSelected()
	{
        Live = !Live;
	}

    public void LookedAt()
	{
		transform.Rotate(0.1f, 1, 0.1f);
	}
}
