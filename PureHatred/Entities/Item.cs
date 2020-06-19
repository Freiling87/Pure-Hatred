﻿using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
    public class Item : Entity
    {
        private int _condition;
        public int Weight { get; set; } 

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

        public Item(Color foreground, Color background, string name, int glyph, int weight = 1, int condition = 100) : base(foreground, background, glyph)
        {
            //Animation.CurrentFrame[0].Foreground = foreground;
            //Animation.CurrentFrame[0].Background = background;
            //Animation.CurrentFrame[0].Glyph = glyph;
            // Per Thraka, not necessary. But when removed from Entity or Actor, they don't work properly. 
            // TODO: What's different about these two classes?
            // Might be because of the :base() in the instantiator above

            Weight = weight;
            Condition = condition;
            Name = name;
        }

        public void Destroy() =>
            GameLoop.World.CurrentMap.Remove(this);
    }
}
