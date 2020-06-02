using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;

namespace PureHatred.UI
{
    public class MessageLogWindow : Window
    {
        private static readonly int _maxLines = 100;
        private readonly Queue<string> _lines;
        private SadConsole.ScrollingConsole _messageConsole;
        private SadConsole.Controls.ScrollBar _messageScrollBar;
        private int _scrollBarCurrentPosition;
        private int _windowBorderThickness = 2;

        public MessageLogWindow(int width, int height, string title) : base(width, height)
        {
            Theme.FillStyle.Background = Color.Black;
            _lines = new Queue<string>();
            CanDrag = false;
            Title = title.Align(HorizontalAlignment.Center, Width); //Possible typo here in Ansi's code, Width instead of width. Same effect for now, so leaving it.

            _messageConsole = new SadConsole.ScrollingConsole(width - _windowBorderThickness, _maxLines);
            _messageConsole.Position = new Point(1, 1);
            _messageConsole.ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorderThickness);

            _messageScrollBar = new SadConsole.Controls.ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorderThickness);
            _messageScrollBar.Position = new Point(_messageConsole.Width + 1, _messageConsole.Position.X);
            _messageScrollBar.IsEnabled = false;
            _messageScrollBar.ValueChanged += MessageScrollBar_ValueChanged; // Subscribe to event handler
            Add(_messageScrollBar); //Different Add() than the local void (see definition)

            UseMouse = true;

            Children.Add(_messageConsole);
        }

        // Controls position of messagelog viewport based on scrollbar position using event handler
        void MessageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            _messageConsole.ViewPort = new Rectangle(0, _messageScrollBar.Value + _windowBorderThickness, _messageConsole.Width, _messageConsole.ViewPort.Height);
        }

        //add a line to the queue of messages
        public void Add(string message)
        {
            _lines.Enqueue(message);

            if (_lines.Count > _maxLines)
                _lines.Dequeue();

            _messageConsole.Cursor.Position = new Point(1, _lines.Count);
            _messageConsole.Cursor.Print(message + '\n');
        }

        // print directly to the queue without adding a new line
        // Not currently used, and I'm not even sure if it works correctly
        public void Print(string text)
        {
            string[] lines = _lines.ToArray();
            lines[lines.Length] += text;
        }

        // Not seeing any references
        public override void Draw(TimeSpan drawTime)
        {
            base.Draw(drawTime);
        }

        public override void Update(TimeSpan time)
        {
            base.Update(time);

            // Ensure that the scrollbar tracks the current position of the _messageConsole.
            if (_messageConsole.TimesShiftedUp != 0 | _messageConsole.Cursor.Position.Y >= _messageConsole.ViewPort.Height + _scrollBarCurrentPosition)
            {
                //enable the scrollbar once the messagelog has filled up with enough text to warrant scrolling
                _messageScrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (_scrollBarCurrentPosition < _messageConsole.Height - _messageConsole.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    _scrollBarCurrentPosition += _messageConsole.TimesShiftedUp != 0 ? _messageConsole.TimesShiftedUp : 1;

                _messageScrollBar.Maximum = (_messageConsole.Height + _scrollBarCurrentPosition) - _messageConsole.Height - _windowBorderThickness;

                // This will follow the cursor since we move the render area in the event.
                _messageScrollBar.Value = _scrollBarCurrentPosition;

                // Reset the shift amount.
                _messageConsole.TimesShiftedUp = 0;
            }
        }
    }
}