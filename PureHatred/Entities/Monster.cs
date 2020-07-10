using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
    public class Monster : Actor
    {
        public Monster(Color foreground, Color background, string name = "Unnamed Monster") : base(foreground, background, 2, name)
        {
        }
    }
}
