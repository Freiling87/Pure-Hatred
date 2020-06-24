using System;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;

using PureHatred.Entities;

/* TODO:
 * - Replace Point(tileIndex % CurrentMap.Width, tileIndex / CurrentMap.Width);
 *   with Point(TileIndexToPoint(TileIndex));
 *   or Overload Point to accept a single int and use that:
 *   Point(TileIndex);
 */

namespace PureHatred
{
    public class World
    {
        private readonly int _mapWidth = 150;
        private readonly int _mapHeight = 150;
        private readonly int _maxRooms = 100;
        private readonly int _minRoomDimension = 4;
        private readonly int _maxRoomDimension = 15;

        private Random rndNum = new Random(); 
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
                if (!CurrentMap.Tiles[i].IsImpassible)
                {
                    Player.Position = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    break;
                }

            CurrentMap.Add(Player);

            for (int i = 1; i < 100; i++)
                Player.Inventory.Add(new Item(Color.Green, Color.Transparent, $"test {i}", 'L', 2));
        }

        private void CreateLoot()
        {
            int numLoot = 20;

            for (int i = 0; i < numLoot; i++)
            {
                Item newLoot = new Item(Color.Green, Color.Transparent, "fancy shirt", 'L', 2);

                int tileIndex = 0;
                while (CurrentMap.Tiles[tileIndex].IsImpassible)
                    tileIndex = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);

                newLoot.Position = new Point(tileIndex % CurrentMap.Width, tileIndex / CurrentMap.Width);

                CurrentMap.Add(newLoot);
            }
        }
    }
}
