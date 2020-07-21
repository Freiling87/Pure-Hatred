using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using SadConsole;

using PureHatred.Tiles;
using PureHatred.Entities;
using GoRogue;

namespace PureHatred
{
    public class Map
    {
        /* TODO:
         * - Replace Point(TileIndex % CurrentMap.Width, tileIndex / CurrentMap.Width);
         *   with Point(TileIndexToPoint(TileIndex));
         *   or Overload Point to accept a single int and use that:
         *   Point(TileIndex);
         */

        public TileBase[] Tiles { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		private GoRogue.MultiSpatialMap<Entity> MapEntities;
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();

        public List<Actor> Actors = new List<Actor>();

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new TileBase[width * height];
            MapEntities = new GoRogue.MultiSpatialMap<Entity>();
        }

        #region TERRAIN

        public Point IndexToPoint(int tileIndex) =>
            new Point(tileIndex % Width, tileIndex / Height);

        public int PointToIndex(Point input)
		{
            //TODO
            return 0;
		}

        public T GetTileAt<T>(int x, int y) where T : TileBase
        {
            int locationIndex = Helpers.GetIndexFromPoint(x, y, Width);

            if (locationIndex <= Width * Height && locationIndex >= 0)
                return Tiles[locationIndex] is T t ? t : null;          //https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching#the-is-type-pattern-expression
            else return null;
        }

        public T GetTileAt<T>(Point tile) where T : TileBase =>
            GetTileAt<T>(tile.X, tile.Y);

        public bool IsWall(Point tile) =>
            GetTileAt<TileWall>(tile.X, tile.Y) != null;

        public Point North(Point context, int distance) =>
            new Point(context.X, context.Y - distance);
        public Point West(Point context, int distance) =>
            new Point(context.X - distance, context.Y);

        public List<T> GetTilesAt<T>(params Point[] tiles) where T : TileBase //TODO: Linq one-liner
        {
            List<T> result = new List<T>();

            for (int i = 0; i < tiles.Length; i++)
                result.Add(GetTileAt<T>(tiles[i].X, tiles[i].Y));

            return result;
        }

        public bool IsTileWalkable(Point location) =>
            location.X >= 0 && 
            location.Y >= 0 && 
            location.X < Width && 
            location.Y < Height && 
            !Tiles[location.ToIndex(Width)].IsImpassible;

		#endregion TERRAIN
		#region ENTITIES

		public T GetEntityAt<T>(Point tile) where T : Entity =>
            MapEntities.GetItems(tile).OfType<T>().FirstOrDefault();

        public List<T> GetEntitiesAt<T>(Point tile) where T : Entity =>
            MapEntities.GetItems(tile).OfType<T>().ToList();

        public void Add(Entity entity)
        {
            MapEntities.Add(entity, entity.Position);

            entity.Moved += OnEntityMoved;
        }

        public void Remove(Entity entity)
        {
            MapEntities.Remove(entity);

            entity.Moved -= OnEntityMoved;
        }

        public void OnMapEntityAdded(object sender, GoRogue.ItemEventArgs<Entity> args)
        {
            int insertionIndex = 0;

            for (int i = 0; i < GameLoop.UIManager.MapConsole.Children.Count; i++)
                if (GameLoop.UIManager.MapConsole.Children[i] is Entity e && e.renderOrder > args.Item.renderOrder)
                {
                    insertionIndex = i;
                    break;
                }
            GameLoop.UIManager.MapConsole.Children.Insert(insertionIndex, args.Item);
        }

        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args) =>
            MapEntities.Move(args.Entity as Entity, args.Entity.Position);

        public void OnMapEntityRemoved(object sender, GoRogue.ItemEventArgs<Entity> args) =>
            GameLoop.UIManager.MapConsole.Children.Remove(args.Item);

        public void SyncMapEntities()
        {
            /* Chris3606:change void SyncMapEntities() to void ConfigureAsRenderer(ScrollingConsole console).  
             *   The function would take the console it's setting up as a parameter, and cache it as a private variable.   
             *     Use that variable everywhere in Map instead of GameLoop.UIManager.MapConsole.  
             * Then have a function RemoveRenderer() that nulls out the variable and unlinks events as needed.   
             * That way map has a more strong guarantee that what it wants is actually there and what it expects.
             */

            GameLoop.UIManager.MapConsole.Children.Clear();

            MapEntities.ItemAdded += OnMapEntityAdded;
            MapEntities.ItemRemoved += OnMapEntityRemoved;

            foreach (Entity entity in MapEntities.Items)
                GameLoop.UIManager.MapConsole.Children.Insert(0, entity);
        }

		#endregion ENTITIES
		#region DECALS

		public void BloodSplatter(Point position, int volume)
		{
            Decal blood = new Decal(Color.DarkRed, Color.Transparent, "blood", 256 + volume)
            { Position = position };
            GameLoop.World.CurrentMap.Add(blood);

            // TODO: Examine SadConsole.CellDecorator
            // TODO: Intensity of splatter should be inverse to distance traveled
            // TODO: Provide source and splatter from here
        }

		#endregion Decals
	}
}
 