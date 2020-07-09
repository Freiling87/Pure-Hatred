using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

// TODO: Set up JSONs for items, body parts, creatures

namespace PureHatred.Entities
{
    public class Monster : Actor
    {
        public Monster(Color foreground, Color background, string name = "Unnamed Monster") : base(foreground, background, 2, name)
        {
        }
    }
}
