using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;

namespace FactorySheduler.Views
{
    /// <summary>
    /// View pro zobrazení mapy zařízení 
    /// </summary>
    public partial class MapView : UserControl
    {
        private List<Cart> carts = new List<Cart>(); //vozíky
        private Action buttonSearchNextDevicesCallback; //callback při kliknutí na tlačítko hledat další zařízení
        private Action buttonReinicializeStaticBeaconsCallback; //callback při kliknutí na tlačítko pro reinicializaci statických majáků
        private Action buttonEditMapCallback; //callback při klinutí natalčítko pro editaci mapy
        private Cart selectedCart; //právě vybraný vozík
        private Action<Cart> reinicializeCart; //callback při kliknutí na tlačítko reinicializace jednoho vozíku
        private const int sizeOfStaticBeacon = 10; //´velikost statického majáku v pixelech
        private List<Point> staticBeacons; //pozice statických majáků
        private int minStaticBeaconValue = 99999999; //max souřadnice statických majáků
        private int maxStaticBeaconValue = 0; //min souřadnice statických majáků
        private List<MapPoint[]> lines = new List<MapPoint[]>(); //seznam čar spojujících body na mapě
        private List<MapPoint> mapPoints = new List<MapPoint>(); //Body na mapě
        private ComponentResourceManager resourcesFromEditMapView; //zdroje s obrázky z editovací mapy


        public MapView(Action buttonSearchNextDevicesCallback, Action buttonReinicializeStaticBeaconsCallback, Action<Cart> reinicializeCart, Action buttonEditMapCallback, ComponentResourceManager resourcesFromEditMapView)
        {
            this.resourcesFromEditMapView = resourcesFromEditMapView;
            this.buttonSearchNextDevicesCallback = buttonSearchNextDevicesCallback;
            this.buttonReinicializeStaticBeaconsCallback = buttonReinicializeStaticBeaconsCallback;
            this.buttonEditMapCallback = buttonEditMapCallback;
            this.reinicializeCart = reinicializeCart;
            InitializeComponent();
            paintStaticBeacons();
            paintLines();
            paintMapPoints();
        }

        /// <summary>
        /// Přidání všech náležitostí ohledně vozíků do view
        /// </summary>
        /// <param name="carts"></param>
        public void addCarts(List<Cart> carts)
        {
            buttonsLayout.Controls.Clear();
            this.carts = carts;
            for (int j = 0; j < carts.Count(); j++)
            {
                Cart cart = carts[j];
                RadioButton button = new RadioButton();
                button.Appearance = Appearance.Button;
                button.AutoSize = true;
                button.Padding = new Padding(2, 2, 2, 2);
                button.Text = cart.name + "\n" + cart.alias;
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.FlatStyle = FlatStyle.Flat;
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
            buttonReinicializeCart.Enabled = enabled;
        }

        /// <summary>
        /// Nastaví do mapy statické majáky
        /// </summary>
        /// <param name="staticBeacons"></param>
        public void setStaticBeaconsPoints(List<Point> staticBeacons) {
            minStaticBeaconValue = 99999999;
            maxStaticBeaconValue = 0;
            this.staticBeacons = staticBeacons;
            foreach (Point beacon in staticBeacons)
            {
                if (beacon.X < minStaticBeaconValue) {
                    minStaticBeaconValue = beacon.X;
                }
                if (beacon.X > maxStaticBeaconValue)
                {
                    maxStaticBeaconValue = beacon.X;
                }
                if (beacon.Y < minStaticBeaconValue)
                {
                    minStaticBeaconValue = beacon.Y;
                }
                if (beacon.Y > maxStaticBeaconValue)
                {
                    maxStaticBeaconValue = beacon.Y;
                }
            }
        }

        /// <summary>
        /// Nastaví cesty na mapě
        /// </summary>
        /// <param name="mapLines">seznam cest</param>
        public void setMapLines(List<MapPoint[]> mapLines)
        {
            lines = mapLines;
        }

        /// <summary>
        /// Nastaví body na mapě
        /// </summary>
        /// <param name="mapPoints">Naměřené body</param>
        public void setMapPoints(List<MapPoint> mapPoints)
        {
            this.mapPoints = mapPoints;
            refreshAll();
        }

        /// <summary>
        /// Vykreslí statické majáky
        /// </summary>
        private void paintStaticBeacons() {
            mapBox.Paint += new PaintEventHandler(
                    delegate (object sender, PaintEventArgs e)
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        Brush brush = new SolidBrush(Color.Green);
                        foreach (Point beacon in staticBeacons)
                        {
                            int x = (int)Math.Round(getRescaledValue(beacon.X, false, false));
                            int y = (int)Math.Round(getRescaledValue(beacon.Y, true, false));
                            g.FillEllipse(brush, new Rectangle(x - (sizeOfStaticBeacon / 2), y - (sizeOfStaticBeacon / 2), sizeOfStaticBeacon, sizeOfStaticBeacon));
                        }
                    }
                );
        }

        /// <summary>
        /// Vykreslí body na mapě
        /// </summary>
        private void paintMapPoints()
        {
            mapBox.Paint += new PaintEventHandler(
                    delegate (object sender, PaintEventArgs e)
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        Brush brush = new SolidBrush(Color.Black);
                        Color backgroundColor;
                        foreach (MapPoint point in mapPoints)
                        {
                            backgroundColor = Color.LightSteelBlue;
                            Brush brushBackground = new SolidBrush(backgroundColor);
                            int x = (int)Math.Round(getRescaledValue(point.position.X, false, false));
                            int y = (int)Math.Round(getRescaledValue(point.position.Y, true, false));
                            g.FillEllipse(brushBackground, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            if (point.type == PointTypeEnum.charge)
                            {
                                Image newImage = ((Image)(resourcesFromEditMapView.GetObject("pictureBox1.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.fullTanks)
                            {
                                Image newImage = ((Image)(resourcesFromEditMapView.GetObject("pictureBox2.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.consumer)
                            {
                                Image newImage = ((Image)(resourcesFromEditMapView.GetObject("pictureBox3.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.emptyTanks)
                            {
                                Image newImage = ((Image)(resourcesFromEditMapView.GetObject("pictureBox4.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else {
                                g.FillEllipse(brush, new Rectangle(x - (sizeOfStaticBeacon / 2), y - (sizeOfStaticBeacon / 2), sizeOfStaticBeacon, sizeOfStaticBeacon));

                            }
                        }
                    }
                );
        }

        /// <summary>
        /// Vykreslení čáry na mapě
        /// </summary>
        private void paintLines()
        {
            mapBox.Paint += new PaintEventHandler(
                    delegate (object senderr, PaintEventArgs ee)
                    {
                        var g = ee.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        foreach (MapPoint[] line in lines)
                        {
                            Color color = Color.Black;
                            using (var p = new Pen(color, 3))
                            {
                                int x1 = (int)Math.Round(getRescaledValue(line[0].position.X, false, false));
                                int y1 = (int)Math.Round(getRescaledValue(line[0].position.Y, true, false));
                                int x2 = (int)Math.Round(getRescaledValue(line[1].position.X, false, false));
                                int y2 = (int)Math.Round(getRescaledValue(line[1].position.Y, true, false));
                                g.DrawLine(p, new Point(x1, y1), new Point(x2, y2));
                            }
                        }
                    });
        }

        /// <summary>
        /// Vrátí hodnotu přeškálovanou na z celkových rozměrů pokryté plochy majáky na plochu zobrazené mapy
        /// </summary>
        /// <param name="value">hodnota, kterou chceme přeškálovat</param>
        /// <param name="reversed">zda se má hodnota přeškálovat reverzně, tedy zda se má brát škála odzadu</param>
        /// <returns>přeškílovaná hodnota</returns>
        private double getRescaledValue(double value, bool reversed, bool normalized) {
            double min = minStaticBeaconValue;
            double max = maxStaticBeaconValue;
            if (normalized)
            {
                min = 0;
                max -= min;
            }

            if (reversed) {
                return MathLibrary.changeScale(value, max, min, 0, mapBox.Height);
            }
            else {
                return MathLibrary.changeScale(value, min, max, 0, mapBox.Height);
            }
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
                cart.asociatedButton.Text = cart.name + "\n" + cart.alias;
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
                selectedCart = cart;
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
            Color color = Color.Black;
            if (cart.asociatedButton.Checked) {
                color = Color.Goldenrod;
            }

            //vykreslení základny vozíku
            Pen pen = new Pen(color, (int)getRescaledValue(cart.distanceFromHedghogToLeftSideOfCart, false, true) + (int)getRescaledValue(cart.distanceFromHedghogToRightSideOfCart, false, true));
            pen.EndCap = LineCap.Round;
            MathLibrary.Point firstPoint = MathLibrary.getPointOnLine(getRescaledValue(cart.position.X, false, false), getRescaledValue(cart.position.Y, true, false), cart.angle - 180, (int)getRescaledValue(cart.longg, false, true) /2);
            MathLibrary.Point secondPoint = MathLibrary.getPointOnLine(getRescaledValue(cart.position.X, false, false), getRescaledValue(cart.position.Y, true, false), cart.angle, (int)getRescaledValue(cart.longg, false, true) /2);
            g.DrawLine(pen, (float)firstPoint.X, (float)firstPoint.Y, (float)secondPoint.X, (float)secondPoint.Y);

            paintCartAlias(g, cart);
        }

        /// <summary>
        /// Vykreslení zkratky na vozíku
        /// </summary>
        /// <param name="g">grafika mapy</param>
        /// <param name="cart">vozík</param>
        private void paintCartAlias(Graphics g, Cart cart) {
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            SizeF stringSize = g.MeasureString(cart.alias, drawFont);
            g.DrawString(cart.alias, drawFont, drawBrush, (int)getRescaledValue(cart.position.X, false, false) - (stringSize.Width / 2), (int)getRescaledValue(cart.position.Y, true, false) - (stringSize.Height / 2), new StringFormat());
        }

        private void buttonSearchNextDevices_Click(object sender, EventArgs e)
        {
            buttonSearchNextDevicesCallback();
        }

        private void buttonFront_Click(object sender, EventArgs e)
        {
            selectedCart.moveFront();
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            selectedCart.turnLeft();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            selectedCart.turnRight();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            selectedCart.moveBack();
        }

        private void buttonReinicializeCart_Click(object sender, EventArgs e)
        {
            reinicializeCart(selectedCart);
        }

        private void buttonReinicializeStaticBeacons_Click(object sender, EventArgs e)
        {
            buttonReinicializeStaticBeaconsCallback();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            String propertyName = e.ChangedItem.PropertyDescriptor.Name;
            selectedCart.propertyChanged(propertyName);
        }

        private void buttonEditMap_Click(object sender, EventArgs e)
        {
            buttonEditMapCallback();
        }
    }
}
