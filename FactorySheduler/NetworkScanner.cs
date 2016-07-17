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
    class NetworkScanner
    {
        public string thisDeviceIP { get; private set; }
        private Action<string> iPFoundObserver;

        public NetworkScanner()
        {
            thisDeviceIP = getThisDeviceIp();
            if (thisDeviceIP == "") {
                return;
            }
        }

        public void subscribeIPFoundObserver(Action<string> observer)
        {
            iPFoundObserver = observer;
        }

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

        public void scanNetwork(string ip) {
            string ipPrefix = thisDeviceIP.Substring(0, thisDeviceIP.LastIndexOf(".")+1);

            for (int i = 1; i < 255; i++)
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

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                // Let the main thread resume. 
                // UserToken is the AutoResetEvent object that the main thread 
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                // Let the main thread resume. 
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        private void DisplayReply(PingReply reply)
        {
            if (reply == null)
                return;

            if (reply.Status == IPStatus.Success && reply.Address.ToString() != thisDeviceIP)
            {
                iPFoundObserver(reply.Address.ToString());
            }
        }
    }
}
