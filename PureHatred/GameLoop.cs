using Microsoft.Xna.Framework;

using PureHatred.Commands;
using PureHatred.UI;
using System;

namespace PureHatred
{
	static class GameLoop
	{
		public const int GameWidth = 160;
		public const int GameHeight = 80;

		public static UIManager UIManager;
		public static GameStates GSManager;
		public static Populator World;

		public static Random rndNum = new Random();

		static void Main(string[] args)
		{
			SadConsole.Game.Create("Cheepicus_12x12.font", GameWidth, GameHeight);
			SadConsole.Game.OnInitialize = Init;
			SadConsole.Game.OnUpdate = Update;
			SadConsole.Game.Instance.Run();

			// Code below will not run until the game window closes.

			SadConsole.Game.Instance.Dispose();
		}

		private static void Update(GameTime time)
		{
		}

		private static void Init()
		{
			UIManager = new UIManager();
			GSManager = new GameStates();
			World = new Populator();

			UIManager.Init();
		}
	}
}
