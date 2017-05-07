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
        protected System.Timers.Timer periodicCheckerStatus; //periodický kontroler stavu vozíku
        protected System.Timers.Timer periodicCheckerHeading; //periodický kontroler směru vozíku
        protected const int positionPeriodLength = 300; //délka periody v milisekundách - jak často se bude aktualizovat poloha vozíku
        protected const int headingPeriodLength = 1000; //délka periody v milisekundách - jak často se bude kontrolovat směr vozíku
        protected const int statusPeriodLength = 10000; //délka periody v milisekundách - jak často se bude kontrolovat stab arduina
        protected Dashboard dashboard; //Objekt představující připojení k Dashboard aplikaci
        protected string errorType = ""; //typ chyby
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
        //TODO smazat nebo sprovoznit
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
        public int angle { get; set; } = 0;
        [Browsable(false)]
        public List<Point> path; //cesta, po které se má vozík aktuálně pohybovat

        public Cart(string ip, Dashboard dashboard, Action dashboarConnectionFaildCallback)
        {
            this.dashboarConnectionFaildCallback = dashboarConnectionFaildCallback;
            this.ip = ip;
            name = ip;
            alias = ip.Substring(ip.Length - 3);
            this.dashboard = dashboard;
            client = new RestClient("http://" + ip + ":8080/");
            errorMessage = "";
        }

        private void setPropertiesFromEEPROM()
        {
            RestRequest request = new RestRequest("readEEPROM/name", Method.GET);
            request.Timeout = 3000;
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content != "NOT SET")
                {
                    name = content;
                }
            }
            request = new RestRequest("readEEPROM/alias", Method.GET);
            request.Timeout = 3000;
            response = client.Execute(request);
            status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content != "NOT SET")
                {
                    alias = content;
                }
            }
            request = new RestRequest("readEEPROM/distances", Method.GET);
            request.Timeout = 3000;
            response = client.Execute(request);
            status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content != "NOT SET")
                {
                    string[] contentArray = content.Split(',');
                    distanceFromHedghogToBackOfCart = int.Parse(contentArray[0]);
                    distanceFromHedghogToFrontOfCart = int.Parse(contentArray[1]);
                    distanceFromHedghogToLeftSideOfCart = int.Parse(contentArray[2]);
                    distanceFromHedghogToRightSideOfCart = int.Parse(contentArray[3]);
                }
            }
        }

        /// <summary>
        /// Zkontroluje, zda se opravdu jedná o arduino a zda se na něj lze připojit a že je nastavené REST API
        /// </summary>
        /// <returns>true, pokud je vše v pořádku</returns>
        public virtual bool checkConnectionToArduino()
        {
            RestRequest request = new RestRequest("check/", Method.GET);

            //RestRequest request = new RestRequest("{order}/", Method.GET);
            //request.AddUrlSegment("order", "digital"); // replaces matching token in request.Resource

            // execute the request
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                if (response.Content.Trim() == "NVC8mK73kAoXzLAYxFMo")
                {
                    setPropertiesFromEEPROM();
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
        /// Započne periodické kontrolování pozice a směru robota
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

            periodicCheckerStatus = new System.Timers.Timer();
            periodicCheckerStatus.Interval = statusPeriodLength;
            periodicCheckerStatus.Enabled = true;
            periodicCheckerStatus.Elapsed += delegate
            {
                getPositionFromArduinoAsync(delegate (Point position) { });
            };

            periodicCheckerHeading = new System.Timers.Timer();
            periodicCheckerHeading.Interval = headingPeriodLength;
            periodicCheckerHeading.Enabled = true;
            periodicCheckerHeading.Elapsed += delegate
            {
                scanHeading();
            };
        }

        /// <summary>
        /// Aktuaizuje aktuální směr vozíku
        /// </summary>
        public virtual void scanHeading()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                angle = getHeading();
            });

            bw.RunWorkerAsync();
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
                errorType = "dashboard";
                dashboarConnectionFaildCallback();
                return false;
            }
            else
            {
                isPositionActual = true;
                if (errorType == "dashboard")
                {
                    errorMessage = "";
                    errorType = "";
                }
                position = shiftPosition(point);
                return true;
            }
        }

        /// <summary>
        /// Vrátí aktuální pozici vozíku pomocí callbacku asynchroně
        /// </summary>
        public virtual void getPositionFromArduinoAsync(Action<Point> callback)
        {
            BackgroundWorker bw = new BackgroundWorker();
            Point position = new Point(0, 0);

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                position = getPositionFromArduino();
            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                callback(position);
            });

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Vrátí aktuální směr vozíku ve stupních
        /// </summary>
        /// <returns>0 pokud nastala chyba</returns>
        public virtual int getHeading()
        {
            RestRequest request = new RestRequest("heading/", Method.GET);
            request.Timeout = 3000;
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                return Int32.Parse(content);
                
            }
            else {
                errorMessage = "Nelze se připojit k Arduino zařízení. Zkontrolujte, zda jste na stejné WiFi síti a zda je zařízení zapnuté. Případně ho restartujte.";
                errorType = "arduino";
                return 0;
            }
        }

        /// <summary>
        /// Vrátí aktuální pozici vozíku
        /// </summary>
        /// <returns>0,0 pokud nastala chyba</returns>
        public virtual Point getPositionFromArduino()
        {
            RestRequest request = new RestRequest("position/", Method.GET);
            request.Timeout = 3000;
            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content == "NO_HEDGEHOG_CONNECTION_OR_NO_POSITION_UPDATE")
                {
                    errorMessage = "Chyba spojení mobilního majáku s Arduino nebo pouze není měřena pozice. Případně zkuste restartovat maják (alespoň 5 sekund podržet restart).";
                    errorType = "arduino";
                    return new Point(0, 0);
                }
                else if (content == "NO_RESPONSE_FROM_LINUX")
                {
                    errorMessage = "Arduino kontrolér nedostává žádná data o poloze z python scriptu běžícím na Linuxového systému. Zkuste restartovat Arduino.";
                    errorType = "arduino";
                    return new Point(0, 0);
                }
                else
                {
                    string[] positions = content.Split(',');
                    if (positions.Length == 2)
                    {
                        if (errorType == "arduino")
                        {
                            errorMessage = "";
                            errorType = "";
                        }
                        return new Point(Int32.Parse(positions[0]), Int32.Parse(positions[1]));
                    }
                    else {
                        errorMessage = "Špatný formát příchozí zprávy o poloze z Arduino.";
                        errorType = "arduino";
                        return new Point(0, 0);
                    }
                }
            }
            else {
                errorMessage = "Nelze se připojit k Arduino zařízení. Zkontrolujte, zda jste na stejné WiFi síti a zda je zařízení zapnuté. Případně ho restartujte.";
                errorType = "arduino";
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Načte naposledy zaznamenané body z arduino při stisku tlačítka
        /// </summary>
        /// <returns>List zaznamenanných bodů</returns>
        public virtual List<MapPoint> getMapPoints()
        {
            RestRequest request = new RestRequest("read-map-points/", Method.GET);
            List<MapPoint> listOfPositions = new List<MapPoint>();

            IRestResponse response = client.Execute(request);
            HttpStatusCode status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                string content = response.Content.Trim();
                if (content != "NO_SET")
                {
                    string[] positions = content.Split(';');
                    for (int i = 0; i < positions.Length; i++)
                    {
                        string[] xy = positions[i].Split(',');
                        int x = Int32.Parse(xy[0]);
                        int y = Int32.Parse(xy[1]);
                        listOfPositions.Add(new MapPoint(new Point(x, y)));
                    }
                    return listOfPositions;
                }
                else {
                    return listOfPositions;
                }
            }
            else {
                errorMessage = "Nelze se připojit k Arduino zařízení. Zkontrolujte, zda jste na stejné WiFi síti a zda je zařízení zapnuté. Případně ho restartujte.";
                errorType = "arduino";
                return listOfPositions;
            }
        }

        /// <summary>
        /// Posune pozici tak, aby odpovídala prostředku vozíku.
        /// </summary>
        /// <returns>pozici uprostřed vozíku</returns>
        protected Point shiftPosition(Point rawPosition)
        {
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

            if (sideShift == 0)
            {
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
            RestRequest request = new RestRequest("drive/180", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Pohne s vozíkem o kus dozadu
        /// </summary>
        public virtual void moveBack()
        {
            RestRequest request = new RestRequest("drive/-180", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Zatočí s vozíkem doleva
        /// </summary>
        public virtual void turnLeft()
        {
            RestRequest request = new RestRequest("raid/8", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Zatočí s vozíkem doprava
        /// </summary>
        public virtual void turnRight()
        {
            RestRequest request = new RestRequest("raid/-8", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Při změně hodnoty proměnné uloží hodnotu do EEPROM vozíku
        /// </summary>
        /// <param name="propertyName">název proměnné</param>
        public virtual void propertyChanged(string propertyName)
        {
            RestRequest request;
            if (propertyName == "distanceFromHedghogToBackOfCart" || propertyName == "distanceFromHedghogToFrontOfCart" || propertyName == "distanceFromHedghogToLeftSideOfCart" || propertyName == "distanceFromHedghogToRightSideOfCart")
            {
                request = new RestRequest("writeEEPROM/distances/" + distanceFromHedghogToBackOfCart + "/" + distanceFromHedghogToFrontOfCart + "/" + distanceFromHedghogToLeftSideOfCart + "/" + distanceFromHedghogToRightSideOfCart, Method.GET);
            }
            else {
                request = new RestRequest("writeEEPROM/" + propertyName + "/" + this.GetType().GetProperty(propertyName).GetValue(this, null), Method.GET);
            }
            client.Execute(request);
        }

        /// <summary>
        /// Nastaví cestu vozíku
        /// </summary>
        /// <param name="path"></param>
        public virtual void setPath(List<Point> path)
        {
            this.path = path;
            string pathString = "";
            foreach (var point in path)
            {
                pathString += point.X + "," + point.Y + ";";
            }

            RestRequest request = new RestRequest("path/" + pathString, Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Otočí vozík o maximální otočku doleva
        /// </summary>
        public virtual void rotateLeft()
        {
            RestRequest request = new RestRequest("max-turn/-1", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Otočí vozík o maximální otočku doprava
        /// </summary>
        public virtual void rotateRight()
        {
            RestRequest request = new RestRequest("max-turn/1", Method.GET);
            client.Execute(request);
        }

        /// <summary>
        /// Ukončí veškerou aktivitu robota
        /// </summary>
        public virtual void stop()
        {
            RestRequest request = new RestRequest("stop/", Method.GET);
            client.Execute(request);
        }
    }
}
