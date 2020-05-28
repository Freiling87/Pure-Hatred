using System;
using System.Linq;

using Microsoft.Xna.Framework;
using SadConsole;

using FuRL.Tiles;
using FuRL.Entities;

//TODO: Convert from Point to TileIndex
// so that instead of _map.Tiles[east.ToIndex(_map.Width)].IsBlockingMove
// we can use NewFn(Point).IsBlockingMove

namespace FuRL
{
    public class Map
    {
        TileBase[] _tiles; // contain all tile objects
        private int _width;
        private int _height;

        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; }}
        public int Width { get { return _width; } set { _width = value; }}
        public int Height { get { return _height; } set { _height = value; }}

        public GoRogue.MultiSpatialMap<Entity> Entities; // Keeps track of all the Entities on the map
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator(); // A static IDGenerator that all Entities can access

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];
            Entities = new GoRogue.MultiSpatialMap<Entity>();
        }

        public bool IsTileWalkable(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        public T GetTileAt<T>(int x, int y) where T : TileBase
        {
            int locationIndex = Helpers.GetIndexFromPoint(x, y, Width);
            // make sure the index is within the boundaries of the map!
            if (locationIndex <= Width * Height && locationIndex >= 0)
            {
                if (Tiles[locationIndex] is T)
                    return (T)Tiles[locationIndex];
                else return null;
            }
            else return null;
        }

        public T GetTileAt<T>(Point location) where T : TileBase
        {
            return GetTileAt<T>(location.X, location.Y);
        }

        public T GetEntityAt<T>(Point location) where T : Entity
        {
            return Entities.GetItems(location).OfType<T>().FirstOrDefault();
        }

        public void Remove(Entity entity)
        {
            Entities.Remove(entity);
            entity.Moved -= OnEntityMoved; // Link entity Moved event to new handler
        }

        public void Add(Entity entity)
        {
            Entities.Add(entity, entity.Position);
            entity.Moved += OnEntityMoved; // Link entity Moved event to new handler
        }

        // When Entity .Moved value changes, triggers this event handler
        // which updates the Entity's current position in the SpatialMap
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args)
        {
            Entities.Move(args.Entity as Entity, args.Entity.Position);
        }
    }
}
