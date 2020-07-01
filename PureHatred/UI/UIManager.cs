using System;
using System.Collections.Generic;
using System.Text;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;

using Console = SadConsole.Console;

/* TODO: Move calls to CenterOnPlayerActor to beginning of player turn, when gamestates are implemented
 * 
 */

namespace PureHatred.UI
{
    public class UIManager : ContainerConsole
    {
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public CollapsibleTreeWindow SideWindow;
        public MessageLogWindow MessageLog;
        public StatusWindow StatusWindow;

        public UIManager()
        {
            IsVisible = true;
            IsFocused = true;

            Parent = Global.CurrentScreen;
        }

		public void Init()
		{
			CreateWindows();

			UseMouse = true;
		}

		public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard(); 
            base.Update(timeElapsed);
        }

        public void CreateWindows()
		{
            int width = GameLoop.GameWidth;
            int height = GameLoop.GameHeight;

            MapConsole = new ScrollingConsole(width, height);
            LoadMap(GameLoop.World.CurrentMap);
            CreateMapWindow(width * 3 / 4, height * 3 / 4, "Map");

			SideWindow = new CollapsibleTreeWindow(width * 1 / 4, height * 3 / 4, "Inventory / Anatomy")
			{
				Position = new Point(width * 3 / 4, 0),
                CanDrag = false,
                UseMouse = true,
			};
			Children.Add(SideWindow);
            SideWindow.Show();

            MessageLog = new MessageLogWindow(width * 3 / 4, height * 1 / 4, "Log")
            {
                Position = new Point(0, height * 3 / 4),
                CanDrag = false,
                UseMouse = true,
            };
            Children.Add(MessageLog);
            MessageLog.Show();

            StatusWindow = new StatusWindow(width * 1 / 4, height * 1 / 4)
            {
                Position = new Point(width * 3 / 4, height * 3 / 4),
                CanDrag = false,
                UseMouse = true,
            };
            Children.Add(StatusWindow);
            StatusWindow.Show();
        }

        // INPUTS

        private static void Console_MouseMove(object sender, SadConsole.Input.MouseEventArgs e)
		{
            var console = (Console)sender;

            // Map Look by Mouseover
            StringBuilder seenString = new StringBuilder("You see:");

            TileBase seenTile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(e.MouseState.CellPosition);
            if (seenTile != null)
                seenString.Append($" {seenTile.Name},");

			List<Entity> seenEntities = GameLoop.World.CurrentMap.GetEntitiesAt<Entity>(e.MouseState.CellPosition);
			if (seenEntities != null)
				foreach (Entity entity in seenEntities)
					seenString.Append($" {entity.Name},");

            seenString.Remove(seenString.Length - 1, 1);                    //comma
            seenString.Append("                                         "); //overwrite
            console.Print(0, console.Height - 1, seenString.ToString());

            // Map _____ by Click
            //if (e.MouseState.Mouse.LeftButtonDown)
            //	console.Print(1, console.Height - 2, $"You've clicked on {e.MouseState.CellPosition}        ");
            //else
            //	console.Print(1, console.Height - 2, $"                                                           ");
        }

        private static void Console_MouseClicked(object sender, SadConsole.Input.MouseEventArgs e)//+
		{
			var console = (Console)sender;
			console.Print(0, console.Height - 2, $"You've clicked on {e.MouseState.CellPosition}               ");
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

        // MAP 

        public void CreateMapWindow(int width, int height, string title)
        {
			MapWindow = new Window(width, height)
            {
                CanDrag = false,
                Title = title.Align(HorizontalAlignment.Center, width)
            };

			MapConsole.ViewPort = new Rectangle(0, 0, width - 2, height - 2);
            MapConsole.Position = new Point(1, 1);

			Button inventoryButton = new Button(15, 3)
			{
				Position = new Point(0, MapWindow.Height - 3),
				Text = "Inventory"
			};
			MapWindow.Add(inventoryButton);

			Button anatomyButton = new Button(15, 3)
			{
				Position = new Point(15, MapWindow.Height - 3),
				Text = "Anatomy"
			};
			MapWindow.Add(anatomyButton);

            MapWindow.Children.Add(MapConsole);
            Children.Add(MapWindow);
            MapWindow.Show();

			MapConsole.MouseMove += Console_MouseMove;
			MapConsole.MouseButtonClicked += Console_MouseClicked;

            MapConsole.CenterViewPortOnPoint(GameLoop.World.Player.Position);
        }

        private void LoadMap(Map map)
        {
            Map currentMap = GameLoop.World.CurrentMap;

            MapConsole = new SadConsole.ScrollingConsole(currentMap.Width, currentMap.Height, Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);

            SyncMapEntities(map);
        }

        private void SyncMapEntities(Map map)
        {
            MapConsole.Children.Clear();

            foreach (Entity entity in map.Entities.Items)
                MapConsole.Children.Add(entity);

            map.Entities.ItemAdded += OnMapEntityAdded;
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        public void OnMapEntityRemoved(object sender, GoRogue.ItemEventArgs<Entity> args) =>
            MapConsole.Children.Remove(args.Item);

        public void OnMapEntityAdded(object sender, GoRogue.ItemEventArgs<Entity> args) =>
            MapConsole.Children.Add(args.Item);
    }
}
