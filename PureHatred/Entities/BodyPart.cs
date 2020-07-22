using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using GoRogue.Random;

using PureHatred.UI;
using GoRogue;
using System.Linq;
using SadConsole.EasingFunctions;
using PureHatred.Entities.Components;

/*TODO: Event handlers:
 * - On move, move descendants
 * 
 */

namespace PureHatred.Entities
{
	public class BodyPart : Item
	{
		new public readonly int renderOrder = (int)RenderOrder.BodyPart;

		//JSON Stuff
		public string foreground { get; set; }
		public string background { get; set; }
		public string name { get; set; }
		public int glyph { get; set; }
		public int hungerComplex { get; set; }
		public int hungerSimple { get; set; }
		public int hpMax { get; set; }
		public int hpCurrent { get; set; }

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

		public int ContentsComplex; 
		public int ContentsSimple; // Both are combined to fill Capacity, but could also represent blood, bile, etc depending on organ context.

		public int ValuePerBiteComplex = 25;
		public int ValuePerBiteSimple = 25;
		public int Metabolism; // Mouth: BiteSize

		public BodyPart(Color foreground, Color background, string name, Spritesheet.Anatomy sprite, int hungerComplex = 10, int hungerSimple = 10, int hpMax = 10, int hpCurrent = 10, Actor owner=null) : base(foreground, background, name, (int)sprite)
		{
			//TODO: Change glyph from int to Spritesheet type, then can eliminate int casting and 
		}

		public void Delete(string message)
		{
			Map Map = GameLoop.World.CurrentMap;

			Map.Remove(this);
			GameLoop.World.CurrentMap.Remove(this);

			if (parent != null)
				parent.children.Remove(this);

			foreach (BodyPart child in children)
				child.parent = null;

			GameLoop.UIManager.MessageLog.AddTextNewline(message);
		}

		public void Decomposition()
		{
			if (HpCurrent-- == 0)
				Delete($"{Name} decomposed into thin air.");
		}

		public bool Mastication(BodyPart target)
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
			/* Bug found: 
			 * First round of devour -125, then -225, -275, 
			*/				

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
			
			while (ContentsSimple > 250)
				Defecation(false);
		}

		public void Defecation(bool isVoluntary)
		{
			if (ContentsSimple > 50) // Double-gate this to allow this to work for vol and invol shidding
			{
				if (isVoluntary)
					GameLoop.UIManager.MessageLog.AddTextNewline("You fard and shid and look proudly upon your creation.");
				else
					GameLoop.UIManager.MessageLog.AddTextNewline("You can't wait any longer an you fard an you shid everywhere");

				Decal shid = new Decal(Color.SaddleBrown, Color.Transparent, "shid", 258)
				{ Position = owner.Position };
				GameLoop.World.CurrentMap.Add(shid);

				ContentsSimple -= GameLoop.rndNum.Next(50, 100);
			}
			else
			{
				GameLoop.UIManager.MessageLog.AddTextNewline("You try to shid but only fard; your butt has bled, you pushed so hard.");

				GameLoop.World.CurrentMap.BloodSplatter(owner.Position, 0);

				HpCurrent--;
			}

			if (isVoluntary)
				GameLoop.GSManager.turnTaken = true;
		}

		public Coord RandomWalkableTileInCircle(Point origin, int radius)
		{
			// TODO: Move this to different class
			// TODO: Add POV limitations

			List<Coord> circle = new RadiusAreaProvider(origin, radius, Radius.CIRCLE).CalculatePositions().ToList();

			Point newLocation = circle.RandomItem();

			while (!GameLoop.World.CurrentMap.IsTileWalkable(newLocation))
				newLocation = circle.RandomItem();

			return newLocation;
		}

		public void Severance(int velocity)
		{
			List<BodyPart> severedAndDescendants = new List<BodyPart>() { this };
			severedAndDescendants.Concat(GetDescendants());

			Point newLocation = RandomWalkableTileInCircle(owner.Position, velocity);

			foreach (BodyPart bodyPart in severedAndDescendants)
			{
				owner.anatomy.Remove(bodyPart);
				bodyPart.Position = newLocation;
			}
			GameLoop.World.CurrentMap.Add(this);
			GameLoop.World.CurrentMap.BloodSplatter(newLocation, velocity);

			owner.anatomy.RecalcNodeCapacities((BodyPart)parent);
			owner.NetBiologyValues();

			if (GameLoop.UIManager.StatusWindow != null && // Allows for pre-game creation
				owner == GameLoop.World.Player)
				GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

			GameLoop.UIManager.MessageLog.AddTextNewline($"{Name} is severed, flying away in a bloody arc!");
		}
	}
}