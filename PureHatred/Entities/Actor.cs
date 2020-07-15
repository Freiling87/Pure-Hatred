using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using PureHatred.Commands;
using PureHatred.Entities.Components;
using PureHatred.Resolutions;
using PureHatred.Tiles;
using PureHatred.UI;
using SadConsole;

namespace PureHatred.Entities
{
    public abstract class Actor : Entity
    {
        new public readonly int renderOrder = (int)RenderOrder.Actor;

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
        public Psyche psyche;

        public BodyPart Brain;
        public BodyPart Core;
        public BodyPart Intestines; //Checks for nutrients to process into system
        public BodyPart Mouth;
        public BodyPart Stomach;

        protected Actor(Color foreground, Color background, int glyph, string name) : base(foreground, background, glyph)
        {
            Name = name;

            anatomy = new Anatomy(this);

            anatomy.HardCodeHumanParts();

            NetBiologyValues();

            Health = HealthMax;
        }

        public void MoveBy(Point positionChange)
        {
            Npc monster = GameLoop.World.CurrentMap.GetEntityAt<Npc>(Position + positionChange);
            BodyPart bodyPart = GameLoop.World.CurrentMap.GetEntityAt<BodyPart>(Position + positionChange);
            TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);

            if (monster != null)
                Combat.BumpAttack(this, monster);
            else if (bodyPart != null)
                Mouth.Mastication(bodyPart);
            else if (door != null && !door.IsOpen)
                UseDoor(door);
            else if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                Position += positionChange;
                GameLoop.UIManager.MapConsole.CenterViewPortOnPoint(GameLoop.World.Player.Position);
            }
            else
                return;

            GameLoop.GSManager.turnTaken = true;
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

            Defense = anatomy.Sum(x => x.Striking);
            DefenseChance = anatomy.Sum(x => x.Dexterity) * 2;
            Attack = anatomy.Sum(x => x.Striking);
            AttackChance = anatomy.Sum(x => x.Dexterity) * 3;
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
            BodyPart currentPart = null;
            Queue<BodyPart> ouchies = new Queue<BodyPart>();
            ouchies.Concat(anatomy.Where(bodyPart => bodyPart.HpCurrent < bodyPart.HpMax));

            while (SatiationSimple > 0 && ouchies.Count > 0)
			{
                currentPart = ouchies.Dequeue();

                currentPart.HpCurrent += HealRate; //TODO: Remainder
                SatiationSimple -= HealRate;

                if (SatiationSimple <= 0)
                    SatiationSimple = 0;
            }
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