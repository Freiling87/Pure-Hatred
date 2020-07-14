using Microsoft.Xna.Framework;

using PureHatred.Entities;
using PureHatred.UI;

using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace PureHatred.Commands
{
	static class MapCommands
	{
        public static void CheckKeyboard()
        {
            Player player = GameLoop.World.Player;
            UIManager UI = GameLoop.UIManager;

            GameLoop.UIManager.turnTaken = false;

            if (UI.IsKeyReleased(Keys.F5))
                SadConsole.Settings.ToggleFullScreen();

            if (UI.IsKeyPressed(Keys.Up))
                player.MoveBy(new Point(0, -1));
            if (UI.IsKeyPressed(Keys.Down))
                player.MoveBy(new Point(0, 1));
            if (UI.IsKeyPressed(Keys.Left))
                player.MoveBy(new Point(-1, 0));
            if (UI.IsKeyPressed(Keys.Right))
                player.MoveBy(new Point(1, 0));

            if (UI.IsKeyReleased(Keys.S))
            {
                player.Intestines.IntestinalExcretion();
                UI.turnTaken = true;
            }

            if (UI.turnTaken)
                GameLoop.CommandManager.EnemyTurn();
        }
    }
}
