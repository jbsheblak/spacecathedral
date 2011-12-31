using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OuterSpaceCathedral
{
    static class GameState
    {
        public enum Mode
        {
            FrontEnd,
            Game
        };
        
        const  int                  skDebugControlMod = 5;
        const  int                  skDefaultDebugControl = skDebugControlMod - 1;

        static Mode                 mGameMode;
        static Texture2D            spriteSheet;
 		static SpriteFont           pixelFont;
        static Level                level;
        static FrontEnd             mFrontEnd;
        static int                  mDebugControl = skDefaultDebugControl; // used to override which gamepad we control (DEBUG)
        static bool                 mChangingDebugControl = false;

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
            AudioManager.Update(deltaTime);

            switch ( mGameMode )
            {
                case Mode.FrontEnd:
                    {
                        mFrontEnd.Update(deltaTime);
                    }
                    break;

                case Mode.Game:
                    {
                        // allow player one to back out of the level
                        // (skip the re-reroute)
                        GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
                        if ( gamePad.Buttons.Back == ButtonState.Pressed )
                        {
                            GameState.GameMode = GameState.Mode.FrontEnd;
                            return;
                        }

                    #if DEBUG
                        {
                            // check for debug control changes
                            GamePadState debugPad = GamePad.GetState(PlayerIndex.One);
                            bool wantsToChangeDebugControl = ( debugPad.Buttons.RightShoulder == ButtonState.Pressed );
                            if ( wantsToChangeDebugControl != mChangingDebugControl )
                            {
                                if ( wantsToChangeDebugControl )
                                {
                                    mDebugControl = ( mDebugControl + 1 ) % skDebugControlMod;
                                }
                                mChangingDebugControl = wantsToChangeDebugControl;
                            }
                        }
                    #endif


                        level.Update(deltaTime);
                    }
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
        
        /// <summary>
        /// Current game mode.
        /// </summary>
        public static Mode GameMode
        {
            get { return mGameMode; }
            set
            {
                if ( mGameMode != value )
                {
                    mGameMode = value;

                    switch ( mGameMode )
                    {
                        case Mode.FrontEnd:
                            {
                                AudioManager.StopPlayerFireSFX();
                                mFrontEnd.ResetKeyCache();
                                level = null;
                            }
                            break;
                    }
                }
            }
        }


        public static Mode GetGameMode()
        {
            return mGameMode;
        }


        /// <summary>
        /// Get the GamePadState for a given player index.
        /// </summary>
        public static GamePadState GetGamePadState(PlayerIndex playerIndex)
        {
        #if DEBUG
            // if we have a non-default debug control, reroute controls
            if ( mDebugControl != skDefaultDebugControl )
            {
                // if the player index matches the debug control, route ctrl 1 through it.
                // if request for player0 comes in, route through something assumed unused
                if ( (int)playerIndex == mDebugControl )
                {
                    return GamePad.GetState(PlayerIndex.One);
                }
                else if ( (int)playerIndex == 0 )
                {
                    return GamePad.GetState(PlayerIndex.Four);
                }
            }
        #endif

            return GamePad.GetState(playerIndex);
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
            get { return level; }
            set { level = value; }
        }
    }
}
