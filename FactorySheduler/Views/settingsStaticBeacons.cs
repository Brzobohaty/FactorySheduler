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
    /// View pro nastavení statických majáků
    /// </summary>
    public partial class SettingsStaticBeacons : Form
    {
        private static SettingsStaticBeacons instance = new SettingsStaticBeacons(); //jediná instance této třídy
        
        public static SettingsStaticBeacons getInstance()
        {
            return instance;
        }

        public SettingsStaticBeacons()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Přidá jeden checkbox s adresou majáku
        /// </summary>
        /// <param name="adress">adresa majáku</param>
        /// <param name="staticc">příznak, zda je maják statický</param>
        private void addBeacon(string adress, bool staticc) {
            CheckBox checkBox = new CheckBox();
            checkBox.Text = adress;
            checkBox.Checked = staticc;
            flowLayoutPanel1.Controls.Add(checkBox);
        }

        /// <summary>
        /// Callback při zobrazení okna
        /// </summary>
        private void SettingsStaticBeacons_Shown(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            Dashboard dahsboard = new Dashboard();
            List<string> beaconAdress = dahsboard.getAllBeaconsAdress();
            for (int i=0;i< beaconAdress.Count; i++) {
                if (FactorySheduler.Properties.Settings.Default["staticBeacons"] != null)
                {
                    addBeacon(beaconAdress[i], ((StringCollection)FactorySheduler.Properties.Settings.Default["staticBeacons"]).Contains(beaconAdress[i]));
                }
                else {
                    addBeacon(beaconAdress[i], false);
                }
            }
        }

        /// <summary>
        /// Callback při kliknutí na tlačítko save
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            StringCollection staticBeacons = new StringCollection();
            for (int i=0;i< flowLayoutPanel1.Controls.Count ; i++){
                CheckBox checkBox = (CheckBox)flowLayoutPanel1.Controls[i];
                if (checkBox.Checked) {
                    staticBeacons.Add(checkBox.Text);
                }
            }
            if (staticBeacons.Count >= 3)
            {
                FactorySheduler.Properties.Settings.Default["staticBeacons"] = staticBeacons;
                FactorySheduler.Properties.Settings.Default.Save();
                DialogResult dialogResult = MessageBox.Show("Pro aplikování změn je potřeba restartovat aplikaci.", "Nastavení statických majáků", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else {
                DialogResult dialogResult = MessageBox.Show("Pro správnou funkčnost aplikace jsou zapotřebí alespoň 3 statické majáky.", "Nastavení statických majáků", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Callback při kliknutí na tlačítko cancel
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
