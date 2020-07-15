using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
    public class Npc : Actor
    {
        new public readonly int renderOrder = (int)RenderOrder.Monster;

        public Npc(Color foreground, Color background, string name = "Unnamed Monster") : base(foreground, background, 2, name)
        {
        }
    }
}
