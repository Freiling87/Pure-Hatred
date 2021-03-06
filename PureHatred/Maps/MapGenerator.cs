﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Expressions;
using GoRogue;
using Microsoft.Xna.Framework;

using PureHatred.Tiles;
using SadConsole;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

//TODO: Explore GoRogue documentation on MapGen. Not fully covered in tutorial yet. https://github.com/Chris3606/GoRogue/blob/master/GoRogue.Docs/articles/gr3-map-gen.md

namespace PureHatred
{
	public class MapGenerator
    {
        Map _map; // Temporarily store the map currently worked on

        public MapGenerator(){}

		public Map MapgenDungeonCannibalized(int mapWidth, int mapHeight, int maxRooms, int minRoomDimension, int maxRoomDimension)
        {
            _map = new Map(mapWidth, mapHeight);

            FillMapWithWalls();

            List<Rectangle> Rooms =
                new List<Rectangle>(GenerateSquareRooms(mapWidth, mapHeight, maxRooms, minRoomDimension, maxRoomDimension));

            foreach (Rectangle room in Rooms)
                DigSquareRoom(room);

            ConnectRoomCentersWithTunnels(Rooms);

            foreach (Rectangle room in Rooms)
                CreateDoor(room);

            return _map;
        }

        #region TOOLS

        private void CreateFloor(Point location) =>
            _map.Tiles[location.ToIndex(_map.Width)] = new TileFloor();

        private void CreateWall(Point location) =>
            _map.Tiles[location.ToIndex(_map.Width)] = new TileWall();

        private List<Point> GetPerimiterSquare(Rectangle room)
        {
            int xA = room.Left;
            int xZ = room.Right;
            int yA = room.Top;
            int yZ = room.Bottom;

            List<Point> borderCells = GetTileLocationsAlongLine(xA, yA, xZ, yA).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xA, yA, xA, yZ));
            borderCells.AddRange(GetTileLocationsAlongLine(xA, yZ, xZ, yZ));
            borderCells.AddRange(GetTileLocationsAlongLine(xZ, yA, xZ, yZ));

            return borderCells;
        }

        private bool IsPotentialDoor(Point center)
        {
            int locationIndex = center.ToIndex(_map.Width);

            if (_map.Tiles[locationIndex] != null && _map.Tiles[locationIndex] is TileWall)
                return false;

            Point east = new Point(center.X + 1, center.Y);
            Point west = new Point(center.X - 1, center.Y);
            Point north = new Point(center.X, center.Y - 1);
            Point south = new Point(center.X, center.Y + 1);

            if (_map.GetTilesAt<TileDoor>(center, east, west, north, south).Any() == false)
                return false;

            //East-West door
            if (!_map.Tiles[east.ToIndex(_map.Width)].IsImpassible &&
                !_map.Tiles[west.ToIndex(_map.Width)].IsImpassible &&
                _map.Tiles[north.ToIndex(_map.Width)].IsImpassible &&
                _map.Tiles[south.ToIndex(_map.Width)].IsImpassible
                )
                return true;

            //North-South door
            if (_map.Tiles[east.ToIndex(_map.Width)].IsImpassible &&
                _map.Tiles[west.ToIndex(_map.Width)].IsImpassible &&
                !_map.Tiles[north.ToIndex(_map.Width)].IsImpassible &&
                !_map.Tiles[south.ToIndex(_map.Width)].IsImpassible
                )
                return true;

            return false;
        }

        public IEnumerable<Point> GetTileLocationsAlongLine(int xA, int yA, int xZ, int yZ)
        {
            xA = ClampX(xA);
            yA = ClampY(yA);
            xZ = ClampX(xZ);
            yZ = ClampY(yZ);

            int dx = Math.Abs(xZ - xA);
            int dy = Math.Abs(yZ - yA);

            int sx = xA < xZ ? 1 : -1;
            int sy = yA < yZ ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                int e2 = 2 * err;

                yield return new Point(xA, yA);

                if (xA == xZ && yA == yZ)
                    break;

                if (e2 > -dy)
                {
                    err -= dy;
                    xA += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    yA += sy;
                }
            }
        }

        private int ClampX(int x) =>
            x < 0 ? 0 : x > _map.Width - 1 ? _map.Width - 1 : x;

        private int ClampY(int y) =>
            y < 0 ? 0 : y > _map.Height - 1 ? _map.Height - 1 : y;

        #endregion TOOLS
        #region SETUP

        private void FillMapWithWalls()
        {
            for (int i = 0; i < _map.Tiles.Length; i++)
                _map.Tiles[i] = new TileWall();
        }

        #endregion SETUP
        #region GENERATION
        public List<Rectangle> GenerateSquareRooms(int mapWidth, int mapHeight, int roomCount, int minRoomDimension, int maxRoomDimension)
        {
            List<Rectangle> Rooms = new List<Rectangle>();

            for (int i = 0; i < roomCount; i++)
            {
                int newRoomWidth = GameLoop.rndNum.Next(minRoomDimension, maxRoomDimension);
                int newRoomHeight = GameLoop.rndNum.Next(minRoomDimension, maxRoomDimension);

                int newRoomX = GameLoop.rndNum.Next(0, mapWidth - newRoomWidth - 1);
                int newRoomY = GameLoop.rndNum.Next(0, mapHeight - newRoomHeight - 1);

                Rectangle newRoom = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);

                bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects) // If disabled, can make some interesting shapes as well
                    Rooms.Add(newRoom);
            }
            return Rooms;
        }

        private void DigSquareRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
                for (int y = room.Top + 1; y < room.Bottom; y++)
                    CreateFloor(new Point(x, y));

			List<Point> perimeter = GetPerimiterSquare(room);

			foreach (Point location in perimeter)
				CreateWall(location);

			//TODO: Refactor, last part is redundant. Still need to track perimiter though.
		}

        public void ConnectRoomCentersWithTunnels(List<Rectangle> rooms)
        {
            for (int r = 1; r < rooms.Count; r++)
            {
                Point roomA = rooms[r - 1].Center;
                Point roomB = rooms[r].Center;

                List<Point> coinToss = new List<Point>() { roomA, roomB }.OrderBy(i => Guid.NewGuid()).ToList();

                TunnelHorizontally(roomA.X, roomB.X, coinToss[0].Y);
                TunnelVertically(roomA.Y, roomB.Y, coinToss[1].X);
            }
        }

        private void TunnelHorizontally(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
                CreateFloor(new Point(x, yPosition));
        }

        private void TunnelVertically(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
                CreateFloor(new Point(xPosition, y));
        }

        private void CreateDoor(Rectangle room)
        {
            List<Point> borderCells = GetPerimiterSquare(room);

            foreach (Point location in borderCells)
            {
                int locationIndex = location.ToIndex(_map.Width);

                if (IsPotentialDoor(location))
                {
                    TileDoor newDoor = new TileDoor(false, false);
                    _map.Tiles[locationIndex] = newDoor;
                }
            }
        }

        #endregion GENERATION

        private void RecursiveDivision()
		{
            // https://en.wikipedia.org/wiki/Maze_generation_algorithm#Recursive_division_method
        }

        private Point GetMidpoint(Point a, Point b) =>
            new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
    }
}
