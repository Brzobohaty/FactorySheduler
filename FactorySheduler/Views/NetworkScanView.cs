using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorySheduler.Views
{
    public partial class NetworkScanView : UserControl
    {
        private static NetworkScanView instance = new NetworkScanView();
        private Dictionary<String, ListViewItem> ipItems = new Dictionary<String, ListViewItem>();

        private NetworkScanView()
        {
            InitializeComponent();
        }

        public static NetworkScanView getInstance()
        {
            return instance;
        }

        public void addDeviceIP(string ip) {
            ListViewItem item = new ListViewItem(ip);
            listView1.Items.Add(item);
            ipItems.Add(ip,item);
        }

        public void showThisDeviceIP(string ip) {
            labelThisDeviceIP.Text = ip;
            string ipPrefix = ip.Substring(0, ip.LastIndexOf(".") + 1);
            labelDvicesIPs.Text = labelDvicesIPs.Text+ipPrefix+"*";
        }

        public void setIPStatus(string ip, bool isCart) {
            ListViewItem item = ipItems[ip];
            
            if (isCart)
            {
                item.BackColor = Color.Green;
            }
            else {
                item.BackColor = Color.Red;
            }
        }

        delegate void SetCountLabelCallback(int count);

        public void setCountLabel(int count) {
            if (labelCount.InvokeRequired)
            {
                SetCountLabelCallback cb = new SetCountLabelCallback(setCountLabel);
                this.Invoke(cb, new object[] { count });
            }
            else
            {
                labelCount.Text = "Bylo nalezeno " + count + " kompatibilních a funkčních zařízení.";
            }
        }
    }
}
