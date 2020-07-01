using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;

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
        public List<Item> Inventory = new List<Item>();
        public List<BodyPart> Anatomy = new List<BodyPart>();
        private List<BodyPart> Stomachs = new List<BodyPart>();
        private List<BodyPart> Intestines = new List<BodyPart>();

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background, glyph, width, height)
        {
            HardCodeHumanParts();
        }

        public void HardCodeHumanParts()
		{
            BodyPart torso = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "torso", '@', 25, 15, null));
            BodyPart leg1 = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
            BodyPart leg2 = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
            BodyPart arm1 = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
            BodyPart arm2 = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
            BodyPart head = AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", '@', 10, 20), torso);
            BodyPart stomach = AddBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", '§', 10, 10, torso));
            BodyPart intestines = AddBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", 'G', 5, 0), stomach);

            Stomachs.Add(stomach);
            Intestines.Add(intestines);
        }

        private Item AddLoot(Item item)
        {
            Inventory.Add(item);

            return item;
        }

        private BodyPart AddBodyPart(BodyPart child, BodyPart parent = null)
        {
            //if (RecalculateBodyPart((BodyPart)bodyPart.parent)); //Cast to BodyPart or it takes it as Item

            Anatomy.Add(child);

            RecalculateBiology();

            RecalculateBodyParts(child, parent);

            return child;
        }

        private void EatAThing(Entity entity)
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

        public void RecalculateBiology()
		{
            int simpleNeed = 0;
            int complexNeed = 0;
            int currentHp = 0;
            int maxHp = 0;

            foreach (BodyPart bodyPart in Anatomy)
			{
                complexNeed += bodyPart.HungerComplex;
                simpleNeed += bodyPart.HungerSimple;
                currentHp += bodyPart.HpCurrent;
                maxHp += bodyPart.HpMax;
			}

            NutComplexMax = complexNeed;
            NutSimpleMax = simpleNeed;
            Health = currentHp;
            HealthMax = maxHp;

            // Doesn't exist yet
            if (GameLoop.UIManager.StatusWindow != null)
                GameLoop.UIManager.StatusWindow.UpdateStatusWindow();
		}

        public void RecalculateBodyParts(params BodyPart[] list)
		{
            /* Recalculate Trunk/Branch space with existing grafts
             */

            foreach (BodyPart bodyPart in list)
			{

			}

		}
    }
}
