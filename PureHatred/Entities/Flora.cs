using Microsoft.Xna.Framework;

namespace PureHatred.Entities
{
	public class Flora : Entity
	{
		new public readonly int renderOrder = (int)RenderOrder.Flora;

		Flora(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
		{

		}
	}
}