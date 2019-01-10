using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines
{
    public class NPC
    {
        private string name;
        private int health;
        private bool isFriendly;

        public NPC(string name, int health, bool isFriendly)
        {
            Name = name;
            Health = health;
            IsFriendly = isFriendly;
        }

        public bool IsFriendly
        {
            get { return isFriendly; }
            set { isFriendly = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
