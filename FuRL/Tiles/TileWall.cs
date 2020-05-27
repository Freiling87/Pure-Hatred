using System;

using Microsoft.Xna.Framework;

namespace FuRL.Tiles
{
    public class TileWall : TileBase
    {
        public TileWall(bool blocksMovement=true, bool blocksLOS=true) : base(Color.Transparent, Color.DarkSlateGray, ' ', blocksMovement, blocksLOS)
        {
            Name = "Wall";
        }
    }
}
