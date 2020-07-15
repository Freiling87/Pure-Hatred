using Microsoft.Xna.Framework;
using SadConsole.Components;

namespace PureHatred.Entities
{
    public abstract class Entity : SadConsole.Entities.Entity, GoRogue.IHasID
    {
        public uint ID { get; set; }
        public readonly int renderOrder = (int)RenderOrder.Entity;

        public enum RenderOrder : int
		{
            Decal,
            Flora,
            BodyPart,
            Item,
            Actor,
            Monster,
            Player,
            Entity
		}

        protected Entity(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height)
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

		public virtual bool IsLastborn() =>
			true; // Entity can only serve as Core Node
	}
}
