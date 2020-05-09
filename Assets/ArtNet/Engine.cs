using System;
using System.Text;
#if !UNITY_EDITOR
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking;
using Windows.Networking.Sockets;
#else
using System.Net.Sockets;
#endif

namespace ArtNet
{
    public class Engine
    {
        short _universe = 0;
        int _port = 6454;
        string _address = "255.255.255.255";

#if !UNITY_EDITOR
        private DatagramSocket _udpClient;
#else
        private UdpClient _udpClient;
#endif
        private byte[] _artNetAddress;
        private byte[] _artNetHeader;

        public Engine()
        {
            Startup(_universe, _address);
        }

        public Engine(short universe, string address)
        {
            Startup(universe, address);
        }

        void Startup(short universe, string address)
        {
            _universe = universe;
            _address = address;

            _artNetHeader = new byte[] { 0x41, 0x72, 0x74, 0x2d, 0x4e, 0x65, 0x74, 0 };
            _artNetAddress = new byte[] { 0x7f, 0, 0, 1, Convert.ToByte(this.LoByte(0x1936)), Convert.ToByte(this.HiByte(0x1936)) };

            StartClient();
        }

        public void Shutdown()
        {
            ShutDownClient();

        }

#if !UNITY_EDITOR
        async void ShutDownClient()
        {
            try
            {
                await _udpClient.CancelIOAsync();
                _udpClient.Dispose();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
            }
        }

        async void StartClient()
        {
            _udpClient = new DatagramSocket();
            await _udpClient.ConnectAsync(new HostName(_address), _port.ToString());
        }

        private async void broadcast(byte[] data)
        {
            try
            {
                var buf = data.AsBuffer();
                await _udpClient.OutputStream.WriteAsync(buf);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
            }
        }
#else
        void StartClient()
        {
            _udpClient = new UdpClient(_port);
            _udpClient.EnableBroadcast = true;
            _udpClient.Client.SendTimeout = 100;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        void ShutDownClient()
        {
            _udpClient.Close();
            _udpClient = null;
        }

        private void broadcast(byte[] data)
        {
            _udpClient.Send(data, data.Length, _address, _port);
        }
#endif

        public void SendDMX(byte[] data)
        {
            int len = data.Length;
            byte[] packet = new byte[(0x11 + len) + 1];
            Buffer.BlockCopy(_artNetHeader, 0, packet, 0, _artNetHeader.Length);
            packet[8] = Convert.ToByte(this.LoByte(0x5000));
            packet[9] = Convert.ToByte(this.HiByte(0x5000));
            packet[10] = 0;          //ProtVerHi
            packet[11] = 14;         //ProtVerLo
            packet[12] = 0;          //Sequence
            packet[13] = 0;          //Physical
            packet[14] = Convert.ToByte(this.LoByte(_universe));
            packet[15] = Convert.ToByte(this.HiByte(_universe));
            packet[0x10] = Convert.ToByte(this.HiByte(len));
            packet[0x11] = Convert.ToByte(this.LoByte(len));

            Buffer.BlockCopy(data, 0, packet, 0x12, len);
            broadcast(packet);
        }


        private object LoByte(int wParam)
        {
            return (wParam & 0xffL);
        }

        private object HiByte(int wParam)
        {
            return ((wParam / 0x100) & 0xffL);
        }
    }
}
