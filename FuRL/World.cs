using System;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;

using PureHatred.Entities;

namespace PureHatred
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
            CurrentMap = mapGen.MapgenDungeonCannibalized(_mapWidth, _mapHeight, _maxRooms, _minRoomDimension, _maxRoomDimension);
        }

        private void CreateMonsters()
        {
            int numMonsters = 100;

            for (int i = 0; i < numMonsters; i++)
            {
                int monsterPosition = 0;
                Monster newMonster = new Monster(Color.Blue, Color.Transparent);

                while (CurrentMap.Tiles[monsterPosition].IsImpassible)
                    monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);

                newMonster.Defense = rndNum.Next(0, 10);
                newMonster.DefenseChance = rndNum.Next(0, 50);
                newMonster.Attack = rndNum.Next(0, 10);
                newMonster.AttackChance = rndNum.Next(0, 50);
                newMonster.HealthMax = rndNum.Next(25, 50);
                newMonster.Health = newMonster.HealthMax;
                newMonster.Name = "a common troll";

                newMonster.Position = new Point(monsterPosition % CurrentMap.Width, monsterPosition / CurrentMap.Width);
                CurrentMap.Add(newMonster);
            }
        }

        private void CreatePlayer()
        {
            Player = new Player(Color.Yellow, Color.Transparent);

            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsImpassible)
                {
                    Player.Position = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    break;
                }
            }
            CurrentMap.Add(Player);
        }

        private void CreateLoot()
        {
            int numLoot = 20;

            for (int i = 0; i < numLoot; i++)
            {
                int lootPosition = 0;
                Item newLoot = new Item(Color.Green, Color.Transparent, "fancy shirt", 'L', 2);

                while (CurrentMap.Tiles[lootPosition].IsImpassible)
                    lootPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);

                newLoot.Position = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Width);

                CurrentMap.Add(newLoot);
            }
        }
    }
}
