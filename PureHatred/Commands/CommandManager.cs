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
            //GameLoop.UIManager.MessageLog.AddTextNewline("(Player Turn here"); //-


            if (!GameLoop.UIManager.turnTaken)
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
			{
                actor.BioRhythm();
            }

            GameLoop.UIManager.StatusWindow.UpdateStatusWindow();

            _gameState = GameState.PlayerTurn;
            PlayerTurn();
		}

        // COMMANDS

        public bool UseDoor(Actor actor, TileDoor door)
        {
            if (door.Locked)
                return false;
            if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} opened a {door.Name}");
                return true;
            }
            return false;
        }

        public bool Pickup(Actor actor, Item item)
        {
            actor.Inventory.Add(item);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} picked up {item.Name}");
            item.Destroy();
            return true;
        }

		//public bool Devour(Actor actor, BodyPart bodyPart)
		//{
		//	bodyPart.parent = actor.Stomach;
		//	actor.Anatomy.Add(bodyPart);
		//	GameLoop.UIManager.SideWindow.InventoryList();
		//	GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} devoured a(n) {bodyPart.Name}");
		//	bodyPart.Destroy();
		//	return true;
		//}

		public bool Drop(Actor actor, Item item)
		{
            actor.Inventory.Remove(item);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name}'s {item.Name} was severed");
            //item.Destroy() opposite?
            return true;
		}
        public bool Drop(Actor actor, BodyPart bodyPart)
		{
            actor.Anatomy.Remove(bodyPart);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name}'s {bodyPart.Name} was severed");
            //item.Destroy() oposite?
            return true;
		}
    }
}
