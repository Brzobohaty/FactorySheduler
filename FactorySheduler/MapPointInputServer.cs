using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// UDP server pro příjímání dat o stavu jednotlivých bodů na mapě
/// </summary>
namespace FactorySheduler
{
    public class MapPointInputServer
    {
        private Dictionary<int,DeviceOnPoint> devices = new Dictionary<int, DeviceOnPoint>(); //slovník všech zařízení, která byla detekována (klíč je jejich virtuální adresa)
        private static MapPointInputServer instance; //insatnce této třídy (singleton)

        public static MapPointInputServer getMapPointInputServer(){
            if (instance == null)
            {
                instance = new MapPointInputServer();
            }
            return instance;
        }

        private MapPointInputServer() {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5555);
            UdpClient newsock = new UdpClient(ipep);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                while (true)
                {
                    readRequest(newsock);
                }
            });

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Vrátí zařízení na dané virtuální adrese
        /// </summary>
        /// <param name="adrress">virtuální adresa zařízení</param>
        /// <returns>zařízení</returns>
        public DeviceOnPoint getDevice(int adrress) {
            if (devices.ContainsKey(adrress))
            {
                return devices[adrress];
            }
            else {
                try {
                    DeviceOnPoint device = getNewDevice(adrress);
                    devices.Add(adrress, device);
                    return device;
                }
                catch (Exception) {
                    DeviceOnPoint device = new DeviceOnPoint(adrress, 'x', 'x');
                    devices.Add(adrress, device);
                    return device;
                }
            }
        }

        /// <summary>
        /// Přečte jeden UDP request
        /// </summary>
        private void readRequest(UdpClient sock) {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = sock.Receive(ref sender);
            if (data.Length == 3) {
                int address = data[0];
                char type = (char)data[1];
                char status = (char)data[2];
                if (devices.ContainsKey(address))
                {
                    devices[address].changeType(type);
                    devices[address].updateStatus(status);
                }
                else {
                    devices.Add(address, new DeviceOnPoint(address, type, status));
                }
                
                sock.Send(data, data.Length, sender);
            }
        }

        /// <summary>
        /// Vrátí zařízení, které je na dané virtuální adrese
        /// </summary>
        /// <param name="adress">virtuální adresa zařízení</param>
        /// <returns>zařízení</returns>
        public DeviceOnPoint getNewDevice(int adress)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint appEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
                udpClient.Connect(appEndPoint);

                byte[] sendBytes = new byte[] { (byte)adress };

                udpClient.Send(sendBytes, sendBytes.Length);

                //Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref appEndPoint);

                int address = receiveBytes[0];
                char type = (char)receiveBytes[1];
                char status = (char)receiveBytes[2];

                udpClient.Close();

                return new DeviceOnPoint(address, type, status);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
