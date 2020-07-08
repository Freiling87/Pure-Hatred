using PureHatred.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PureHatred.Commands
{
	public class Anatomy : List<BodyPart>
	//suggested (IEnumerable<BodyPart> parts) : base(parts) by VGist on C# Discord
	{
		public Actor _owner;

		public Anatomy(Actor owner) : base()
		{
			_owner = owner;
		}

		public void Reorder() 
		{
			Stack<BodyPart> stack = new Stack<BodyPart>();
			Anatomy result = new Anatomy(_owner);

			BodyPart root = _owner.Core;

			stack.Push(root);

			while (stack.Count > 0) 
			{
				BodyPart node = stack.Pop();

				result.Add(node);

				for (int i = node.children.Count - 1; i >= 0; i--)
					stack.Push((BodyPart)node.children[i]);
			}

			Clear();
			AddRange(result);
		}
	}
}