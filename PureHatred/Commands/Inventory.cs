using PureHatred.Entities;
using System.Collections.Generic;

namespace PureHatred.Commands
{
	public class Inventory : List<Item>
	{
		public Actor owner;

		public Inventory(Actor owner) : base()
		{
		}
	}
}
