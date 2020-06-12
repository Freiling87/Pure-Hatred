﻿using System;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography.X509Certificates;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;

using Console = SadConsole.Console;
using SadConsole.Debug;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/* TODO: Move calls to CenterOnPlayerActor to beginning of player turn, when gamestates are implemented
 * 
 */

namespace PureHatred.UI
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

        public void Init()
        {
            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
            MessageLog = new MessageLogWindow(GameLoop.GameWidth, GameLoop.GameHeight / 4, "Message Log");

            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight * 3 / 4);

            LoadMap(GameLoop.World.CurrentMap);
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight * 3 / 4, "Game Map");
            UseMouse = true;

            MapConsole.CenterViewPortOnPoint(GameLoop.World.Player.Position);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard(); // May turn this into a string that returns gamestates or generic commands
            base.Update(timeElapsed);
        }

		// INPUTS

		private static void Console_MouseMove(object sender, SadConsole.Input.MouseEventArgs e)//+
		{
            var console = (Console)sender;
            StringBuilder seenString = new StringBuilder("You see:");

            TileBase seenTile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(e.MouseState.CellPosition);

            if (seenTile != null)
                seenString.Append(" " + $"{seenTile.Name}" + ",");
            
            //List<Monster> seenEntities = GameLoop.World.CurrentMap.GetEntitiesAt<Monster>(e.MouseState.CellPosition);
			//if (seenEntities != null)
			//	foreach (Entity entity in seenEntities)
			//		seenString.Append(" " + "test" + ",");

            Monster seenEntity = GameLoop.World.CurrentMap.GetEntityAt<Monster>(e.MouseState.CellPosition);
			if (seenEntity != null)
                seenString.Append(" " + "test" + ",");
            //Consider e.MouseState.CellPosition.ToIndex() to convert to cell integer index?

            seenString.Remove(seenString.Length - 1, 1); //comma
            seenString.Append("                                         "); //overwrite
            console.Print(1, console.Height - 1, seenString.ToString());

            //if (e.MouseState.Mouse.LeftButtonDown)
            //	console.Print(1, console.Height - 2, $"You've clicked on {e.MouseState.CellPosition}        ");
            //else
            //	console.Print(1, console.Height - 2, $"                                                           ");
        }

        private static void Console_MouseClicked(object sender, SadConsole.Input.MouseEventArgs e)//+
		{
			var console = (Console)sender;
			console.Print(1, console.Height - 3, $"You've clicked on {e.MouseState.CellPosition}               ");
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
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = false;

            MapConsole.ViewPort = new Rectangle(0, 0, width - 2, height - 2);
            MapConsole.Position = new Point(1, 1);

            Button inventoryButton = new Button(15, 3);
            inventoryButton.Position = new Point(0, MapWindow.Height - 3);
            inventoryButton.Text = "Inventory";
            MapWindow.Add(inventoryButton);

            Button anatomyButton = new Button(15, 3);
            anatomyButton.Position = new Point(15, MapWindow.Height - 3);
            anatomyButton.Text = "Anatomy";
            MapWindow.Add(anatomyButton);

			MapWindow.Title = title.Align(HorizontalAlignment.Center, width);

            MapWindow.Children.Add(MapConsole);
            Children.Add(MapWindow);
            MapWindow.Show();

			MapConsole.MouseMove += Console_MouseMove;//+
			MapConsole.MouseButtonClicked += Console_MouseClicked;//+
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
