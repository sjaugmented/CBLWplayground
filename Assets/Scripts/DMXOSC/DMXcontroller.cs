using UnityEngine;

public class DMXcontroller : MonoBehaviour
{
    ArtNet.Engine _artNet;

    byte[] _dmxData;

    void Awake()
    {
        _artNet = new ArtNet.Engine();
        ResetDMX();

        Debug.Log(_dmxData); //todo remove
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _artNet.SendDMX(_dmxData);
    }

    public void ResetDMX() // set lights to zero
    {
        _dmxData = GetEmpty512();
        _artNet.SendDMX(_dmxData);
    }

    public static byte[] GetEmpty512() // set channel/byte arrays to zero
    {
        byte[] DMXData = new byte[512];
        for (int i = 0; i < DMXData.Length; i++)
        {
            DMXData[i] = 0;
        }

        return DMXData;
    }

    public void SetAddress(int channel, int value) // send channel and brightness info
    {
        Debug.Log("DMXcontroller received channel: " + channel + ", value: " + value); //todo remove

        if (channel <= 0) return;

        int x = value;
        if (x < 0) x = 0;
        if (x > 255) x = 255;

        _dmxData[channel - 1] = (byte)x;
    }
}
