using Microsoft.Xna.Framework;
using System.Collections.Generic;

// TODO: Many of these should only be recalculated when the Item is grafted/removed, etc.

namespace PureHatred.Entities
{
    public class Item : Entity
    {
        new public readonly int renderOrder = (int)RenderOrder.Item;

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

        //ANCESTORS

        public Actor GetOwner() =>
            (Actor)GetAncestor(CountAncestors() + 1);

        public int CountAncestors() =>
            (parent != null) ? 1 + parent.CountAncestors() : 0;

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

            while (context.parent != null) //For Leg, added Torso twice
			{
                ancestry.Add(context.parent);
                context = context.parent;
			}

            return ancestry;
		}

        //SIBLINGS

        public override bool IsLastborn() =>
            parent == null || this == parent.children[^1];

        //DESCENDANTS

        public int CountDescendants() =>
            //TODO
            0;

        public List<Item> GetDescendants()
		{
            List<Item> descendants = new List<Item>();

            // TODO

            return descendants;
		}

        //NODES

        public bool IsNodeVisible() // TODO: Can Linq a oneliner here
        {
            foreach (Item parent in GetAncestors())
                if (!parent.isNodeExpanded)
                    return false;
            return true;
        }
    }
}
