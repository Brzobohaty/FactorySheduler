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
        private Action changeDeviceForDetectingPointOnMap; //callback při kliknutí na talčítko změny detekovacího zařízení
        private const int sizeOfStaticBeacon = 10; //´velikost statického majáku v pixelech
        private List<Point> staticBeacons = new List<Point>(); //pozice statických majáků
        private List<MapPoint> mapPoints = new List<MapPoint>(); //Body na mapě
        private Cart detectingDevice; //zařízení, které detekuje body na mapě
        private List<MapPoint[]> lines = new List<MapPoint[]>(); //seznam čar spojujících body
        private List<MapPoint[]> linesForDeleting = new List<MapPoint[]>(); //seznam čar, které mají být smazány
        private MapPoint[] markedLine; //zvýrazněná čára
        private int minStaticBeaconValue = 99999999; //min souřadnice statických majáků
        private int maxStaticBeaconValue = 0; //max souřadnice statických majáků
        private const int PEN_WIDTH = 3; //šířka vykreslované čáry
        MapPoint mouseSecondPoint = new MapPoint(Point.Empty); //druhý bod pro vykreslení aktuální čáry (v reálných souřadnicích)
        MapPoint mouseFirstPoint = new MapPoint(Point.Empty); //první bod pro vykreslení aktuální čáry (v reálných souřadnicích)
        Point mouseSecondPointRescaled = Point.Empty; //druhý bod pro vykreslení aktuální čáry (ve vykreslovacích souřadnicích)
        Point mouseFirstPointRescaled = Point.Empty; //první bod pro vykreslení aktuální čáry (ve vykreslovacích souřadnicích)
        private bool wasFirstClickedOnMap = false; //příznak, zda bylo kliknuto na mapu poprvé
        private MapPoint selectedPointTemp = new MapPoint(Point.Empty); //bod, s kterým se budou provádět akce pomocí pravého tlačítka
        private MapPoint selectedPoint = new MapPoint(Point.Empty); //bod, jehož proměnné jsou právě zobrazeny
        private ContextMenu pointMenu; //menu při kliknutí pravým tlačítkem na bod na mapě
        Point currentPointRescaledTemp = new Point(); //bod v přepočítaných souřadnicicíh, na který bylo právě kliknuto
        MapPoint currentPointTemp = new MapPoint(Point.Empty); //bod ve skutečné souřadnicích, na který bylo právě kliknuto
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMapView));

        public EditMapView(Action buttonFinishCallback, Action buttonDetectPointsCallback, Action changeDeviceForDetectingPointOnMap)
        {
            this.changeDeviceForDetectingPointOnMap = changeDeviceForDetectingPointOnMap;
            this.buttonFinishCallback = buttonFinishCallback;
            this.buttonDetectPointsCallback = buttonDetectPointsCallback;

            InitializeComponent();

            paintLines();
            paintStaticBeacons();
            paintMapPoints();
            paintPositionOfDevice();

            startPeriodicRefresh();
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
        /// Nastaví naměřené body na mapě
        /// </summary>
        /// <param name="mapPoints">Naměřené body</param>
        public void setMapPoints(List<MapPoint> mapPoints)
        {
            this.mapPoints = mapPoints;
            refreshAll();
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
            refreshAll();
        }

        /// <summary>
        /// Nastavení zařízení, s kterým bude docházet k detekování bodů
        /// </summary>
        /// <param name="device">zařízení</param>
        public void setDetectingDevice(Cart device)
        {
            detectingDevice = device;
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
                        if (detectingDevice != null)
                        {
                            var g = e.Graphics;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            Brush brush = new SolidBrush(Color.Gold);
                            int x = (int)Math.Round(getRescaledValue(detectingDevice.position.X, false, false));
                            int y = (int)Math.Round(getRescaledValue(detectingDevice.position.Y, true, false));
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
                            if (point == selectedPoint)
                            {
                                backgroundColor = Color.Tomato;
                            }
                            else {
                                backgroundColor = Color.LightSteelBlue;
                            }
                            Brush brushBackground = new SolidBrush(backgroundColor);
                            int x = (int)Math.Round(getRescaledValue(point.position.X, false, false));
                            int y = (int)Math.Round(getRescaledValue(point.position.Y, true, false));
                            g.FillEllipse(brushBackground, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            if (point.type == PointTypeEnum.charge)
                            {
                                Image newImage = ((Image)(resources.GetObject("pictureBox1.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.fullTanks)
                            {
                                Image newImage = ((Image)(resources.GetObject("pictureBox2.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.consumer)
                            {
                                Image newImage = ((Image)(resources.GetObject("pictureBox3.Image")));
                                g.DrawImage(newImage, x - sizeOfStaticBeacon, y - sizeOfStaticBeacon, sizeOfStaticBeacon * 2, sizeOfStaticBeacon * 2);
                            }
                            else if (point.type == PointTypeEnum.emptyTanks)
                            {
                                Image newImage = ((Image)(resources.GetObject("pictureBox4.Image")));
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
                            if (line.Equals(markedLine))
                            {
                                color = Color.Red;
                            }
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
        /// Přidá unikátní cestu do seznamu cest
        /// </summary>
        /// <param name="newLine">nová cesta</param>
        private void addLine(MapPoint[] newLine)
        {
            foreach (MapPoint[] oldLine in lines)
            {
                if ((oldLine[0].Equals(newLine[0]) && oldLine[1].Equals(newLine[1])) || (oldLine[0].Equals(newLine[1]) && oldLine[1].Equals(newLine[0])))
                {
                    return;
                }
            }
            lines.Add(newLine);
            newLine[0].addPath(newLine[1]);
            newLine[1].addPath(newLine[0]);
        }

        /// <summary>
        /// Smaže bod z mapy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deletePoint(object sender, EventArgs e)
        {
            List<MapPoint[]> linesForDeleting = new List<MapPoint[]>();
            foreach (MapPoint[] line in lines)
            {
                if (line[0].Equals(selectedPointTemp) || line[1].Equals(selectedPointTemp))
                {
                    linesForDeleting.Add(line);
                }
            }
            foreach (MapPoint[] line in linesForDeleting)
            {
                lines.Remove(line);
                line[0].removePath(line[1]);
                line[1].removePath(line[0]);
            }
            mapPoints.Remove(selectedPointTemp);
            mapBox.Refresh();
        }

        /// <summary>
        /// Smaže cestu z mapy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteLine(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            MapPoint[] line = linesForDeleting.ElementAt(item.Index - 5);
            line[0].removePath(line[1]);
            line[1].removePath(line[0]);
            lines.Remove(line);
            mapBox.Refresh();
        }

        /// <summary>
        /// Zvýrazní cestu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void markLine(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item.Index >= 5)
            {
                markedLine = linesForDeleting.ElementAt(item.Index - 5);
            }
            else {
                markedLine = new MapPoint[2];
            }
            mapBox.Refresh();
        }

        /// <summary>
        /// Zobrazí menu pro bod
        /// </summary>
        /// <param name="currentPoint"> bod pro který se má menu zobrazit</param>
        /// <param name="mouseLocation"> pozice kurzoru myši</param>
        private void showMenuForPoint(MapPoint currentPoint, Point mouseLocation)
        {
            selectedPointTemp = currentPoint;
            linesForDeleting.Clear();

            List<MenuItem> menuItemsList = new List<MenuItem>();

            MenuItem deletePointMenuItem = new MenuItem("Smazat bod");
            deletePointMenuItem.Click += new EventHandler(deletePoint);
            deletePointMenuItem.Select += new EventHandler(markLine);
            menuItemsList.Add(deletePointMenuItem);

            if (selectedPointTemp.type != PointTypeEnum.charge)
            {
                MenuItem makeChargingStationFromPointMenuItem = new MenuItem("Udělat z bodu nabíjecí stanici");
                makeChargingStationFromPointMenuItem.Click += new EventHandler(makeChargingStationFromPoint);
                makeChargingStationFromPointMenuItem.Select += new EventHandler(markLine);
                menuItemsList.Add(makeChargingStationFromPointMenuItem);
            }

            if (selectedPointTemp.type != PointTypeEnum.fullTanks)
            {
                MenuItem makeStationWithFullTanksFromPointMenuItem = new MenuItem("Udělat z bodu stanici s plnými zásobníky");
                makeStationWithFullTanksFromPointMenuItem.Click += new EventHandler(makeStationWithFullTanksFromPoint);
                makeStationWithFullTanksFromPointMenuItem.Select += new EventHandler(markLine);
                menuItemsList.Add(makeStationWithFullTanksFromPointMenuItem);
            }

            if (selectedPointTemp.type != PointTypeEnum.emptyTanks)
            {
                MenuItem makeStationForEmptyTanksFromPointMenuItem = new MenuItem("Udělat z bodu stanici pro prázdné zásobníky");
                makeStationForEmptyTanksFromPointMenuItem.Click += new EventHandler(makeStationForEmptyTanksFromPoint);
                makeStationForEmptyTanksFromPointMenuItem.Select += new EventHandler(markLine);
                menuItemsList.Add(makeStationForEmptyTanksFromPointMenuItem);
            }

            if (selectedPointTemp.type != PointTypeEnum.consumer)
            {
                MenuItem makeConsumerStationFromPointMenuItem = new MenuItem("Udělat z bodu spotřební stanici");
                makeConsumerStationFromPointMenuItem.Click += new EventHandler(makeConsumerStationFromPoint);
                makeConsumerStationFromPointMenuItem.Select += new EventHandler(markLine);
                menuItemsList.Add(makeConsumerStationFromPointMenuItem);
            }

            if (selectedPointTemp.type != PointTypeEnum.init)
            {
                MenuItem makeConsumerStationFromPointMenuItem = new MenuItem("Udělat z bodu průchozí bod");
                makeConsumerStationFromPointMenuItem.Click += new EventHandler(makeInitFromPoint);
                makeConsumerStationFromPointMenuItem.Select += new EventHandler(markLine);
                menuItemsList.Add(makeConsumerStationFromPointMenuItem);
            }

            int index = 0;
            foreach (MapPoint[] line in lines)
            {
                if (line[0].Equals(currentPoint) || line[1].Equals(currentPoint))
                {
                    MenuItem deleteLineMenuItem = new MenuItem("Smazat cestu " + (index++ + 1));
                    deleteLineMenuItem.Click += new EventHandler(deleteLine);
                    deleteLineMenuItem.Select += new EventHandler(markLine);
                    menuItemsList.Add(deleteLineMenuItem);
                    linesForDeleting.Add(line);
                }
            }

            MenuItem[] menuItems = menuItemsList.ToArray();
            pointMenu = new ContextMenu(menuItems);

            pointMenu.Show(mapBox, mouseLocation);
        }

        /// <summary>
        /// Udělá z bodu nabíjecí stanici
        /// </summary>
        private void makeChargingStationFromPoint(object sender, EventArgs e)
        {
            selectedPointTemp.type = PointTypeEnum.charge;
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Udělá z bodu stanici pro plné zásobníky
        /// </summary>
        private void makeStationWithFullTanksFromPoint(object sender, EventArgs e)
        {
            selectedPointTemp.type = PointTypeEnum.fullTanks;
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Udělá z bodu stanici pro prázdné zásobníky
        /// </summary>
        private void makeStationForEmptyTanksFromPoint(object sender, EventArgs e)
        {
            selectedPointTemp.type = PointTypeEnum.emptyTanks;
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Udělá z bodu spotřební stanici
        /// </summary>
        private void makeConsumerStationFromPoint(object sender, EventArgs e)
        {
            selectedPointTemp.type = PointTypeEnum.consumer;
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Udělá z bodu průchozí bod
        /// </summary>
        private void makeInitFromPoint(object sender, EventArgs e)
        {
            selectedPointTemp.type = PointTypeEnum.init;
            propertyGrid.Refresh();
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

        private void buttonChooseDevice_Click(object sender, EventArgs e)
        {
            changeDeviceForDetectingPointOnMap();
        }

        private void mapBox_MouseMove(object sender, MouseEventArgs e)
        {
            markedLine = new MapPoint[2];

            bool isPointTemp = isPoint(e.Location);

            if (isPointTemp)
            {
                Cursor.Current = Cursors.Hand;
            }

            if (wasFirstClickedOnMap)
            {
                Bitmap bm = new Bitmap(mapBox.ClientSize.Width, mapBox.ClientSize.Height);
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Clear last line drawn
                    using (Pen clear_pen = new Pen(mapBox.BackColor, PEN_WIDTH))
                    {
                        g.DrawLine(clear_pen, mouseSecondPointRescaled, mouseFirstPointRescaled);
                    }

                    // Update previous point
                    mouseFirstPointRescaled = e.Location;
                    Color color;
                    if (isPointTemp)
                    {
                        color = Color.Black;
                    }
                    else {
                        color = Color.Gray;
                    }

                    // Draw the new line
                    using (Pen draw_pen = new Pen(color, PEN_WIDTH))
                    {
                        g.DrawLine(draw_pen, mouseSecondPointRescaled, e.Location);
                    }
                }
                mapBox.Image = bm;
            }

            mapBox.Refresh();
        }

        private void mapBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (isPoint(e.Location))
            {
                if (e.Button == MouseButtons.Left)
                {
                    pointClicked(currentPointTemp);
                }
                else {
                    showMenuForPoint(currentPointTemp, e.Location);
                }
            }
        }

        private void mapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPoint(e.Location) && e.Button == MouseButtons.Left)
            {
                mouseSecondPointRescaled = currentPointRescaledTemp;
                mouseSecondPoint = currentPointTemp;
                wasFirstClickedOnMap = true;
            }
        }

        private void mapBox_MouseUp(object sender, MouseEventArgs e)
        {

            if (isPoint(e.Location) && e.Button == MouseButtons.Left && !mouseSecondPoint.Equals(currentPointTemp))
            {
                wasFirstClickedOnMap = false;

                MapPoint[] line = new MapPoint[2];
                line[0] = mouseSecondPoint;
                line[1] = currentPointTemp;
                addLine(line);
                mapBox.Refresh();

                clearLastLine();
                mapBox.Refresh();
            }
            else {
                wasFirstClickedOnMap = false;
                clearLastLine();
            }
        }

        /// <summary>
        /// Odebere naposledy vykreslenou čáru na mapě
        /// </summary>
        private void clearLastLine()
        {
            Bitmap bm = new Bitmap(mapBox.ClientSize.Width, mapBox.ClientSize.Height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen clear_pen = new Pen(mapBox.BackColor, PEN_WIDTH))
                {
                    g.DrawLine(clear_pen, mouseSecondPointRescaled, mouseFirstPointRescaled);
                }
            }
            mapBox.Image = bm;
        }

        /// <summary>
        /// Jedná se o souřadnice nějakého bodu na mapě
        /// </summary>
        /// <param name="location">souřadnice</param>
        /// <returns>true pokud se jedná o bod na mapě</returns>
        private bool isPoint(Point location)
        {
            currentPointRescaledTemp = new Point();
            currentPointTemp = new MapPoint(Point.Empty);
            foreach (MapPoint point in mapPoints)
            {
                Point pointRescaled = new Point((int)Math.Round(getRescaledValue(point.position.X, false, false)), (int)Math.Round(getRescaledValue(point.position.Y, true, false)));

                if (Math.Abs(location.X - pointRescaled.X) < sizeOfStaticBeacon && Math.Abs(location.Y - pointRescaled.Y) < sizeOfStaticBeacon)
                {
                    currentPointRescaledTemp = pointRescaled;
                    currentPointTemp = point;
                    return true;
                }
            }
            return false;
        }

        private void mapBox_ClientSizeChanged(object sender, EventArgs e)
        {
            refreshAll();
        }

        /// <summary>
        /// Listener pro kliknutí na bod
        /// </summary>
        private void pointClicked(MapPoint point)
        {
            selectedPoint = point;
            selectedPoint.setUpdateCallback(updatePropertzGrid);
            propertyGrid.SelectedObject = point;
        }

        /// <summary>
        /// Callback, že došlo ke změně proměnné v tabulce vlastností bodů
        /// </summary>
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string propertyName = e.ChangedItem.PropertyDescriptor.Name;
            selectedPoint.propertyChanged(propertyName);
            propertyGrid.Refresh();
        }

        /// <summary>
        /// Aktualizuje tabulku s vlastnostmi bodů
        /// </summary>
        delegate void UpdatePropertzGrid();
        private void updatePropertzGrid() {
            if (propertyGrid.InvokeRequired)
            {
                UpdatePropertzGrid cb = new UpdatePropertzGrid(updatePropertzGrid);
                this.Invoke(cb, new object[] {});
            }
            else
            {
                propertyGrid.Refresh();
            }
            
        }
    }
}
