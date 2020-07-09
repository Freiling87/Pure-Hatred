using System;
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
        public int HealRate { get; set; } = 1;

        public int NetHungerSimple { get; set; }
        public int SatiationSimple { get; set; }
        public int NetHungerComplex { get; set; }
        public int SatiationComplex { get; set; }

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
                return UseDoor(door);

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
            NetHungerComplex = anatomy.Sum(x => x.HungerComplex);
            NetHungerSimple = anatomy.Sum(x => x.HungerSimple);
            Health = anatomy.Sum(x => x.HpCurrent);
            HealthMax = anatomy.Sum(x => x.HpMax);
		}

        public void BioRhythm()
        {
            Alimentation();
            HealWounds();
        }

        public void Alimentation()
        {
            Stomach.StomachDigestion();
            Intestines.IntestinalDigestion();

            SatiationComplex -= NetHungerComplex;
            if (SatiationComplex < 0)
            {
                Kwashiorkor(0 - SatiationComplex);
                SatiationComplex = 0;
            }

            SatiationSimple -= NetHungerSimple;
            if (SatiationSimple < 0)
			{
                Starvation(0 - SatiationSimple);
                SatiationSimple = 0;
			}
        }

        public void Kwashiorkor(int deficiency)
		{
		}

        public void Starvation(int deficiency)
		{
		}

        public void HealWounds()
        {
            bool multiBreak = false;

            for (int i = 0; i < HealRate; i++) //Always heal same HP per nutrients, just at faster rate if possible
                if (!multiBreak)
                    foreach (BodyPart bodyPart in anatomy.Where(bodyPart => bodyPart.HpCurrent < bodyPart.HpMax))
                    {
                        if (!multiBreak)
                            bodyPart.HpCurrent++;
                        else
                            break;

                        if (SatiationSimple-- == 0)
                            multiBreak = true;
                    }
                else
                    break;
        }

        public bool UseDoor(TileDoor door)
        {
            if (door.Locked)
                return false; // TODO
            if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                return true;
            }
            return false;
        }
    }
}