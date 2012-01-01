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
            BackBufferRect   = new Rectangle(0, 0, 960, 540);
        }

        /// <summary>
        /// Screen transition rates
        /// </summary>
        public static float     ScreenTransitionRotationRate { get { return 2.0f; } }
        public static float     ScreenTransitionScaleRate { get { return 4.0f; } }
        public static float     ScreenTransitionScaleMin { get { return 0.1f; } }
        public static float     ScreenTransitionScaleMax { get { return 10.0f; } }

        /// <summary>
        /// Back buffer dimensions
        /// </summary>
        public static Rectangle BackBufferRect      { get; private set; }
        public static int       BackBufferWidth     { get { return BackBufferRect.Width; } }
        public static int       BackBufferHeight    { get { return BackBufferRect.Height; } }
        public static Vector2   BackBufferCenter    { get { return new Vector2(BackBufferWidth/2, BackBufferHeight/2); } }

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
        
        public static Color GetColorForPlayer(PlayerIndex playerIndex)
        {
            switch ( playerIndex )
            {
                case PlayerIndex.One:       return Color.LightYellow;
                case PlayerIndex.Two:       return Color.Orange;
                case PlayerIndex.Three:     return Color.Violet;
                case PlayerIndex.Four:      return Color.Cyan;
                default:                    return Color.White;
            }
        }
    }
}
