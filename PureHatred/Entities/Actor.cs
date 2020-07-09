using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using PureHatred.Commands;
using PureHatred.Resolutions;
using PureHatred.Tiles;
using PureHatred.UI;

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
        public int HungerSimple { get; set; }
        public int NutSimple { get; set; }
        public int HungerComplex { get; set; }
        public int NutComplex { get; set; }
        public int HealRate { get; set; } = 1;
        public int NutrientSurplus { get; set; } = 0;

        public List<Mutation> Mutations = new List<Mutation>();
        public Anatomy anatomy;
        public Inventory inventory;

        public BodyPart Brain;
        public BodyPart Core;
        public BodyPart Intestines; //Checks for nutrients to process into system
        public BodyPart Mouth;
        public BodyPart Stomach;

        protected Actor(Color foreground, Color background, int glyph, string name) : base(foreground, background, glyph)
        {
            Name = name;

            anatomy = new Anatomy(this);
            inventory = new Inventory(this);

            anatomy.HardCodeHumanParts();
        }

        private Item AddLoot(Item item)
        {
            inventory.Add(item);

            return item;
        }

        public bool MoveBy(Point positionChange)
        {
            Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(Position + positionChange);
            Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(Position + positionChange);
            BodyPart bodyPart = GameLoop.World.CurrentMap.GetEntityAt<BodyPart>(Position + positionChange);
            TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);
            TileWall wall = GameLoop.World.CurrentMap.GetTileAt<TileWall>(Position + positionChange);

            if (monster != null)
                //return GameLoop.CommandManager.BumpAttack(this, monster);
                Combat.BumpAttack(this, monster);
            else if (bodyPart != null)
                return Mouth.Masticate(bodyPart);
            //else if (item != null)
            //    return GameLoop.CommandManager.Pickup(this, item);
            else if (door != null && !door.IsOpen)
                return GameLoop.CommandManager.UseDoor(this, door);

            else if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                Position += positionChange;
                return true;
            }

            return false;
        }

        public bool MoveTo(Point newPosition)
        {
            Position = newPosition;
            return true;
        }

        public void NetBiologyValues()
		{
            HungerComplex = anatomy.Sum(x => x.HungerComplex);
            HungerSimple = anatomy.Sum(x => x.HungerSimple);
            Health = anatomy.Sum(x => x.HpCurrent);
            HealthMax = anatomy.Sum(x => x.HpMax);
		}

        public void BioRhythm()
        {
            Alimentation();
            HealWounds();
            if (this == GameLoop.World.Player)
                GameLoop.UIManager.MessageLog.AddTextNewline("Player Biorhythm now");
            //TODO: Apparently this isn't running for the player, only for enemies. Will need to make sur Player is in Actors().
        }

        public void Alimentation()
        {
            Stomach.StomachDigestion();
            Intestines.IntestinalDigestion();

            foreach (BodyPart bodyPart in anatomy)
            {
                NutSimple -= bodyPart.HungerSimple;
                NutComplex -= bodyPart.HungerComplex;
            }
        }

        public void HealWounds()
        {
            bool multiBreak = false;

            IEnumerable<BodyPart> ouchies = anatomy.Where(bodyPart => bodyPart.HpCurrent < bodyPart.HpMax);

            for (int i = 0; i < HealRate; i++) //Always heal same HP per nutrients, just at faster rate if possible
                if (!multiBreak)
                    foreach (BodyPart bodyPart in ouchies)
                    {
                        if (!multiBreak)
                            bodyPart.HpCurrent++;
                        else
                            break;

                        if (NutrientSurplus-- == 0)
                            multiBreak = true;
                    }
                else
                    break;
        }
    }
}