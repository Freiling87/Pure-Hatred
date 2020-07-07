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

		//public void Reorder()
		//{
		//	//Haven't tested this yet.
		//	List<Item> result = new List<Item>();
		//	Stack<Item> stack = new Stack<Item>();

		//	Item root = owner.Core;

		//	stack.Push(root);

		//	while (stack.Count > 0)
		//	{
		//		Item node = stack.Pop();

		//		result.Add(node);

		//		for (int i = node.children.Count - 1; i >= 0; i--)
		//			stack.Push((Item)node.children[i]);
		//	}

		//	owner.inventory = (Inventory)result;
		//}
	}
}
