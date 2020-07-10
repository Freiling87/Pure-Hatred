using Microsoft.Xna.Framework;
using SadConsole.Components;

namespace PureHatred.Entities
{
    public abstract class Entity : SadConsole.Entities.Entity, GoRogue.IHasID
    {
		//private Color foreground;
		//private Color background;
		//private int glyph;

		public uint ID { get; set; }

        public Entity(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height)
        {
			Animation.CurrentFrame[0].Foreground = foreground;
			Animation.CurrentFrame[0].Background = background;
			Animation.CurrentFrame[0].Glyph = glyph;

			ID = Map.IDGenerator.UseID();

            // Ensure that the entity position/offset is tracked by scrollingconsoles
            Components.Add(new EntityViewSyncComponent());

            animation.UseMouse = false;
            UseMouse = false;
        }

		//public Entity(Color foreground, Color background, string name, int glyph) : base(width, height)
		//{
		//	this.foreground = foreground;
		//	this.background = background;
		//	Name = name;
		//	this.glyph = glyph;
		//}

		public virtual bool IsLastborn() =>
			true; // Entity can only serve as Core Node
	}
}
