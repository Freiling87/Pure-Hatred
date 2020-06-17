using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

namespace PureHatred.UI
{
    public class MessageLogWindow : Window
    {
        private static readonly int _maxLines = 100;
        private readonly Queue<string> _lines;
        private ScrollingConsole _console;
        private ScrollBar _scrollBar;
        private int _scrollPosition;
        private int _windowBorder = 2;

        public MessageLogWindow(int width, int height, string title) : base(width, height)
        {
            _lines = new Queue<string>();

            CanDrag = false;
            UseMouse = true;

            Title = title.Align(HorizontalAlignment.Center, Width);

			_console = new ScrollingConsole(width - _windowBorder, _maxLines)
			{
				Position = new Point(1, 1),
				ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorder)
			};

			_scrollBar = new ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorder)
			{
				Position = new Point(_console.Width + 1, _console.Position.X),
				IsEnabled = false
			};
			_scrollBar.ValueChanged += MessageScrollBar_ValueChanged;
            Add(_scrollBar); //Different Add() than the local void (see definition)

            Children.Add(_console);
        }

        void MessageScrollBar_ValueChanged(object sender, EventArgs e) =>
            _console.ViewPort = new Rectangle(0, _scrollBar.Value + _windowBorder, _console.Width, _console.ViewPort.Height);

        public void AddTextNewline(string message)
        {
            _lines.Enqueue(message);

            if (_lines.Count > _maxLines)
                _lines.Dequeue();

            _console.Cursor.Position = new Point(1, _lines.Count);
            _console.Cursor.Print(message + '\n');
        }

        public void AddTextNoNewline(string text)
        {
            string[] lines = _lines.ToArray();
            lines[lines.Length] += text;
        }

		// Not seeing any references
		public override void Draw(TimeSpan drawTime) =>
            base.Draw(drawTime);

        public override void Update(TimeSpan time)
        {
            base.Update(time);


            // Ensure that the scrollbar tracks the current position of the _messageConsole.
            if (_console.TimesShiftedUp != 0 | 
                _console.Cursor.Position.Y >= _console.ViewPort.Height + _scrollPosition)
            {
                //enable the scrollbar once the messagelog has filled up with enough text to warrant scrolling
                _scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (_scrollPosition < _console.Height - _console.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    _scrollPosition += (_console.TimesShiftedUp != 0 ? _console.TimesShiftedUp : 1);

                _scrollBar.Maximum = (_console.Height + _scrollPosition) - _console.Height - _windowBorder;

                // This will follow the cursor since we move the render area in the event.
                _scrollBar.Value = _scrollPosition;
                _console.TimesShiftedUp = 0;
            }
        }
    }
}