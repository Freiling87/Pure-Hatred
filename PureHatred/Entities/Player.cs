using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Player : Actor
    {
        new public readonly int renderOrder = (int)RenderOrder.Player;

        public Player(Color foreground, Color background, string name= "Cherub Bully") : base(foreground, background, 1, name)
        {
        }
    }
}