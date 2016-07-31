using FactorySheduler.Views;
using System;
using System.Collections.Generic;
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
            if (networkScanner.thisDeviceIP != "")
            {
                mainWindow.showMessage(MessageTypeEnum.progress, "Hledám a rozeznávám zařízení v síti ...");
                networkScannerView.showThisDeviceIP(networkScanner.thisDeviceIP);
                networkScanner.scanNetwork(networkScanner.thisDeviceIP);
            }
            else {
                mainWindow.showMessage(MessageTypeEnum.error, "Není připojena žádná vyhovující WiFi síť.");
            }
        }

        /// <summary>
        /// Vytvoří instanci vozíku a přidá ho do seznamu vozíků
        /// </summary>
        /// <param name="ip">IP arduino zařízení na vozíku</param>
        private void createCart(string ip) {
            Cart cart = new Cart(ip, dashboard);
            networkScannerView.addDeviceIP(ip);
            needCheckCount++;

            var t = Task.Run(() => {
                if (cart.checkConnectionToArduino())
                {
                    carts.Add(ip, cart);
                    networkScannerView.setIPStatus(ip, true);
                }
                else {
                    networkScannerView.setIPStatus(ip, false);
                }
                needCheckCount--;
            });

            createPeriodicCheckerOfFinishSearching();

            t.Wait();
        }

        private System.Timers.Timer periodicCheckerOfFinishSearching;

        /// <summary>
        /// Periodiccká kontrola dokončení skenování sítě
        /// </summary>
        private void createPeriodicCheckerOfFinishSearching()
        {
            periodicCheckerOfFinishSearching = new System.Timers.Timer();
            periodicCheckerOfFinishSearching.Interval = 10000;
            periodicCheckerOfFinishSearching.Enabled = true;
            periodicCheckerOfFinishSearching.Elapsed += delegate {
                if (needCheckCount == 0) {
                    if (carts.Count == 0)
                    {
                        mainWindow.showMessage(MessageTypeEnum.error, "V síti nebylo nalezeno žádné kompatibilní zařízení.");
                    }
                    else {
                        mainWindow.showMessage(MessageTypeEnum.success, "Bylo nalezeno " + carts.Count + " kompatibilních zařízení.");
                        networkScannerView.enableNextButton();
                    }
                    networkScannerView.setCountLabel(carts.Count);
                }
                
                periodicCheckerOfFinishSearching.Dispose();
            };
        }

        /// <summary>
        /// Callback pro stisknutí tlačítka pro pokračování na obrazovce skenování sítě
        /// </summary>
        private void nextStepAfterNetworkScan() {
            MapView ultrasonicSystemScanView = new MapView();
            mainWindow.setView(ultrasonicSystemScanView);

            ultrasonicSystemScanView.addCarts(carts.Values.ToList());

            mainWindow.showMessage(MessageTypeEnum.progress, "Připojování k aplikaci dashboard ...");
            if (dashboard.checkConnectionToDashboard())
            {
                mainWindow.showMessage(MessageTypeEnum.success, "Připojování k aplikaci dashboard bylo úspěšné.");
                pairArduinosWithBeacons();
                //TODO co když není k počítači připojen router?
            }
            else {
                mainWindow.showMessage(MessageTypeEnum.error, "Připojování k aplikaci dashboard se nezdařilo. Zapněte aplikaci Dashboard a nastavte jí UDP port 4444.");
            }
        }

        /// <summary>
        /// Spáruje adresy nalezených majáků s Arduino zařízením
        /// </summary>
        private void pairArduinosWithBeacons() {
            //TODO spustit ve vlákně
            List<Cart> cartsList = carts.Values.ToList();
            for (int address=0; address<=20;address++) {
                if (dashboard.isDeviceConnected(address)) {
                    Point positionFromDashboard = dashboard.getDevicePosition(address);
                    for (int j = 0; j < cartsList.Count(); j++)
                    {
                        Cart cart = cartsList[j];
                        Point positionFromArduino = cart.getPositionFromArduino();
                        //TODO co když nastane chyba při dotazování polohy
                        if (Math.Abs(positionFromArduino.X - positionFromDashboard.X) < 20 && Math.Abs(positionFromArduino.Y - positionFromDashboard.Y) < 20) {
                            cart.beaconAddress = address;
                            cart.startPeriodicScanOfPosition();
                            cartsList.Remove(cartsList[j]);
                        }
                    }
                }
            }
        }
    }
}
