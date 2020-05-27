using System;

using Microsoft.Xna.Framework;

namespace FuRL.Entities
{
    public class Monster : Actor
    {
        private Random rndNum = new Random();

        public Monster(Color foreground, Color background) : base(foreground, background, 'M')
        {
            int lootNum = rndNum.Next(1, 4);

            for (int i = 0; i < lootNum; i++)
            {
                Item newLoot = new Item(Color.HotPink, Color.Transparent, "spork", 'L', 2, 50);
                newLoot.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
                Inventory.Add(newLoot);
            }
        }
    }
}
