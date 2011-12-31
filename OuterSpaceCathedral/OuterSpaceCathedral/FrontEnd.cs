using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OuterSpaceCathedral
{
    public class FrontEnd
    {
        // single front end entry
        private class LevelEntry
        {
            public LevelEntry(string label, string path)
            {
                Label = label;
                Path = path;
            }

            public string Label { get; private set; }
            public string Path  { get; private set; }
        };

        private List<LevelEntry> mLevelEntries = new List<LevelEntry>();
        private int              mSelectedIdx = 0;
        private bool             mPrevPressingSelect = true;
        private int              mPrevNagivationDelta = 0;

        private float           mElapsedTime = 0.0f;

        public FrontEnd()
        {
            foreach ( string path in Directory.GetFiles( "content\\levels", "*.xml" ) )
            {
                mLevelEntries.Add(new LevelEntry(Path.GetFileNameWithoutExtension(path), path));
            }
        }

        public void ResetKeyCache()
        {
            mPrevPressingSelect = true;
            mPrevNagivationDelta = 0;
        }

        public void Update(float deltaTime)
        {
            GamePadState primaryPadState = GamePad.GetState(0);

            // check for level selection
            bool isSelectButtonDown = primaryPadState.IsButtonDown(Buttons.A);
            if ( !mPrevPressingSelect && isSelectButtonDown )
            {   
                if ( mSelectedIdx < mLevelEntries.Count )
                {
                    GameState.GameMode = GameState.Mode.Game;
                    GameState.Level = Level.BuildLevelFromFile( mLevelEntries[mSelectedIdx].Path );
                    AudioManager.PlayCursorSelectSFX();
                    return;
                }
            }
            mPrevPressingSelect = isSelectButtonDown;

            // check for menu navigations
            if ( mLevelEntries.Count > 0 )
            {   
                const float sin45 = 0.70710678f;

                int nagivationDelta = 0;

                if ( primaryPadState.DPad.Up == ButtonState.Pressed || primaryPadState.ThumbSticks.Left.Y >= sin45 )
                {
                    nagivationDelta = -1;
                }

                if ( primaryPadState.DPad.Down == ButtonState.Pressed || primaryPadState.ThumbSticks.Left.Y <= -sin45 )
                {
                    nagivationDelta = +1;
                }

                if ( nagivationDelta != mPrevNagivationDelta )
                {
                    if (nagivationDelta != 0)
                    {
                        AudioManager.PlayCursorMoveSFX();
                    }
                    mSelectedIdx = ( mSelectedIdx + nagivationDelta + mLevelEntries.Count ) % mLevelEntries.Count;
                    mPrevNagivationDelta = nagivationDelta;
                }
            }

            mElapsedTime += deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the level entries
            Vector2 levelEntryPosition = new Vector2(15, 25);

            for ( int i = 0; i < mLevelEntries.Count; ++i )
            {
                Color color = ( i == mSelectedIdx ) ? Color.Red : Color.White;
                spriteBatch.DrawString(GameState.PixelFont, mLevelEntries[i].Label, levelEntryPosition, color);
                levelEntryPosition += new Vector2(0, GameState.PixelFont.LineSpacing + 5);
            }

            // draw the time
            string timeString = DateTime.Now.ToString("h:mm:ss tt");

            Vector2 stringSize = new Vector2(100, GameState.PixelFont.LineSpacing);
            Vector2 timeStringPos = new Vector2(GameConstants.RenderTargetWidth, GameConstants.RenderTargetHeight) - stringSize;
            spriteBatch.DrawString(GameState.PixelFont, timeString, timeStringPos, Color.Red);

            DrawSomeBadassTitleArt(spriteBatch);
        }

        public void DrawSomeBadassTitleArt(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.9f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.9f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.6f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.6f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.3f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.3f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.8f, (float)Math.Sin(mElapsedTime) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f) / 10f, SpriteEffects.None, 0);


            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 30), new Rectangle(544, 0, 128, 16), Color.White, 0f, new Vector2(64, 8), 2f + ((float)Math.Sin(mElapsedTime * 7) + 2f) / 9f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(450, 30), new Rectangle(544, 16, 128, 16), Color.SkyBlue, 0f, new Vector2(64, 8), 2f + ((float)Math.Sin((mElapsedTime + Math.PI) * 7) + 2f) / 9f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(325, 70), new Rectangle(544, 32, 128, 16), Color.Lerp(Color.White, Color.Yellow, ((float)Math.Sign(mElapsedTime * 10) + 1f) / 2f), 0f, new Vector2(64, 8), 2f + ((float)Math.Sin((mElapsedTime + Math.PI / 2f) * 7) + 2f) / 9f, SpriteEffects.None, 0);
            //spriteBatch.Draw(GameState.SpriteSheet, new Vector2(325, 100), new Rectangle(544, 16, 128, 16), Color.White, 0f, new Vector2(64, 32), 1f + ((float)Math.Sin(mElapsedTime * 7) + 2f) / 3f, SpriteEffects.None, 0);
            //spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(200, 75, 128, 128), new Rectangle(416, 0, 128, 128), Color.White * 0.5f);
        }
    }
}
