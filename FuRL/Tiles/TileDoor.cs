using System;

using Microsoft.Xna.Framework;

namespace FuRL.Tiles
{
    public class TileDoor : TileBase
    {
        public bool Locked;
        public bool IsOpen;

        public TileDoor(bool locked, bool open) : base(Color.LightGray, Color.SaddleBrown, '=')
        {
            Glyph = '=';

            Locked = locked;
            IsOpen = open;

            if (!Locked && IsOpen)
                Open();
            else if (Locked || !IsOpen)
                Close();
        }

        public void Close()
        {
            IsOpen = false;
            Glyph = '=';
            IsBlockingLOS = true;
            IsBlockingMove = true;
            this.Background = Color.SaddleBrown;
        }

        public void Open()
        {
            IsOpen = true;
            IsBlockingLOS = false;
            IsBlockingMove = false;
            Glyph = ' ';
            this.Background = Color.DarkGray;
        }
    }
}
