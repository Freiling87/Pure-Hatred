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
        public List<BodyPart> Anatomy = new List<BodyPart>();

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
        }

        public bool MoveBy(Point positionChange)
        {
            if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(Position + positionChange);
                Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(Position + positionChange);
                BodyPart bodyPart = GameLoop.World.CurrentMap.GetEntityAt<BodyPart>(Position + positionChange);

                if (monster != null)
                    return GameLoop.CommandManager.BumpAttack(this, monster);
                else if (item != null)
                    return GameLoop.CommandManager.Pickup(this, item);
                else if (bodyPart != null)
                    return GameLoop.CommandManager.Pickup(this, bodyPart);

                Position += positionChange;
                return true;
            }
            else
            {
                TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);
                if (door != null)
                    return GameLoop.CommandManager.UseDoor(this, door);
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
