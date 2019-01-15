using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines
{
    public class Player
    {
        private int gold;
        private string name;
        private int health;
        private int roomsVisited = 0;
        private Room currentRoom;
        private List<Item> inventory = new List<Item>();
        private Item equippedItem;
        private int damage = 6;
        private int armor;

        public int Armor
        {
            get { return armor; }
            set { armor = value; }
        }


        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }


        public Item EquippedItem
        {
            get { return equippedItem; }
            set { equippedItem = value; }
        }


        public List<Item> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public int RoomsVisited
        {
            get { return roomsVisited; }
            set { roomsVisited = value; }
        }


        public Player(int health, string name, int gold)
        {
            Health = health;
            Name = name;
            Gold = gold;
        }

        public Player(int health, Room currentroom, string name, int gold) : this(health, name, gold)
        {
            Currentroom = currentroom;
        }
        public Room Currentroom
        {
            get { return currentRoom; }
            set { currentRoom = value; }
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

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public Item GetItemByNum(int num)
        {
            Item item;
            if (num > 0 && num <= inventory.Count)
            {
                item = inventory[num - 1];
            }
            else
            {
                return null;
            }
            return item;
        }
        public Item GetItemByName(string input)
        {
            foreach (Item item in inventory)
            {
                if (item.Name.ToLower() == input.ToLower())
                {
                    return item;
                }
            }
            return null;
        }

        public Item SearchCurrentRoom()
        {
            if (currentRoom != null)
            {
                return currentRoom.Item;
            }
            else
            {
                return null;
            }
        }

        public int TakeDamage(int damage)
        {
            int damageTaken = damage - armor;
            if (damageTaken < 0) //damagetaken < 0 - bonuspoint for kreativitet :)
            {
                damageTaken = 0;
            }
            return damageTaken;
        }

    }
}
