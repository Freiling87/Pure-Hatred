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
        private readonly ScrollingConsole _console;
        private readonly ScrollBar _scrollBar;
        private int _scrollPosition;
        private int _windowBorder = 2;

        public MessageLogWindow(int width, int height, string title) : base(width, height)
        {
            _lines = new Queue<string>();

            height -= _windowBorder;
            width -= _windowBorder;

            Title = title.Align(HorizontalAlignment.Center, Width);

			_console = new ScrollingConsole(width, _maxLines)
			{
				Position = new Point(1, 1),
				ViewPort = new Rectangle(0, 0, width - 1, height)
			};

			_scrollBar = new ScrollBar(Orientation.Vertical, height)
			{
				Position = new Point(width + 1, _console.Position.Y),
				IsEnabled = false
			};
			_scrollBar.ValueChanged += MessageScrollBar_ValueChanged;
            Add(_scrollBar); //Different Add() than the local void (see definition)

            Children.Add(_console);
        }

        void MessageScrollBar_ValueChanged(object sender, EventArgs e) =>
            _console.ViewPort = new Rectangle(
                0, _scrollBar.Value + _windowBorder, _console.Width, _console.ViewPort.Height);

		// Not seeing any references
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

                // Follow cursor since render area moves in event.
                _scrollBar.Value = _scrollPosition;
                _console.TimesShiftedUp = 0;
            }
        }

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
    }
}