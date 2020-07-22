using System;
using System.Collections.Generic;
using System.Text;

namespace PureHatred.Entities.Components
{
	public class Spritesheet
	{
		public enum Anatomy : int
		{
			Arm = 288,
			Leg = 289,
			Head = 290,
			Spine = 291,
			Torso = 292,
			Beanus = 293,
			Intestines = 294,
			Eyeball = 295,
			Stomach = 296,
			Lung = 297,
			Trachea = 298,
			Mouth = 299,
			Brain = 300,

		}
		public enum Flora : int
		{
			Spindly = 304,
			Robust = 305,
			Small = 306,
			Slime = 307,
			Miasmic = 308,
			Bioluminescent = 309,
			Flammable = 310,
			Poisonous = 311
		}
	}
}