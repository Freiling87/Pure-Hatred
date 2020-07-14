using PureHatred.Entities;

namespace PureHatred.Commands
{
    public class GameStates
    {
        public GameState _gameState = GameState.PlayerTurn;
        public bool turnTaken = false;

        public GameStates()
        {
        }

        public enum GameState : int
		{
            Setup,
            PlayerTurn,
            EnemyTurn, 
            PassiveTurn,
            MenuGeneric, // Base Esc/Enter, Arrow Keys
		}

        public void PlayerTurn()
		{
            GameLoop.GSManager.turnTaken = false;

            //GameLoop.UIManager.MessageLog.AddTextNewline("(Player Turn here"); //-
            if (GameLoop.GSManager.turnTaken)
                _gameState = GameState.EnemyTurn;
		}

        public void PlayerTurnEnd()
		{
		}

        public void EnemyTurn()
        {
            //GameLoop.UIManager.MessageLog.AddTextNewline("(Enemy Turn here)"); //-
            _gameState = GameState.PassiveTurn;
            PassiveTurn();
        }

        public void PassiveTurn()
		{
            //GameLoop.UIManager.MessageLog.AddTextNewline("(Passive Turn here)"); //-

            foreach (Actor actor in GameLoop.World.CurrentMap.Actors)
                actor.BioRhythm();

            GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            _gameState = GameState.PlayerTurn;
            PlayerTurn();
		}

        // COMMANDS

    }
}