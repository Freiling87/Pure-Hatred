﻿using System;
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
        private ListBox _listBox;
        private int _windowBorder = 2;

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
            _listBox.Items.Clear();
            Player player = GameLoop.World.Player;
            List<bool> finishedTiers = new List<bool>();
            // This should track whether the last member of this generation has been registered into the list, 
            // In order to cease the generation of this tier's Uncle-branches in child tiers

            int i = 0;

            if (player.Inventory.Count > 0)
                foreach (Item item in player.Inventory)
			    {
                    _listBox.Items.Add($"{GetNodeTreeSymbols(item)}{player.Inventory[i++].Name}");
                    if (item == item.parent.children[item.parent.children.Count - 1])
                        finishedTiers.Add(true);
                }

            i = 0;

            if (player.Anatomy.Count > 0)
                foreach (BodyPart bodyPart in player.Anatomy)
				{
                    _listBox.Items.Add($"{GetNodeTreeSymbols(bodyPart)}{player.Anatomy[i++].Name}");
				}
        }

        public StringBuilder GetNodeTreeSymbols(Item item)
		{
            StringBuilder output = new StringBuilder(" ");

            if (item.parent == null)
                return output.Append($"{(char)194}{(char)196}");
			else
			{
                /* Uncle branch     │ 179
                 * Self branch 
                 *   Not Lastborn   ├ 195
                 *   Lastborn       └ 192
                 * Offspring Branch
                 *   Childless      ─ 196
                 *   With child     ┬ 194
                 * ○┬Adam
                    ├┬Parent
                    │├─Child
                    │├─Child
                    │└┬Child
                    │ ├─Grandchild
                    │ └─Grandchild
                    ├─Parent
                    └┬Parent
                     ├─Child
                     └─Child
                 */

                for (int i = 0; i < GetTier(item); i++)
                    output.Append((char)179);
                //These are "Uncle-branches", which allow parent to connect to its siblings. 
                //Need to suppress if parent-or-higher tier has already expressed lastborn
                // See FinishedTiers in InventoryList() for the first steps I've implemented. 
                // If it's unreachable here we should just merge these two functions, but it won't be pretty.

                List<Item> siblings = item.parent.children;

                if (item == siblings[siblings.Count - 1])
                    output.Append((char)192); //Lastborn
                else
                    output.Append((char)196);
            }

            return output;
		}

        private int GetTier(Item item)
		{
            if (item.parent == null)
                return 0;
            else
                return 1 + GetTier(item.parent);
		}

        enum TierSymbol
		{
            Expanded = '-',
            Collapsed = '+',
            Terminal = 'o'
		}
        
        public override void Draw(TimeSpan drawTime) =>
            base.Draw(drawTime);

        public override void Update(TimeSpan time) =>
            base.Update(time);
    }
}
