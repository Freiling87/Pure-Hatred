using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.ComponentModel;

namespace PureHatred.Entities
{
    public class Item : Entity
    {
        public bool isNodeExpanded = true;
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

        public List<Item> GetAncestors()
		{
            List<Item> ancestry = new List<Item>();
            Item context = this;

            while (context.parent != null)
			{
                ancestry.Add(parent);
                context = context.parent;
			}

            return ancestry;
		}

        public bool IsNodeVisible() // TODO: Can Linq a oneliner here
        {
            foreach (Item parent in GetAncestors())
                if (!parent.isNodeExpanded)
                    return false;
            return true;
        }

        public Actor GetOwner() =>
            (Actor)GetAncestor(CountParents() +1);

        public override bool IsLastborn() =>
            parent == null || this == parent.children[^1];
            // Above notation returns true if true, rest if false

        public int CountParents() =>
            (parent != null) ? 1 + parent.CountParents() : 0;

        // TODO: The above three should only be recalculated when the Item is grafted/removed, etc.
    }
}
