using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    public class MapPoint
    {
        [DisplayName("Pozice")]
        [Description("Reálná pozice bodu na mapě vůči souřadnicovému systému majáků.")]
        [ReadOnly(true)]
        public Point position { get; set; } //reálné souřadnice bodu
        [DisplayName("Typ")]
        [Description("Druh bodu na mapě určuje, co bude možné na daném bodu provádět za akce.")]
        [TypeConverter(typeof(MapPointTypeConverter))]
        public PointTypeEnum type { get; set; } //typ bodu
        [DisplayName("Adresa")]
        [Description("Virtuální adresa zařízení na daném bodě, která je posílána v UDP zprávě.")]
        public int adrress { get; set; } = -1; //adresa zařízení na daném bodě
        [DisplayName("Stav")]
        [Description("Aktuální stav zařízení na daném bodě.")]
        [ReadOnly(true)]
        public string translatedState { get; set; } //stav zařízení na daném bodě, ale už přeložený
        [DisplayName("Chyba")]
        [Description("Aktuální chyba na daném zařízení.")]
        [ReadOnly(true)]
        public string error { get; set; } //stav zařízení na daném bodě
        [Browsable(false)]
        private Dictionary<string, double> neighborsDistances = new Dictionary<string, double>(); //slovník hran, kde klíč je id uzlu do kterého hrana vede a hodnota je váha hrany (vzdálenost)
        [Browsable(false)]
        private DeviceOnPoint device; //zařízení na tomto bodě (UDP virtualizace)
        [Browsable(false)]
        private Action updateStatusCallback; //callback při aktualizaci statusu
        [Browsable(false)]
        public string state { get; set; } //stav zařízení na daném bodě
        [Browsable(false)]
        public string id { get; set; } //unikátní id tohoto bodu
        [Browsable(false)]
        public bool printPath = false; //přiznak, že tento bod je součástí vykreslované cesty

        private MapPoint() { }

        public MapPoint(Point position)
        {
            this.position = position;
            id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            type = PointTypeEnum.init;
        }

        /// <summary>
        /// Incialiyuje bod po načtení ze souboru
        /// </summary>
        public void inicialize() {
            setDevice();
            printPath = false;
        }

        /// <summary>
        /// Vrátí vzdálenosti k sousedním bodům
        /// </summary>
        /// <returns>slovník vzdáleností</returns>
        public Dictionary<string, double> getNeighborsDistances(){
            return neighborsDistances;
        }

        /// <summary>
        /// Přidání hrany vedoucí z/do tohoto bodu
        /// </summary>
        /// <param name="point"> druhý bod do kterého hrana vede</param>
        public void addNeighbor(MapPoint point)
        {
            neighborsDistances.Add(point.id, MathLibrary.getPointsDistance(point.position.X, point.position.Y, position.X, position.Y));
        }

        /// <summary>
        /// Odebrání hrany vedoucí z/do tohoto bodu
        /// </summary>
        /// <param name="point"> druhý bod do kterého hrana vede</param>
        public void removeNeighbor(MapPoint point)
        {
            neighborsDistances.Remove(point.id);
        }

        /// <summary>
        /// Callback pro případ, že byla změněna nějaká proměnná tohoto bodu
        /// </summary>
        /// <param name="propertyName">jméno proměnné</param>
        public void propertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "adrress":
                case "type":
                    setDevice();
                    break;
            }
        }

        /// <summary>
        /// Nastaví/najde zařízení podle aktuální virtuální adresy
        /// </summary>
        public void setDevice()
        {
            translatedState = "";

            device = MapPointInputServer.getMapPointInputServer().getDevice(adrress);

            if (device.type == type)
            {
                error = "";
                state = device.status;
                translatedState = translateStatus(device.status);
                device.setStatusUpdateCallback(updateStatus);
            }
            else {
                if (device.type == PointTypeEnum.init)
                {
                    error = "Pod danou adresou není přihlášeno žádné zařízení.";
                    device.setStatusUpdateCallback(updateStatus);
                }
                else {
                    error = "Zařízení na dané adrese je jiného typu (" + device.type + ")";
                }
            }
        }

        /// <summary>
        /// Callback pro aktualizaci stavu zařízení na tomto bodě
        /// </summary>
        public void updateStatus()
        {
            error = "";
            state = device.status;
            translatedState = translateStatus(device.status);
            if (updateStatusCallback != null)
            {
                updateStatusCallback();
            }
        }

        /// <summary>
        /// Přeloží text stavu zařízení z kódové indikace do čitelné podoby
        /// </summary>
        /// <param name="status">zakódovaný stav zařízení</param>
        /// <returns>stav v čitelné podobě</returns>
        private string translateStatus(string status)
        {
            switch (status)
            {
                case "free": return "volno";
                case "full": return "obsazeno";
                case "filled": return "naplněný kanister";
                case "filling": return "plní";
                case "empty": return "prázdný kanister";
                default: return "neznámý stav";
            }
        }

        /// <summary>
        /// Nastavení callbacku pro aktualizaci stavu zařízení na tomto bodě
        /// </summary>
        /// <param name="callback">callback</param>
        public void setUpdateCallback(Action callback)
        {
            updateStatusCallback = callback;
        }
    }
}
