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

        public BodyPart Core;
        public BodyPart Intestines; //Checks for nutrients to process into system
        public BodyPart Mouth;
        public BodyPart Stomach;

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
            anatomy = new Anatomy(this);
            inventory = new Inventory(this);

            HardCodeHumanParts();
        }

        public void HardCodeHumanParts()
		{
            // These are rudimentary demo parts to get the Anatomy Window working correctly.
            BodyPart Core = GraftBodyPart(new BodyPart(Color.OldLace, Color.Transparent, "spine", 'I', 1, 0), null);
			BodyPart torso = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "torso", '@', 25, 15), Core);
			BodyPart leg1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart leg2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart arm1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
			BodyPart arm2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
            BodyPart beanus = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "beanus", ',', 1, 1), torso);

			BodyPart neck = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "neck", 'i', 1, 5), Core);
			BodyPart head = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", 'O', 10, 20), neck);
            BodyPart trachea = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "trachea", 'j', 0, 1), neck);
            BodyPart brain = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "brain", '@', 10, 40), head);
            BodyPart eye1 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
            BodyPart eye2 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
            BodyPart mouth = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "mouth", 'D', 0, 1, 5, 5), head); 

			BodyPart stomach = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", '§', 10, 10), torso); // includes Duodenum, Spleen, etc.
			BodyPart intestines = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", 'G', 5, 0), stomach);
            BodyPart poop = GraftBodyPart(new BodyPart(Color.RosyBrown, Color.Transparent, "yummy poop", '-', 0, 0), intestines);

			BodyPart lung1 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'd', 0, 0), torso);
			BodyPart lung2 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'b', 0, 0), torso);

            BodyPart tail = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "tail", 'S', 2, 5), torso);
            BodyPart stinger = GraftBodyPart(new BodyPart(Color.Black, Color.Transparent, "stinger", ',', 1, 0), tail);

            Stomach = stomach;
			Intestines = intestines;
            Mouth = mouth;

            inventory.Reorder();
		}

        private Item AddLoot(Item item)
        {
            inventory.Add(item);

            return item;
        }

        public BodyPart GraftBodyPart(BodyPart target, BodyPart parent = null)
        {
            //if (RecalculateBodyPart((BodyPart)bodyPart.parent)); //Cast to BodyPart or it takes it as Item

            anatomy.Add(target);
            target.owner = this;

            if (parent != null)
			{
                target.parent = parent;
                parent.children.Add(target);
			}

            NetBiologyValues();

            if (GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
                GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            RecalcNodeCapacities(target, parent);

            return target;
        }

        private BodyPart SeverBodyPart(BodyPart target, BodyPart parent = null)
		{
            anatomy.Remove(target);

            if (target.children.Count != 0)
                foreach (BodyPart bodyPart in target.children)
                    anatomy.Remove(bodyPart);

            NetBiologyValues();

            if (GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
                GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            RecalcNodeCapacities(parent);

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

        public void RecalcNodeCapacities(params BodyPart[] list)
		{
            /* Recalculate Trunk/Branch space with existing grafts
             */

            foreach (BodyPart bodyPart in list)
			{

			}

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