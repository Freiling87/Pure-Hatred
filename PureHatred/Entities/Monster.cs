using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

// TODO: Set up JSONs for items, body parts, creatures

namespace PureHatred.Entities
{
    public class Monster : Actor
    {
        private Random rndNum = new Random();
        public string name = "Monster";

        public Monster(Color foreground, Color background) : base(foreground, background, 2)
        {
            //for (int i = 0; i < rndNum.Next(1, 2); i++)
            //AddLoot(new Item(Color.HotPink, Color.Transparent, "spork", 'L', 2, 50));

            BodyPart leg = new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", 256, 10, 10);
            BodyPart arm = new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", 256, 10, 10);

            AddBodyPart(arm);
            AddBodyPart(arm);
            AddBodyPart(leg);
            AddBodyPart(leg);
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", 3, 10, 10));
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "torso", 3, 10, 10));
        }

        private void AddLoot(Item item)
		{
            item.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
            Inventory.Add(item);
		}

        private void AddBodyPart(BodyPart bodypart)
		{
            bodypart.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
            Anatomy.Add(bodypart);
		}
    }
}
