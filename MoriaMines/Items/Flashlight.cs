using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines.Items
{
    class Flashlight : Item
    {
        private int charge;
        private int brightness;

        public Flashlight(string name, string description, bool hidden, int rarity, int damageModifier, int brightness, int charge) : base(rarity, hidden, description, name, damageModifier)
        {
            Brightness = brightness;
            Charge = charge;
        }

        public int Brightness
        {
            get { return brightness; }
            set { brightness = value; }
        }

        public int Charge
        {
            get { return charge; }
            set { charge = value; }
        }



    }
}
