using PureHatred.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PureHatred.Behaviors
{
	class BioRhythm
	{
		private int hungerSimple;
		private int hungerComplex;

		public void Alimentation(Actor actor)
		{
			hungerSimple = 0;
			hungerComplex = 0;

			foreach (BodyPart bodyPart in actor.Anatomy)
			{
				hungerSimple += bodyPart.HungerSimple;
				hungerComplex += bodyPart.HungerComplex;
			}

		}
	}
}
