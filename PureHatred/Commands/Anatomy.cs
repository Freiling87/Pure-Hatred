using PureHatred.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PureHatred.Commands
{
	public class Anatomy : List<BodyPart>
	{
		private Actor _owner;

		public Anatomy(Actor owner) : base()
		{
			_owner = owner;
		}

		public void Reorder() 
		{
			List<BodyPart> result = new List<BodyPart>();
			Stack<BodyPart> stack = new Stack<BodyPart>(_owner.anatomy);

			BodyPart root = _owner.Core; // Core is null here, but anatomy above isn't. Core is not null before the function call.

			stack.Push(root);

			while (stack.Count > 0) 
			{
				BodyPart node = stack.Pop();

				result.Add(node);

				for (int i = node.children.Count - 1; i >= 0; i--)
					stack.Push((BodyPart)node.children[i]);
			}

			_owner.anatomy = (Anatomy)result;
		}
	}
}