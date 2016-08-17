using FactorySheduler.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        private System.Windows.Forms.Timer periodicCheckerOfDashboardConnection = new System.Windows.Forms.Timer(); //periodický kontroler připojení k aplikaci dashboard (v případě selhání připojení)
        private const bool test = true; //proměnná, která indikuje, že se má v případě selhání připojit simulační wifi síť se simulačními vozíky
        private bool networkScanCompleted;

        /// <param name="mainWindow">hlavní okno aplikace</param>
        public Controller(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            networkScannerView = NetworkScanView.getInstance();
            mainWindow.subscribeWindowShownObserver(inicialize);
        }

        /// <summary>
        /// Inicializace aplikace
        /// </summary>
        private void inicialize()
        {
            networkScanner = new NetworkScanner();
            networkScanner.subscribeIPFoundObserver(createCart);
            networkScannerView.subscribeButtonNextListener(nextStepAfterNetworkScan);
            networkScannerView.subscribeButtonRefreshListener(scanNetwork);
            scanNetwork();
        }

        /// <summary>
        /// Proskenuje Wifi síť a najde vyhovující zařízení
        /// </summary>
        private void scanNetwork() {
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
                if (test) {
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
        private void addTestCarts(int count) {
            for (int i = 0; i < count; i++)
            {
                string ip = "192.254.48."+i;
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
            if (!carts.ContainsKey(ip)) {
                Cart cart = new Cart(ip, dashboard, startPeriodicScanOfDashboardConnection);
                carts.Add(ip, cart);
            }
            needCheckCount++;
            networkScannerView.addDeviceIP(ip);
        }

        /// <summary>
        /// Callback pro dokončení hledání IP, na kterých by mohlo být arduino zařízení
        /// </summary>
        private void finishIPSearching() {
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
                if (mapView != null) {
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
            mapView = new MapView(searchNextDevices);
            mainWindow.setView(mapView);
            mapView.addCarts(carts.Values.ToList());
            if (!connectToDashBoard()) {
                startPeriodicScanOfDashboardConnection();
            }
        }

        private void searchNextDevices() {
            mainWindow.setProgress(0);
            scanNetwork();
            //TODO dodělat párování
        }

        private void searchNextDevicesStep2()
        {
            mainWindow.setProgress(0);
            mapView.addCarts(carts.Values.ToList());
            if (!connectToDashBoard())
            {
                startPeriodicScanOfDashboardConnection();
            }
            //TODO dodělat párování
        }

        /// <summary>
        /// Započne periodické kontrolování připojení k aplikaci dashboard
        /// </summary>
        public void startPeriodicScanOfDashboardConnection()
        {
            if (periodicCheckerOfDashboardConnection.Enabled) {
                return;
            }
            periodicCheckerOfDashboardConnection = new System.Windows.Forms.Timer();
            periodicCheckerOfDashboardConnection.Interval = 3000;
            periodicCheckerOfDashboardConnection.Enabled = true;
            periodicCheckerOfDashboardConnection.Tick += delegate
            {
                if (connectToDashBoard())
                {
                    periodicCheckerOfDashboardConnection.Dispose();
                }
            };
        }

        /// <summary>
        /// Připojení k aplikaci dashboard
        /// </summary>
        private bool connectToDashBoard() {
            mainWindow.showMessage(MessageTypeEnum.progress, "Připojování k aplikaci dashboard ...");
            if (dashboard.checkConnectionToDashboard())
            {
                mapView.setEnableRefreshButton(true);
                mainWindow.showMessage(MessageTypeEnum.progress, "Párování Arduino zařízení s ultrazvukovými majáky ...");
                pairArduinosWithBeacons();
                return true;
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
                int maxBeaconAdress = 20;
                for (int address = 1; address <= maxBeaconAdress; address++)
                {
                    if (dashboard.isDeviceConnected(address))
                    {
                        Point positionFromDashboard = dashboard.getDevicePosition(address);
                        if (positionFromDashboard.X == 0 && positionFromDashboard.Y == 0)
                        {
                            b.ReportProgress((int)Math.Floor((address + 1) * (100.0 / (maxBeaconAdress + 1))));
                            continue;
                        }
                        int cartsCount = cartsList.Count();
                        for (int j = 0; j < cartsCount; j++)
                        {
                            Cart cart = cartsList[j];
                            Point positionFromArduino = cart.getPositionFromArduino();
                            if (positionFromDashboard.X == 0 && positionFromDashboard.Y == 0)
                            {
                                continue;
                            }
                            if (Math.Abs(positionFromArduino.X - positionFromDashboard.X) < 20 && Math.Abs(positionFromArduino.Y - positionFromDashboard.Y) < 20)
                            {
                                cart.beaconAddress = address;
                                cart.startPeriodicScanOfPosition();
                                cartsList.Remove(cartsList[j]);
                            }
                        }
                    }
                    b.ReportProgress((int)Math.Floor((address + 1) * (100.0 / (maxBeaconAdress + 1))));
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
    }
}
