using System;
using System.Text;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;
using System.Collections.Generic;


/*
 *Depth-First pre-order traversal appears to be the way to go, if we're going through the tree but generating the menu linearly.
 * https://en.wikipedia.org/wiki/Tree_traversal
 * 
 * 1. Process Node
 *  a. Indent #spaces equal to Node depth
 *  b. Leading symbol for row
 *  c. Name & any relevant data (Wt, condition, etc.)
 * 2. Check if Node collapsed/expanded
 * 3. If yes, process Node's child, return to 2
 * 4. If no, check for sibling
 * 5. If yes, process sibling
 * 6. If no, process 4 on parent
 * 
 * ListBox Themes, suggested by Thraka: https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ListBoxTheme.cs#L280
 * 
 * TODO: Figure out inventory/anatomy sorting according to parent. Currently they'll be sorted in order of acquisition.
 * 
 * The NuGet pkg I added: https://github.com/davidwest/TreeCollections
 * https://github.com/aalhour/C-Sharp-Algorithms/tree/master/DataStructures/Trees
 */

namespace PureHatred.UI
{
	public class CollapsibleTreeWindow : Window
	{
        private readonly ListBox _listBox;
        private int _windowBorder = 2;
        List<bool> finishedTiers = new List<bool>();
        List<List<Item>> TierLists = new List<List<Item>>();

        public CollapsibleTreeWindow(int width, int height, string title) : base(width, height)
        {
            height -= _windowBorder;
            width -= _windowBorder;

            Title = title.Align(HorizontalAlignment.Center, width);

			_listBox = new ListBox(width - _windowBorder / 2, height - _windowBorder / 2)
            {
                Position = new Point(1, 1),
            };
            Add(_listBox);

            InventoryList();
        }

        public void InventoryList()
		{
            Player player = GameLoop.World.Player;
            int i;

            _listBox.Items.Clear();

            for (i = 0; i < finishedTiers.Count; i++)
                finishedTiers[i] = false;


            // Do this like a BST
            // Pick a node; if last, close uncle
            // Then iterate through its children
            // Out of children? Break

            i = 0;
            if (player.inventory.Count > 0)
                foreach (Item item in player.inventory)
                {
                    Item current = player.inventory[i++];
                    _listBox.Items.Add($"{GetNodeTreeSymbols(item)}{current.Name}");
                }

            i = 0;
            if (player.anatomy.Count > 0)
                foreach (BodyPart bodyPart in player.anatomy)
				{
                    BodyPart current = player.anatomy[i++];

                    _listBox.Items.Add($"{GetNodeTreeSymbols(bodyPart)}{current.Name}    {current.HpCurrent}/{current.HpMax}");
				}
        }

        public StringBuilder GetNodeTreeSymbols(Item item)
		{
            StringBuilder output = new StringBuilder("");
            int itemTier = item.CountParents();

            if (item.parent == null)
                return output.Append($"{(char)196}{(char)194}"); // ─┬ Adam Trunk
			else
			{
                List<Item> siblings = item.parent.children;

                for (int i = itemTier; i > 0; i--)
                    if (item.GetAncestor(i).IsLastborn())
                        output.Append(" "); // No further Uncles for that tier
                    else
                        output.Append((char)179); // │ Uncle-Branch(es)

                if (item == siblings[^1])
                    output.Append((char)192); // └ Lastborn
                else
                    output.Append((char)195); // ├ Non-Lastborn

                if (item.children.Count != 0)
                    output.Append((char)194); // ┬ Childsome
                else
                    output.Append((char)196); // ─ Childless
            }
            return output;
		}

        public override void Draw(TimeSpan drawTime) =>
            base.Draw(drawTime);

        public override void Update(TimeSpan time) =>
            base.Update(time);
    }
}
