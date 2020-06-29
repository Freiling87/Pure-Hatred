using System;

using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Player : Actor
    {
        public Player(Color foreground, Color background) : base(foreground, background, 1)
        {
            Name = "A friendly Cherub";

            Attack = 5;
            AttackChance = 60;

            Defense = 5;
            DefenseChance = 40;
            
            Health = 5;

            NutComplex = 5;

            NutSimple = 5;

            GiveHumanoidParts();
        }
    }
}