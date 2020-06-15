using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using SadConsole;

namespace PureHatred.UI
{
	public class CollapsibleTreeWindow : Window
	{
        private static readonly int _maxLines = 100;
        private readonly Queue<string> _lines;
        private SadConsole.ScrollingConsole _console;
        private SadConsole.Controls.ScrollBar _scrollBar;
        private int _scrollPosition;
        private int _windowBorder = 2;

        public CollapsibleTreeWindow(int width, int height, string title) : base(width, height)
        {
            Theme.FillStyle.Background = Color.Black;
            _lines = new Queue<string>();
            CanDrag = false;
            Title = title.Align(HorizontalAlignment.Center, Width);

            _console = new SadConsole.ScrollingConsole(width - _windowBorder, _maxLines);
            _console.Position = new Point(1, 1);
            _console.ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorder);

            _scrollBar = new SadConsole.Controls.ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorder);
            _scrollBar.Position = new Point(_console.Width + 1, _console.Position.X);
            _scrollBar.IsEnabled = false;
            _scrollBar.ValueChanged += MessageScrollBar_ValueChanged; // Subscribe to event handler
            Add(_scrollBar); //Different Add() than the local void (see definition)

            UseMouse = true;

            Children.Add(_console);
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
        }
    }
}
