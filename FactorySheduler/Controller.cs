using FactorySheduler.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    class Controller
    {
        private MainWindow mainWindow; // hlavní okno apklikace
        private NetworkScanView networkScannerView; //view skaneru sítě
        private NetworkScanner networkScanner; //skaner sítě, hledající arduino zařízení na vozecích
        private Dictionary<String, Cart> carts = new Dictionary<String, Cart>(); //slovník kompatibilních vozíků podle jejich IP
        private int needCheckCount = 0; //Příznak, kolik zařízení v síti ještě potřebuje zkontrolovat
        private Dashboard dashboard = new Dashboard(); //Objekt představující připojení k Dashboard aplikaci
        private MapView mapView; //View pro zobrazení mapy zařízení 
        private EditMapView editMapView; //View pro editaci mapy
        private System.Windows.Forms.Timer periodicCheckerOfDashboardConnection = new System.Windows.Forms.Timer(); //periodický kontroler připojení k aplikaci dashboard (v případě selhání připojení)
        private System.Windows.Forms.Timer periodicCheckerOfMapPoints = new System.Windows.Forms.Timer(); //periodický kontroler naměřených bodů na mapě (v případě měření nových bodů mapy)
        private const bool test = true; //proměnná, která indikuje, že se má v případě selhání připojit simulační wifi síť se simulačními vozíky
        private List<MapPoint> mapPoints = new List<MapPoint>(); //body na mapě
        private List<MapPoint[]> mapLines = new List<MapPoint[]>(); //seznam čar spojujících body na mapě
        private List<Point> staticBeacons = new List<Point>(); //statické majáky na mapě
        private Cart mapPointCheckerDevice; //Zařízení s kterým se detekují bdy na mapě

        /// <param name="mainWindow">hlavní okno aplikace</param>
        public Controller(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            networkScannerView = NetworkScanView.getInstance();
            mainWindow.subscribeWindowShownObserver(inicialize);
            ChooseDeviceForPointDetectView.getInstance().setChooseCallback(deviceForDetectPointWasSelected);
        }

        /// <summary>
        /// Inicializace aplikace
        /// </summary>
        private void inicialize()
        {
            MapPointInputServer mapPointInputServer = MapPointInputServer.getMapPointInputServer();

            try
            {
                mapLines = MapMemory.DeSerializeObject<List<MapPoint[]>>("mapLines.xml");
            }
            catch (FileNotFoundException) { }
            try
            {
                mapPoints = MapMemory.DeSerializeObject<List<MapPoint>>("mapPoints.xml");
            }
            catch (FileNotFoundException) { }
            try
            {
                staticBeacons = MapMemory.DeSerializeObject<List<Point>>("staticBeacons.xml");
            }
            catch (FileNotFoundException) { }

            //TEST

            //staticBeacons.Add(new Point(0, 0));
            //staticBeacons.Add(new Point(100, 0));
            //staticBeacons.Add(new Point(0, 100));
            //staticBeacons.Add(new Point(100, 100));
            //mapPoints.Add(new MapPoint(new Point(50, 50)));
            //mapPoints.Add(new MapPoint(new Point(80, 20)));
            //mapPoints.Add(new MapPoint(new Point(10, 20)));

            //editMapView = new EditMapView(finishEditingMap, detectMapPoints, changeDeviceForDetectingPointOnMap);
            //mainWindow.setView(editMapView);
            //editMapView.setStaticBeaconsPoints(staticBeacons);
            //editMapView.setMapPoints(mapPoints);
            //editMapView.setMapLines(mapLines);

            nextStepAfterNetworkScan();

            //TEST

            //networkScanner = new NetworkScanner();
            //networkScanner.subscribeIPFoundObserver(createCart);
            //networkScannerView.subscribeButtonNextListener(nextStepAfterNetworkScan);
            //networkScannerView.subscribeButtonRefreshListener(scanNetwork);
            //scanNetwork();
        }

        /// <summary>
        /// Proskenuje Wifi síť a najde vyhovující zařízení
        /// </summary>
        private void scanNetwork()
        {
            if (networkScanner.getThisDeviceIp() != "")
            {
                if (test)
                {
                    addTestCarts(5);
                }
                networkScannerView.showThisDeviceIP(networkScanner.thisDeviceIP);
                mainWindow.showMessage(MessageTypeEnum.progress, "Hledám a rozeznávám zařízení v síti ...");
                networkScanner.scanNetwork(networkScanner.thisDeviceIP, finishIPSearching);

            }
            else {
                if (test)
                {
                    addTestCarts(5);
                    nextStepAfterNetworkScan();
                    mapView.startPeriodicRefresh();
                }
                mainWindow.showMessage(MessageTypeEnum.error, "Není připojena žádná vyhovující WiFi síť.");
            }
        }

        /// <summary>
        /// Přidá další testovací simulace vozíků pro testování
        /// </summary>
        /// <param name="count">počet přidaných vozíků</param>
        private void addTestCarts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                string ip = "192.254.48." + i;
                if (!carts.ContainsKey(ip))
                {
                    TestCart cart = new TestCart(ip, dashboard, startPeriodicScanOfDashboardConnection);
                    carts.Add(ip, cart);
                    cart.beaconAddress = i + 50;
                    cart.startPeriodicScanOfPosition();
                }
                needCheckCount++;
                networkScannerView.addDeviceIP(ip);
            }
        }

        /// <summary>
        /// Vytvoří instanci vozíku a přidá ho do seznamu vozíků
        /// </summary>
        /// <param name="ip">IP arduino zařízení na vozíku</param>
        private void createCart(string ip)
        {
            if (!carts.ContainsKey(ip))
            {
                Cart cart = new Cart(ip, dashboard, startPeriodicScanOfDashboardConnection);
                carts.Add(ip, cart);
            }
            needCheckCount++;
            networkScannerView.addDeviceIP(ip);
        }

        /// <summary>
        /// Callback pro dokončení hledání IP, na kterých by mohlo být arduino zařízení
        /// </summary>
        private void finishIPSearching()
        {
            List<Cart> cartsList = carts.Values.ToList();
            int cartsCount = cartsList.Count();
            for (int j = 0; j < cartsCount; j++)
            {
                Cart cart = cartsList[j];
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;

                bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                {
                    if (cart.checkConnectionToArduino())
                    {
                        networkScannerView.setIPStatus(cart.ip, true);
                    }
                    else {
                        carts.Remove(cart.ip);
                        networkScannerView.setIPStatus(cart.ip, false);
                    }
                });

                // what to do when worker completes its task (notify the user)
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object o, RunWorkerCompletedEventArgs args)
                {
                    mainWindow.setProgress((int)((cartsCount - needCheckCount) / (double)cartsCount * 100.0));
                    needCheckCount--;
                    if (needCheckCount == 0)
                    {
                        mainWindow.setProgress(100);
                        finishNetworkSearching();
                    }
                });

                bw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Dokončení skenování sítě
        /// </summary>
        private void finishNetworkSearching()
        {
            if (carts.Count == 0)
            {
                mainWindow.showMessage(MessageTypeEnum.error, "V síti nebylo nalezeno žádné kompatibilní zařízení.");
            }
            else {
                mainWindow.showMessage(MessageTypeEnum.success, "Bylo nalezeno " + carts.Count + " kompatibilních zařízení.");
                networkScannerView.enableNextButton();
                if (mapView != null)
                {
                    searchNextDevicesStep2();
                }
            }
            networkScannerView.setCountLabel(carts.Count);
        }

        /// <summary>
        /// Callback pro stisknutí tlačítka pro pokračování na obrazovce skenování sítě
        /// </summary>
        private void nextStepAfterNetworkScan()
        {
            mainWindow.setProgress(0);
            editMapView = new EditMapView(finishEditingMap, detectMapPoints, changeDeviceForDetectingPointOnMap);
            editMapView.setStaticBeaconsPoints(staticBeacons);
            editMapView.setMapPoints(mapPoints);
            editMapView.setMapLines(mapLines);
            mapView = new MapView(searchNextDevices, reinicializeStaticBeacons, reinicializeCart, editMap, editMapView.getResources());
            mapView.setStaticBeaconsPoints(staticBeacons);
            mapView.setMapPoints(mapPoints);
            mapView.setMapLines(mapLines);
            
            mainWindow.setView(mapView);
            mapView.addCarts(carts.Values.ToList());
            if (!connectToDashBoardAndStaticBeacons())
            {
                startPeriodicScanOfDashboardConnection();
            }
        }

        /// <summary>
        /// Dokončení editace mapy
        /// </summary>
        private void finishEditingMap() {
            if (periodicCheckerOfMapPoints != null) {
                periodicCheckerOfMapPoints.Dispose();
            }
            MapMemory.SerializeObject(mapPoints, "mapPoints.xml");
            MapMemory.SerializeObject(staticBeacons, "staticBeacons.xml");
            MapMemory.SerializeObject(mapLines, "mapLines.xml");
            mainWindow.setView(mapView);
        }

        /// <summary>
        /// Reinicializuje a statické majáky
        /// </summary>
        private void reinicializeStaticBeacons() {
            if (checkConnectionToStaticBeacons())
            {
                List<Point> staticBeaconsPositions = dashboard.getStaticBeaconsPositions();
                mapView.setStaticBeaconsPoints(staticBeaconsPositions);
                editMapView.setStaticBeaconsPoints(staticBeaconsPositions);
            }
        }

        /// <summary>
        /// Hledání dalších zařízení (první krok při opětovné inicializaci na stránce mapy)
        /// </summary>
        private void searchNextDevices()
        {
            mainWindow.setProgress(0);
            scanNetwork();
        }

        /// <summary>
        /// Párování zařízení (druhý krok při opětovné inicializaci na stránce mapy)
        /// </summary>
        private void searchNextDevicesStep2()
        {
            mainWindow.setProgress(0);
            mapView.addCarts(carts.Values.ToList());
            if (!connectToDashBoardAndStaticBeacons())
            {
                startPeriodicScanOfDashboardConnection();
            }
        }

        /// <summary>
        /// Spustí okno pro editaci mapy
        /// </summary>
        private void editMap() {
            mainWindow.setView(editMapView);
        }

        /// <summary>
        /// Započne periodické kontrolování připojení k aplikaci dashboard
        /// </summary>
        public void startPeriodicScanOfDashboardConnection()
        {
            if (periodicCheckerOfDashboardConnection.Enabled)
            {
                return;
            }
            periodicCheckerOfDashboardConnection = new System.Windows.Forms.Timer();
            periodicCheckerOfDashboardConnection.Interval = 3000;
            periodicCheckerOfDashboardConnection.Enabled = true;
            periodicCheckerOfDashboardConnection.Tick += delegate
            {
                if (connectToDashBoardAndStaticBeacons())
                {
                    periodicCheckerOfDashboardConnection.Dispose();
                }
            };
        }

        /// <summary>
        /// Připojení k aplikaci dashboard
        /// </summary>
        private bool connectToDashBoardAndStaticBeacons()
        {
            mainWindow.showMessage(MessageTypeEnum.progress, "Připojování k aplikaci dashboard ...");
            if (dashboard.checkConnectionToDashboard())
            {
                mapView.setEnableRefreshButton(true);
                mainWindow.showMessage(MessageTypeEnum.progress, "Kontrola statických majáků ...");
                if (((StringCollection)Properties.Settings.Default["staticBeacons"]).Count > 0)
                {
                    if (checkConnectionToStaticBeacons())
                    {
                        mainWindow.showMessage(MessageTypeEnum.progress, "Párování Arduino zařízení s ultrazvukovými majáky ...");
                        List<Point> staticBeaconsPositions = dashboard.getStaticBeaconsPositions();
                        mapView.setStaticBeaconsPoints(staticBeaconsPositions);
                        editMapView.setStaticBeaconsPoints(staticBeaconsPositions);
                        pairArduinosWithBeacons();
                        return true;
                    }
                    else {
                        mapView.setEnableRefreshButton(false);
                        return false;
                    }
                }
                else {
                    SettingsStaticBeacons.getInstance().ShowDialog();
                    return connectToDashBoardAndStaticBeacons();
                }
            }
            else {
                mapView.setEnableRefreshButton(false);
                mainWindow.showMessage(MessageTypeEnum.error, "Připojování k aplikaci dashboard se nezdařilo. Zapněte aplikaci Dashboard a nastavte jí UDP port 4444.");
                return false;
            }
        }



        /// <summary>
        /// Spáruje adresy nalezených majáků s Arduino zařízením
        /// </summary>
        private void pairArduinosWithBeacons()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                List<Cart> cartsList = carts.Values.ToList();
                List<string> beaconList = dashboard.getMobileBeaconsAdress();
                foreach (string address in beaconList)
                {
                    int addresss = int.Parse(address);
                    Point positionFromDashboard = dashboard.getDevicePosition(addresss);
                    for (int j = 0; j < cartsList.Count(); j++)
                    {
                        Cart cart = cartsList[j];
                        Point positionFromArduino = cart.getPositionFromArduino();
                        if (Math.Abs(positionFromArduino.X - positionFromDashboard.X) < 20 && Math.Abs(positionFromArduino.Y - positionFromDashboard.Y) < 20)
                        {
                            cart.beaconAddress = addresss;
                            cart.startPeriodicScanOfPosition();
                            cartsList.Remove(cartsList[j]);
                        }
                    }
                    b.ReportProgress((int)Math.Floor((beaconList.IndexOf(address) + 1) * (100.0 / beaconList.Count)));
                }
                for (int j = 0; j < cartsList.Count(); j++)
                {
                    Cart cart = cartsList[j];
                    cart.beaconAddress = -1;
                    if (cart.errorMessage == "")
                    {
                        cart.errorMessage = "Nebyl nalezen maják, který by šel spárovat s daným zařízením.";
                    }
                }
            });

            // what to do when progress changed (update the progress bar for example)
            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                mainWindow.setProgress(args.ProgressPercentage);
            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                if (test)
                {
                    List<Cart> cartsList = carts.Values.ToList();
                    for (int i = 0; i < cartsList.Count(); i++)
                    {
                        if (cartsList[i] is TestCart)
                        {
                            cartsList[i].beaconAddress = i + 50;
                            cartsList[i].errorMessage = "";
                        }
                    }
                }
                mainWindow.showMessage(MessageTypeEnum.success, "Párování bylo dokončeno.");
                mapView.startPeriodicRefresh();
            });

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Znovu spustí celý inicializační proces kromě vyhledávání zařízení v síti pro daný vozík
        /// </summary>
        /// <param name="cart">vozík</param>
        private void reinicializeCart(Cart cart)
        {
            mainWindow.setProgress(0);
            mainWindow.showMessage(MessageTypeEnum.progress, "Reinicializace zařízení...");
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                if (cart.errorMessage == "")
                {
                    cart.errorMessage = "Nebyl nalezen maják, který by šel spárovat s daným zařízením.";
                }
                List<string> beaconList = dashboard.getMobileBeaconsAdress();
                foreach (string address in beaconList)
                {
                    int addresss = int.Parse(address);

                    Point positionFromDashboard = dashboard.getDevicePosition(addresss);
                    Point positionFromArduino = cart.getPositionFromArduino();
                    if (Math.Abs(positionFromArduino.X - positionFromDashboard.X) < 20 && Math.Abs(positionFromArduino.Y - positionFromDashboard.Y) < 20)
                    {
                        cart.beaconAddress = addresss;
                        cart.startPeriodicScanOfPosition();
                        cart.errorMessage = "";
                        b.ReportProgress(100);
                        break;
                    }
                    else {
                        cart.beaconAddress = -1;
                        if (cart.errorMessage == "")
                        {
                            cart.errorMessage = "Nebyl nalezen maják, který by šel spárovat s daným zařízením.";
                        }
                    }

                    b.ReportProgress((int)Math.Floor((beaconList.IndexOf(address) + 1) * (100.0 / beaconList.Count)));
                }
            });

            // what to do when progress changed (update the progress bar for example)
            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                mainWindow.setProgress(args.ProgressPercentage);
            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                if (cart is TestCart)
                {
                    cart.beaconAddress = 50;
                    cart.errorMessage = "";
                }

                mainWindow.showMessage(MessageTypeEnum.success, "Párování bylo dokončeno.");
            });

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Zkontroluje, zda jsou připojeny všechny statické majáky
        /// </summary>
        /// <returns></returns>
        private bool checkConnectionToStaticBeacons()
        {
            bool ok = true;
            StringCollection notConnected = new StringCollection();
            StringCollection staticBeacons = (StringCollection)Properties.Settings.Default["staticBeacons"];
            for (int i = 0; i < staticBeacons.Count; i++)
            {
                if (!dashboard.isDeviceConnected(int.Parse(staticBeacons[i])))
                {
                    ok = false;
                    notConnected.Add(staticBeacons[i]);
                }
            }
            if (!ok)
            {
                string adressString = "";
                foreach (string adress in notConnected)
                {
                    if (adressString != "")
                    {
                        adressString += ", ";
                    }
                    adressString += adress;
                }
                mainWindow.showMessage(MessageTypeEnum.error, "Nepodařilo se získat informace ze statických majáků s adresou: " + adressString);
            }
            return ok;
        }

        /// <summary>
        /// Načte body na mapě ze zařízení, pokud je nějaké zvoleno nebo nechá nejdřív vybrat zařízení
        /// </summary>
        private void detectMapPoints()
        {
            if (mapPointCheckerDevice != null)
            {
                readMapPoints();
            }
            else {
                changeDeviceForDetectingPointOnMap();
            }
        }

        /// <summary>
        /// Zobrazí volbu zařízení pro detekci bodů na mapě
        /// </summary>
        private void changeDeviceForDetectingPointOnMap() {
            ChooseDeviceForPointDetectView dialog = ChooseDeviceForPointDetectView.getInstance();
            dialog.setDevices(carts.Values.ToList());
            dialog.ShowDialog();
        }

        /// <summary>
        /// Bylo vybráno zařízení pro detekci bodů na mapě
        /// </summary>
        /// <param name="device">zařízení</param>
        private void deviceForDetectPointWasSelected(Cart device) {
            mapPointCheckerDevice = device;
            editMapView.setDetectingDevice(device);
            readMapPoints();
        }

        /// <summary>
        /// Přečte ze zařízení poslední naměřené body na mapě
        /// </summary>
        private void readMapPoints() {
            List<MapPoint> points = mapPointCheckerDevice.getMapPoints();
            if (points.Count != 0) {
                mapPoints.AddRange(points);
            }
            editMapView.setMapPoints(mapPoints);
        }
    }
}
