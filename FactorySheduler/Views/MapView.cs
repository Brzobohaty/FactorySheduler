using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FactorySheduler.Views
{
    /// <summary>
    /// View pro zobrazení mapy zařízení 
    /// </summary>
    public partial class MapView : UserControl
    {
        private List<Cart> carts; //vozíky
        private Action buttonSearchNextDevicesCallback; //callback při kliknutí na tlačítko hledat další zařízení

        public MapView(Action buttonSearchNextDevicesCallback)
        {
            this.buttonSearchNextDevicesCallback = buttonSearchNextDevicesCallback;
            InitializeComponent();
        }

        /// <summary>
        /// Přidání všech náležitostí ohledně vozíků do view
        /// </summary>
        /// <param name="carts"></param>
        public void addCarts(List<Cart> carts)
        {
            this.carts = carts;
            for (int j = 0; j < carts.Count(); j++)
            {
                Cart cart = carts[j];
                RadioButton button = new RadioButton();
                button.Appearance = Appearance.Button;
                button.AutoSize = true;
                button.Padding = new Padding(2, 2, 2, 2);
                button.Text = cart.name; // + "\n" + "připojeno"
                buttonsLayout.Controls.Add(button);
                button.CheckedChanged += buttonDeviceClicked;
                button.Tag = cart;
                if (j==0)
                {
                    button.Checked = true;
                    showProperties(cart);
                }
                cart.asociatedButton = button;
            }
            paintCarts();
        }

        /// <summary>
        /// Započne periodické obnovování všech potřebných komponent
        /// </summary>
        public void startPeriodicRefresh()
        {
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Zapnutí/vypnutí tlačítka pro hledání dalších zařízení
        /// </summary>
        /// <param name="enabled">true pokud zapnout</param>
        public void setEnableRefreshButton(bool enabled) {
            buttonSearchNextDevices.Enabled = enabled;
        }

        /// <summary>
        /// Všechno co se má v čase aktualzovat bude aktualizováno
        /// </summary>
        private void refreshAll() {
            if (!propertyGrid.ContainsFocus) {
                propertyGrid.Refresh();
            }
            for (int j = 0; j < carts.Count(); j++)
            {
                Cart cart = carts[j];
                cart.asociatedButton.Text = cart.name;
                if (cart.errorMessage == "")
                {
                    cart.asociatedButton.BackColor = Color.Green;
                }
                else {
                    cart.asociatedButton.BackColor = Color.Red;
                }
            }
            mapBox.Refresh();
        }

        /// <summary>
        /// Ukáže všechny proměnné daného vozíku
        /// </summary>
        /// <param name="cart">vozík, jehož proměnné se mají zobrazit</param>
        private void showProperties(Cart cart) {
            propertyGrid.SelectedObject = cart;
        }

        /// <summary>
        /// Listener pro změnu stavu tlačítka pro výběr zařízení.
        /// </summary>
        private void buttonDeviceClicked(object sender, EventArgs e)
        {
            RadioButton button = (RadioButton)sender;

            if (button.Checked)
            {
                Cart cart = (Cart)button.Tag;
                showProperties(cart);
            }
        }

        /// <summary>
        /// Jeden cyklus timeru
        /// </summary>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            refreshAll();
        }

        private void buttonSearchNextDevices_Click(object sender, EventArgs e)
        {
            buttonSearchNextDevicesCallback();
        }

        /// <summary>
        /// Nastavení vykreslování všech vozíků na mapě
        /// </summary>
        private void paintCarts()
        {
            for (int i = 0; i < carts.Count(); i++)
            {
                Cart cart = carts[i];
                mapBox.Paint += new PaintEventHandler(
                    delegate (object sender, PaintEventArgs e)
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        paintCart(g, cart);
                    }
                );
            }
        }

        /// <summary>
        /// Vykreslení vozíku
        /// </summary>
        /// <param name="g">grafika mapy</param>
        /// <param name="cart">vozík</param>
        private void paintCart(Graphics g, Cart cart)
        {
            //vykreslení základny vozíku
            Pen pen = new Pen(Color.Black, cart.distanceFromHedghogToLeftSideOfCart+cart.distanceFromHedghogToRightSideOfCart);
            MathLibrary.Point firstPoint = MathLibrary.getPointOnLine(cart.position.X, cart.position.Y, cart.angle - 180, cart.longg/2);
            MathLibrary.Point secondPoint = MathLibrary.getPointOnLine(cart.position.X, cart.position.Y, cart.angle, cart.longg/2);
            g.DrawLine(pen, (float)firstPoint.X, (float)firstPoint.Y, (float)secondPoint.X, (float)secondPoint.Y);

            //vzkreslení názvu
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            SizeF stringSize = g.MeasureString(cart.alias, drawFont);
            g.DrawString(cart.alias, drawFont, drawBrush, cart.position.X-(stringSize.Width/2), cart.position.Y - (stringSize.Height / 2), new StringFormat());
        }
    }
}
