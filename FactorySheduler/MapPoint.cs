﻿using System;
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
        public Point position { get; private set; } //reálné souřadnice bodu
        [DisplayName("Typ")]
        [Description("Druh bodu na mapě určuje, co bude možné na daném bodu provádět za akce.")]
        [TypeConverter(typeof(MapPointTypeConverter))]
        public PointTypeEnum type { get; set; } //typ bodu
        private List<MapPoint> paths = new List<MapPoint>(); //seynam bodů do kterých vede cesta z tohoto bodu

        public MapPoint(Point position) {
            this.position = position;
            type = PointTypeEnum.init;
        }

        /// <summary>
        /// Přidání bodu, do/z kterého vede cesta k tomuhle bodu
        /// </summary>
        /// <param name="point"> druhý bod</param>
        public void addPath(MapPoint point) {
            paths.Add(point);
        }

        /// <summary>
        /// Odebrání bodu, do/z kterého vede cesta k tomuhle bodu
        /// </summary>
        /// <param name="point"> druhý bod</param>
        public void removePath(MapPoint point)
        {
            paths.Remove(point);
        }
    }
}