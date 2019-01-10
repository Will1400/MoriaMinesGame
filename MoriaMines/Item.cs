using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines
{
    public class Item
    {
        private string name;
        private string description;
        private bool hidden;
        private int rarity;
        private int damageModifier;

        public int DamageModifier
        {
            get { return damageModifier; }
            set { damageModifier = value; }
        }


        public int Rarity
        {
            get { return rarity; }
            set { rarity = value; }
        }

        public Item(int rarity, bool hidden, string description, string name, int damageModifer)
        {
            Hidden = hidden;
            Description = description;
            Name = name;
            Rarity = rarity;
            DamageModifier = damageModifer;
        }

        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }


        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }


    }
}
