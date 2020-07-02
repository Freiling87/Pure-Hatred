using System;
using System.Text;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;


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
 * TODO: Take a look at ListBoxes as an alternative to the buttons. Possibly the high number of controls is not supportable, so the Listbox would be a more lightweight way to do it.
 * ListBox Themes, suggested by Thraka: https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ListBoxTheme.cs#L280
 * 
 * TODO: Figure out inventory/anatomy sorting according to parent. Currently they'll be sorted in order of acquisition.
 * 
 * The NuGet pkg I added: https://github.com/davidwest/TreeCollections
 * 
 * https://github.com/aalhour/C-Sharp-Algorithms/tree/master/DataStructures/Trees
 * 
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

            int i = 0;

            if (player.Inventory.Count > 0)
                foreach (Item item in player.Inventory)
			    {
                    StringBuilder tierString = new StringBuilder();

                    for (int j = 0; j < GetNodeIndent(item); j++)
                        tierString.Append("-");

                    tierString.Append(player.Inventory[i++].Name);

                    _listBox.Items.Add(tierString);
                }

            i = 0;

            if (player.Anatomy.Count > 0)
                foreach (BodyPart bodyPart in player.Anatomy)
				{
                    StringBuilder tierString = new StringBuilder();

                    for (int j = 0; j < GetNodeIndent(bodyPart); j++)
                        tierString.AppendFormat("-");

                    tierString.Append(player.Anatomy[i++].Name);

                    _listBox.Items.Add(tierString);
				}
        }

        public int GetNodeIndent(Item item)
		{
            if (item.parent == null)
                return 0;
            else 
                return 1 + GetNodeIndent(item.parent);
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
