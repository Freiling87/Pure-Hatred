using Microsoft.Xna.Framework;
using PureHatred.UI;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PureHatred.Entities
{
	public class BodyPart : Item
	{
		public int HpMax;
		public int HpCurrent;
		public int HungerComplex;
		public int HungerSimple;

		public int Size;
		public int Capacity;
		public int Trunks;
		public int Branches;

		public int Dexterity;
		public int Striking;
		public int Stability;

		public int DecompositionTimer; // Loop 10 turns or so? Or varied rate, who knows

		//TODO: Consider Enum PartType to streamline biorhythms

		public int ContentsComplex; // Stand-in for Complex Nutrients
		public int ContentsSimple; // Stand-in for Simple Nutrients
									// Both are combined to fill Capacity, but could also represent blood, bile, etc depending on organ context.

		public int ValuePerBiteComplex = 1;
		public int ValuePerBiteSimple = 1;

		public StringBuilder TierPrefix = new StringBuilder("");
		public int AnatomyTier = 0;

		public bool NodeVisited = false;

		public BodyPart(Color foreground, Color background, string name, int glyph, int hungerComplex, int hungerSimple, int hpMax = 10, int hpCurrent = 10, Actor owner=null) : base(foreground, background, name, glyph)
		{
			Name = name;

			HpCurrent = hpCurrent;
			HpMax = hpMax;

			HungerComplex = hungerComplex;
			HungerSimple = hungerSimple;
		}

		public void Delete(string message)
		{
			Map Map = GameLoop.World.CurrentMap;

			Map.Remove(this);
			GameLoop.World.CurrentMap.Entities.Remove(this);

			if (parent != null)
				parent.children.Remove(this);
			
			foreach (BodyPart child in children)
				child.parent = null;

			GameLoop.UIManager.MessageLog.AddTextNewline(message);
		}

		public void Decompose()
		{
			if (HpCurrent-- == 0)
				Delete($"{Name} decomposed into thin air.");
		}

		public bool Masticate(BodyPart target)
		{
			if (target.HpCurrent-- <= 0)
				target.Delete($"{target.Name} was devoured.");

			owner.Stomach.ContentsComplex += target.ValuePerBiteComplex;
			owner.Stomach.ContentsSimple += target.ValuePerBiteSimple;

			GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

			return true;
		}

		public void StomachDigestion()
		{
			if (ContentsComplex > 0)
			{
				ContentsComplex--;
				owner.Intestines.ContentsComplex++;
			}

			if (ContentsSimple > 0)
			{
				ContentsSimple--;
				owner.Intestines.ContentsSimple++;
			}
		}

		public void IntestinalDigestion()
		{
			// TODO: Add excretion

			if (ContentsComplex > 0)
			{
				ContentsComplex--;
				owner.NutComplex++;
			}

			if (ContentsSimple > 0)
			{
				ContentsSimple++;
				owner.NutSimple++;
			}
		}	
	}
}
