using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using PureHatred.Tiles;

namespace PureHatred.Entities
{
    public abstract class Actor : Entity
    {
        public int Attack { get; set; } 
        public int AttackChance { get; set; } 
        public int Defense { get; set; }
        public int DefenseChance { get; set; } 
        public int Gold { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public List<Item> Inventory = new List<Item>();

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, width, height, glyph)
        {
        }

        public bool MoveBy(Point positionChange)
        {
            if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(Position + positionChange);
                Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(Position + positionChange);

                if (monster != null)
                {
                    GameLoop.CommandManager.Attack(this, monster);
                    return true;
                }
                else if (item != null)
                {
                    GameLoop.CommandManager.Pickup(this, item);
                    return true;
                }

                Position += positionChange;
                return true;
            }
            else
            {
                TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);
                if (door != null)
                {
                    GameLoop.CommandManager.UseDoor(this, door);
                    return true;
                }
                return false;
            }
        }

        public bool MoveTo(Point newPosition)
        {
            Position = newPosition;
            return true;
        }
    }
}
