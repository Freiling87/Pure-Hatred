using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FuRL.Entities;
using FuRL.UI;
using FuRL.Commands;

namespace FuRL
{
    class GameLoop
    {

        public const int GameWidth = 80;
        public const int GameHeight = 25;

        // Managers
        public static UIManager UIManager;
        public static CommandManager CommandManager;

        public static World World;

        static void Main(string[] args)
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;
                        
            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //
            
            SadConsole.Game.Instance.Dispose();
        }
        
        private static void Update(GameTime time)
        {

        }

        private static void Init()
        {
            //Instantiate the UIManager
            UIManager = new UIManager();

            //Instantiate a new CommandManager
            CommandManager = new CommandManager();

            // Build the world!
            World = new World();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.Init();
        }
    }
}
