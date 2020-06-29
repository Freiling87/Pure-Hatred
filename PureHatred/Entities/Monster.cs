using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

// TODO: Set up JSONs for items, body parts, creatures

namespace PureHatred.Entities
{
    public class Monster : Actor
    {
        private readonly Random rndNum = new Random();

        public Monster(Color foreground, Color background) : base(foreground, background, 2)
        {

        }
    }
}
