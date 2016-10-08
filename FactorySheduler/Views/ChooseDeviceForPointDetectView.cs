using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorySheduler.Views
{
    /// <summary>
    /// View pro výběr zařízení, s kterým se budou detekovat body na mapě
    /// </summary>
    public partial class ChooseDeviceForPointDetectView : Form
    {
        private static ChooseDeviceForPointDetectView instance = new ChooseDeviceForPointDetectView(); //jediná instance této třídy
        private List<Cart> devices; //Zařízení, s kterých lze vybírat
        private Action<Cart> deviceForDetectPointWasSelectedCallback; //Callback pro výběr zařízení

        public static ChooseDeviceForPointDetectView getInstance()
        {
            return instance;
        }

        public ChooseDeviceForPointDetectView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Předání všech dostupných zařízení
        /// </summary>
        /// <param name="devices"> list zařízení</param>
        public void setDevices(List<Cart> devices) {
            this.devices = devices;
        }

        /// <summary>
        /// Nastavení callbacku pro výběr zařízení
        /// </summary>
        /// <param name="deviceForDetectPointWasSelectedCallback">callback</param>
        public void setChooseCallback(Action<Cart> deviceForDetectPointWasSelectedCallback) {
            this.deviceForDetectPointWasSelectedCallback = deviceForDetectPointWasSelectedCallback;
        }

        /// <summary>
        /// Přidá jeden radiobutton se jménem zařízení
        /// </summary>
        /// <param name="device">zařízení</param>
        private void addDevice(Cart device) {
            RadioButton button = new RadioButton();
            button.Text = device.name;
            button.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
            flowLayoutPanel1.Controls.Add(button);
        }

        /// <summary>
        /// Callback při zobrazení okna
        /// </summary>
        private void SettingsStaticBeacons_Shown(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].errorMessage == "") {
                    addDevice(devices[i]);
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].name == ((RadioButton)sender).Text)
                {
                    Close();
                    deviceForDetectPointWasSelectedCallback(devices[i]);
                    break;
                }
            }
        }
    }
}
