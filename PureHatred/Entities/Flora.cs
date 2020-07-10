
using Microsoft.Xna.Framework;
using PureHatred.Entities.Interfaces;

namespace PureHatred.Entities
{
	public class Flora : Entity, IEdible
	{
		Flora (Color foreground, Color background, int glyph) : base(foreground, background, glyph)
		{

		}
	}
}
