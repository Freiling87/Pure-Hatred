using System;
using System.Text;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

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
 * 
 * TODO: Take a look at ListBoxes as an alternative to the buttons. Possibly the high number of controls is not supportable, so the Listbox would be a more lightweight way to do it.
 * ListBox Themes, suggested by Thraka: https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ListBoxTheme.cs#L280
 */

namespace PureHatred.UI
{
	public class CollapsibleTreeWindow : Window
	{
        private readonly ControlsConsole _ScrollingConsole;
        private readonly ScrollBar _scrollBar;
        private int _scrollPosition;
        private int _windowBorder = 2;

        public CollapsibleTreeWindow(int width, int height, string title) : base(width, height)
        {
            CanDrag = false; 
            UseMouse = true;

            height -= _windowBorder;
            width -= _windowBorder;

            Title = title.Align(HorizontalAlignment.Center, Width);

			_ScrollingConsole = new ControlsConsole(width - 3, 256)
			{
				Position = new Point(1, 1),
                ViewPort = new Rectangle(0, 0, width, height)
            };

            _scrollBar = new ScrollBar(Orientation.Vertical, height)
            {
                Position = new Point(width, _ScrollingConsole.Position.Y),
                IsEnabled = true
			};
			_scrollBar.ValueChanged += ScrollBar_ValueChanged;

            Add(_scrollBar); 
            InventoryList();

            Children.Add(_ScrollingConsole);
        }

        void ScrollBar_ValueChanged(object sender, EventArgs e) =>
            _ScrollingConsole.ViewPort = new Rectangle(
                0, _scrollBar.Value + _windowBorder, _ScrollingConsole.Width, _ScrollingConsole.ViewPort.Height);

        public override void Draw(TimeSpan drawTime) =>
            base.Draw(drawTime);

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            if (_ScrollingConsole.TimesShiftedUp != 0 |
                _ScrollingConsole.Cursor.Position.Y >= _ScrollingConsole.ViewPort.Height + _scrollPosition)
            {
                if (_scrollPosition < _ScrollingConsole.Height - _ScrollingConsole.ViewPort.Height)
                    _scrollPosition += (_ScrollingConsole.TimesShiftedUp != 0 ? _ScrollingConsole.TimesShiftedUp : 1);

                _scrollBar.Maximum = _scrollPosition - _windowBorder;

                _scrollBar.Value = _scrollPosition;
                _ScrollingConsole.TimesShiftedUp = 0;

                InventoryList();
            }
        }

        public void InventoryList()
		{
            int i = 1;

            ButtonTheme buttonTheme = new ButtonTheme()
            {
                ShowEnds = false
            };

            foreach (Item item in GameLoop.World.Player.Inventory)
			{
				Button button = new Button(10)
				{
					Text = item.Name,
					TextAlignment = HorizontalAlignment.Left,
					Position = new Point(1, i),
					Theme = buttonTheme
				};
				_ScrollingConsole.Add(button);

				//StringBuilder rowString = new StringBuilder($"- {item.Name}");

				//_ScrollingConsole.Cursor.Position = new Point(1, i);
				//_ScrollingConsole.Cursor.Print(rowString.ToString() + "\n");

				i++;
            }
        }
    }
}
