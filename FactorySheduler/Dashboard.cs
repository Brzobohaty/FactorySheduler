using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    /// <summary>
    /// Třída představuje konektor k dashboard aplikaci
    /// </summary>
    public class Dashboard
    {

        /// <summary>
        /// Zkontroluje, zda se lze připojit k aplikaci dashboard na localhostu přes UDP na portu 4444
        /// </summary>
        /// <returns>true pokud ano</returns>
        public bool checkConnectionToDashboard() {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint dashboardEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444);
                udpClient.Connect(dashboardEndPoint);

                byte[] sendBytes = crc(new byte[] { (byte)0, 0x47, 0x01, 0x00, 0x04, 0x00, 0x00, 0x10, 0x00, 0x00 });

                udpClient.Send(sendBytes, sendBytes.Length);

                //Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref dashboardEndPoint);

                udpClient.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Zjistí, zda je ultrazvukový maják s danou adresou připojen.
        /// </summary>
        /// <param name="adress">adresa majáku</param>
        /// <returns>true pokud je připojen</returns>
        public bool isDeviceConnected(int adress) {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint dashboardEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444);
                udpClient.Connect(dashboardEndPoint);

                byte[] sendBytes = crc(new byte[] { (byte)adress, 0x47, 0x01, 0x00, 0x04, 0x00, 0x00, 0x10, 0x00, 0x00 });

                udpClient.Send(sendBytes, sendBytes.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref dashboardEndPoint);

                int xPosition = BitConverter.ToInt16(receiveBytes, 9);
                int yPosition = BitConverter.ToInt16(receiveBytes, 11);
                int zPosition = BitConverter.ToInt16(receiveBytes, 13);

                udpClient.Close();

                if (xPosition == 0 && yPosition == 0 && zPosition == 0)
                {
                    return false;
                }
                else {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Zjistí aktuální polohu majáku s danou adresou
        /// </summary>
        /// <param name="adress">adresa majáku</param>
        /// <returns>souřadnice aktuální polohy majáku nebo 0,0 pokud nastala chyba</returns>
        public Point getDevicePosition(int adress)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint dashboardEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444);
                udpClient.Connect(dashboardEndPoint);

                byte[] sendBytes = crc(new byte[] { (byte)adress, 0x47, 0x01, 0x00, 0x04, 0x00, 0x00, 0x10, 0x00, 0x00 });

                udpClient.Send(sendBytes, sendBytes.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref dashboardEndPoint);

                int xPosition = BitConverter.ToInt16(receiveBytes, 9);
                int yPosition = BitConverter.ToInt16(receiveBytes, 11);

                udpClient.Close();

                return new Point(xPosition, yPosition);
            }
            catch (Exception e)
            {
                return new Point(0,0);
            }
        }

        /// <summary>
        /// Vypočítá kontrolní součet dané zprávy
        /// </summary>
        /// <param name="buf">pole bytů, z kterých se má kontrolní součet spočítat</param>
        /// <returns>kontrolní součet</returns>
        private byte[] crc(byte[] buf)
        {
            UInt16 crc = 0xFFFF;
            int len = buf.Length - 2;
            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos]; // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1; // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else // Else LSB is not set
                        crc >>= 1; // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            //return crc;
            byte[] c = BitConverter.GetBytes(crc);
            buf[len] = c[0];
            buf[len + 1] = c[1];
            return buf;
        }
    }
}
