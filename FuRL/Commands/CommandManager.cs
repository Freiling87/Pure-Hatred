using System;
using System.Text;

using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;

using PureHatred.Entities;
using PureHatred.Tiles;

namespace PureHatred.Commands
{
    public class CommandManager
    {
        private Point _lastMoveActorPoint;
        private Actor _lastMoveActor;

        public CommandManager()
        {
        }

        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);
            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            GameLoop.UIManager.MessageLog.AddTextNewline(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                GameLoop.UIManager.MessageLog.AddTextNewline(defenseMessage.ToString());

            int damage = hits - blocks;
            ResolveDamage(defender, damage);
        }

        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            attackMessage.AppendFormat($"{attacker.Name} attacks {defender.Name}, " );

            int hits = 0;

            for (int i = 0; i < attacker.Attack; i++)
            {
                if (Dice.Roll("1d100") <= attacker.AttackChance)
                    hits++;
            }
            return hits;
        }

        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;
            if (hits > 0)
            {
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defenseMessage.AppendFormat(" {0} defends and rolls: ", defender.Name);

                for (int i = 0; i < defender.Defense; i++)
                {
                    if (Dice.Roll("1d100") <= defender.DefenseChance)
                        blocks++;
                }
                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
                attackMessage.Append("and misses completely!");
            return blocks;
        }

        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;
                GameLoop.UIManager.MessageLog.AddTextNewline($" {defender.Name} was hit for {damage} damage");
                if (defender.Health <= 0)
                    ResolveDeath(defender);
            }
            else
                GameLoop.UIManager.MessageLog.AddTextNewline($"{defender.Name} blocked all damage!");
        }

        private static void ResolveDeath(Actor defender)
        {
            StringBuilder deathMessage = new StringBuilder($"{defender.Name} died");

            if (defender.Inventory.Count > 0)
            {
                deathMessage.Append(" and dropped");

                foreach (Item item in defender.Inventory)
                {
                    item.Position = defender.Position;
                    GameLoop.World.CurrentMap.Add(item);
                    deathMessage.Append(", " + item.Name);
                }
                defender.Inventory.Clear();
            }
            deathMessage.Append(".");
            GameLoop.World.CurrentMap.Remove(defender);
            GameLoop.UIManager.MessageLog.AddTextNewline(deathMessage.ToString());
        }

        public void UseDoor(Actor actor, TileDoor door)
        {
            if (door.Locked)
            {
            }
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} opened a {door.Name}");
            }
        }

        public bool MoveActorBy(Actor actor, Point position)
        {
            _lastMoveActor = actor;
            _lastMoveActorPoint = position;

            return actor.MoveBy(position);
        }

        public void Pickup(Actor actor, Item item)
        {
            actor.Inventory.Add(item);
            GameLoop.UIManager.MessageLog.AddTextNewline($"{actor.Name} picked up {item.Name}");
            item.Destroy();
        }
    }
}
