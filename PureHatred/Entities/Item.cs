

using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.ComponentModel;

namespace PureHatred.Entities
{
    public class Item : Entity
    {
        private int _condition;
        public int weight { get; set; }
        public bool isNodeExpanded;
        public Item parent;
        public List<Item> children = new List<Item>();
        public Actor owner; // TODO: Owner should just be a hidden Core node

        public int Condition
        {
            get { return _condition; }
            set
            {
                _condition += value;
                if (_condition <= 0)
                    Destroy();
            }
        }

        public Item(Color foreground, Color background, string name, int glyph, Item parent=null, int weight = 1, int condition = 100) : base(foreground, background, glyph)
        {
            //Animation.CurrentFrame[0].Foreground = foreground;
            //Animation.CurrentFrame[0].Background = background;
            //Animation.CurrentFrame[0].Glyph = glyph;
            // Per Thraka, not necessary. But when removed from Entity or Actor, they don't work properly. 
            // TODO: What's different about these two classes?
            // Might be because of the :base() in the instantiator above

            this.weight = weight;
            Condition = condition;
            Name = name;
        }

        public void Destroy() =>
            GameLoop.World.CurrentMap.Remove(this);

        public Entity getAncestor(int generations) // 0 returns self
		{
            Item ancestor = this;

            for (int i = 0; i < generations; i++)
                ancestor = ancestor.parent;

            return ancestor;
		}

        public bool isLastborn()
		{
            //TODO: For some reason it's always detecting parent as null...

            if (parent == null)
                return true;

            return this == parent.children[parent.children.Count - 1];
        }

        public int CountParents()
        {
            if (parent != null)
                return 1 + parent.CountParents();
            return 0;
        }

        public Actor GetOwner()
		{
            return (Actor)getAncestor(CountParents());
		}
    }
}
