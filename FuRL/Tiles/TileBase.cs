using System;

using Microsoft.Xna.Framework;
using SadConsole;

namespace PureHatred
{
    public abstract class TileBase : Cell
    {
        public bool IsBlockingMove;
        public bool IsBlockingLOS;
        public string Name;

        public TileBase(Color foreground, Color background, int glyph, bool blockingMove = false, bool blockingLOS = false, String name = "") : base(foreground, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = blockingLOS;
            Name = name;
        }
    }
}
