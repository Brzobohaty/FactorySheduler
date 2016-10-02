using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace FactorySheduler.Views
{
    /// <summary>
    /// View pro editaci mapy 
    /// </summary>
    public partial class EditMapView : UserControl
    {
        private Action buttonFinishCallback; //callback při kliknutí na tlačítko dokončení editace
        private Action buttonDetectPointsCallback; //callback při kliknutí na tlačítko detekce bodů
        private const int sizeOfStaticBeacon = 10; //´velikost statického majáku v pixelech
        private List<Point> staticBeacons; //pozice statických majáků
        private List<Point> mapPoints; //Body na mapě
        private Cart detectingDevice; //zařízení, které detekuje body na mapě
        //max a min souřadnice statických majáků
        private int minStaticBeaconValue = 99999999;
        private int maxStaticBeaconValue = 0;


        public EditMapView(Action buttonFinishCallback, Action buttonDetectPointsCallback)
        {
            this.buttonFinishCallback = buttonFinishCallback;
            this.buttonDetectPointsCallback = buttonDetectPointsCallback;
            InitializeComponent();
        }

        /// <summary>
        /// Nastaví naměřené body na mapě
        /// </summary>
        /// <param name="mapPoints">Naměřené body</param>
        public void setMapPoints(List<Point> mapPoints)
        {
            this.mapPoints = mapPoints;
            paintMapPoints();
        }

        /// <summary>
        /// Nastaví do mapy statické majáky
        /// </summary>
        /// <param name="staticBeacons"></param>
        public void setStaticBeaconsPoints(List<Point> staticBeacons)
        {
            minStaticBeaconValue = 99999999;
            maxStaticBeaconValue = 0;
            this.staticBeacons = staticBeacons;
            foreach (Point beacon in staticBeacons)
            {
                if (beacon.X < minStaticBeaconValue)
                {
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
            paintStaticBeacons();
        }

        /// <summary>
        /// Nastavení zařízení, s kterým bude docházet k detekování bodů
        /// </summary>
        /// <param name="device">zařízení</param>
        public void setDetectingDevice(Cart device)
        {
            detectingDevice = device;
            paintPositionOfDevice();
            startPeriodicRefresh();
        }

        /// <summary>
        /// Vykreslí statické majáky
        /// </summary>
        private void paintStaticBeacons()
        {
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
        /// Vykreslí aktuální pozici zařízení pro detekování bodů na mapě
        /// </summary>
        private void paintPositionOfDevice()
        {
            mapBox.Paint += new PaintEventHandler(
                    delegate (object sender, PaintEventArgs e)
                    {
                        var g = e.Graphics;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        Brush brush = new SolidBrush(Color.Gold);
                        int x = (int)Math.Round(getRescaledValue(detectingDevice.position.X, false, false));
                        int y = (int)Math.Round(getRescaledValue(detectingDevice.position.Y, true, false));
                        g.FillEllipse(brush, new Rectangle(x - (sizeOfStaticBeacon / 2), y - (sizeOfStaticBeacon / 2), sizeOfStaticBeacon, sizeOfStaticBeacon));
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
                        foreach (Point point in mapPoints)
                        {
                            int x = (int)Math.Round(getRescaledValue(point.X, false, false));
                            int y = (int)Math.Round(getRescaledValue(point.Y, true, false));
                            g.FillEllipse(brush, new Rectangle(x - (sizeOfStaticBeacon / 2), y - (sizeOfStaticBeacon / 2), sizeOfStaticBeacon, sizeOfStaticBeacon));
                        }
                    }
                );
        }

        /// <summary>
        /// Vrátí hodnotu přeškálovanou na z celkových rozměrů pokryté plochy majáky na plochu zobrazené mapy
        /// </summary>
        /// <param name="value">hodnota, kterou chceme přeškálovat</param>
        /// <param name="reversed">zda se má hodnota přeškálovat reverzně, tedy zda se má brát škála odzadu</param>
        /// <returns>přeškílovaná hodnota</returns>
        private double getRescaledValue(double value, bool reversed, bool normalized)
        {
            double min = minStaticBeaconValue;
            double max = maxStaticBeaconValue;
            if (normalized)
            {
                min = 0;
                max -= min;
            }

            if (reversed)
            {
                return MathLibrary.changeScale(value, max, min, 0, mapBox.Height);
            }
            else {
                return MathLibrary.changeScale(value, min, max, 0, mapBox.Height);
            }
        }

        /// <summary>
        /// Započne periodické obnovování všech potřebných komponent
        /// </summary>
        public void startPeriodicRefresh()
        {
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Všechno co se má v čase aktualzovat bude aktualizováno
        /// </summary>
        private void refreshAll()
        {
            mapBox.Refresh();
        }

        /// <summary>
        /// Jeden cyklus timeru
        /// </summary>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            refreshAll();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            buttonFinishCallback();
        }

        private void buttonDetectPoints_Click(object sender, EventArgs e)
        {
            buttonDetectPointsCallback();
        }
    }
}
