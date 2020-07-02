﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using PureHatred.Commands;
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
        public int NutSimpleMax { get; set; }
        public int NutSimple { get; set; }
        public int NutComplexMax { get; set; }
        public int NutComplex { get; set; }
        public int HealRate { get; set; } = 1;
        public int NutrientSurplus { get; set; } = 0;

        public List<BodyPart> Anatomy = new List<BodyPart>();
        public List<BodyPart> Intestines = new List<BodyPart>(); //Checks for nutrients to process into system
        public List<Item> Inventory = new List<Item>();
        public List<Mutation> Mutations = new List<Mutation>();
        public List<BodyPart> Stomachs = new List<BodyPart>(); //Checks for contents to process into nutrients

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
            HardCodeHumanParts();
        }

        public void HardCodeHumanParts()
		{
            BodyPart spine = GraftBodyPart(new BodyPart(Color.OldLace, Color.Transparent, "spine", 'I', 1, 0), null);
			BodyPart torso = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "torso", '@', 25, 15), spine);
			BodyPart leg1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart leg2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart arm1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
			BodyPart arm2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);

			BodyPart neck = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "neck", 'i', 1, 5), torso);
			BodyPart head = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", 'O', 10, 20), neck);
            BodyPart brain = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "brain", '@', 10, 40), head);
            BodyPart eye1 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
            BodyPart eye2 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);

			BodyPart stomach = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", '§', 10, 10), torso);
			BodyPart mushroom1 = GraftBodyPart(new BodyPart(Color.LightGray, Color.Transparent, "mushroom", 5, 0, 0), stomach);
			BodyPart mushroom2 = GraftBodyPart(new BodyPart(Color.LightGray, Color.Transparent, "mushroom", 5, 0, 0), stomach);
			BodyPart intestines = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", 'G', 5, 0), stomach);
			BodyPart fecalSlime = GraftBodyPart(new BodyPart(Color.SaddleBrown, Color.Transparent, "fecal slime", 'x', 0, 0), intestines);

			BodyPart lung1 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'd', 0, 0), torso);
			BodyPart lung2 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'b', 0, 0), torso);

			Stomachs.Add(stomach);
			Intestines.Add(intestines);
		}

        private Item AddLoot(Item item)
        {
            Inventory.Add(item);

            return item;
        }

        private BodyPart GraftBodyPart(BodyPart target, BodyPart parent = null)
        {
            //if (RecalculateBodyPart((BodyPart)bodyPart.parent)); //Cast to BodyPart or it takes it as Item

            Anatomy.Add(target);

            if (parent != null)
			{
                target.parent = parent;
                parent.children.Add(target);
			}

            NetBiologyValues();

            if (GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
                GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            RecalculateBodyParts(target, parent);

            return target;
        }

        private BodyPart SeverBodyPart(BodyPart target, BodyPart parent = null)
		{
            Anatomy.Remove(target);

            if (target.children.Count != 0)
                foreach (BodyPart bodyPart in target.children)
                    Anatomy.Remove(bodyPart);

            NetBiologyValues();

            if (GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
                GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            RecalculateBodyParts(parent);

            return target;
		}

        public bool MoveBy(Point positionChange)
        {
            Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(Position + positionChange);
            Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(Position + positionChange);
            BodyPart bodyPart = GameLoop.World.CurrentMap.GetEntityAt<BodyPart>(Position + positionChange);
            TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);
            TileWall wall = GameLoop.World.CurrentMap.GetTileAt<TileWall>(Position + positionChange);

            if (monster != null)
                return GameLoop.CommandManager.BumpAttack(this, monster);
            else if (bodyPart != null)
                return GameLoop.CommandManager.Devour(this, bodyPart);
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

        public void BioRhythm()
		{
            Alimentation();
		}

        public void NetBiologyValues()
		{
            NutComplexMax = Anatomy.Sum(x => x.HungerComplex);
            NutSimpleMax = Anatomy.Sum(x => x.HungerSimple);
            Health = Anatomy.Sum(x => x.HpCurrent);
            HealthMax = Anatomy.Sum(x => x.HpMax);

		}

        public void RecalculateBodyParts(params BodyPart[] list)
		{
            /* Recalculate Trunk/Branch space with existing grafts
             */

            foreach (BodyPart bodyPart in list)
			{

			}

		}

        public int Alimentation()
        {
            foreach (BodyPart bodyPart in Anatomy)
            {
                NutSimple -= bodyPart.HungerSimple;
                NutComplex -= bodyPart.HungerComplex;
            }

            NutrientSurplus += NutSimple;
            NutrientSurplus += NutComplex;

            return NutrientSurplus;
        }

        public void ProcessStomachContents(Actor actor)
        {
            //Break contents down slowly into Nutrients, pass to child Intestine
        }

        public void ProcessIntestineContents(Actor actor)
        {
            // Send contained Nutrients into system metabolism
        }

        public void HealWounds(Actor actor)
        {
            bool multiBreak = false;

            IEnumerable<BodyPart> ouchies = Anatomy.Where(bodyPart => bodyPart.HpCurrent < bodyPart.HpMax);

            for (int i = 0; i < actor.HealRate; i++) //Always heal same HP per nutrients, just at faster rate if possible
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
