using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines
{
    public class Room
    {
        private Room north;
        private Room east;
        private Room south;
        private Room west;

        private int gold;
        private string description = "";
        private Item item;
        private bool isDark;
        private bool lightsource = false;
        private NPC npc;

        public Room(string description, int gold, int darkLevel)
        {
            Description = description;

            Gold = gold;

            #region Dark room
            if (darkLevel == 0)
            {
                isDark = false;
            }
            else
            {
                if (darkLevel == 1)
                {
                    isDark = true;
                }
            }
            #endregion


        }

        public Room(string description, int gold, Room west, Room south, Room east, Room north, int darkLevel) : this(description, gold, darkLevel)
        {
            West = west;
            South = south;
            East = east;
            North = north;
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public NPC NPC
        {
            get { return npc; }
            set { npc = value; }
        }

        public bool Lightcource
        {
            get { return lightsource; }
            set { lightsource = value; }
        }


        public bool IsDark
        {
            get { return isDark; }
            set { isDark = value; }
        }


        public Item Item
        {
            get { return item; }
            set { item = value; }
        }

        public Room West
        {
            get { return west; }
            set { west = value; }
        }

        public Room South
        {
            get { return south; }
            set { south = value; }
        }


        public Room East
        {
            get { return east; }
            set { east = value; }
        }

        public Room North
        {
            get { return north; }
            set { north = value; }
        }

        private string GetRandomEncounter(int id)
        {
            string encounterDetails = "";

            return encounterDetails;
        }
    }
}
