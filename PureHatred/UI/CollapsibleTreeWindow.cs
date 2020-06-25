using System;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

using PureHatred.Entities;
using System.Collections.Generic;

/*Symbols CAN be layered so we'll only need the following for the interface:
 * Hollow nodes, can hold + or - for expand/collapse
 * + and - themselves, possibly different from main font symbols
 * Node line connectors to parents, children and siblings
 *
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
 * 
 * TODO: Take a look at ListBoxes as an alternative to the buttons. Possibly the high number of controls is not supportable, so the Listbox would be a more lightweight way to do it.
 * ListBox Themes, suggested by Thraka: https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ListBoxTheme.cs#L280
 */

namespace PureHatred.UI
{
	public class CollapsibleTreeWindow : Window
	{
        private ListBox _listBox;
        private int _windowBorder = 2;

        public CollapsibleTreeWindow(int width, int height, string title) : base(width, height)
        {
            CanDrag = false; 
            UseMouse = true;

            height -= _windowBorder;
            width -= _windowBorder;

            Title = title.Align(HorizontalAlignment.Center, width);

			_listBox = new ListBox(width - _windowBorder, height - _windowBorder)
            {
                Position = new Point(1, 1),
                IsEnabled = true,
                IsVisible = true
            };
            Add(_listBox);

            InventoryList();
        }

        public void InventoryList()
		{
            _listBox.Items.Clear();

            int i = 0;

            foreach (Item item in GameLoop.World.Player.Inventory)
			{
                Item currentItem = GameLoop.World.Player.Inventory[i];

                /*
                if (IsParentNodeExpanded(Item))
                {
                    StringBuilder tierString = new StringBuilder();
                    for (int i = 0; i < currentItem.tierLevel; i++)
                    {
                        tierString += "|"
                    }
				}
                */

                _listBox.Items.Add(GameLoop.World.Player.Inventory[i++].Name);
            }
        }

        public bool IsParentNodeExpanded(Item item)
		{
            return true; //-
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
