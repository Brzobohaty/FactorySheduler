using FactorySheduler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    class Controller
    {
        private MainWindow mainWindow; // hlavní okno apklikace
        private NetworkScanView networkScannerView;
        private NetworkScanner networkScanner;
        private Dictionary<String, Cart> carts = new Dictionary<String, Cart>();
        private int needCheckCount = 0;

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

        private void createCart(string ip) {
            Cart cart = new Cart(ip);
            networkScannerView.addDeviceIP(ip);
            needCheckCount++;

            var t = Task.Run(() => {
                if (cart.checkConnection())
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
                    }
                    networkScannerView.setCountLabel(carts.Count);
                }
                
                periodicCheckerOfFinishSearching.Dispose();
            };
        }
    }
}
