using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
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
        private int score;
        private List<List<string>> highScores = new List<List<string>>();


        public Game(string gameName)
        {
            if (File.Exists("HighScores.txt") == false)
            {
                File.Create("HighScores.txt");
            }
            using (StreamReader reader = new StreamReader("HighScores.txt"))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    highScores.Add(new List<string>() {line.Split(':')[0], line.Split(':')[1] });
                }
            }

            GameName = gameName;
            CreateNewGame();
        }

        public int HighScore
        {
            get { return score; }
            set { score = value; }
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

        // Create new game
        #region Create New Game
        private void CreateNewGame()
        {
            GetSize(out int main, out int side);
            Console.WriteLine("Creating map");
            CreateRandomRooms(main, side);
            Console.WriteLine("Created map Success");

            Console.WriteLine("Adding items to map");
            AddItemsToRooms();
            Console.WriteLine("Adding items to map success");

            Console.WriteLine("Adding monsters to map");
            AddMonstersToRooms();
            Console.WriteLine("Adding monsters to map success");
            Console.Clear();
            AddPlayer();

            GetHelp();

            RunGame();
        }

        private void GetSize(out int main, out int side)
        {
            while (true)
            {

                main = 0;
                side = 0;
                Console.Clear();
                Console.WriteLine("Choose map size");
                Console.WriteLine();
                Console.WriteLine("-Small");
                Console.WriteLine("-Normal");
                Console.WriteLine("-Big");
                Console.WriteLine("-Extreme");
                Console.WriteLine();
                Console.WriteLine("Custom map: (amount of main rooms) (average side room length)");

                string input = GetPlayerInput();
                switch (input)
                {
                    case "small":
                        main = 6;
                        side = 2;
                        return;
                    case "normal":
                        main = 10;
                        side = 5;
                        return;
                    case "big":
                        main = 20;
                        side = 10;
                        return;
                    case "extreme":
                        main = 50;
                        side = 20;
                        return;
                    default:
                        if (int.TryParse(input.Split(' ')[0], out main))
                        {
                            if (int.TryParse(input.Split(' ')[1], out side))
                            {
                                return;
                            }
                        }
                        break;
                }
            }
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
                main.Add(new Room($"Room: {i + 1}", rnd.Next(1, 50), rnd.Next(0, 2)));
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

                        // Randomize lenght
                        sidePathLenght = rnd.Next(sidePathLenght - 1, sidePathLenght + 1);

                        // Add rooms
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
                            // Make sure the room is linkable
                            if (currentSideRoom.North == null || currentSideRoom.West == null || currentSideRoom.South == null || currentSideRoom.East == null)
                            {
                                while (directionChosen == 0 && s < sideRooms.Count)
                                {
                                    int rndNum = rnd.Next(0, 5);

                                    if (rndNum == 0)
                                    {

                                        if (currentSideRoom.West == null)
                                        {
                                            directionChosen = 1;
                                            currentSideRoom.West = sideRooms[s + 1];
                                            sideRooms[s + 1].East = currentSideRoom;
                                            break;
                                        }
                                    }
                                    else if (rndNum == 1)
                                    {
                                        if (currentSideRoom.North == null)
                                        {
                                            directionChosen = 2;
                                            currentSideRoom.North = sideRooms[s + 1];
                                            sideRooms[s + 1].South = currentSideRoom;
                                            break;
                                        }
                                    }
                                    else if (rndNum == 2)
                                    {
                                        if (currentSideRoom.East == null)
                                        {
                                            directionChosen = 3;
                                            currentSideRoom.East = sideRooms[s + 1];
                                            sideRooms[s + 1].West = currentSideRoom;
                                            break;
                                        }
                                    }
                                    else if (rndNum == 3)
                                    {
                                        if (currentSideRoom.South == null)
                                        {
                                            directionChosen = 4;
                                            currentSideRoom.South = sideRooms[s + 1];
                                            sideRooms[s + 1].North = currentSideRoom;
                                            break;
                                        }
                                    }
                                    else if (rndNum == 4)
                                    {
                                        // Link to random room
                                        Room rndRoom = main[rnd.Next(1, rooms.Count)];
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
                                                directionChosen = 1;
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
                                if (currentRoom.Item is Sword sword)
                                {
                                    sword.Damage = rnd.Next(2, 10);
                                }
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
            List<string> monsterType = new List<string>() { "Dragon", "Orc", "Knight", "Sink" };

            // Add attacks & place in a room
            for (int i = 0; i < rooms.Count; i += 2)
            {
                if (i >= 2)
                {
                    Room current = rooms[i];
                    if (rnd.Next(0, 4) == 0)
                    {
                        Monster monster = new Monster(monsterType[rnd.Next(0, monsterType.Count)], rnd.Next(1, 140), rnd.Next(3, 25), "Head");
                        if (monster.Name == "Dragon")
                        {
                            monster.AddAttack("Hit you with it's tail", rnd.Next(40, 50));
                            monster.AddAttack("Roared", 10);
                            monster.AddAttack("Sat on you", rnd.Next(55, 65));
                            monster.AddAttack("Fired a spike at you", rnd.Next(20, 35));
                        }
                        else if (monster.Name == "Orc")
                        {
                            monster.AddAttack("Hit you with it's bat", rnd.Next(19, 23));
                            monster.AddAttack("Head butted you", rnd.Next(20, 30));
                            monster.WeaponType = "bat";
                        }
                        else if (monster.Name == "Knight")
                        {
                            monster.AddAttack("Hit you with their sword", rnd.Next(17, 25));
                            monster.AddAttack("Charged their sword and hit you", rnd.Next(35, 43));
                            monster.AddAttack("Bashed into you", 10);
                            monster.WeaponType = "Sword";
                            monster.IsScared = true;

                        }
                        else if (monster.Name == "Sink")
                        {
                            monster.AddAttack("Fired water at you", 10);
                            monster.AddAttack("Fell off the wall, and destroyed the floor", rnd.Next(87, 100));
                            monster.AddAttack("Poured water onto the ground", 1);
                            monster.WeaponType = "Water";
                        }

                        current.NPC = monster;
                        monster.CurrentRoom = current;
                    }
                }
            }
        }

        private void AddPlayer()
        {
            Console.WriteLine("Choose character name");
            Player newPlayer = new Player(100, rooms[0], GetPlayerInput(), 10);

            player = newPlayer;
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
                if (player.Currentroom.Item.Hidden == false)
                {
                    player.Inventory.Add(player.Currentroom.Item);
                    Console.WriteLine($"You found {player.Currentroom.Item.Name}");
                    player.Currentroom.Item = null;
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
                string input = GetPlayerInput();

                if (PlayerAction(input, out bool next))
                {
                    if (next)
                    {
                        nextTurn = true;
                    }
                }
            }
        }
        #endregion

        // Base Game logic
        #region Base Game logic

        private void PressToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press a key to continue");
            Console.ReadKey();
        }

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
                bool exists = false;
                switch (direction)
                {
                    case "north":
                        if (currentRoom.North != null)
                        {
                            exists = true;
                        }
                        break;

                    case "east":
                        if (currentRoom.East != null)
                        {
                            exists = true;
                        }
                        break;

                    case "south":
                        if (currentRoom.South != null)
                        {
                            exists = true;
                        }
                        break;

                    case "west":
                        if (currentRoom.West != null)
                        {
                            exists = true;
                        }
                        break;

                    default:
                        break;
                }
                if (exists)
                {
                    peekDescription = "You see a room";
                }
                else
                {
                    peekDescription = "You see a wall";
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
                            Console.WriteLine("You don't see a room that way.");
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
                            Console.WriteLine("You don't see a room that way.");
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
                            Console.WriteLine("You don't see a room that way.");
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
                            Console.WriteLine("You don't see a room that way.");
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

        private string GetPlayerInput()
        {
            string input = Console.ReadLine().ToLower();
            currentInput = input;
            return input;
        }

        private bool PlayerAction(string input, out bool nextTurn)
        {
            bool foundAction = false;
            nextTurn = false;
            if (inCombat == false)
            {
                if (IsDirection(input))
                {
                    if (MoveRoom(input))
                    {
                        Console.WriteLine("You found a room!");
                        player.RoomsVisited += 1;
                        nextTurn = true;
                    }
                    foundAction = true;
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
                                string result = InitiateCombat(0, player.Currentroom.NPC);
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

            }
            // Standard input
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
                if (input.Length > 4 && ((int.TryParse(input.Substring(4), out int num) && player.GetItemByNum(num) != null) || player.GetItemByName(input.Substring(4)) != null))
                {
                    Item usedItem;
                    // Use item
                    if (player.GetItemByNum(num) != null)
                    {
                        usedItem = player.GetItemByNum(num);
                    }
                    else
                    {
                        usedItem = player.GetItemByName(input.Substring(4));
                    }
                    if (usedItem is Flashlight flashlight)
                    {
                        flashlight.Charge -= 5;
                        player.EquippedItem = usedItem;
                        Console.WriteLine("You took out your flashlight");
                    }
                    else if (usedItem is Sword sword)
                    {
                        sword.Durability -= 5;
                        player.EquippedItem = usedItem;
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
                else
                {
                    Console.WriteLine("To use a item: use (item number/name)");
                }
                foundAction = true;
            }
            else if (input.Length > 5 && input.Substring(0, 5) == "equip")
            {
                if ((int.TryParse(input.Substring(6), out int num) || player.GetItemByNum(num) != null) || player.GetItemByName(input.Substring(6)) != null)
                {
                    Item usedItem;
                    if (player.GetItemByNum(num) != null)
                    {
                        usedItem = player.GetItemByNum(num);
                    }
                    else
                    {
                        usedItem = player.GetItemByName(input.Substring(6));
                    }

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
                    Console.WriteLine("Could not find item");
                    Console.WriteLine("To equip a item: equip (item number/name)");
                    Thread.Sleep(600);
                }
                foundAction = true;
            }
            else if (input == "highscore")
            {
                Console.WriteLine($"Your score: {GetScore()}");
                Console.WriteLine();
                Console.WriteLine("Highscores");
                Console.WriteLine();

                List<int> scoreNumbers = new List<int>();
                foreach (var score in highScores)
                {
                    scoreNumbers.Add(int.Parse(score[1]));
                }

                scoreNumbers.Sort();
                scoreNumbers.Reverse();
                List<List<string>> sortedScores = new List<List<string>>();
                foreach (int scoreNum in scoreNumbers)
                {
                    foreach (List<string> score in highScores)
                    {
                        if (scoreNum == int.Parse(score[1]))
                        {
                            sortedScores.Add(score);
                        }
                    }
                }
                foreach (var score in sortedScores)
                {
                    Console.WriteLine($"{score[0]} : {score[1]}");
                }
                highScores = sortedScores;

                PressToContinue();

                foundAction = true;
            }

            if (foundAction == false)
            {
                Console.WriteLine("Not a command");
                Console.WriteLine("use 'help' to see all commands");
            }
            return foundAction;
        }

        private int GetScore()
        {
            return int.Parse(Math.Round(player.Health + (player.Gold * 0.5) + (player.Inventory.Count * 2)).ToString());
        }
        #endregion

        // Combat
        #region Combat

        private string InitiateCombat(int initiator = 0, NPC Monster = null)
        {
            inCombat = true;
            if (Monster == null)
            {
                Monster = player.Currentroom.NPC;
            }
            if (initiator == 1)
            {
                Console.Clear();
                Console.WriteLine($"You got seen.");
                PressToContinue();
            }

            string result = RunCombat(initiator, Monster);
            inCombat = false;
            return result;
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
                inCombat = false;
                return "player dead";
            }
            else if (monster.Health < 0)
            {
                // Add loot
                if (monster is Monster monsterDrop)
                {
                    Console.WriteLine($"{monster.Name} defeated");
                    Console.WriteLine();
                    List<Item> drops = new List<Item>(monsterDrop.DropItems(rnd.Next(1, 5)));
                    Console.WriteLine($"The {monster.Name} dropped");
                    Console.WriteLine();
                    foreach (Item item in drops)
                    {
                        player.Inventory.Add(item);
                        Console.WriteLine($"-{item.Name}");
                    }
                    monsterDrop.CurrentRoom.NPC = null;
                    Thread.Sleep(1000);
                }
                inCombat = false;
                return "monster dead";
            }

            inCombat = false;
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
            string input = GetPlayerInput();

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
                PlayerAction(input, out bool ignoreMe);
                extraTurn = true;
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
            score = GetScore();
            if (state == "won")
            {
                Console.Clear();
                Console.WriteLine("You Won!");
                Console.WriteLine();
                score += 100;
            }
            else if (state == "dead")
            {
                Console.Clear();
                Console.WriteLine("You Died!");
                score -= 50;

            }
            else
            {
                Console.Clear();
                Console.WriteLine("Adventure ended.");
                Console.WriteLine();
            }

            using (StreamWriter writer = new StreamWriter("HighScores.txt", true))
            {
                writer.WriteLine($"{player.Name} : {score}");
            }

            Console.WriteLine("Character statistics");
            Console.WriteLine();
            Console.WriteLine($"Name: {player.Name}");
            Console.WriteLine();
            Console.WriteLine($"Score: {score}");
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
            Console.WriteLine("-(Direction) | Move around using directions (North, East, South, West)");
            Console.WriteLine();
            Console.WriteLine("-Health | Displays health");
            Console.WriteLine("-Gold | Displays current gold");
            Console.WriteLine("-HighScore | Displays your score and all highscores");
            Console.WriteLine("-Help | Lists all the commands in the game");
            Console.WriteLine();
            Console.WriteLine("-Peek (direction) | See what is in the direction");
            Console.WriteLine("-Use (Item number/name) | Use an item");
            Console.WriteLine("-Equip (Item number/name) | Equip an item");
            Console.WriteLine("-Search | Search the room for hidden items");
            Console.WriteLine("-Attack | Attack the monster in the room");
            Console.WriteLine("-Inventory | Displays your items along with their number");
            Console.WriteLine();
            Console.WriteLine("-Quit | Ends the game.");
            Console.WriteLine();

            PressToContinue();
        }
    }
}