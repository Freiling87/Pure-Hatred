using System;

using Microsoft.Xna.Framework;

namespace FuRL.Tiles
{
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false) : base(Color.Transparent, Color.DarkGray, ' ', blocksMovement, blocksLOS)
        {
            Name = "Floor";
        }
    }
}
