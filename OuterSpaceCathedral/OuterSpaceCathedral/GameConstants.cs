using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    static internal class GameConstants
    {
        static GameConstants()
        {
            RenderTargetRect = new Rectangle(0, 0, 480, 270);
        }

        /// <summary>
        /// Rectangle representing render target dimensions.
        /// </summary>
        public static Rectangle RenderTargetRect    { get; private set; }
        public static int       RenderTargetWidth   { get { return RenderTargetRect.Width; } }
        public static int       RenderTargetHeight  { get { return RenderTargetRect.Height; } }
        public static Vector2   RenderTargetCenter  { get { return new Vector2(RenderTargetWidth/2, RenderTargetHeight/2); } }
        public static Vector2   RenderTargetMax     { get { return new Vector2(RenderTargetWidth,   RenderTargetHeight  ); } }

        public static Rectangle CalcRectFor32x32Sprite( int row, int col )
        {
            return new Rectangle(col * 32, row * 32, 32, 32);
        }
    }
}
