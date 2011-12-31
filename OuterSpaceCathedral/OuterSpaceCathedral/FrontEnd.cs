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
        private float            mElapsedTime;

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
                    GameState.SetGameMode(GameState.Mode.Game);
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

            DrawSomeBadassTrippyBackgroundArt(spriteBatch);

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
        }

        public void DrawSomeBadassTrippyBackgroundArt(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, GameConstants.RenderTargetRect, new Rectangle(0, 0, 16, 16), new Color(26, 48, 78));

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(i * 64, j * 64, 64, 64), new Rectangle(352, 0, 64, 64), Color.White);
                }
            }

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(-32 + i * 64 - (int)((mElapsedTime * 32) % 64), j * 64, 64, 64), new Rectangle(352, 0, 64, 64), Color.White);
                }
            }
        }
    }
}
