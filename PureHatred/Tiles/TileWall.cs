﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace PureHatred.Tiles
{
    public class TileWall : TileBase
    {
        public TileWall(bool blocksMovement=true, bool blocksLOS=true) : base(Color.LightBlue, Color.DarkSlateGray, ' ', blocksMovement, blocksLOS)
        {
            Name = "Wall";

			//This doesn't work, would probably need to implement randomization downstream
			//var random = new Random();
			//var list = new List<char> { ' ', '#' };
			//Glyph = random.Next(list.Count);
		}
    }
}
