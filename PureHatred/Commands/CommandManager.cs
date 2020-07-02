using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using GoRogue;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;

using PureHatred.Entities;
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
            Setup = 0,
            PlayerTurn = 1,
            EnemyTurn = 2, 
            PassiveTurn = 3,
            MenuGeneric = 10, // Base Esc/Enter, Arrow Keys
		}

        private void EnemyTurn()
		{
            //placeholder
            GameLoop.UIManager.MessageLog.AddTextNewline("(Enemy Turn here)");
            _gameState = GameState.PlayerTurn;
		}

        // COMBAT

        public bool BumpAttack(Actor attacker, Actor defender)
        {
            MessageLogWindow MessageLog = GameLoop.UIManager.MessageLog;

            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);
            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);
            int damage = hits - blocks;

            MessageLog.AddTextNewline(attackMessage.ToString());

            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                MessageLog.AddTextNewline(defenseMessage.ToString());

            ResolveDamage(defender, damage);

            return true;
        }

        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            attackMessage.AppendFormat($"{attacker.Name} attacks {defender.Name}: " );

            int hits = 0;

            for (int i = 0; i < attacker.Attack; i++)
                if (Dice.Roll("10d10") <= attacker.AttackChance) 
                    hits++;

            return hits;
        }

        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;
            if (hits > 0)
            {
                attackMessage.AppendFormat($"{hits} hits.");
                defenseMessage.AppendFormat($" {defender.Name} defends: ");

                for (int i = 0; i < defender.Defense; i++)
                {
                    if (Dice.Roll("1d100") <= defender.DefenseChance)
                        blocks++;
                }
                defenseMessage.AppendFormat($"resulting in {blocks} blocks.");
            }
            else
                attackMessage.Append("and misses completely!");
            return blocks;
        }

        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                BodyPart hitPart = defender.Anatomy.RandomItem();
                hitPart.HpCurrent -= damage;

                defender.Health -= damage;
                GameLoop.UIManager.MessageLog.AddTextNewline($" {hitPart.Name} was hit for {damage} damage, now at {hitPart.HpCurrent}. {defender.Name} at {defender.Health}.");
                if (defender.Health <= 0)
                    ResolveDeath(defender);
            }
            else
                GameLoop.UIManager.MessageLog.AddTextNewline($"{defender.Name} blocked all damage!");
        }

        private static void ResolveDeath(Actor defender)
        {
            StringBuilder deathMessage = new StringBuilder($"{defender.Name} died");

			Decal blood = new Decal(Color.DarkRed, Color.Transparent, "blood", 258)
			    { Position = defender.Position };

			GameLoop.World.CurrentMap.Add(blood);
            //TODO: Examine SadConsole.CellDecorator

			if (defender.Inventory.Count > 0 || defender.Anatomy.Count > 0)
                deathMessage.Append(" and dropped:");

            foreach (Item item in defender.Inventory)
            {
                item.Position = defender.Position;
                GameLoop.World.CurrentMap.Add(item);
                deathMessage.Append($" {item.Name},");
            }
            defender.Inventory.Clear();

            foreach (BodyPart bodyPart in defender.Anatomy)
            {
                bodyPart.Position = defender.Position;
                GameLoop.World.CurrentMap.Add(bodyPart);
                deathMessage.Append($" {bodyPart.Name},");
            }
            defender.Anatomy.Clear();

            deathMessage.Remove(deathMessage.Length - 1, 1); //Trim comma
            deathMessage.Append(".");

            GameLoop.World.CurrentMap.Remove(defender);
            GameLoop.UIManager.MessageLog.AddTextNewline(deathMessage.ToString());
        }

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

        public bool Devour(Actor actor, Item item)
		{
            item.parent = actor.Stomachs[0];
            actor.Inventory.Add(item);
            GameLoop.UIManager.SideWindow.InventoryList();
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} devoured a(n) {item.Name}");
            item.Destroy();
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
