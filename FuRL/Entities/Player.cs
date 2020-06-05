﻿using System;

using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Player : Actor
    {
        public Player(Color foreground, Color background) : base(foreground, background, 1)
        {
            Attack = 5;
            AttackChance = 60;
            Defense = 5;
            DefenseChance = 40;
            Name = "Milhouse";
        }
    }
}