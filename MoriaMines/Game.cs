using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// Moria Mines
using MoriaMines.NPCs;
using MoriaMines.Items;

namespace MoriaMines
{
    public class Game
    {
        private List<Room> rooms = new List<Room>();
        private Player player;
        private string gameName;
        private string currentInput = ""; // used to quit the game
        private bool inCombat = false;

        public Game(string gameName)
        {
            GameName = gameName;
            CreateNewGame();
        }

        public Game(string gameName, Player player = null, List<Room> rooms = null) : this(gameName)
        {
            Player = player;
            Rooms = rooms;
        }


        public string GameName
        {
            get { return gameName; }
            set { gameName = value; }
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }
        public List<Room> Rooms
        {
            get { return rooms; }
            set { rooms = value; }
        }

        private void PressToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press a key to continue");
            Console.ReadKey();
        }


        // Create new game
        #region Create New Game
        private void CreateNewGame()
        {
            CreateRandomRooms();

            AddPlayer();

            AddItemsToRooms();

            AddMonstersToRooms();

            RunGame();
        }

        private static Random rnd = new Random();

        private void CreateRandomRooms(int mainRooms = 10, int sidePathLenght = 4)
        {
            List<Room> main = new List<Room>();
            List<Room> side = new List<Room>();


            main.Add(new Room("Entrance ", 0, 0));

            // Create main rooms
            for (int i = 0; i < mainRooms; i++)
            {
                main.Add(new Room($"Room: {i + 1}", rnd.Next(1, 50), rnd.Next(0, 10)));
            }

            main.Add(new Room("End", 1000, 0));

            // Link main rooms

            Room previousRoom = main[0];
            string previousDirection = "north";

            for (int i = 0; i < main.Count; i++)
            {
                Room currentRoom = main[i];
                // Entrance link
                if (i == 0)
                {
                    currentRoom.North = main[i + 1];
                }
                else if (i < main.Count - 1)// Other links
                {
                    switch (previousDirection)
                    {
                        case "north":
                            currentRoom.South = previousRoom;
                            break;
                        case "east":
                            currentRoom.West = previousRoom;
                            break;
                        case "south":
                            currentRoom.North = previousRoom;
                            break;
                        case "west":
                            currentRoom.East = previousRoom;
                            break;

                        default:
                            break;
                    }

                    bool linked = false;
                    while (!linked)
                    {
                        int rndNumber = rnd.Next(0, 4);

                        if (rndNumber == 0)
                        {
                            if (previousDirection != "east")
                            {
                                currentRoom.West = main[i + 1];
                                previousDirection = "west";
                                previousRoom = main[i];
                                linked = true;
                            }
                        }
                        else if (rndNumber == 3)
                        {
                            if (previousDirection != "west")
                            {
                                currentRoom.East = main[i + 1];
                                previousDirection = "east";
                                previousRoom = main[i];
                                linked = true;
                            }
                        }
                        else
                        {
                            currentRoom.North = main[i + 1];
                            previousDirection = "north";
                            previousRoom = main[i];
                            linked = true;
                        }
                    }
                }
            }

            rooms = main;

            #region SideRooms
            for (int i = 0; i < main.Count; i++)
            {
                List<Room> sideRooms = new List<Room>();
                if (i != 0)
                {
                    if (rnd.Next(1, 3) == 1)
                    {
                        Room mainRoom = main[i];

                        // Add rooms

                        // Randomize lenght
                        sidePathLenght = rnd.Next(sidePathLenght - 1, sidePathLenght + 1);

                        for (int s = 0; s < sidePathLenght; s++)
                        {
                            sideRooms.Add(new Room("Hallway", rnd.Next(1, 50), rnd.Next(1, 10)));
                            side.Add(sideRooms[s]);
                            rooms.Add(sideRooms[s]);
                        }

                        // Link rooms
                        for (int s = 0; s < sideRooms.Count - 1; s++)
                        {

                            Room currentSideRoom = sideRooms[s];
                            if (s == 0)
                            {
                                currentSideRoom = mainRoom;
                            }

                            int directionChosen = 0;

                            while (directionChosen == 0 && s < sideRooms.Count)
                            {
                                switch (rnd.Next(1, 6))
                                {
                                    case 1:
                                        if (currentSideRoom.West == null)
                                        {
                                            directionChosen = 1;
                                            currentSideRoom.West = sideRooms[s + 1];
                                            sideRooms[s + 1].East = currentSideRoom;
                                        }
                                        break;

                                    case 2:
                                        if (currentSideRoom.North == null)
                                        {
                                            directionChosen = 2;
                                            currentSideRoom.North = sideRooms[s + 1];
                                            sideRooms[s + 1].South = currentSideRoom;
                                        }
                                        break;

                                    case 3:
                                        if (currentSideRoom.East == null)
                                        {
                                            directionChosen = 3;
                                            currentSideRoom.East = sideRooms[s + 1];
                                            sideRooms[s + 1].West = currentSideRoom;
                                        }
                                        break;

                                    case 4:
                                        if (currentSideRoom.South == null)
                                        {
                                            directionChosen = 4;
                                            currentSideRoom.South = sideRooms[s + 1];
                                            sideRooms[s + 1].North = currentSideRoom;
                                        }
                                        break;

                                    case 5:
                                        // Link to random room
                                        for (int r = 0; r < 20; r++)
                                        {
                                            Room rndRoom = rooms[rnd.Next(1, rooms.Count)];
                                            if (rndRoom.North == null)
                                            {
                                                if (currentSideRoom.South == null)
                                                {
                                                    rndRoom.North = currentSideRoom;
                                                    currentSideRoom.South = rndRoom;
                                                    directionChosen = 2;
                                                    break;
                                                }
                                            }
                                            if (rndRoom.West == null)
                                            {
                                                if (currentSideRoom.East == null)
                                                {
                                                    rndRoom.West = currentSideRoom;
                                                    currentSideRoom.East = rndRoom;
                                                    directionChosen = 11;
                                                    break;
                                                }
                                            }
                                            if (rndRoom.South == null)
                                            {
                                                if (currentSideRoom.North == null)
                                                {
                                                    rndRoom.North = currentSideRoom;
                                                    currentSideRoom.South = rndRoom;
                                                    directionChosen = 4;
                                                    break;
                                                }
                                            }
                                            if (rndRoom.East == null)
                                            {
                                                if (currentSideRoom.West == null)
                                                {
                                                    rndRoom.East = currentSideRoom;
                                                    currentSideRoom.West = rndRoom;
                                                    directionChosen = 3;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void AddItemsToRooms()
        {
            // Create items
            #region Items
            Flashlight wetFlashlight = new Flashlight("Wet Flashlight", "Commonly found in ponds of water.", false, 0, -3, 70, 40);
            Flashlight standardFlashlight = new Flashlight("Flashlight", "Your average flashlight from the local store.", false, 2, -1, 60, 60);
            Flashlight highTechFlashlight = new Flashlight("High Tech Flashlight", "The best flashlight", true, 10, 2, 100, 100);

            Sword rustySword = new Sword(1, 3, "Sword", "Bit rusty but still functional", false, 2);

            Potion healingPotion = new Potion(2, false, "Heals you for some life", "Healing Potion", 3, "Healing", 25);

            Armor armor = new Armor(rnd.Next(5, 10), 1, false, "Protects you from some damage", "Armor");
            #endregion
            player.Inventory.Add(rustySword);
            // Add items to rooms
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                if (i != 0)
                {
                    // 25% chance to add a item to the room
                    if (rnd.Next(1, 4) == 1)
                    {
                        Room currentRoom = rooms[i];

                        switch (rnd.Next(0, 5))
                        {
                            case 0:
                                currentRoom.Item = rustySword;
                                break;
                            case 1:
                                if (rnd.Next(1, 11) == 10)
                                {
                                    currentRoom.Item = highTechFlashlight;
                                }
                                else
                                {
                                    currentRoom.Item = standardFlashlight;
                                }
                                break;
                            case 2:
                                currentRoom.Item = wetFlashlight;
                                break;
                            case 3:
                                currentRoom.Item = healingPotion;
                                break;
                            case 4:
                                armor.ArmorValue = rnd.Next(1, 10);
                                currentRoom.Item = armor;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void AddMonstersToRooms()
        {
            //Generate monsters
            List<string> type = new List<string>() { "Dragon", "Orc", "Knight", "Sink" };

            // Add attacks & place in a room
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i <= 2)
                {

                }
                else
                {

                    Room current = rooms[i];
                    if (rnd.Next(0, 4) == 0)
                    {
                        Monster monster = new Monster(type[rnd.Next(0, type.Count)], rnd.Next(1, 200), rnd.Next(3, 30), "Head");
                        if (monster.Name == "Dragon")
                        {
                            monster.AddAttack("Hit you with it's tail", 46);
                            monster.AddAttack("Roared", 12);
                            monster.AddAttack("Jumped up in the air, and landed on you", 63);
                            monster.AddAttack("Fired a spike at you", 34);
                        }
                        else if (monster.Name == "Orc")
                        {
                            monster.AddAttack("Hit you with it's bat", 21);
                            monster.AddAttack("Head butted you", 17);
                            monster.WeaponType = "bat";
                        }
                        else if (monster.Name == "Knight")
                        {
                            monster.AddAttack("Hit you with their sword", 19);
                            monster.AddAttack("Charged their sword and hit you", 35);
                            monster.AddAttack("Bashed into you", 10);
                            monster.WeaponType = "Sword";
                            monster.IsScared = true;

                        }
                        else if (monster.Name == "Sink")
                        {
                            monster.AddAttack("Fired water at you", 10);
                            monster.AddAttack("Fired water at you", 10);
                            monster.AddAttack("Fell off the wall, and destroyed the floor", 90);
                            monster.AddAttack("Poured water onto the ground", 1);
                            monster.AddAttack("Poured water onto the ground", 1);
                            monster.WeaponType = "Water";

                        }

                        current.NPC = monster;
                        monster.CurrentRoom = current;
                    }
                }
            }
        }

        private void AddPlayer(Player existingPlayer = null)
        {
            Console.WriteLine("Choose character name");
            Player newPlayer = new Player(100, rooms[0], Console.ReadLine(), 10);

            if (existingPlayer == null)
            {
                player = newPlayer;
            }
            else
            {
                player = existingPlayer;
            }
        }
        #endregion

        // Running game
        #region Running Game
        private void RunGame()
        {
            while (player.Currentroom.Description != "End" && currentInput != "quit" && player.Health > 0)
            {
                Turn();
                Thread.Sleep(100);
            }

            if (currentInput == "quit")
            {
                EndScreen("quit");
            }
            else if (player.Health < 0)
            {
                EndScreen("dead");
            }
            else
            {
                EndScreen("won");
            }
        }
        private void Turn()
        {
            bool nextTurn = false;
            Console.Clear();
            // Room Description
            Console.WriteLine(player.Currentroom.Description);

            // Room Item
            if (player.Currentroom.Item != null)
            {
                if (player.Currentroom.Item.Hidden != true)
                {
                    player.Inventory.Add(player.Currentroom.Item);
                    Console.WriteLine($"You found {player.Currentroom.Item.Name}");
                }
            }
            // Room Gold

            Console.WriteLine($"You found {player.Currentroom.Gold} Gold!");
            player.Gold += player.Currentroom.Gold;
            player.Currentroom.Gold = 0;


            // Monster
            if (player.Currentroom.NPC != null)
            {
                if (rnd.Next(0, 3) == 0)
                {
                    string result = InitiateCombat(1, player.Currentroom.NPC);
                    if (result == "player fled" || result == "monster fled")
                    {
                        Console.Clear();
                        Console.WriteLine(player.Currentroom.Description);
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"You see a {player.Currentroom.NPC.Name} in the room, it dosn't look like it has seen you");
                }
            }
            Thread.Sleep(100);

            // Player actions
            while (!nextTurn && player.Health > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Were do you go now?");
                string input = Console.ReadLine().ToLower();

                if (PlayerAction(input, out bool next))
                {
                    if (next)
                    {
                        nextTurn = true;
                    }
                }
                else
                {
                }
            }
        }
        #endregion

        // Base Game logic
        #region Base Game logic
        private bool IsDirection(string input)
        {
            bool isDirection = false;
            switch (input.ToLower())
            {
                case "north":
                    isDirection = true;
                    break;

                case "east":
                    isDirection = true;
                    break;

                case "south":
                    isDirection = true;
                    break;

                case "west":
                    isDirection = true;
                    break;

                default:
                    break;
            }
            return isDirection;
        }

        private string PeekNextRoom(string direction)
        {
            string peekDescription = "";
            Room currentRoom = player.Currentroom;
            if (currentRoom.IsDark && !(player.EquippedItem is Flashlight))
            {
                peekDescription = "It's so dark you can't see anything";
            }
            else
            {
                switch (direction)
                {
                    case "north":
                        if (currentRoom.North != null)
                        {
                            peekDescription = "You see a room";
                        }
                        else
                        {
                            peekDescription = "You see a wall";
                        }
                        break;

                    case "east":
                        if (currentRoom.East != null)
                        {
                            peekDescription = "You see a room";
                        }
                        else
                        {
                            peekDescription = "You see a wall";
                        }
                        break;

                    case "south":
                        if (currentRoom.South != null)
                        {
                            peekDescription = "You see a room";
                        }
                        else
                        {
                            peekDescription = "You see a wall";
                        }
                        break;

                    case "west":
                        if (currentRoom.West != null)
                        {
                            peekDescription = "You see a room";
                        }
                        else
                        {
                            peekDescription = "You see a wall";
                        }
                        break;

                    default:
                        break;
                }
            }

            return peekDescription;
        }
        private bool MoveRoom(string direction)
        {
            bool succes = false;
            if (player.Currentroom != null)
            {
                Room currentRoom = player.Currentroom;
                switch (direction.ToLower())
                {
                    case ("north"):
                        if (currentRoom.North != null)
                        {
                            player.Currentroom = player.Currentroom.North;
                            succes = true;
                        }
                        else
                        {
                            Console.WriteLine("You were in such a hurry that you walked right into a wall! (-1 health)");
                            player.Health -= 1;
                        }
                        break;

                    case ("east"):
                        if (currentRoom.East != null)
                        {
                            player.Currentroom = player.Currentroom.East;
                            succes = true;
                        }
                        else
                        {
                            Console.WriteLine("You were in such a hurry that you walked right into a wall! (-1 health)");
                            player.Health -= 1;
                        }
                        break;

                    case ("south"):
                        if (currentRoom.South != null)
                        {
                            player.Currentroom = player.Currentroom.South;
                            succes = true;
                        }
                        else
                        {
                            Console.WriteLine("You were in such a hurry that you walked right into a wall! (-1 health)");
                            player.Health -= 1;
                        }
                        break;

                    case ("west"):
                        if (currentRoom.West != null)
                        {
                            player.Currentroom = player.Currentroom.West;
                            succes = true;
                        }
                        else
                        {
                            Console.WriteLine("You were in such a hurry that you walked right into a wall! (-1 health)");
                            player.Health -= 1;
                        }
                        break;

                    default:
                        succes = false;
                        break;
                }
            }
            return succes;
        }

        private void PrintInventory()
        {
            Console.Clear();
            Console.WriteLine("Inventory");
            Console.WriteLine();

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Item item = player.Inventory[i];
                Console.WriteLine($"{i + 1} {item.Name} : {item.Description}");
            }

            PressToContinue();
        }

        private bool PlayerAction(string input, out bool nextTurn)
        {
            bool foundAction = false;
            nextTurn = false;
            if (inCombat)
            {
            }
            else
            {
                if (IsDirection(input))
                {
                    if (MoveRoom(input))
                    {
                        Console.WriteLine("You found a room!");
                        player.RoomsVisited += 1;
                        nextTurn = true;
                        foundAction = true;
                    }
                }
                else if (input == "search" || input == "attack")
                {
                    switch (input)
                    {
                        case "search":
                            if (player.SearchCurrentRoom() != null)
                            {
                                player.Inventory.Add(player.SearchCurrentRoom());

                                Console.WriteLine();
                                Console.WriteLine($"You found a {player.SearchCurrentRoom().Name}!");
                                player.Currentroom.Item = null;
                            }
                            break;

                        case "attack":
                            if (player.Currentroom.NPC != null)
                            {
                                InitiateCombat(0, player.Currentroom.NPC);
                                nextTurn = true;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("There is nothing to attack.");
                            }
                            break;

                        default:
                            break;
                    }
                    foundAction = true;
                }
                else if (input.Length >= 4 && input.Substring(0, 4) == "peek")
                {
                    Console.WriteLine();
                    if (input.Length >= 5 && IsDirection(input.Substring(5)))
                    {
                        Console.WriteLine(PeekNextRoom(input.Substring(5)));
                    }
                    else
                    {
                        Console.WriteLine("To peek use:  Peek (direction)");
                    }
                    foundAction = true;
                }
                else
                {
                    Console.WriteLine("Invalid command");
                }
            }

            if (input == "help")
            {
                GetHelp();
                foundAction = true;
            }
            else if (input == "health" || input == "gold" || input == "quit" || input == "inventory")
            {
                switch (input)
                {
                    case "health":
                        Console.WriteLine($"Health: {player.Health}");
                        break;

                    case "gold":
                        Console.WriteLine($"Gold: {player.Gold}");
                        break;

                    case "quit":
                        currentInput = "quit";
                        nextTurn = true;
                        break;

                    case "inventory":
                        PrintInventory();

                        nextTurn = true;
                        break;
                    default:
                        break;
                }
                foundAction = true;
            }
            else if (input.Length >= 3 && input.Substring(0, 3) == "use")
            {
                Console.WriteLine();
                if (input.Length > 4)
                {
                    if (player.GetItemByNum(int.Parse(input.Substring(4))) != null)
                    {
                        // Use item
                        Item usedItem = player.GetItemByNum(int.Parse(input.Substring(4)));
                        player.EquippedItem = usedItem;
                        if (usedItem is Flashlight flashlight)
                        {
                            flashlight.Charge -= 5;
                            Console.WriteLine("You took out your flashlight");
                        }
                        else if (usedItem is Sword sword)
                        {
                            sword.Durability -= 5;
                            Console.WriteLine("You took out you your sword");
                        }
                        else if (usedItem is Armor armor)
                        {
                            player.Armor += armor.ArmorValue;
                            player.Inventory.Remove(armor);
                            Console.WriteLine("You equipped armor");
                            Thread.Sleep(1000);
                        }
                        if (usedItem is Potion potion)
                        {
                            if (potion.Type == "Healing")
                            {
                                player.Health += potion.Healing;
                                player.Inventory.Remove(usedItem);
                                Console.WriteLine($"You healed for {potion.Healing}");
                                Thread.Sleep(1200);
                            }
                        }

                    }
                }
                else
                {
                    Console.WriteLine("To use a item: use (item name)");
                }
                foundAction = true;
            }
            else if (input.Length > 5 && input.Substring(0, 5) == "equip")
            {
                if (int.TryParse(input.Substring(6), out int num) && player.Inventory.Count >= num && player.Inventory[num - 1] != null)
                {
                    Item usedItem = player.Inventory[int.Parse(input.Substring(6)) - 1];
                    if (usedItem is Armor armor)
                    {
                        player.Armor += armor.ArmorValue;
                        player.Inventory.Remove(usedItem);
                        Console.WriteLine("You equipped some armor");
                    }
                    else
                    {
                        player.EquippedItem = usedItem;
                        Console.WriteLine($"You equipped {usedItem.Name}");
                    }
                    Thread.Sleep(1200);
                }
                else
                {
                    Console.WriteLine($"Item does not exist");
                    Thread.Sleep(600);
                }
                foundAction = true;
            }


            return foundAction;
        }
        #endregion

        // Combat
        #region Combat

        private string InitiateCombat(int initiator = 0, NPC Monster = null)
        {
            if (Monster == null)
            {
                Monster = player.Currentroom.NPC;
            }
            return RunCombat(initiator, Monster);
        }

        private string RunCombat(int initiator, NPC monster)
        {
            bool stop = false;
            int whoseTurn = initiator;
            while (monster.Health > 0 && player.Health > 0 && !stop)
            {
                Console.Clear();
                Console.WriteLine($"{Player.Name} Health: {player.Health}     VS      {monster.Name} Health: {monster.Health}");
                Console.WriteLine();

                // Monster Turn
                if (whoseTurn == 1)
                {
                    Console.WriteLine($"{monster.Name}'s Turn");
                    Console.WriteLine();

                    int monsterDamage = MonsterTurn(monster, out bool fled);
                    if (fled)
                    {
                        stop = true;
                        Console.WriteLine($"The {monster.Name} fled.");
                        whoseTurn = 0;
                        return "monster escaped";
                    }
                    else if (monsterDamage == 0)
                    {
                        Console.WriteLine("The monster did nothing");
                    }
                    else
                    {
                        player.Health -= monsterDamage;
                    }

                    whoseTurn = 0;
                }
                // Player Turn
                else
                {
                    Console.WriteLine("Your Turn");
                    Console.WriteLine();
                    int playerDamage = PlayerTurn(out bool fled, out bool extraTurn);

                    if (fled)
                    {
                        //player flee's
                        if (rnd.Next(0, 2) == 0)
                        {
                            stop = true;
                            Console.WriteLine($"The {monster.Name} let you escape.");
                            whoseTurn = 0;
                            Thread.Sleep(1000);
                            return "player fled";
                        }
                        else
                        {
                            Console.WriteLine("You were not able to flee");
                            Thread.Sleep(1000);
                            whoseTurn = 1;
                        }
                    }
                    else if (extraTurn)
                    {
                        // reset screen
                        whoseTurn = 0;
                    }
                    else
                    {
                        monster.Health -= playerDamage;
                        PrintCombatTurn(0, "", playerDamage, monster);
                        whoseTurn = 1;
                    }
                }
            }
            if (player.Health < 0)
            {
                return "player dead";
            }
            else if (monster.Health < 0)
            {
                // Add loot
                if (monster is Monster monsterDrop)
                {
                    Console.WriteLine($"{monster.Name} defeated");
                    List<Item> drops = new List<Item>(monsterDrop.DropItems(rnd.Next(1, 5)));
                    Console.WriteLine($"The {monster.Name} dropped");
                    foreach (Item item in drops)
                    {
                        player.Inventory.Add(item);
                        Console.WriteLine($"-{item.Name}");
                    }
                    monsterDrop.CurrentRoom.NPC = null;
                    Thread.Sleep(1000);
                }
                return "monster dead";
            }

            return "";
        }

        private void PrintCombatTurn(int whoseTurn, string description, int damage, NPC monster)
        {
            if (whoseTurn == 1)
            {
                Console.WriteLine($"The {monster.Name} {description} (-{damage} Health)");
            }
            else
            {
                Console.WriteLine();
                if (description == string.Empty)
                {
                    if (player.EquippedItem != null)
                    {
                        if (player.EquippedItem is Sword)
                        {
                            Console.WriteLine($"You slashed the {monster.Name} for {damage} damage");
                        }
                        else if (player.EquippedItem is Flashlight)
                        {
                            Console.WriteLine($"You blinded the {monster.Name} for {damage} damage");
                        }
                        else if (player.EquippedItem is Armor)
                        {
                            Console.WriteLine($"You bashed the {monster.Name} for {damage} damage");

                        }
                    }
                    else
                    {
                        Console.WriteLine($"You hit the {monster.Name} for {damage} damage");
                    }
                }
                else
                {
                    Console.WriteLine($"You {description} the {monster.Name} for {damage} damage");
                }
            }
            PressToContinue();
        }

        private int PlayerTurn(out bool fled, out bool extraTurn)
        {
            extraTurn = false;
            fled = false;
            int damage = 0;
            // player options
            #region Player Options
            Console.WriteLine("Your options are:");
            Console.WriteLine();
            Console.WriteLine("-Attack | Attacks with your currently equipped item");
            Console.WriteLine("-Use (item number) | You use an item from your inventory");
            Console.WriteLine("-Equip (item number) | You equip an item from your inventory");
            Console.WriteLine("-Flee | You try to flee from combat");
            Console.WriteLine("-Inventory | See what is in your inventory");
            Console.WriteLine();
            Console.WriteLine("What do you do?");
            #endregion
            string input = Console.ReadLine().ToLower();

            if (input == "attack")
            {
                if (player.EquippedItem is Sword sword)
                {
                    damage = player.Damage + sword.Damage;
                    sword.Durability -= 5;
                }
                else
                {
                    if (player.EquippedItem != null)
                    {
                        damage = player.Damage + player.EquippedItem.DamageModifier;
                    }
                    else
                    {
                        damage = player.Damage;
                    }
                }
            }
            else if (input == "flee")
            {
                if (rnd.Next(0, 2) == 0)
                {
                    fled = true;
                }
                else
                {
                    Console.WriteLine($"You were not able to flee.");
                    Thread.Sleep(1000);
                }
            }
            else
            {
                if (PlayerAction(input, out bool ignorMe) == false)
                {
                    extraTurn = true;
                    Console.WriteLine("Invalid action");
                }
                Thread.Sleep(600);
            }
            return damage;
        }

        private int MonsterTurn(NPC Monster, out bool fled)
        {
            fled = false;
            if (Monster is Monster monster)
            {
                int randomNr = 0;
                if (monster.IsScared)
                {
                    randomNr = rnd.Next(0, monster.Attacks.Count + 1);
                }
                else
                {
                    randomNr = rnd.Next(0, monster.Attacks.Count);
                }

                if (randomNr == monster.Attacks.Count)
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        // Escaped
                        fled = true;
                        return 0;
                    }
                }
                if (randomNr == 0)
                {
                    randomNr++;
                }
                int playerDamageTaken = player.TakeDamage(monster.Attacks.ElementAt(randomNr - 1).Value);

                PrintCombatTurn(1, monster.Attacks.ElementAt(randomNr - 1).Key, playerDamageTaken, monster);
                return playerDamageTaken;
            }
            else
            {
                // Did nothing
                return 0;
            }
        }

        #endregion

        // EndScreen
        private void EndScreen(string state)
        {

            if (state == "won")
            {
                Console.Clear();
                Console.WriteLine("You Won!");
                Console.WriteLine();
            }
            else if (state == "dead")
            {
                Console.Clear();
                Console.WriteLine("You Died!");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Adventure ended.");
                Console.WriteLine();
            }

            Console.WriteLine("Character statistics");
            Console.WriteLine();
            Console.WriteLine($"Name: {player.Name}");
            Console.WriteLine();
            Console.WriteLine($"Health: {player.Health}");
            Console.WriteLine($"Gold: {player.Gold}");
            Console.WriteLine($"Rooms visited: {player.RoomsVisited}");

            Console.ReadKey();
        }

        // Help command

        private void GetHelp()
        {
            Console.WriteLine();

            Console.WriteLine("These are all the commands in the game");

            Console.WriteLine();
            Console.WriteLine("-Health | Displays health");
            Console.WriteLine("-Gold | Displays current gold");
            Console.WriteLine("-Peek (direction) | See what is in the direction");
            Console.WriteLine("-Use (Item name) | Equip and use item");
            Console.WriteLine("-Search | Search the room for hidden items");
            Console.WriteLine("-Attack | attack the monster in the room");
            Console.WriteLine();
            Console.WriteLine("-Quit | Ends the game.");
            Console.WriteLine();

            PressToContinue();
        }
    }
}