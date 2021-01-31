using System.Collections.Generic;
using UnityEngine;

public class DMXChannels : MonoBehaviour
{
    [Header("SkyPanel channels")]
    public List<int> SkyPanel1 = new List<int>();
    public List<int> SkyPanel2 = new List<int>();

    [Header("CCT & RGBW channel assignments")]
    public int skyPanelDimmer;
    public int skyPanelKelvin;
    public int skyPanelXOver;
    public int skyPanelRed;
    public int skyPanelGreen;
    public int skyPanelBlue;
    public int skyPanelWhite;

    [Header("CCT & HSI channel assignments")]
    public int cctDimmer = 1;
    public int cctKelvin = 2;
    public int cctGNSat = 3;
    public int cctXFadeToColor = 4;
    public int cctHue = 5;
    public int cctSat = 6;

    [Header("HSI channel assignments")]
    public int hsiDimmer = 1;
    public int hsiHue = 2;
    public int hsiSat = 3;

    [Header("SpotLight channels")]
    public int spotRed;
    public int spotGreen;
    public int spotBlue;
    public int spotWhite;




}
