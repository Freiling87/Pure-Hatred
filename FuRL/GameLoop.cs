using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;

using FuRL.Commands;
using FuRL.Entities;
using FuRL.UI;

using Console = SadConsole.Console;

namespace FuRL
{
    class GameLoop
    {
        public const int GameWidth = 240;
        public const int GameHeight = 60;

        public static UIManager UIManager;
        public static CommandManager CommandManager;

        public static World World;

        static void Main(string[] args)
        {
            SadConsole.Game.Create(GameWidth, GameHeight);
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
