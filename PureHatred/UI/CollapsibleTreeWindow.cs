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
 */

namespace PureHatred.UI
{
	public class CollapsibleTreeWindow : Window
	{
        private readonly ScrollingConsole _console;
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

			_console = new ScrollingConsole(width - 3, 256)
			{
				Position = new Point(1, 1),
                ViewPort = new Rectangle(0, 0, width, height)
            };

            _scrollBar = new ScrollBar(Orientation.Vertical, height)
            {
                Position = new Point(width, _console.Position.Y),
                IsEnabled = false
			};
			_scrollBar.ValueChanged += ScrollBar_ValueChanged;

            Add(_scrollBar); //Different Add() than the local void (see definition)
            Children.Add(_console);
        }

        void ScrollBar_ValueChanged(object sender, EventArgs e) =>
            _console.ViewPort = new Rectangle(
                0, _scrollBar.Value + _windowBorder, _console.Width, _console.ViewPort.Height);

        public override void Draw(TimeSpan drawTime) =>
            base.Draw(drawTime);

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            // Scrollbar tracks current position of console
            if (_console.TimesShiftedUp != 0 |
                _console.Cursor.Position.Y >= _console.ViewPort.Height + _scrollPosition)
            {
                _scrollBar.IsEnabled = true;

                // Prevent scroll past buffer
                // Record amount scrolled to enable how far back bar can see
                if (_scrollPosition < _console.Height - _console.ViewPort.Height)
                    _scrollPosition += (_console.TimesShiftedUp != 0 ? _console.TimesShiftedUp : 1);

                _scrollBar.Maximum = _scrollPosition - _windowBorder;

                // Follows cursor since the event moves the render area.
                _scrollBar.Value = _scrollPosition;
                _console.TimesShiftedUp = 0;
            }
            InventoryList();
        }

        private void  InventoryList()
		{
            int i = 1;

            foreach (Item item in GameLoop.World.Player.Inventory)
			{
                StringBuilder rowString = new StringBuilder($"- {item.Name}");

                _console.Cursor.Position = new Point(1, i);
                _console.Cursor.Print(rowString.ToString() + "\n");

				//Button button = new Button(30);
				//button.Position = new Point(1, i);
				//button.Text = rowString.ToString();
				//this.Add(button);

				i++;
            }
        }
    }
}
