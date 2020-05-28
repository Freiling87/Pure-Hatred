using System;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;

using FuRL.Entities;

namespace FuRL
{
    public class World
    {
        private int _mapWidth = 50;
        private int _mapHeight = 50;
        private int _maxRooms = 100;
        private int _minRoomDimension = 4;
        private int _maxRoomDimension = 15;

        Random rndNum = new Random(); 
        private TileBase[] _mapTiles;
        public Map CurrentMap { get; set; }
        public Player Player { get; set; }

        public World()
        {
            CreateMap();
            CreatePlayer();
            CreateMonsters();
            CreateLoot();
        }

        private void CreateMap()
        {
            _mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomDimension, _maxRoomDimension);
        }

        // Create some random monsters
        // and drop them all over the map in
        // random places.
        private void CreateMonsters()
        {
            // number of monsters to create
            int numMonsters = 100;

            // Create several monsters with random attack and defense values and 
            // pick a random position on the map to place them.
            // check if the placement spot is blocking (e.g. a wall)
            // and if it is, try a new position
            for (int i = 0; i < numMonsters; i++)
            {
                int monsterPosition = 0;
                Monster newMonster = new Monster(Color.Blue, Color.Transparent);

                while (CurrentMap.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // plug in some magic numbers for attack and defense values
                newMonster.Defense = rndNum.Next(0, 10);
                newMonster.DefenseChance = rndNum.Next(0, 50);
                newMonster.Attack = rndNum.Next(0, 10);
                newMonster.AttackChance = rndNum.Next(0, 50);
                newMonster.Name = "a common troll";

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                newMonster.Position = new Point(monsterPosition % CurrentMap.Width, monsterPosition / CurrentMap.Width);
                CurrentMap.Add(newMonster);
            }
        }

        // Create a player using the Player class
        // and set its starting position
        private void CreatePlayer()
        {
            Player = new Player(Color.Yellow, Color.Transparent);

            // Place the player on the first non-movement-blocking tile on the map
            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove)
                {
                    // Set the player's position to the index of the current map position
                    Player.Position = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    break;
                }
            }

            // add the player to the global EntityManager's collection of Entities
            CurrentMap.Add(Player);
        }

        // Create some sample treasure
        // that can be picked up on the map
        private void CreateLoot()
        {
            // number of treasure drops to create
            int numLoot = 20;

            // Produce lot up to a max of numLoot
            for (int i = 0; i < numLoot; i++)
            {
                // Create an Item with some standard attributes
                int lootPosition = 0;
                Item newLoot = new Item(Color.Green, Color.Transparent, "fancy shirt", 'L', 2);

                // Try placing the Item at lootPosition; if this fails, try random positions on the map's tile array
                while (CurrentMap.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // set the loot's new position
                newLoot.Position = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Width);

                // add the Item to the MultiSpatialMap
                CurrentMap.Add(newLoot);
            }

        }
    }
}
