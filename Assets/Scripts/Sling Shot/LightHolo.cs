using LW.HSL;
using LW.SlingShot;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EyeTrackingTarget))]
public class LightHolo : MonoBehaviour
{
    [SerializeField] GameObject liveHUD;
    
    DMXcontroller dmx;
    DMXChannels channels;
    SlingShotDirector director;
    ColorPicker colorPicker;

    public bool Live { get; set; }

    private EyeTrackingTarget eyeTracking;

    void Start()
    {
        dmx = GameObject.FindGameObjectWithTag("DMX").GetComponent<DMXcontroller>();
        channels = GameObject.FindGameObjectWithTag("DMX").GetComponent < DMXChannels>();
        director = GameObject.FindGameObjectWithTag("Director").GetComponent<SlingShotDirector>();
        colorPicker = GameObject.FindGameObjectWithTag("ColorPicker").GetComponent<ColorPicker>();

        //eyeTracking = GetComponent<EyeTrackingTarget>();
        //eyeTracking.OnSelected.AddListener(TargetSelected);
        //eyeTracking.OnLookAtStart.AddListener(LookedAt);
        //eyeTracking.OnLookAway.AddListener(LookedAway);
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
            float val;
            Color.RGBToHSV(colorPicker.LiveColor, out hue, out sat, out val);

            ChangeLight(hue, sat, val);
		}

        if (Input.GetKeyDown(KeyCode.X))
		{
            TargetSelected();
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

	public void ChangeLight(float hue, float sat, float val)
	{
        dmx.SetAddress(channels.hsiHue, Mathf.RoundToInt(hue * 255));
        dmx.SetAddress(channels.hsiSat, Mathf.RoundToInt(sat * 255));
        dmx.SetAddress(channels.hsiDimmer, Mathf.RoundToInt(val * 255));
	}
}
