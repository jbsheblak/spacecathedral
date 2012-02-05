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
        static Texture2D            happyNewYears;
 		static SpriteFont           pixelFont;
        static Level                level;
        static FrontEnd             mFrontEnd;

#if DEBUG
        static int                  mDebugControl = skDefaultDebugControl; // used to override which gamepad we control (DEBUG)
        static bool                 mChangingDebugControl = false;
#endif

        public static FrontEnd FrontEnd
        {
            get;
            set;
        }

        public static void Initialize(Texture2D sprites, Texture2D happyNewYearsTexture, SpriteFont font)
        {
            spriteSheet = sprites;
            happyNewYears = happyNewYearsTexture;
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
                    #if DEBUG
                        ControllerInput.IController controller = ControllerInput.GetController(PlayerIndex.One);

                        // allow player one to back out of the level
                        // (skip the re-reroute)
                        {
                            if ( controller.GetButtonState(ControllerInput.ButtonAction.Debug_Back) == ButtonState.Pressed )
                            {
                                GameState.GameMode = GameState.Mode.FrontEnd;
                                return;
                            }
                        }
                                            
                        {
                            // check for debug control changes
                            bool wantsToChangeDebugControl = ( controller.GetButtonState(ControllerInput.ButtonAction.Debug_SwapControl) == ButtonState.Pressed );
                            if ( wantsToChangeDebugControl != mChangingDebugControl )
                            {
                                if ( wantsToChangeDebugControl )
                                {
                                    mDebugControl = ( mDebugControl + 1 ) % skDebugControlMod;
                                }
                                mChangingDebugControl = wantsToChangeDebugControl;
                            }
                        }
                    #endif // DEBUG


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
                                AudioManager.StopAllMusic();
                                mFrontEnd.ResetKeyCache();
                                level = null;
                            }
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the controller for a given player.
        /// </summary>
        public static ControllerInput.IController GetController(PlayerIndex playerIndex)
        {
        #if DEBUG
            // if we have a non-default debug control, reroute controls
            if ( mDebugControl != skDefaultDebugControl )
            {
                // if the player index matches the debug control, route ctrl 1 through it.
                // if request for player0 comes in, route through something assumed unused
                if ( (int)playerIndex == mDebugControl )
                {
                    return ControllerInput.GetController(PlayerIndex.One);
                }
                else if ( (int)playerIndex == 0 )
                {
                    return ControllerInput.GetController(PlayerIndex.Four);
                }
            }
        #endif

            return ControllerInput.GetController(playerIndex);
        }

        public static Texture2D SpriteSheet
        {
            get
            {
                return spriteSheet;
            }
        }

        public static Texture2D HappyNewYearsTexture
        {
            get { return happyNewYears; }
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
