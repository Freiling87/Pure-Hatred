using System;

using Microsoft.Xna.Framework;
using SadConsole;

using PureHatred.Entities;
using System.Linq;
using PureHatred.Entities.Components;

namespace PureHatred
{
    public class Populator
    {
        private readonly int _mapWidth = 51;
        private readonly int _mapHeight = 51;
        private readonly int _maxRooms = 100;
        private readonly int _minRoomDimension = 4;
        private readonly int _maxRoomDimension = 15;

        private TileBase[] _mapTiles;
        public Map CurrentMap { get; set; }
        public Player Player { get; set; }

        public Populator()
        {
            CreateMap();
            CreatePlayer();
            CreateFlora();
            CreateFauna();
        }

        private void CreateMap()
        {
            _mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            MapGenerator mapGen = new MapGenerator();
            CurrentMap = mapGen.MapgenDungeonCannibalized(_mapWidth, _mapHeight, _maxRooms, _minRoomDimension, _maxRoomDimension);
        }

        private void CreateFlora()
        {
            int numFlora = 100;

            for (int i = 0; i < numFlora; i++)
            {
                int tileIndex = 0;
                BodyPart mushroom = new BodyPart(Color.GreenYellow, Color.Transparent, "mushroom", (Spritesheet.Anatomy)Spritesheet.Flora.Spindly); //Temporary, while mushrooms are BPs

                while (CurrentMap.Tiles[tileIndex].IsImpassible)
                    tileIndex = GameLoop.rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);

                mushroom.Name = "a mushroom";

                mushroom.Position = new Point(tileIndex % CurrentMap.Width, tileIndex / CurrentMap.Width);
                CurrentMap.Add(mushroom);
            }
        }

        private void CreateFauna()
        {
            int numFauna = 100;

            for (int i = 0; i < numFauna; i++)
            {
                int tileIndex = 0;
                Npc fauna = new Npc(Color.Blue, Color.Transparent, "a helpless Cherub");

                while (CurrentMap.Tiles[tileIndex].IsImpassible)
                    tileIndex = GameLoop.rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);

                fauna.Position = new Point(tileIndex % CurrentMap.Width, tileIndex / CurrentMap.Width);
                CurrentMap.Add(fauna);

                CurrentMap.Actors.Add(fauna);
            }
        }

        private void CreatePlayer()
        {
            Player = new Player(Color.Yellow, Color.Transparent, "Cherub Bully");

            for (int i = 0; i < CurrentMap.Tiles.Length; i++)
                if (!CurrentMap.Tiles[i].IsImpassible)
                {
                    Player.Position = Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    break;
                }

            CurrentMap.Add(Player);
            CurrentMap.Actors.Add(Player);
        }
    }
}
