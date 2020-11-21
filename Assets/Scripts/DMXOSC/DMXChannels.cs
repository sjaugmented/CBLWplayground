using System.Collections.Generic;
using UnityEngine;

public class DMXChannels : MonoBehaviour
{
    [Header("SkyPanel channels")]
    public List<int> SkyPanel1 = new List<int>();
    public List<int> SkyPanel2 = new List<int>();

    [Header("Individual channel assignments")]
    public int skyPanelDimmer;
    public int skyPanelKelvin;
    public int skyPanelXOver;
    public int skyPanelRed;
    public int skyPanelGreen;
    public int skyPanelBlue;
    public int skyPanelWhite;

    [Header("SpotLight channels")]
    public int spotRed;
    public int spotGreen;
    public int spotBlue;
    public int spotWhite;




}
