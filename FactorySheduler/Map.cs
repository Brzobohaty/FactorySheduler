using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    /// <summary>
    /// Objekt představující mapu v podobě neorientovaného grafu
    /// </summary>
    public class Map
    {
        public Dictionary<string, MapPoint> points = new Dictionary<string, MapPoint>(); //slovník bodů na mapě, kde klíčem je jejich id
        public List<MapPoint[]> lines = new List<MapPoint[]>(); //seznam čar spojujících body na mapě
        public List<Point> staticBeacons = new List<Point>(); //statické majáky na mapě

        public Map(){}

        /// <summary>
        /// Přidá cestu mezi dvěma body
        /// </summary>
        /// <param name="newLine">cesta</param>
        public void addLine(MapPoint[] newLine) {
            foreach (MapPoint[] oldLine in lines)
            {
                if ((oldLine[0].Equals(newLine[0]) && oldLine[1].Equals(newLine[1])) || (oldLine[0].Equals(newLine[1]) && oldLine[1].Equals(newLine[0])))
                {
                    return;
                }
            }

            newLine[0] = points[newLine[0].id];
            newLine[1] = points[newLine[1].id];

            lines.Add(newLine);
            newLine[0].addNeighbor(newLine[1]);
            newLine[1].addNeighbor(newLine[0]);
        }

        /// <summary>
        /// Odstraní cestu mezi dvěma body
        /// </summary>
        /// <param name="line">cesta</param>
        public void removeLine(MapPoint[] line)
        {
            lines.Remove(line);
            line[0].removeNeighbor(line[1]);
            line[1].removeNeighbor(line[0]);
        }

        /// <summary>
        /// Načte mapu uloženou v souborech
        /// </summary>
        public void loadMap() {
            try
            {
                List<MapPoint> pointsList = MapMemory.DeSerializeObject<List<MapPoint>>("mapPoints.xml");
                points.Clear();
                foreach (var mapPoint in pointsList)
                {
                    points.Add(mapPoint.id, mapPoint);
                    mapPoint.inicialize();
                }
            }
            catch (FileNotFoundException) { }
            try
            {
                List<MapPoint[]> linesFromFile = MapMemory.DeSerializeObject<List<MapPoint[]>>("mapLines.xml");
                lines.Clear();
                foreach (var line in linesFromFile)
                {
                    addLine(line);
                }
            }
            catch (FileNotFoundException) { }
            try
            {
                staticBeacons = MapMemory.DeSerializeObject<List<Point>>("staticBeacons.xml");
            }
            catch (FileNotFoundException) { }
        }

        /// <summary>
        /// Uloží celou mapu do souborů
        /// </summary>
        public void saveToFiles() {
            MapMemory.SerializeObject(points.Values.ToList(), "mapPoints.xml");
            MapMemory.SerializeObject(staticBeacons, "staticBeacons.xml");
            MapMemory.SerializeObject(lines, "mapLines.xml");
        }

        /// <summary>
        /// Přidání bodu do mapy
        /// </summary>
        /// <param name="point">bod</param>
        public void addPoint(MapPoint point) {
            points.Add(point.id, point);
        }

        /// <summary>
        /// Vymazání mapy
        /// </summary>
        public void clear() {
            points.Clear();
            lines.Clear();
        }

        /// <summary>
        /// Vrátí seznam bodů, které představují nejkratší cestu z daného bodu do druhého daného bodu
        /// </summary>
        /// <param name="startPoint">počáteční bod</param>
        /// <param name="finishPoint">cílový bod</param>
        /// <returns>seynam bodů</returns>
        public List<MapPoint> getShortestPath(Point startPoint, MapPoint finishPoint) {
            return _getShortestPath(findNeerestPoint(startPoint), finishPoint);
        }

        /// <summary>
        /// Najde nejbližší bod na mapě k daným souřadnicím
        /// </summary>
        /// <param name="point">souřadnice</param>
        /// <returns>bod na mapě</returns>
        private MapPoint findNeerestPoint(Point point) {
            double shortestDistance = int.MaxValue;
            MapPoint neerestMapPoint = null;
            foreach (var mapPoint in points.Values) {
                if (mapPoint.getNeighborsDistances().Count > 0) {
                    double distance = MathLibrary.getPointsDistance(mapPoint.position.X, mapPoint.position.Y, point.X, point.Y);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        neerestMapPoint = mapPoint;
                    }
                }
            }

            return neerestMapPoint;
        }

        /// <summary>
        /// Vrátí seznam bodů, které představují nejkratší cestu z daného bodu do druhého daného bodu
        /// </summary>
        /// <param name="startPoint">počáteční bod</param>
        /// <param name="finishPoint">cílový bod</param>
        /// <returns>seynam bodů</returns>
        private List<MapPoint> _getShortestPath(MapPoint startPoint, MapPoint finishPoint)
        {
            Dictionary<string, MapPoint> previous = new Dictionary<string, MapPoint>();
            Dictionary<string, double> distances = new Dictionary<string, double>();
            List<MapPoint> nodes = new List<MapPoint>();

            List<MapPoint> path = new List<MapPoint>();

            foreach (var point in points)
            {
                if (point.Key == startPoint.id)
                {
                    distances[point.Key] = 0;
                }
                else
                {
                    distances[point.Key] = int.MaxValue;
                }

                nodes.Add(point.Value);
            }

            while (nodes.Count != 0)
            {
                nodes = nodes.OrderBy(o => distances[o.id] * 1000).ToList();

                MapPoint smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest.id == finishPoint.id)
                {
                    while (previous.ContainsKey(smallest.id))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest.id];
                    }
                    path.Add(startPoint);

                    break;
                }

                if (distances[smallest.id] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in smallest.getNeighborsDistances())
                {
                    var alt = distances[smallest.id] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            path.Reverse();

            return path;
        }
    }
}
