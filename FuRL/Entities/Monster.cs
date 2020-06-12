using System;

using Microsoft.Xna.Framework;

// TODO: Set up JSONs for items, body parts, creatures

namespace PureHatred.Entities
{
    public class Monster : Actor
    {
        private Random rndNum = new Random();

        public Monster(Color foreground, Color background) : base(foreground, background, 2)
        {
            //for (int i = 0; i < rndNum.Next(1, 2); i++)
                //AddLoot(new Item(Color.HotPink, Color.Transparent, "spork", 'L', 2, 50));

            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", 256, 10, 10));
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", 257, 10, 10));
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", 256, 10, 10));
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", 257, 10, 10));
            AddBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", 3, 10, 10));
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
