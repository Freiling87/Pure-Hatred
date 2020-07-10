using PureHatred.Entities;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using PureHatred.Entities.Interfaces;

namespace PureHatred.Commands
{
	public class Anatomy : List<BodyPart>
	//suggested (IEnumerable<BodyPart> parts) : base(parts) by VGist on C# Discord
	{
		public Actor _owner;
		private bool preCreation = true;

		public Anatomy(Actor owner) : base()
		{
			_owner = owner;
		}

		public void HardCodeHumanParts()
		{
			// These are rudimentary demo parts to get the Anatomy Window working correctly.
			_owner.Core = GraftBodyPart(new BodyPart(Color.OldLace, Color.Transparent, "spine", 'I', 1, 0), null);
			BodyPart torso = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "torso", '@', 25, 15), _owner.Core);
			BodyPart leg1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart leg2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "leg", '@', 5, 10), torso);
			BodyPart arm1 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
			BodyPart arm2 = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "arm", '@', 5, 10), torso);
			BodyPart beanus = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "beanus", ',', 1, 1), torso);

			BodyPart neck = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "neck", 'i', 1, 5), _owner.Core);
			BodyPart head = GraftBodyPart(new BodyPart(Color.LightSeaGreen, Color.Transparent, "head", 'O', 10, 20), neck);
			BodyPart trachea = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "trachea", 'j', 0, 1), neck);
			_owner.Brain = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "brain", '@', 10, 40), head);
			BodyPart eye1 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
			BodyPart eye2 = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
			_owner.Mouth = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "mouth", 'D', 0, 1, 5, 5), head);

			_owner.Stomach = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", '§', 10, 10), torso); // includes Duodenum, Spleen, etc.
			_owner.Intestines = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", 'G', 5, 0), _owner.Stomach);
			BodyPart lung1 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'd', 0, 0), torso);
			BodyPart lung2 = GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'b', 0, 0), torso);

			_owner.Mouth.Metabolism = 3;
			_owner.Stomach.Metabolism = 2;
			_owner.Intestines.Metabolism = 1;

			preCreation = false;
			_owner.NetBiologyValues();
			Reorder();
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

		public BodyPart GraftBodyPart(BodyPart newChild, BodyPart newParent)
		{
			Add(newChild);
			newChild.owner = _owner; //TODO: Change to owner of core

			if (newParent != null)
			{
				newChild.parent = newParent;
				newParent.children.Add(newChild);
			}

			if (!preCreation)
			{
				_owner.NetBiologyValues();
				RecalcNodeCapacities(newChild, newParent);
				if (newChild.owner == GameLoop.World.Player && GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
					GameLoop.UIManager.StatusWindow.UpdateStatusWindow();
			}

			return newChild;
		}

		private BodyPart SeverBodyPart(BodyPart target, BodyPart parent = null)
		{
			Remove(target);

			if (target.children.Count != 0)
				foreach (BodyPart bodyPart in target.children)
					Remove(bodyPart);

			_owner.NetBiologyValues();

			if (GameLoop.UIManager.StatusWindow != null) // Allows for pre-game creation
				GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

			RecalcNodeCapacities(parent);

			return target;
		}

		public void RecalcNodeCapacities(params BodyPart[] list)
		{
			/* Recalculate Trunk/Branch space with existing grafts
             */

			foreach (BodyPart bodyPart in list)
			{

			}

		}

		public bool Pickup(Actor actor, Item item)
		{
			actor.inventory.Add(item);
			GameLoop.UIManager.SideWindow.InventoryList();
			GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} picked up {item.Name}");
			item.Destroy();
			return true;
		}

		public bool Drop(Item item)
		{
			item.owner.inventory.Remove(item);
			GameLoop.UIManager.SideWindow.InventoryList();
			GameLoop.UIManager.MessageLog.AddTextNewline($"{item.owner.Name} dropped {item.Name}");
			//item.Destroy() opposite?
			return true;
		}
		public bool Drop(BodyPart bodyPart)
		{
			bodyPart.owner.anatomy.Remove(bodyPart);
			GameLoop.UIManager.SideWindow.InventoryList();
			GameLoop.UIManager.MessageLog.AddTextNewline($"{bodyPart.owner.Name}'s {bodyPart.Name} was severed");
			//item.Destroy() oposite?
			return true;
		}
	}
}