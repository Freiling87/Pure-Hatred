using Microsoft.Xna.Framework;

using PureHatred.Entities;
using PureHatred.UI;

using Keys = Microsoft.Xna.Framework.Input.Keys;

// TranscendenceRL has a way to do a case statement for key input: https://github.com/INeedAUniqueUsername/TranscendenceRL/blob/master/TranscendenceRL/Screens/ArenaScreen.cs#L141

namespace PureHatred.Commands
{
	static class MapCommands
	{
        public static void CheckKeyboard()
        {
            Player player = GameLoop.World.Player;
            UIManager UI = GameLoop.UIManager;

            GameLoop.GSManager.turnTaken = false;

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
                player.Intestines.Defecation(true);

            if (GameLoop.GSManager.turnTaken)
                GameLoop.GSManager.EnemyTurn();
        }
    }
}
