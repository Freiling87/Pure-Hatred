using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using GoRogue;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;

using PureHatred.Entities;
using PureHatred.Resolutions;
using PureHatred.Tiles;
using PureHatred.UI;

namespace PureHatred.Commands
{
    public class CommandManager
    {
        public GameState _gameState = GameState.PlayerTurn;

        public CommandManager()
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
            GameLoop.UIManager.turnTaken = false;

            //GameLoop.UIManager.MessageLog.AddTextNewline("(Player Turn here"); //-
            if (GameLoop.UIManager.turnTaken)
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