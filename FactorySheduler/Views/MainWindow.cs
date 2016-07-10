using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorySheduler.Views
{
    public partial class MainWindow : Form
    {
        private static MainWindow instance = new MainWindow(); //jediná instance této třídy
        private Action inicializeObserver; //callback pro dokončení view
        private NetworkScanView networkScanView; //panel se skenováním sítě a hledáním správných zařízení

        private MainWindow()
        {
            InitializeComponent();

            networkScanView = NetworkScanView.getInstance();
            networkScanView.Dock = DockStyle.Fill;

            this.Controls.Add(networkScanView);
        }

        public static MainWindow getInstance()
        {
            return instance;
        }

        /// <summary>
        /// Přiřazení posluchače pro dokončení a zobrazení view
        /// </summary>
        /// <param name="observer">metoda vykonaná při eventu</param>
        public void subscribeWindowShownObserver(Action observer)
        {
            inicializeObserver = observer;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            inicializeObserver();
        }

        delegate void ShowMessageCallback(MessageTypeEnum type, string message);

        /// <summary>
        /// Zobrazí hlášku v dolní části aplikace
        /// </summary>
        /// <param name="type">typ hlášky</param>
        /// <param name="message">text hlášky</param>
        public void showMessage(MessageTypeEnum type, string message)
        {
            switch (type)
            {
                case MessageTypeEnum.error:
                    messageLabel.ForeColor = Color.Red;
                    break;
                case MessageTypeEnum.success:
                    messageLabel.ForeColor = Color.Green;
                    break;
                case MessageTypeEnum.progress:
                    messageLabel.ForeColor = Color.Blue;
                    break;
            }

            if (messageLabel.InvokeRequired)
            {
                ShowMessageCallback cb = new ShowMessageCallback(showMessage);
                this.Invoke(cb, new object[] { type, message });
            }
            else
            {
                messageLabel.Text = message;
            }
        }
    }
}
