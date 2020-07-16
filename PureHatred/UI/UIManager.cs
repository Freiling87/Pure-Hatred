using System;
using System.Collections.Generic;
using System.Text;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Commands;
using PureHatred.Entities;

using Console = SadConsole.Console;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

/* TODO: Move calls to CenterOnPlayerActor to beginning of player turn, when gamestates are implemented
 */

namespace PureHatred.UI
{
    public class UIManager : ContainerConsole
    {
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public SideWindow SideWindow;
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
            if (GameLoop.GSManager._gameState == GameStates.GameState.PlayerTurn)
                MapCommands.CheckKeyboard();

            base.Update(timeElapsed);
        }

        public void CreateWindows()
        {
            int height_1_4 = GameLoop.GameHeight * 1 / 4;
            int height_3_4 = GameLoop.GameHeight * 3 / 4;
            int width14 = GameLoop.GameWidth * 1 / 4;
            int width34 = GameLoop.GameWidth * 3 / 4;

            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
            LoadMap(GameLoop.World.CurrentMap);
            CreateMapWindow(width34, height_3_4, "Map");

            SideWindow = new SideWindow(width14, height_3_4, "Inventory / Anatomy")
            {
                Position = new Point(width34, 0),
                CanDrag = false,
                UseMouse = true,
            };
            Children.Add(SideWindow);
            SideWindow.Show();

            MessageLog = new MessageLogWindow(width34, height_1_4, "Log")
            {
                Position = new Point(0, height_3_4),
                CanDrag = false,
                UseMouse = true,
            };
            Children.Add(MessageLog);
            MessageLog.Show();

            StatusWindow = new StatusWindow(width14, height_1_4)
            {
                Position = new Point(width34, height_3_4),
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

            // Map _____ by Click
            //if (e.MouseState.Mouse.LeftButtonDown)
            //	console.Print(1, console.Height - 2, $"You've clicked on {e.MouseState.CellPosition}        ");
            //else
            //	console.Print(1, console.Height - 2, $"                                                           ");
        }

        private static void Console_MouseClicked(object sender, SadConsole.Input.MouseEventArgs e)//+
		{
			var console = (Console)sender;
            StringBuilder seenString = new StringBuilder("You see:");
            // Or just list Adam objects only, and allow further inspection explicitly

            TileBase seenTile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(e.MouseState.CellPosition);
            if (seenTile != null)
                seenString.Append($" {seenTile.Name},");

            List<Entity> seenEntities = GameLoop.World.CurrentMap.GetEntitiesAt<Entity>(e.MouseState.CellPosition);
            if (seenEntities != null)
                foreach (Entity entity in seenEntities)
                    seenString.Append($" {entity.Name},");

            seenString.Remove(seenString.Length - 1, 1);                    //trim comma

            GameLoop.UIManager.MessageLog.AddTextNewline(seenString.ToString());
        }

        public bool IsKeyReleased(Keys input) =>
            Global.KeyboardState.IsKeyReleased(input);

        public bool IsKeyPressed(Keys input) =>
            Global.KeyboardState.IsKeyPressed(input);

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

            GameLoop.UIManager.MapConsole = new ScrollingConsole(currentMap.Width, currentMap.Height, Global.FontDefault, new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);

            map.SyncMapEntities();
        }
    }
}
