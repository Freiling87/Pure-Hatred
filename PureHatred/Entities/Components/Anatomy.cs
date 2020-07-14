using System.Collections.Generic;

using Microsoft.Xna.Framework;

using PureHatred.Entities;

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
			BodyPart torso = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "torso", '@', 25, 15), _owner.Core);
			GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "leg", 257, 5, 10), torso);
			GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "leg", 257, 5, 10), torso);
			GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "arm", 256, 5, 10), torso);
			GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "arm", 256, 5, 10), torso);
			GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "beanus", ',', 1, 1), torso);

			BodyPart neck = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "neck", 'i', 1, 5), _owner.Core);
			BodyPart head = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "head", 'O', 10, 20), neck);
			GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "trachea", 'j', 0, 1), neck);
			_owner.Brain = GraftBodyPart(new BodyPart(Color.LightPink, Color.Transparent, "brain", '@', 10, 40), head);
			GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
			GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", '.', 2, 1, 2, 2), head);
			_owner.Mouth = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "mouth", 'D', 0, 1, 5, 5), head);

			_owner.Stomach = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", '§', 10, 10), torso); // includes Duodenum, Spleen, etc.
			_owner.Intestines = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", 'G', 5, 0), _owner.Stomach);
			GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'd', 0, 0), torso);
			GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", 'b', 0, 0), torso);

			_owner.Mouth.Metabolism = 3;
			_owner.Stomach.Metabolism = 2;
			_owner.Intestines.Metabolism = 1;

			preCreation = false;

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

		public void RecalcNodeCapacities(params BodyPart[] list)
		{
			/* Recalculate Trunk/Branch space with existing grafts
             */

			foreach (BodyPart bodyPart in list)
			{

			}

		}

		public bool Pickup(Actor actor, BodyPart item)
		{
			actor.anatomy.Add(item);
			GameLoop.UIManager.SideWindow.RefreshAnatomy();
			GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} picked up {item.Name}");
			item.Destroy();
			return true;
		}

		public bool Drop(BodyPart bodyPart)
		{
			bodyPart.owner.anatomy.Remove(bodyPart);
			GameLoop.UIManager.SideWindow.RefreshAnatomy();
			GameLoop.UIManager.MessageLog.AddTextNewline($"{bodyPart.owner.Name}'s {bodyPart.Name} was severed");
			//item.Destroy() oposite?
			return true;
		}
	}
}