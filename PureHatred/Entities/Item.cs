using Microsoft.Xna.Framework;
using PureHatred.Entities.Interfaces;
using System.Collections.Generic;

namespace PureHatred.Entities
{
    public class Item : Entity, INode
    {
        public bool isNodeExpanded { get; set; }
        public INode parent { get; set; }
        public List<INode> children { get; set; }
        public Actor owner { get; set; } // TODO: Owner should just be a hidden Core node

        public Item(Color foreground, Color background, string name, int glyph) : base(foreground, background, glyph)
        {
            Name = name;
            isNodeExpanded = true;
        }

        public void Destroy() =>
            GameLoop.World.CurrentMap.Remove(this);
    }
}
