using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace PureHatred.Tiles
{
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false) : base(Color.LightBlue, Color.DarkGray, ' ', blocksMovement, blocksLOS)
        {
            Name = "Floor";
		}
    }
}
