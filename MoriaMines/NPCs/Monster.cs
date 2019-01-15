using MoriaMines.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoriaMines.NPCs
{
    public class Monster : NPC
    {
        private Room currentRoom;
        private bool isScared;
        private int damage;
        private Dictionary<string, int> attacks = new Dictionary<string, int>();
        private string weaponType;

        private static Random rnd = new Random();

        public Monster(string name, int health, int damage, string weaponType = "sword", bool isFriendly = false, bool isScared = false) : base(name, health, isFriendly)
        {
            Damage = damage;
            IsScared = isScared;
            attacks.Add($"Hit you with their {weaponType}", damage);
        }
        public string WeaponType
        {
            get { return weaponType; }
            set { weaponType = value; }
        }

        public Dictionary<string, int> Attacks
        {
            get { return attacks; }
            set { attacks = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public bool IsScared
        {
            get { return isScared; }
            set { isScared = value; }
        }

        public Room CurrentRoom
        {
            get { return currentRoom; }
            set { currentRoom = value; }
        }

        public void AddAttack(string description, int damage)
        {
            try
            {
                attacks.Add(description, damage);
            }
            catch (Exception)
            {
            }
        }

        public Item DropItem()
        {
            Item item = new Item(1, false, "", "", 0);
            switch (rnd.Next(0, 4))
            {
                case 0:
                    Flashlight newFlashlight = new Flashlight("Trusty Flashlight", $"{Name}'s beloved flashlight", false, 4, 7, 100, 100);
                    item = newFlashlight;
                    break;
                case 1:
                    Sword newSword = new Sword(200, 20 + damage, $"{Name}'s sword", $"The mighty {Name}'s sword", false, 4);
                    item = newSword;
                    break;
                case 2:
                    Potion newPotion = new Potion(4, false, $"Made by {Name}", $"{Name}'s Potion", -20, "Healing", 100);
                    item = newPotion;
                    break;
                case 3:
                    if (WeaponType == "Water")
                    {
                        Potion newWaterPotion = new Potion(5, false, "Just a not so normal bottle", $"{Name}'s bottle", 43 + damage, "Damage");
                        item = newWaterPotion;
                    }
                    break;
                default:
                    break;
            }
            return item;
        }

        public List<Item> DropItems(int count)
        {
            List<Item> items = new List<Item>();

            for (int i = 0; i < count; i++)
            {
                items.Add(DropItem());
            }

            return items;
        }
    }
}