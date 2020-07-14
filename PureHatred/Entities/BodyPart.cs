using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

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

		public int Dexterity = 1;
		public int Striking = 1;
		public int Stability = 1;

		public int DecompositionTimer = 10;

		//TODO: Consider Enum PartType to streamline biorhythms

		public int ContentsComplex; // Stand-in for Complex Nutrients
		public int ContentsSimple; // Stand-in for Simple Nutrients
									// Both are combined to fill Capacity, but could also represent blood, bile, etc depending on organ context.

		public int ValuePerBiteComplex = 25;
		public int ValuePerBiteSimple = 25;
		public int Metabolism; // Mouth: BiteSize

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
			int bitesTaken = Math.Min(Metabolism, target.HpCurrent);
			
			target.HpCurrent -= bitesTaken;

			if (target.HpCurrent == 0)
				target.Delete($"{target.Name} was devoured.");

			owner.Stomach.ContentsComplex += target.ValuePerBiteComplex * bitesTaken;
			owner.Stomach.ContentsSimple += target.ValuePerBiteSimple * bitesTaken;

			return true;
		}

		public void StomachDigestion()
		{
			if (ContentsSimple > 0) // Carbs absorbed in UDT
			{
				ContentsSimple -= Metabolism * 3 / 2;
				owner.SatiationSimple += Metabolism * 3 / 2;
				owner.Intestines.ContentsSimple += Metabolism * 3 / 4;
			}
			if (ContentsComplex > 0) // Complex nuts passed to LDT
			{
				ContentsComplex -= Metabolism;
				owner.Intestines.ContentsComplex += Metabolism;
			}
		}

		public void IntestinalDigestion()
		{
			if (ContentsComplex > 0) // LDT processes complex nuts
			{
				ContentsComplex -= Metabolism;
				owner.SatiationComplex += Metabolism;
				ContentsSimple += Metabolism * 3 / 4; //feces
			}
		}

		public void VoluntaryDefecation()
		{
			if (ContentsSimple > 50) // Double-gate this to allow this to work for vol and invol shidding
			{
				GameLoop.UIManager.MessageLog.AddTextNewline("You fard and shid and look proudly upon your creation.");

				Decal shid = new Decal(Color.SaddleBrown, Color.Transparent, "shid", 258)
				{ Position = owner.Position };
				GameLoop.World.CurrentMap.Add(shid);

				ContentsSimple -= 50;
			}
			else
			{
				GameLoop.UIManager.MessageLog.AddTextNewline("You try to shid but only fard; you butt has bled, you pushed so hard.");

				Decal blood = new Decal(Color.DarkRed, Color.Transparent, "blood", 258)
				{ Position = owner.Position };
				GameLoop.World.CurrentMap.Add(blood);

				HpCurrent--;
			}

			GameLoop.GSManager.turnTaken = true;
		}
	}
}
