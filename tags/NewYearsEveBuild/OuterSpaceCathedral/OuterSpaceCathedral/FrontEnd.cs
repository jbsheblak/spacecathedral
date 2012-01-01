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
            public LevelEntry(string label, string path, string unlockTime, bool unlocked )
            {
                Label = label;
                Path = path;
                UnlockTime = unlockTime;
                Unlocked = unlocked;
            }

            public string Label         { get; private set; }
            public string Path          { get; private set; }
            public string UnlockTime    { get; private set; }
            public bool   Unlocked      { get;         set; }
        };

        private List<LevelEntry> mLevelEntries = new List<LevelEntry>();
        private int              mSelectedIdx = 0;
        private bool             mShouldLaunchFinalLevel = false;
        private bool             mPrevPressingSelect = true;
        private int              mPrevNagivationDelta = 0;

        public FrontEnd()
        {
            mLevelEntries.Add( new LevelEntry("First Frontier",     "content\\levels\\Level0.xml", string.Empty, true) );
            
            bool unlockedByDefault = false;

        #if DEBUG
            unlockedByDefault = true;
        #endif

            // level unlocks
            mLevelEntries.Add( new LevelEntry("Ocean Orchestra",    "content\\levels\\Level1.xml", "22:00:00",   unlockedByDefault) );
            mLevelEntries.Add( new LevelEntry("Cathedral 300",      "content\\levels\\Level2.xml", "23:00:00",   unlockedByDefault) );
            mLevelEntries.Add( new LevelEntry("SECRET",             "content\\levels\\Level3.xml", "23:58:00",   unlockedByDefault) );

            if ( mLevelEntries.Count > 0 )
            {
                // load the first level to prime the XmlSerializer cache
                Level.BuildLevelFromFile( mLevelEntries[0].Path );
            }

            CheckForLevelUnlocks(true);
        }

        /// <summary>
        /// Reset all of the key caching mechanisms.
        /// </summary>
        public void ResetKeyCache()
        {
            mPrevPressingSelect = true;
            mPrevNagivationDelta = 0;
        }

        public void Update(float deltaTime)
        {
            GamePadState primaryPadState = GamePad.GetState(0);
            
            // check for unlocks
            CheckForLevelUnlocks(false);

            // check for final level auto launch
            if ( mShouldLaunchFinalLevel )
            {
                mShouldLaunchFinalLevel = false;
                GameState.GameMode = GameState.Mode.Game;
                GameState.Level = Level.BuildLevelFromFile( mLevelEntries[ mLevelEntries.Count - 1 ].Path );
                AudioManager.PlayCursorSelectSFX();
                AudioManager.PlayCitySong();
                return;
            }

            // check for level selection
            bool isSelectButtonDown = primaryPadState.IsButtonDown(Buttons.A);
            if ( !mPrevPressingSelect && isSelectButtonDown )
            {   
                if ( mSelectedIdx < mLevelEntries.Count )
                {
                    GameState.GameMode = GameState.Mode.Game;
                    GameState.Level = Level.BuildLevelFromFile( mLevelEntries[mSelectedIdx].Path );
                    AudioManager.PlayCursorSelectSFX();

                    if (mSelectedIdx == 0)
                    {
                        AudioManager.PlaySpaceSong();
                    }

                    if ( mSelectedIdx == 1)
                    {
                        AudioManager.PlayOceanSong();
                    }

                    if (mSelectedIdx == 2)
                    {
                        AudioManager.PlayMaxSong();
                    }

                    if (mSelectedIdx == 3)
                    {
                        AudioManager.PlayCitySong();
                    }
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
                    int prevSelectedIdx = mSelectedIdx;

                    do
                    {
                        mSelectedIdx = ( mSelectedIdx + nagivationDelta + mLevelEntries.Count ) % mLevelEntries.Count;
                    }
                    while ( !mLevelEntries[mSelectedIdx].Unlocked );

                    if ( prevSelectedIdx != mSelectedIdx )
                    {
                        AudioManager.PlayCursorMoveSFX();
                    }

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
                if ( mLevelEntries[i].Unlocked )
                {
                    Color color = GetColorForIndex(i);
                    spriteBatch.DrawString(GameState.PixelFont, mLevelEntries[i].Label, levelEntryPosition, color);
                }
                levelEntryPosition += new Vector2(0, GameState.PixelFont.LineSpacing + 5);
            }

            // draw the time
            string timeString = DateTime.Now.ToString("h:mm:ss tt");

            Vector2 stringSize = new Vector2(100, GameState.PixelFont.LineSpacing);
            Vector2 timeStringPos = new Vector2(GameConstants.RenderTargetWidth, GameConstants.RenderTargetHeight) - stringSize;
            spriteBatch.DrawString(GameState.PixelFont, timeString, timeStringPos, Color.Red);
        }

        /// <summary>
        /// Get the color for the given menu index.
        /// </summary>
        private Color GetColorForIndex(int idx)
        {
            if ( idx == mSelectedIdx )
            {
                return Color.Red;
            }
            else if ( mLevelEntries[idx].Unlocked )
            {
                return Color.White;
            }
            else
            {
                return Color.LightGray;
            }
        }

        /// <summary>
        /// For for unlocks in our level entries.
        /// </summary>
        private void CheckForLevelUnlocks(bool initialUnlock)
        {
            bool playUnlockSound = false;

            int timeNow = GameUtility.GetCurrentTimeValue();

            for ( int i = 0; i < mLevelEntries.Count; ++i )
            {
                LevelEntry e = mLevelEntries[i];

                if ( !e.Unlocked )
                {
                    if ( !string.IsNullOrEmpty(e.UnlockTime) )
                    {
                        int timeQuery = GameUtility.GetQueryTimeValue(e.UnlockTime);
                        if ( timeNow >= timeQuery )
                        {
                            e.Unlocked = true;
                            if ( !initialUnlock )
                            {
                                playUnlockSound = true;

                                // auto-launch last level
                                if ( i == mLevelEntries.Count - 1 )
                                {
                                    mShouldLaunchFinalLevel = true;
                                }
                            }
                        }
                    }
                }
            }

            if ( playUnlockSound )
            {
                AudioManager.PlayLevelUnlockedSFX();
            }
        }
    }
}
