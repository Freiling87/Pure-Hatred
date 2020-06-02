using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;

using PureHatred.Commands;
using PureHatred.Entities;
using PureHatred.UI;

using Console = SadConsole.Console;

namespace PureHatred
{
    class GameLoop
    {
        public const int GameWidth = 160;
        public const int GameHeight = 80;

        public static UIManager UIManager;
        public static CommandManager CommandManager;

        public static World World;

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
            World = new World();

            UIManager.Init();
        }
    }
}
