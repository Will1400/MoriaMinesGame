using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines.Items
{
    public class Potion : Item
    {
        private string type;
        private int healing;



        public Potion(int rarity, bool hidden, string description, string name, int damageModifer, string type, int healing = 0) : base(rarity, hidden, description, name, damageModifer)
        {
            Type = type;
            Healing = healing;
        }
        public int Healing
        {
            get { return healing; }
            set { healing = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
