using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines.Items
{
    class Armor : Item
    {
        private int armorValue;


        public Armor(int armorValue, int rarity, bool hidden, string description, string name, int damageModifer = 0) : base(rarity, hidden, description, name, damageModifer)
        {
            ArmorValue = armorValue;
        }
        public int ArmorValue
        {
            get { return armorValue; }
            set { armorValue = value; }
        }
    }
}
