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
        private static NetworkScanView instance = new NetworkScanView(); //instance této třídy
        private Dictionary<String, ListViewItem> ipItems = new Dictionary<String, ListViewItem>(); //slovník všech nalezených zařízení v seznamu podle jejich IP
        private Action buttonNextClickCallback;
        private Action buttonRefreshClickCallback;

        private NetworkScanView()
        {
            InitializeComponent();
        }

        public static NetworkScanView getInstance()
        {
            return instance;
        }

        /// <summary>
        /// Zapsání posluchače pro stisknutí tlačítka pokračování
        /// </summary>
        /// <param name="callback"></param>
        public void subscribeButtonNextListener(Action callback)
        {
            this.buttonNextClickCallback = callback;
        }

        /// <summary>
        /// Zapsání posluchače pro stisknutí tlačítka pro refresh
        /// </summary>
        /// <param name="callback"></param>
        public void subscribeButtonRefreshListener(Action callback)
        {
            this.buttonRefreshClickCallback = callback;
        }

        /// <summary>
        /// Přidá do seznamu nalezených zařízení další IP
        /// </summary>
        /// <param name="ip">IP nalezeného zařízení</param>
        public void addDeviceIP(string ip) {
            ListViewItem item = new ListViewItem(ip);
            listView1.Items.Add(item);
            ipItems.Add(ip,item);
        }

        /// <summary>
        /// Zobrazí IP adresu počítače, na kterém je aplikace spuštěna
        /// </summary>
        /// <param name="ip"></param>
        public void showThisDeviceIP(string ip) {
            labelThisDeviceIP.Text = ip;
            string ipPrefix = ip.Substring(0, ip.LastIndexOf(".") + 1);
            labelDvicesIPs.Text = labelDvicesIPs.Text+ipPrefix+"*";
        }

        /// <summary>
        /// Označí IP v seznamu příslušnou barvou, podle toho, zda se jedná o vyhovující zařízení nebo ne
        /// </summary>
        /// <param name="ip">IP zařízení</param>
        /// <param name="isCart">Je to vozík s Arduino zařízením?</param>
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

        /// <summary>
        /// Nastaví počet nalezených kompatibilních arduino zařízení
        /// </summary>
        /// <param name="count">počet vyhovujících zařízení</param>
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

        delegate void EnableNextButtonCallback();

        /// <summary>
        /// Zapne tlačítko pro pokračování
        /// </summary>
        public void enableNextButton() {
            if (buttonNext.InvokeRequired)
            {
                EnableNextButtonCallback cb = new EnableNextButtonCallback(enableNextButton);
                this.Invoke(cb, new object[] {});
            }
            else
            {
                buttonNext.Enabled = true;
            }
            
        }
        
        private void buttonNext_Click(object sender, EventArgs e)
        {
            buttonNextClickCallback();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            labelCount.Text = "";
            ipItems = new Dictionary<String, ListViewItem>();
            listView1.Clear();
            buttonRefreshClickCallback();
        }
    }
}
