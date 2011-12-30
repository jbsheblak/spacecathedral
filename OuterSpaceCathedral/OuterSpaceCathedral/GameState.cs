using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    static class GameState
    {
        public enum Mode
        {
            FrontEnd,
            Game
        };
        
        static Mode         mGameMode;
        static Texture2D    spriteSheet;
 		static SpriteFont   pixelFont;
        static Level        level;
        static FrontEnd     mFrontEnd;

        public static FrontEnd FrontEnd
        {
            get;
            set;
        }

        public static void Initialize(Texture2D sprites, SpriteFont font)
        {
            spriteSheet = sprites;
            mFrontEnd = new FrontEnd();
            pixelFont = font;
        }

        public static void Update(float deltaTime)
        {
            switch ( mGameMode )
            {
                case Mode.FrontEnd:
                    mFrontEnd.Update(deltaTime);
                    break;

                case Mode.Game:
                    level.Update(deltaTime);
                    break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            switch ( mGameMode )
            {
                case Mode.FrontEnd:
                    mFrontEnd.Draw(spriteBatch);
                    break;

                case Mode.Game:
                    level.Draw(spriteBatch);
                    break;
            }
        }

        public static Texture2D SpriteSheet
        {
            get
            {
                return spriteSheet;
            }
        }

        public static Mode GameMode
        {
            get { return mGameMode; }
            set { mGameMode = value; }
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
            get { return level; }
            set { level = value; }
        }
    }
}
