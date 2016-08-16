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
using System.Windows.Forms;

namespace FactorySheduler
{
    public class Cart
    {
        protected RestClient client; //client pro odesílání REST dotazů na Arduino
        protected System.Timers.Timer periodicCheckerPosition; //periodický kontroler polohy vozíku
        protected const int positionPeriodLength = 1000; //délka periody v milisekundách - jak často se bude aktualizovat poloha vozíku
        protected Dashboard dashboard; //Objekt představující připojení k Dashboard aplikaci
        [DisplayName("Název")]
        [Description("Název zařízení pouze pro identifikaci v této aplikaci.")]
        public string name { get; set; } //název vozíku
        [DisplayName("Značka")]
        [Description("Krátká značka pro jednoznačnou identifikaci vozíku na mapě.")]
        public string alias { get; set; } //zkrácený název pro vozík
        [DisplayName("IP adresa")]
        [Description("IP adresa Arduino zařízení na vozíku.")]
        public string ip { get; protected set; } //ip adresa Arduino zařízení
        [ReadOnly(true)]
        [DisplayName("Adresa majáku")]
        [Description("Adresa ultrazvukového majáku, který je umístěn na tomto vozíku.")]
        public int beaconAddress { get; set; } //adresa ultrazvukového majáku umístěného na vozíku
        [ReadOnly(true)]
        [DisplayName("Chybová hláška")]
        [Description("V případě, že nastala nějaká chyba, proměnná obsahuje chybovou hlášku.")]
        public string errorMessage { get; set; } //pokud nastala nějaká chyba, bude tato proměnná obsahovat chybovou hlášku, jinak ""
        [DisplayName("Pozice")]
        [Description("Poslední známá pozice majáku.")]
        public Point position { get; protected set; } //poslední známá pozice
        [DisplayName("Aktuální pozice")]
        [Description("Příznak, zda je pozice aktuální nebo zda se jedná pouze o poslední známou pozici.")]
        public bool isPositionActual { get; protected set; } //příznak, zda je pozice aktuální
        [Browsable(false)]
        public RadioButton asociatedButton { get; set; } //tlačítko ve view přiřazené k tomuto vozíku 
        [Browsable(false)]
        protected Action dashboarConnectionFaildCallback; //callback pro případ, že selže připojení k aplikaci dashboard
        [DisplayName("Zadní vzdálenost majáku")]
        [Description("Vzdálenost umístění majáku od zadní části vozíku [cm]")]
        public int distanceFromHedghogToBackOfCart { get; set; } = 10;//vzdálenost umístění majáku od zadní části vozíku [cm]
        [DisplayName("Přední vzdálenost majáku")]
        [Description("Vzdálenost umístění majáku od přední části vozíku [cm]")]
        public int distanceFromHedghogToFrontOfCart { get; set; } = 10;//vzdálenost umístění majáku od přední části vozíku [cm]
        [DisplayName("Levá vzdálenost majáku")]
        [Description("Vzdálenost umístění majáku od levé části vozíku [cm]")]
        public int distanceFromHedghogToLeftSideOfCart { get; set; } = 10;//vzdálenost umístění majáku od levé části vozíku [cm]
        [DisplayName("Pravá vzdálenost majáku")]
        [Description("Vzdálenost umístění majáku od pravé části vozíku [cm]")]
        public int distanceFromHedghogToRightSideOfCart { get; set; } = 10; //vzdálenost umístění majáku od právé části vozíku [cm]
        [DisplayName("Šířka vozíku")]
        [Description("Šířka vozíku [cm]")]
        public double width { get; protected set; } //šířka vozíku
        [DisplayName("Délka vozíku")]
        [Description("Délka vozíku [cm]")]
        public double longg { get; protected set; } //délka vozíku

    public int angle { get; set; } = 45;

        public Cart(string ip, Dashboard dashboard, Action dashboarConnectionFaildCallback)
        {
            this.dashboarConnectionFaildCallback = dashboarConnectionFaildCallback;
            this.ip = ip;
            name = ip;
            alias = ip.Substring(ip.Length - 3);
            this.dashboard = dashboard;
            client = new RestClient("http://" + ip + "/");
            errorMessage = "";
        }

        /// <summary>
        /// Zkontroluje, zda se opravdu jedná o arduino a zda se na něj lze připojit a že je nastavené REST API
        /// </summary>
        /// <returns>true, pokud je vše v pořádku</returns>
        public virtual bool checkConnectionToArduino()
        {
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
        public virtual void startPeriodicScanOfPosition()
        {
            periodicCheckerPosition = new System.Timers.Timer();
            periodicCheckerPosition.Interval = positionPeriodLength;
            periodicCheckerPosition.Enabled = true;
            periodicCheckerPosition.Elapsed += delegate
            {
                scanPosition();
                longg = distanceFromHedghogToBackOfCart + distanceFromHedghogToFrontOfCart;
                width = distanceFromHedghogToLeftSideOfCart + distanceFromHedghogToRightSideOfCart;
            };
        }

        /// <summary>
        /// Aktualizuje pozici vozíku
        /// </summary>
        /// <returns>true pokud se podařilo úspěšně načíst novou pozici</returns>
        public virtual bool scanPosition()
        {
            Point point = dashboard.getDevicePosition(beaconAddress);
            if (point.X == 0 && point.Y == 0)
            {
                isPositionActual = false;
                errorMessage = "Nastala chyba při dotazování pozice majáku na aplikaci Dashboard";
                dashboarConnectionFaildCallback();
                return false;
            }
            else
            {
                isPositionActual = true;
                errorMessage = "";
                position = shiftPosition(point);
                return true;
            }
        }

        /// <summary>
        /// Vrátí aktuální pozici vozíku
        /// </summary>
        /// <returns>0,0 pokud nastala chyba</returns>
        public virtual Point getPositionFromArduino()
        {
            RestRequest request = new RestRequest("arduino/position/", Method.GET);
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content == "NO_HEDGEHOG_CONNECTION_OR_NO_POSITION_UPDATE")
                {
                    isPositionActual = false;
                    errorMessage = "Chyba spojení mobilního majáku s Arduino nebo pouze není měřena pozice. Případně zkuste restartovat maják (alespoň 5 sekund podržet restart).";
                    return new Point(0, 0);
                }
                else if (content == "NO_RESPONSE_FROM_LINUX")
                {
                    isPositionActual = false;
                    errorMessage = "Arduino kontrolér nedostává žádná data o poloze z python scriptu běžícím na Linuxového systému. Zkuste restartovat Arduino.";
                    return new Point(0, 0);
                }
                else
                {
                    string[] positions = content.Split(',');
                    if (positions.Length == 2)
                    {
                        isPositionActual = true;
                        errorMessage = "";
                        return new Point(Int32.Parse(positions[0]), Int32.Parse(positions[1]));
                    }
                    else {
                        isPositionActual = false;
                        errorMessage = "Špatný formát příchozí zprávy o poloze z Arduino.";
                        return new Point(0, 0);
                    }
                }
            }
            else {
                isPositionActual = false;
                errorMessage = "Nelze se připojit k Arduino zařízení. Zkontrolujte, zda jste na stejné WiFi síti a zda je zařízení zapnuté. Případně ho restartujte.";
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Posune pozici tak, aby odpovídala prostředku vozíku.
        /// </summary>
        /// <returns>pozici uprostřed vozíku</returns>
        protected Point shiftPosition(Point rawPosition) {
            //vystředění vůči bokům vozíku
            double sideShift = (distanceFromHedghogToLeftSideOfCart - distanceFromHedghogToRightSideOfCart) / 2;
            MathLibrary.Point position = new MathLibrary.Point(rawPosition.X, rawPosition.Y);
            MathLibrary.Line line = new MathLibrary.Line(position.X, position.Y, angle);
            MathLibrary.Line normal = line.getNormal(position);
            MathLibrary.Point[] distanceSitePoints = MathLibrary.getPointOnLineInDistance(normal, position, sideShift);
            MathLibrary.Point endPoint = MathLibrary.getPointOnLine(rawPosition.X, rawPosition.Y, angle, distanceFromHedghogToFrontOfCart);
            MathLibrary.Point shiftedSitePosition;
            MathLibrary.Point pointOnLeft;
            if (sideShift < 0)
            {
                if (MathLibrary.isPointOnTheLeftSideOfVector(position, endPoint, distanceSitePoints[0]))
                {
                    pointOnLeft = distanceSitePoints[0];
                    shiftedSitePosition = distanceSitePoints[1];
                }
                else {
                    pointOnLeft = distanceSitePoints[1];
                    shiftedSitePosition = distanceSitePoints[0];
                }
            }
            else {
                if (MathLibrary.isPointOnTheLeftSideOfVector(position, endPoint, distanceSitePoints[0]))
                {
                    pointOnLeft = distanceSitePoints[0];
                    shiftedSitePosition = distanceSitePoints[0];
                }
                else {
                    pointOnLeft = distanceSitePoints[1];
                    shiftedSitePosition = distanceSitePoints[1];
                }
            }

            if (sideShift==0) {
                MathLibrary.Point[] distancePointsTemp = MathLibrary.getPointOnLineInDistance(normal, position, 20);
                if (MathLibrary.isPointOnTheLeftSideOfVector(position, endPoint, distancePointsTemp[0]))
                {
                    pointOnLeft = distanceSitePoints[0];
                }
                else {
                    pointOnLeft = distanceSitePoints[1];
                }
            }

            //vystředění vůči předku a zadku vozíku
            double frontShift = (distanceFromHedghogToFrontOfCart - distanceFromHedghogToBackOfCart) / 2;
            MathLibrary.Line parallel = normal.getNormal(shiftedSitePosition);
            MathLibrary.Point[] distanceFrontPoints = MathLibrary.getPointOnLineInDistance(parallel, shiftedSitePosition, frontShift);
            MathLibrary.Point shiftedPosition;
            if (frontShift > 0)
            {
                if (MathLibrary.isPointOnTheLeftSideOfVector(position, pointOnLeft, distanceFrontPoints[0]))
                {
                    shiftedPosition = distanceFrontPoints[1];
                }
                else {
                    shiftedPosition = distanceFrontPoints[0];
                }
            }
            else {
                if (MathLibrary.isPointOnTheLeftSideOfVector(position, pointOnLeft, distanceFrontPoints[0]))
                {
                    shiftedPosition = distanceFrontPoints[0];
                }
                else {
                    shiftedPosition = distanceFrontPoints[1];
                }
            }

            return new Point((int)Math.Round(shiftedPosition.X), (int)Math.Round(shiftedPosition.Y));
        }

        /// <summary>
        /// Pohne s vozíkem o kus dopředu
        /// </summary>
        public virtual void moveFront()
        {

        }

        /// <summary>
        /// Pohne s vozíkem o kus dozadu
        /// </summary>
        public virtual void moveBack()
        {

        }

        /// <summary>
        /// Zatočí s vozíkem doleva
        /// </summary>
        public virtual void turnLeft()
        {

        }

        /// <summary>
        /// Zatočí s vozíkem doprava
        /// </summary>
        public virtual void turnRight()
        {

        }
    }
}
