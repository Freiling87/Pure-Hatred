using System.Text;

using GoRogue;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;
using SadConsole;

using PureHatred.Entities;
using PureHatred.UI;

namespace PureHatred.Resolutions
{
	public static class Combat
	{
        public static bool BumpAttack(Actor attacker, Actor defender)
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
            attackMessage.AppendFormat($"{attacker.Name} attacks {defender.Name}: ");

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
            MessageLogWindow MessageLog = GameLoop.UIManager.MessageLog;

            if (damage > 0)
            {
                BodyPart hitPart = defender.anatomy.RandomItem();
                hitPart.HpCurrent -= damage;

                defender.Health -= damage;
                MessageLog.AddTextNewline($" {hitPart.Name} was hit for {damage} damage, now at {hitPart.HpCurrent}. {defender.Name} at {defender.Health}.");
                if (defender.Health <= 0)
                    ResolveDeath(defender);
            }
            else
                MessageLog.AddTextNewline($"{defender.Name} blocked all damage!");
        }

        private static void ResolveDeath(Actor defender)
        {
            Map Map = GameLoop.World.CurrentMap;

            StringBuilder deathMessage = new StringBuilder($"{defender.Name} died");

            Decal blood = new Decal(Color.DarkRed, Color.Transparent, "blood", 258)
            { Position = defender.Position };

            GameLoop.World.CurrentMap.Add(blood);
            //TODO: Examine SadConsole.CellDecorator

            if (defender.anatomy.Count > 0)
                deathMessage.Append(" and dropped:");

            foreach (BodyPart bodyPart in defender.anatomy)
            {
                bodyPart.Position = defender.Position;
                Map.Add(bodyPart);
                deathMessage.Append($" {bodyPart.Name},");
            }
            defender.anatomy.Clear();

            deathMessage.Remove(deathMessage.Length - 1, 1); //Trim comma
            deathMessage.Append(".");

            Map.Remove(defender);
            GameLoop.World.CurrentMap.Actors.Remove(defender);
            GameLoop.UIManager.MessageLog.AddTextNewline(deathMessage.ToString());
        }
    }
}
