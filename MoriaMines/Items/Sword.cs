using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines.Items
{
    class Sword : Item
    {
        private int damage;
        private int durability;

        public Sword(int durability, int damage, string name, string description, bool hidden, int rarity) : base(rarity, hidden, description, name, damage)
        {
            Durability = durability;
            Damage = damage;
        }

        public int Durability
        {
            get { return durability; }
            set { durability = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

    }
}
