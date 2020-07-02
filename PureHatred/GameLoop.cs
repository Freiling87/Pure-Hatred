using Microsoft.Xna.Framework;

using PureHatred.Commands;
using PureHatred.UI;

namespace PureHatred
{
	class GameLoop
	{
		public const int GameWidth = 160;
		public const int GameHeight = 80;

		public static UIManager UIManager;
		public static CommandManager CommandManager;
		public static SpatialWorld World;

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
			CommandManager = new CommandManager();
			World = new SpatialWorld();

			UIManager.Init();
		}

		private static void PassTurn()
		{

		}
	}
}
