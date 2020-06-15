using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using SadConsole;

using PureHatred.Tiles;
using PureHatred.Entities;

namespace PureHatred
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
            return !_tiles[location.ToIndex(Width)].IsImpassible;
        }

        public T GetEntityAt<T>(Point tile) where T : Entity
        {
            return Entities.GetItems(tile).OfType<T>().FirstOrDefault();
        }
        public List<T> GetEntitiesAt<T>(Point tile) where T : Entity
		{
            return Entities.GetItems(tile).OfType<T>().ToList();
        }

        public T GetTileAt<T>(int x, int y) where T : TileBase
        {
            int locationIndex = Helpers.GetIndexFromPoint(x, y, Width);

            if (locationIndex <= Width * Height && locationIndex >= 0)
            {
                if (Tiles[locationIndex] is T)
                    return (T)Tiles[locationIndex];
                else return null;
            }
            else return null;
        }
        
        public T GetTileAt<T>(Point tile) where T : TileBase
        {
            return GetTileAt<T>(tile.X, tile.Y);
        }

        public List<T> GetTilesAt<T>(params Point[] tiles) where T : TileBase
		{
            //Returns list of Tiles at multiple Points
            //A Point can only hold one tile - do not try to detect stacks of tiles!
            List<T> result = new List<T>();

            for (int i = 0; i < tiles.Length; i++)
                result.Add(GetTileAt<T>(tiles[i].X, tiles[i].Y));

            return result;
		}

        public void Remove(Entity entity)
        {
            if (!Entities.Remove(entity))
                throw new Exception("Failed to remove entity from map");
            entity.Moved -= OnEntityMoved; // Link entity Moved event to new handler
        }

        public void Add(Entity entity)
        {
            if (!Entities.Add(entity, entity.Position))
                throw new Exception("Failed to add entity to map");

            entity.Moved += OnEntityMoved; // Link entity Moved event to new handler
        }

        // If Entity .Moved changes, this event handler updates Entity's position in SpatialMap
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args) =>
            Entities.Move(args.Entity as Entity, args.Entity.Position);
    }
}