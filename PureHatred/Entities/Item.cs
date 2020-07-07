using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.ComponentModel;

namespace PureHatred.Entities
{
    public class Item : Entity
    {
        public bool isNodeExpanded;
        public Item parent;
        public List<Item> children = new List<Item>();
        public Actor owner; // TODO: Owner should just be a hidden Core node

        public Item(Color foreground, Color background, string name, int glyph) : base(foreground, background, glyph)
        {
            Name = name;
        }

        public void Destroy() =>
            GameLoop.World.CurrentMap.Remove(this);

        public Entity GetAncestor(int generations) // 0 returns self
		{
            Item ancestor = this;

            for (int i = 0; i < generations; i++)
                ancestor = ancestor.parent;

            return ancestor;
        }

        public Actor GetOwner() =>
            (Actor)GetAncestor(CountParents());

        public bool IsLastBorn() =>
            parent == null || this == parent.children[^1];
            // Above notation returns true if true, rest if false

        public int CountParents() =>
            (parent != null) ? 1 + parent.CountParents() : 0;
    }
}
