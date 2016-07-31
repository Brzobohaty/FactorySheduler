using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FactorySheduler
{
    public class Cart
    {
        //System.ComponentModel.
        //[Browsable(false)]
        private RestClient client; //client pro odesílání REST dotazů na Arduino
        private Timer periodicCheckerPosition; //periodický kontroler polohy vozíku
        private const int positionPeriodLength = 1000; //délka periody v milisekundách - jak často se bude aktualizovat poloha vozíku
        private Dashboard dashboard; //Objekt představující připojení k Dashboard aplikaci
        [DisplayName("Název")]
        [Description("Název zařízení pouze pro identifikaci v této aplikaci.")]
        public string name { get; set; } //název vozíku
        [DisplayName("IP adresa")]
        [Description("IP adresa Arduino zařízení na vozíku.")]
        public string ip { get; private set; } //ip adresa Arduino zařízení
        [DisplayName("Chybová hláška")]
        [Description("V případě, že nastala nějaká chyba, proměnná obsahuje chybovou hlášku.")]
        public string errorMessage { get; private set; } //pokud nastala nějaká chyba, bude tato proměnná obsahovat chybovou hlášku, jinak ""
        [DisplayName("Pozice")]
        [Description("Poslední známá pozice majáku.")]
        public Point position { get; private set; } //poslední známá pozice
        [DisplayName("Aktuální pozice")]
        [Description("Příznak, zda je pozice aktuální nebo zda se jedná pouze o poslední známou pozici.")]
        public bool isPositionActual { get; private set; } //příznak, zda je pozice aktuální
        [ReadOnly(true)]
        [DisplayName("Adresa majáku")]
        [Description("Adresa ultrazvukového majáku, který je umístěn na tomto vozíku.")]
        public int beaconAddress { get; set; } //adresa ultrazvukového majáku umístěného na vozíku
        
        public Cart(string ip, Dashboard dashboard) {
            this.ip = ip;
            this.name = ip;
            this.dashboard = dashboard;
            client = new RestClient("http://" + ip + "/");
            errorMessage = "";
        }

        /// <summary>
        /// Zkontroluje, zda se opravdu jedná o arduino a zda se na něj lze připojit a že je nastavené REST API
        /// </summary>
        /// <returns>true, pokud je vše v pořádku</returns>
        public bool checkConnectionToArduino() {
            RestRequest request = new RestRequest("arduino/check/", Method.GET);

            //RestRequest request = new RestRequest("arduino/{order}/", Method.GET);
            //request.AddUrlSegment("order", "digital"); // replaces matching token in request.Resource

            // execute the request
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                if (response.Content.Trim() == "NVC8mK73kAoXzLAYxFMo")
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Započne periodické kontrolování pozice robota
        /// </summary>
        public void startPeriodicScanOfPosition() {
            periodicCheckerPosition = new Timer();
            periodicCheckerPosition.Interval = positionPeriodLength;
            periodicCheckerPosition.Enabled = true;
            periodicCheckerPosition.Elapsed += delegate
            {
                scanPosition();
                //TODO dodělat aktivní hlášení chyb do view
            };
        }

        /// <summary>
        /// Aktualizuje pozici vozíku
        /// </summary>
        /// <returns>true pokud se podařilo úspěšně načíst novou pozici</returns>
        public bool scanPosition() {
            Point point = dashboard.getDevicePosition(beaconAddress);
            if (point.X == 0 && point.Y == 0)
            {
                errorMessage = "Nastala chyba při dotazování pozice majáku na aplikaci Dashboard";
                return false;
            }
            else
            {
                errorMessage = "";
                position = point;
                return true;
            }
        }

        /// <summary>
        /// Vrátí aktuální pozici vozíku
        /// </summary>
        /// <returns>0,0 pokud nastala chyba</returns>
        public Point getPositionFromArduino() {
            RestRequest request = new RestRequest("arduino/position/", Method.GET);
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content == "NO_HEDGEHOG_CONNECTION_OR_NO_POSITION_UPDATE")
                {
                    errorMessage = "Chyba spojení mobilního majáku s Arduino nebo pouze není měřena pozice. Případně zkuste restartovat maják (alespoň 5 sekund podržet restart).";
                    return new Point(0, 0);
                }
                else if (content == "NO_RESPONSE_FROM_LINUX")
                {
                    errorMessage = "Arduino kontrolér nedostává žádná data o poloze z python scriptu běžícím na Linuxového systému. Zkuste restartovat Arduino.";
                    return new Point(0, 0);
                }
                else
                {
                    string[] positions = content.Split(',');
                    if (positions.Length == 2)
                    {
                        errorMessage = "";
                        return new Point(Int32.Parse(positions[0]), Int32.Parse(positions[1]));
                    }
                    else {
                        errorMessage = "Špatný formát příchozí zprávy o poloze z Arduino.";
                        return new Point(0, 0);
                    }
                }
            }
            else {
                isPositionActual = false;
                errorMessage = "Nelze se připojit k Arduino zařízení. Zkontrolujte, zda jste na stejné WiFi síti a zda je zařízení zapnuté. Případně ho restartujte.";
                return new Point(0,0);
            }
        }
    }
}
