using System;

using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Player : Actor
    {
        public Player(Color foreground, Color background, string name="Player") : base(foreground, background, 1, name)
        {
            Name = "Cherub Bully";

            Attack = 5;
            AttackChance = 60;

            Defense = 5;
            DefenseChance = 40;
            
            Health = 5;

            NutComplex = 50;

            NutSimple = 500;
        }
    }
}