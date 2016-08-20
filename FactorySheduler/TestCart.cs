using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySheduler
{
    class TestCart : Cart
    {
        private Point testPosition;
        private static int inc = 5;

        public TestCart(string ip, Dashboard dashboard, Action dashboarConnectionFaildCallback) : base(ip, dashboard, dashboarConnectionFaildCallback){
            inc += 40;
            testPosition = new Point(inc,51);
        }

        /// <summary>
        /// Zkontroluje, zda se opravdu jedná o arduino a zda se na něj lze připojit a že je nastavené REST API
        /// </summary>
        /// <returns>true, pokud je vše v pořádku</returns>
        public override bool checkConnectionToArduino()
        {
            return true;
        }

        /// <summary>
        /// Aktualizuje pozici vozíku
        /// </summary>
        /// <returns>true pokud se podařilo úspěšně načíst novou pozici</returns>
        public override bool scanPosition()
        {
            position = shiftPosition(testPosition);
            isPositionActual = true;
            return true;
        }

        /// <summary>
        /// Vrátí aktuální pozici vozíku
        /// </summary>
        /// <returns>0,0 pokud nastala chyba</returns>
        public override Point getPositionFromArduino()
        {
            errorMessage = "";
            return testPosition;
        }

        /// <summary>
        /// Pohne s vozíkem o kus dopředu
        /// </summary>
        public override void moveFront()
        {
            testPosition.Y -= 15;
        }

        /// <summary>
        /// Pohne s vozíkem o kus dozadu
        /// </summary>
        public override void moveBack()
        {
            testPosition.Y += 15;
        }

        /// <summary>
        /// Zatočí s vozíkem doleva
        /// </summary>
        public override void turnLeft()
        {
            testPosition.X -= 15;
        }

        /// <summary>
        /// Zatočí s vozíkem doprava
        /// </summary>
        public override void turnRight()
        {
            testPosition.X += 15;
        }
    }
}
