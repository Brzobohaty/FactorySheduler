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
        private IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        private byte[] data = new byte[1024];
        private IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5555);
        private UdpClient newsock;
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
            newsock = new UdpClient(ipep);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                while (true)
                {
                    readRequest();
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
                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Přečte jeden UDP request
        /// </summary>
        private void readRequest() {
            data = newsock.Receive(ref sender);
            if (data.Length == 3) {
                int address = data[0];
                char type = (char)data[1];
                char status = (char)data[2];
                if (devices.ContainsKey(address))
                {
                    devices[address].updateStatus(status);
                }
                else {
                    devices.Add(address, new DeviceOnPoint(address, type, status));
                }
                
                newsock.Send(data, data.Length, sender);
            }
        }
    }
}
