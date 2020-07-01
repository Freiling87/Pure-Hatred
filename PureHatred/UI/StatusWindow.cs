using System;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

using PureHatred.Entities;

/*
 * @Freiling  If you're just trying to draw on top of the progress bar, then either printing or using decorators would work. But you want to do this in the theme for the progress bar. Here is the progress bar theme code. You would want to use your own version of this and apply that theme to the progress bar.

https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ProgressBarTheme.cs#L103

This is the code that draws the progress bar based on the modes (horizontal vs vertical and also the alignment (fill direction) of those modes) and this is where you would print. The cell decorator is displayed on top of a cell, so the cell's fore/back/glyph are still drawn and then any decorator is layed on top of that. However, you have to do this cell-by-cell.

You want to use Surface.SetDecorator with the position of the cell you're updating and a count of 1. Count will apply that same decorator across adjacent cells but since you're probably printing text (such as 50%) you'll be working with different decorators (one for each character in that string) for each cell.

not frequently used and they are only a few versions old. Generally people don't have the need to just have a few cells with layers of glyphs.
Unless you're careful, you may end up with the decorators hanging around. If you zero out the cell by setting the glyph to 0 so that it's empty, the decorators remain until you remove them or clear the surface

 */


namespace PureHatred.UI
{
	public class StatusWindow : Window
	{
		private ProgressBar healthBar;
		private ProgressBar NutrientSimple;
		private ProgressBar NutrientComplex;
		private Label healthBarLabel;
		private Label NutrientComplexLabel;
		private Label NutrientSimpleLabel;
		//private Label healthOverlay;

		public StatusWindow (int width, int height) : base (width, height)
		{
			int i = 0;
			int indent = 1;
			int contentWidth = width - 2;

			healthBarLabel = new Label(contentWidth)
			{
				Position = new Point(indent, i++),
				DisplayText = "Health",
			};
			Add(healthBarLabel);

			healthBar = new ProgressBar(contentWidth, 1, HorizontalAlignment.Left)
			{
				Position = new Point(indent, i++),
			};
			Add(healthBar);

			NutrientComplexLabel = new Label(contentWidth)
			{
				Position = new Point(indent, i++),
				DisplayText = "Complex Nutrients",
			};
			Add(NutrientComplexLabel);

			NutrientComplex = new ProgressBar(contentWidth, 1, HorizontalAlignment.Left)
			{
				Position = new Point(indent, i++)
			};
			Add(NutrientComplex);

			NutrientSimpleLabel = new Label(contentWidth)
			{
				Position = new Point(indent, i++),
				DisplayText = "Simple Nutrients",
			};
			Add(NutrientSimpleLabel);

			NutrientSimple = new ProgressBar(contentWidth, 1, HorizontalAlignment.Left)
			{
				Position = new Point (indent, i++)
			};
			Add(NutrientSimple);

			ApplyColorTheme();

			UpdateStatusWindow();
		}

		public void ApplyColorTheme()
		{
			var colors = Colors.CreateDefault();

			colors.ControlBack = Color.Black;
			colors.Text = Color.White;

			colors.RebuildAppearances();

			int i = 0;

			foreach (ControlBase control in Controls)
				Controls[i++].ThemeColors = colors;
		}

		public void UpdateStatusWindow()
		{
			Player Player = GameLoop.World.Player;

			healthBar.Progress = (float)Player.Health / (float)Player.HealthMax;
			NutrientComplex.Progress = (float)Player.NutComplex / (float)Player.NutComplexMax;
			NutrientSimple.Progress = (float)Player.NutSimple / (float)Player.NutSimpleMax;
		}

		public override void Draw(TimeSpan drawTime) =>
			base.Draw(drawTime);

		public override void Update(TimeSpan time) =>
			base.Update(time);
	}
}