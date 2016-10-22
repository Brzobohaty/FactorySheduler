using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    /// <summary>
    /// Zařízení na daném bodě na mapě
    /// </summary>
    public class DeviceOnPoint
    {
        public int address { get; private set; } //virtuální adresa zařízení
        public PointTypeEnum type { get; private set; } //typ zařízení
        public string status { get; private set; } //aktuální stav zařízení
        private Action updateStatusCallback; //callback při aktualizaci statusu
        
        /// <param name="address">virtuální adresa zařízení</param>
        /// <param name="type">znak typu zařízení</param>
        /// <param name="status">zank stavu zařízení</param>
        public DeviceOnPoint(int address, char type, char status)
        {
            this.address = address;
            switch (type)
            {
                case 'E':
                    this.type = PointTypeEnum.emptyTanks;
                    switch (status)
                    {
                        case 'F':
                            this.status = "free";
                            break;
                        case 'T':
                            this.status = "full";
                            break;
                    }
                    break;
                case 'F':
                    this.type = PointTypeEnum.fullTanks;
                    switch (status)
                    {
                        case 'F':
                            this.status = "filled";
                            break;
                        case 'T':
                            this.status = "filling";
                            break;
                    }
                    break;
                case 'C':
                    this.type = PointTypeEnum.charge;
                    switch (status)
                    {
                        case 'F':
                            this.status = "free";
                            break;
                        case 'T':
                            this.status = "full";
                            break;
                    }
                    break;
                case 'O':
                    this.type = PointTypeEnum.consumer;
                    switch (status)
                    {
                        case 'F':
                            this.status = "empty";
                            break;
                        case 'T':
                            this.status = "free";
                            break;
                        case 'K':
                            this.status = "full";
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// nastavení callbacku pro aktualizaci stavu zařízení
        /// </summary>
        /// <param name="updateStatusCallback">callback</param>
        public void setStatusUpdateCallback(Action updateStatusCallback)
        {
            this.updateStatusCallback = updateStatusCallback;
        }

        /// <summary>
        /// Aktualizuje stav zařízení
        /// </summary>
        /// <param name="status">stav zařízení zakódovaný v jednom znaku</param>
        public void updateStatus(char status)
        {
            switch (type)
            {
                case PointTypeEnum.emptyTanks:
                    switch (status)
                    {
                        case 'F':
                            this.status = "free";
                            break;
                        case 'T':
                            this.status = "full";
                            break;
                    }
                    break;
                case PointTypeEnum.fullTanks:
                    switch (status)
                    {
                        case 'F':
                            this.status = "filled";
                            break;
                        case 'T':
                            this.status = "filling";
                            break;
                    }
                    break;
                case PointTypeEnum.charge:
                    switch (status)
                    {
                        case 'F':
                            this.status = "free";
                            break;
                        case 'T':
                            this.status = "full";
                            break;
                    }
                    break;
                case PointTypeEnum.consumer:
                    switch (status)
                    {
                        case 'F':
                            this.status = "empty";
                            break;
                        case 'T':
                            this.status = "free";
                            break;
                        case 'K':
                            this.status = "full";
                            break;
                    }
                    break;
            }
            updateStatusCallback();
        }
    }
}
