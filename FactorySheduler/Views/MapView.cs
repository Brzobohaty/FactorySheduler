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
    /// <summary>
    /// View pro zobrazení mapy zařízení 
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Přidání všech náležitostí ohledně vozíků do view
        /// </summary>
        /// <param name="carts"></param>
        public void addCarts(List<Cart> carts)
        {
            for (int j = 0; j < carts.Count(); j++)
            {
                //CartProperties cart = new CartProperties(carts[j]);
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
            }
        }

        /// <summary>
        /// Proskenuje všechny změny a náležitě je zobrazí
        /// </summary>
        public void refreshAll() {
            propertyGrid.Refresh();
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
    }
}
