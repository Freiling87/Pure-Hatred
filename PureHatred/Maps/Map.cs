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

        public List<Actor> Actors = new List<Actor>();

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];
            Entities = new GoRogue.MultiSpatialMap<Entity>();
        }

        public bool IsTileWalkable(Point location) =>
            location.X >= 0 && 
            location.Y >= 0 && 
            location.X < Width && 
            location.Y < Height && 
            !_tiles[location.ToIndex(Width)].IsImpassible;

        public T GetEntityAt<T>(Point tile) where T : Entity =>
            Entities.GetItems(tile).OfType<T>().FirstOrDefault();

        public List<T> GetEntitiesAt<T>(Point tile) where T : Entity =>
            Entities.GetItems(tile).OfType<T>().ToList();

        public T GetTileAt<T>(int x, int y) where T : TileBase
        {
            int locationIndex = Helpers.GetIndexFromPoint(x, y, Width);

            if (locationIndex <= Width * Height && locationIndex >= 0)
				return Tiles[locationIndex] is T t ? t : null;          //https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching#the-is-type-pattern-expression
            else return null;
        }
        
        public T GetTileAt<T>(Point tile) where T : TileBase =>
            GetTileAt<T>(tile.X, tile.Y);

        public List<T> GetTilesAt<T>(params Point[] tiles) where T : TileBase //TODO: Linq one-liner
		{
            List<T> result = new List<T>();

            for (int i = 0; i < tiles.Length; i++)
                result.Add(GetTileAt<T>(tiles[i].X, tiles[i].Y));

            return result;
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

        // If Entity .Moved changes, this event handler updates Entity's position in SpatialMap
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args) =>
            Entities.Move(args.Entity as Entity, args.Entity.Position);
    }
}