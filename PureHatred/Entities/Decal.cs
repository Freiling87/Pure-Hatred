using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Decal : Entity
	{
		new public readonly int renderOrder = (int)RenderOrder.Decal;

		public Decal(Color foreground, Color background, string name, int glyph) : base(foreground, background, glyph)
		{
			Name = name;
		}
	}
}
