using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;
using System.Runtime.CompilerServices;
using System.Text;

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
 *  b. Leading Node symbol
 *  c. Name & any relevant data
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
        private SadConsole.ScrollingConsole _console;
        private SadConsole.Controls.ScrollBar _scrollBar;
        private int _scrollPosition;
        private int _windowBorder = 2;

        public CollapsibleTreeWindow(int width, int height, string title) : base(width, height)
        {
            CanDrag = false;
            Title = title.Align(HorizontalAlignment.Center, Width);

            _console = new SadConsole.ScrollingConsole(width, 256);
            _console.Position = new Point(1, 1);
            _console.ViewPort = new Rectangle(1, 1, width - _windowBorder, height - _windowBorder);

            _scrollBar = new SadConsole.Controls.ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorder);
            _scrollBar.Position = new Point(_console.Width - _windowBorder, _console.Position.Y);
            _scrollBar.IsEnabled = true;
            _scrollBar.ValueChanged += MessageScrollBar_ValueChanged; // Subscribe to event handler
            Add(_scrollBar); //Different Add() than the local void (see definition)

            UseMouse = true;

            Children.Add(_console);


            //Button button = new Button(15, 3);
            //button.Position = new Point(1,2);
            //button.Text = "Test button";
            //this.Add(button);
        }

        // Controls position of viewport based on scrollbar position using event handler
        void MessageScrollBar_ValueChanged(object sender, EventArgs e) =>
            _console.ViewPort = new Rectangle(0, _scrollBar.Value + _windowBorder, _console.Width, _console.ViewPort.Height);

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            // Scrollbar tracks current position of console
            if (_console.TimesShiftedUp != 0 | _console.Cursor.Position.Y >= _console.ViewPort.Height + _scrollPosition)
            {
                //enable scrollbar when enough text to scroll
                _scrollBar.IsEnabled = true;

                // Prevent scroll past size of buffer
                // Record amount scrolled to enable how far back bar can see
                if (_scrollPosition < _console.Height - _console.ViewPort.Height)
                    _scrollPosition += _console.TimesShiftedUp != 0 ? _console.TimesShiftedUp : 1;

                _scrollBar.Maximum = (_console.Height + _scrollPosition) - _console.Height - _windowBorder;

                // This will follow the cursor since we move the render area in the event.
                _scrollBar.Value = _scrollPosition;
                _console.TimesShiftedUp = 0;
            }



            InventoryList();
        }

        private void  InventoryList()
		{


            int i = 0;
            foreach (Item item in GameLoop.World.Player.Inventory)
			{
                StringBuilder rowString = new StringBuilder($"- {item.Name}");
                
                
                _console.Print(0, i, rowString.ToString());
                i++;

            }
        }
    }
}
