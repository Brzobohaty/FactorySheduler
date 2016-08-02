using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FactorySheduler
{
    /// <summary>
    /// Skener sítě, hledající arduino zařízení na vozících
    /// </summary>
    class NetworkScanner
    {
        public string thisDeviceIP { get; private set; } // IP tohoto zařízení
        private Action<string> iPFoundObserver; //callback pro nalezení zařízení
        private int countOfDoneTestedAdresses = 0; //Počet již otestovaných adres
        private Action finishCallback; //callback, že byly proskenovány všechny adresy

        public NetworkScanner()
        {
            thisDeviceIP = getThisDeviceIp();
            if (thisDeviceIP == "") {
                return;
            }
        }

        /// <summary>
        /// Zapsání posluchače pro dokončení ping příkazu pro jednu IP
        /// </summary>
        /// <param name="observer"></param>
        public void subscribeIPFoundObserver(Action<string> observer)
        {
            iPFoundObserver = observer;
        }

        /// <summary>
        /// Vrátí IP tohoto zařízení
        /// </summary>
        /// <returns>ip</returns>
        private string getThisDeviceIp() {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            interfaces = interfaces.Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && i.OperationalStatus == OperationalStatus.Up).ToArray();
            if (interfaces.Length == 0) {
                return "";
            }

            Regex regexIp = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
            UnicastIPAddressInformation[] ipInfo = interfaces[0].GetIPProperties().UnicastAddresses.Where(i => regexIp.Match(i.Address.ToString()).Success).ToArray();
            if (ipInfo.Length == 0)
            {
                return "";
            }

            return ipInfo[0].Address.ToString();
        }

        /// <summary>
        /// Skenuje sít pro libovolná zařízení
        /// Hledá se v 254 adres, které se liší od adresy tohot počítače pouze v posledním oktetu
        /// </summary>
        /// <param name="ip">IP zařízení, z kterého je tato aplikace spuštěna</param>
        public void scanNetwork(string ip, Action finishCallback) {
            this.finishCallback = finishCallback;
            string ipPrefix = thisDeviceIP.Substring(0, thisDeviceIP.LastIndexOf(".")+1);

            for (int i = 1; i <= 254; i++)
            {
                string who = ipPrefix + i;
                AutoResetEvent waiter = new AutoResetEvent(false);

                Ping pingSender = new Ping();

                // When the PingCompleted event is raised,
                // the PingCompletedCallback method is called.
                pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                // Wait 12 seconds for a reply.
                int timeout = 12000;

                // Set options for transmission:
                // The data can go through 64 gateways or routers
                // before it is destroyed, and the data packet
                // cannot be fragmented.
                PingOptions options = new PingOptions(64, true);

                // Send the ping asynchronously.
                // Use the waiter as the user token.
                // When the callback completes, it can wake up this thread.
                pingSender.SendAsync(who, timeout, buffer, options, waiter);
            }
        }

        /// <summary>
        /// Callback, že jeden ping příkaz byl dokončen
        /// </summary>
        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;

            sendResult(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        /// <summary>
        /// Odeslání výsledku ping příkazu observeroj
        /// </summary>
        /// <param name="reply"></param>
        private void sendResult(PingReply reply)
        {
            countOfDoneTestedAdresses++;
            if (countOfDoneTestedAdresses == 254) {
                finishCallback();
            }
            if (reply == null)
                return;

            if (reply.Status == IPStatus.Success && reply.Address.ToString() != thisDeviceIP)
            {
                iPFoundObserver(reply.Address.ToString());
            }
        }
    }
}
