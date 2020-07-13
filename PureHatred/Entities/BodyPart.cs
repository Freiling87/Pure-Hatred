﻿using System;
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
			if (ContentsComplex > 0)
			{
				ContentsComplex -= Metabolism;
				owner.Intestines.ContentsComplex += Metabolism;
			}

			if (ContentsSimple > 0)
			{
				ContentsSimple -= Metabolism;
				owner.Intestines.ContentsSimple += Metabolism;
			}
		}

		public void IntestinalDigestion()
		{
			// TODO: Add excretion

			if (ContentsComplex > 0)
			{
				ContentsComplex -= Metabolism;
				owner.SatiationComplex += Metabolism;
			}

			if (ContentsSimple > 0)
			{
				ContentsSimple -= Metabolism;
				owner.SatiationSimple += Metabolism;
			}
		}	
	}
}
