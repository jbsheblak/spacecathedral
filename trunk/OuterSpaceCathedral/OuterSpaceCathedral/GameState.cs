using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    static class GameState
    {
        static Texture2D spriteSheet;
        static SpriteFont pixelFont;
        static Level level;

        public static void ChangeLevel(Level newLevel)
        {
            level = newLevel;
        }

        public static void Initialize(Texture2D sprites, SpriteFont font)
        {
            spriteSheet = sprites;
            pixelFont = font;
        }

        public static Texture2D SpriteSheet
        {
            get
            {
                return spriteSheet;
            }
        }

        public static SpriteFont PixelFont
        {
            get
            {
                return pixelFont;
            }
        }

        public static Level Level
        {
            get
            {
                return level;
            }
        }
    }
}
