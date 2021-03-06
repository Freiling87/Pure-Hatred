﻿using System;

using Microsoft.Xna.Framework;

namespace PureHatred.Tiles
{
    public class TileDoor : TileBase
    {
        public bool Locked;
        public bool IsOpen;

        public TileDoor(bool locked, bool isOpen) : base(Color.LightGray, Color.SaddleBrown, '=', true, true, "Door")
        {
            Locked = locked;
            IsOpen = isOpen;

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
            IsImpassible = true;
        }

        public void Open()
        {
            IsOpen = true;
            IsBlockingLOS = false;
            IsImpassible = false;
            Glyph = 239;
            this.Background = Color.DarkGray;
            this.Foreground = Color.SaddleBrown;
        }
    }
}
