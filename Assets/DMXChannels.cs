using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMXChannels : MonoBehaviour
{
    [Header("SkyPanel channels")]
    public int skyPanelDimmer;
    public int skyPanelKelvin;
    public int skyPanelXOver;
    public int skyPanelRed;
    public [Range(0, 255)] int redVal;
    public int skyPanelGreen;
    public [Range(0, 255)] int greenVal;
    public int skyPanelBlue;
    public [Range(0, 255)] int blueVal;
    public int skyPanelWhite;
    public [Range(0, 255)] int whiteVal;

    [Header("SpotLight channels")]
    public int spotRed;
    public int spotGreen;
    public int spotBlue;
    public int spotWhite;

    

    
}
