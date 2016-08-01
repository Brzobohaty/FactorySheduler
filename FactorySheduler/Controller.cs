﻿using FactorySheduler.Views;
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
        private void createCart(string ip)
        {
            Cart cart = new Cart(ip, dashboard);
            networkScannerView.addDeviceIP(ip);
            needCheckCount++;

            var t = Task.Run(() =>
            {
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
            periodicCheckerOfFinishSearching.Elapsed += delegate
            {
                if (needCheckCount == 0)
                {
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
        private void nextStepAfterNetworkScan()
        {
            mapView = new MapView();
            mainWindow.setView(mapView);

            mapView.addCarts(carts.Values.ToList());

            mainWindow.showMessage(MessageTypeEnum.progress, "Připojování k aplikaci dashboard ...");
            if (dashboard.checkConnectionToDashboard())
            {
                mainWindow.showMessage(MessageTypeEnum.progress, "Párování Arduino zařízení s ultrazvukovými majáky ...");
                pairArduinosWithBeacons();
            }
            else {
                mainWindow.showMessage(MessageTypeEnum.error, "Připojování k aplikaci dashboard se nezdařilo. Zapněte aplikaci Dashboard a nastavte jí UDP port 4444.");
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

                //TODO spustit ve vlákně
                List<Cart> cartsList = carts.Values.ToList();
                int maxBeaconAdress = 20;
                for (int address = 0; address <= maxBeaconAdress; address++)
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
                mainWindow.showMessage(MessageTypeEnum.success, "Párování bylo dokončeno.");
                mapView.refreshAll();
            });

            bw.RunWorkerAsync();
        }
    }
}
