using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using PureHatred.Entities;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

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