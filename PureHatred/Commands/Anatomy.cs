using PureHatred.Entities;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using System.Linq;

namespace PureHatred.Commands
{
	public class Anatomy : List<BodyPart>
	{
		public Actor owner;

		public Anatomy(Actor owner) : base()
		{
		}
	}
}
