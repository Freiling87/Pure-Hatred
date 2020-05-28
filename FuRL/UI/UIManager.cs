using System;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using FuRL.Entities;

/* TODO: Move calls to CenterOnPlayerActor to beginning of player turn, when gamestates are implemented
 * 
 */

namespace FuRL.UI
{
    public class UIManager : ContainerConsole
    {
        public SadConsole.ScrollingConsole MapConsole;
        public MessageLogWindow MessageLog;
        public Window MapWindow;

        public UIManager()
        {
            IsVisible = true;
            IsFocused = true;

            Parent = SadConsole.Global.CurrentScreen;
        }

        public void CreateChildConsoles()
        {


            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard(); // May turn this into a string that returns gamestates or generic commands
            base.Update(timeElapsed);
        }

        private void CheckKeyboard()
        {
            Player player = GameLoop.World.Player;
            bool playerMoved = false;

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
                SadConsole.Settings.ToggleFullScreen();

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                GameLoop.CommandManager.MoveActorBy(player, new Point(0, -1));
                playerMoved = true;
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                GameLoop.CommandManager.MoveActorBy(player, new Point(0, 1));
                playerMoved = true;
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                GameLoop.CommandManager.MoveActorBy(player, new Point(-1, 0));
                playerMoved = true;
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                GameLoop.CommandManager.MoveActorBy(player, new Point(1, 0));
                playerMoved = true;
            }

            if (playerMoved == true)
                MapConsole.CenterViewPortOnPoint(player.Position);
        }

        public void Init()
        {
            CreateChildConsoles();

            MessageLog = new MessageLogWindow(GameLoop.GameWidth, GameLoop.GameHeight / 4, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight * 3/4 );

            LoadMap(GameLoop.World.CurrentMap);
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight * 3/4, "Game Map");
            UseMouse = true;

            MapConsole.CenterViewPortOnPoint(GameLoop.World.Player.Position);
        }

        private void SyncMapEntities(Map map)
        {
            MapConsole.Children.Clear();

            foreach (Entity entity in map.Entities.Items)
            {
                MapConsole.Children.Add(entity);
            }

            // Subscribe to Entities listeners
            map.Entities.ItemAdded += OnMapEntityAdded;
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        public void OnMapEntityRemoved(object sender, GoRogue.ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Remove(args.Item);
        }

        public void OnMapEntityAdded(object sender, GoRogue.ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Add(args.Item);
        }

        private void LoadMap(Map map)
        {
            Map currentMap = GameLoop.World.CurrentMap;

            MapConsole = new SadConsole.ScrollingConsole(currentMap.Width, currentMap.Height, Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);

            SyncMapEntities(map);
        }

        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = false;

            // Trim to show the window title and borders, and position away from borders
            MapConsole.ViewPort = new Rectangle(0, 0, width - 2, height - 2);
            MapConsole.Position = new Point(1, 1);

            //Button closeButton = new Button(3, 1);
            //closeButton.Position = new Point(0, 0);
            //closeButton.Text = "[X]";
            //MapWindow.Add(closeButton);

            MapWindow.Title = title.Align(HorizontalAlignment.Center, width);

            MapWindow.Children.Add(MapConsole);
            Children.Add(MapWindow);
            MapWindow.Show();
        }
    }
}
