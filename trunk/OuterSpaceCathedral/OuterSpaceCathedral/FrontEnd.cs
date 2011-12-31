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

        public FrontEnd()
        {
            foreach ( string path in Directory.GetFiles( "content\\levels", "*.xml" ) )
            {
                mLevelEntries.Add(new LevelEntry(Path.GetFileNameWithoutExtension(path), path));
            }

            if ( mLevelEntries.Count > 0 )
            {
                // load the first level to prime the XmlSerializer cache
                Level.BuildLevelFromFile( mLevelEntries[0].Path );
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
        }
    }
}
