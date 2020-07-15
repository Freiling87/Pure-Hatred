using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace PureHatred.Tiles
{
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false, int glyph=(int)FontKey.RoughGround) : base(Color.LightGray, Color.DarkGray, glyph, blocksMovement, blocksLOS)
        {
            Name = "Floor";

            Glyph += GameLoop.rndNum.Next(0, 3);
		}
    }
}
