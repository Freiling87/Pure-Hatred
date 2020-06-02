using System;

using Microsoft.Xna.Framework;

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

        public Item(Color foreground, Color background, string name, char glyph, int weight = 1, int condition = 100, int width = 1, int height = 1) : base(foreground, background, glyph)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            Weight = weight;
            Condition = condition;
            Name = name;
        }

        public void Destroy()
        {
            GameLoop.World.CurrentMap.Remove(this);
        }
    }
}
