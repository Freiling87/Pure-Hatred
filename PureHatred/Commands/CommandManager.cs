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
        private Point _lastMoveActorPoint;
        private Actor _lastMoveActor;
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

        private void PlayerTurn()
		{
            GameLoop.UIManager.MessageLog.AddTextNewline("(Player Turn here");
            _gameState = GameState.EnemyTurn;
		}

        private void EnemyTurn()
        {
            GameLoop.UIManager.MessageLog.AddTextNewline("(Enemy Turn here)");
            _gameState = GameState.PassiveTurn;
        }

        private void PassiveTurn()
		{

            _gameState = GameState.PlayerTurn;
		}

        // COMBAT

        public bool UseDoor(Actor actor, TileDoor door)
        {
            if (door.Locked)
            {
                return false;
            }
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} opened a {door.Name}");
                return true;
            }
            return false;
        }

        public bool MoveActorBy(Actor actor, Point position)
        {
            _lastMoveActor = actor;
            _lastMoveActorPoint = position;

            return actor.MoveBy(position);
        }

        public bool Pickup(Actor actor, Item item)
        {
            actor.Inventory.Add(item);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} picked up {item.Name}");
            item.Destroy();
            return true;
        }

        public bool Devour(Actor actor, BodyPart bodyPart)
		{
            bodyPart.parent = actor.Stomachs[0];
            actor.Anatomy.Add(bodyPart);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} devoured a(n) {bodyPart.Name}");
            bodyPart.Destroy();
            return true;
		}

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
