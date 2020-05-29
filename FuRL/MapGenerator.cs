using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using FuRL.Tiles;

namespace FuRL
{
	public class MapGenerator
    {
        Random randNum = new Random();
        Map _map; // Temporarily store the map currently worked on

        public MapGenerator()
        {
        }

        public Map GenerateMap(int mapWidth, int mapHeight, int maxRooms, int minRoomDimension, int maxRoomDimension)
        {
            _map = new Map(mapWidth, mapHeight);
            List<Rectangle> Rooms = new List<Rectangle>();

            for (int i = 0; i < maxRooms; i++)
            {
                int newRoomWidth = randNum.Next(minRoomDimension, maxRoomDimension);
                int newRoomHeight = randNum.Next(minRoomDimension, maxRoomDimension);

                int newRoomX = randNum.Next(0, mapWidth - newRoomWidth - 1);
                int newRoomY = randNum.Next(0, mapHeight - newRoomHeight - 1);

                Rectangle newRoom = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);

                bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects) // If disabled, can make some interesting shapes as well
                {
                    Rooms.Add(newRoom);
                }
            }

            FillMapWithWalls();

            foreach (Rectangle room in Rooms)
                DigSquareRoom(room);

            // carve tunnels between room centers in order of their creation 
            // Coin toss determines L or 7 shaped hallway
            for (int r = 1; r < Rooms.Count; r++)
            {
                List<int> coinToss = new List<int>() { 0, 1 };
                coinToss = coinToss.OrderBy(i => Guid.NewGuid()).ToList();

                TunnelHorizontally(Rooms[r - 1].Center.X, Rooms[r].Center.X, Rooms[r - coinToss[0]].Center.Y);
                TunnelVertically(Rooms[r - 1].Center.Y, Rooms[r].Center.Y, Rooms[r - coinToss[1]].Center.X);

                // I got it to four lines but made it more complicated than it needs to be :D 
                // TODO: Change it back when you get a chance
            }

            foreach (Rectangle room in Rooms)
                CreateDoor(room);

            return _map;
        }

        private void FillMapWithWalls()
        {
            for (int i = 0; i < _map.Tiles.Length; i++)
                _map.Tiles[i] = new TileWall();
        }

        private void DigSquareRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
                for (int y = room.Top + 1; y < room.Bottom; y++)
                    CreateFloor(new Point(x,y));

            List<Point> perimeter = GetPerimiterSquare(room);

            foreach (Point location in perimeter)
                CreateWall(location);
        }

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
            if (!_map.Tiles[east.ToIndex(_map.Width)].IsBlockingMove && 
                !_map.Tiles[west.ToIndex(_map.Width)].IsBlockingMove && 
                _map.Tiles[north.ToIndex(_map.Width)].IsBlockingMove && 
                _map.Tiles[south.ToIndex(_map.Width)].IsBlockingMove
                )
                return true;

            //North-South door
            if (_map.Tiles[east.ToIndex(_map.Width)].IsBlockingMove && 
                _map.Tiles[west.ToIndex(_map.Width)].IsBlockingMove && 
                !_map.Tiles[north.ToIndex(_map.Width)].IsBlockingMove && 
                !_map.Tiles[south.ToIndex(_map.Width)].IsBlockingMove
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
                yield return new Point(xA, yA);
                if (xA == xZ && yA == yZ)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err = err - dy;
                    xA = xA + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    yA = yA + sy;
                }


            }
        }

        private int ClampX(int x) =>
            x < 0 ? 0 : 
            x > _map.Width - 1 ? _map.Width - 1 : 
            x;

        private int ClampY(int y) =>
            y < 0 ? 0 : 
            y > _map.Height - 1 ? _map.Height - 1 : 
            y;
    }
}
