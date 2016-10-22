using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    /// <summary>
    /// Výčet typů speciálních bodů
    /// </summary>
    public enum PointTypeEnum {
        [Description("Průchozí")]
        init,
        [Description("Nabíjecí")]
        charge,
        [Description("S plnými kanistry")]
        fullTanks,
        [Description("S prázdnými kanistry")]
        emptyTanks,
        [Description("Konzumní")]
        consumer };
}
