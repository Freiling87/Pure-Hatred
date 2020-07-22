using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PureHatred.Entities;
using PureHatred.Entities.Components;

namespace PureHatred.Commands
{
	public partial class Anatomy : List<BodyPart>
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
			_owner.Core = GraftBodyPart(new BodyPart(Color.OldLace, Color.Transparent, "spine", Spritesheet.Anatomy.Spine), null);
			BodyPart torso = GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "torso", Spritesheet.Anatomy.Torso), _owner.Core);
			GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "leg", Spritesheet.Anatomy.Leg), torso);
			GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "leg", Spritesheet.Anatomy.Leg), torso);
			GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "arm", Spritesheet.Anatomy.Arm), torso);
			GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "arm", Spritesheet.Anatomy.Arm), torso);
			GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "beanus", Spritesheet.Anatomy.Beanus), torso);

			BodyPart neck = GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "neck", Spritesheet.Anatomy.Trachea), _owner.Core);
			BodyPart head = GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "head", Spritesheet.Anatomy.Head, 10, 20), neck);
			GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "trachea", Spritesheet.Anatomy.Trachea), neck);
			_owner.Brain = GraftBodyPart(new BodyPart(Color.PeachPuff, Color.Transparent, "brain", Spritesheet.Anatomy.Brain), head);
			GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", Spritesheet.Anatomy.Eyeball), head);
			GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "eyeball", Spritesheet.Anatomy.Eyeball), head);
			_owner.Mouth = GraftBodyPart(new BodyPart(Color.White, Color.Transparent, "mouth", Spritesheet.Anatomy.Mouth), head);

			_owner.Stomach = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "stomach", Spritesheet.Anatomy.Stomach), torso); // includes Duodenum, Spleen, etc.
			_owner.Intestines = GraftBodyPart(new BodyPart(Color.DarkRed, Color.Transparent, "intestines", Spritesheet.Anatomy.Intestines), _owner.Stomach);
			GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", Spritesheet.Anatomy.Lung), torso);
			GraftBodyPart(new BodyPart(Color.AliceBlue, Color.Transparent, "lung", Spritesheet.Anatomy.Lung), torso);

			_owner.Mouth.Metabolism = 3;
			_owner.Stomach.Metabolism = 2;
			_owner.Intestines.Metabolism = 1;

			//string jsonString = File.ReadAllText("Entities\\BodyParts.json");
			//BodyPart myDeserializedClass = JsonConvert.DeserializeObject<BodyPart>(jsonString);
			// This one might work if JSON covers all public properties, according to C# discord
			// Build a BodyPart2 class from scratch and see what's causing the issue!

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