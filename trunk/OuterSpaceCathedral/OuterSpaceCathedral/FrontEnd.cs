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

        public FrontEnd()
        {
            foreach ( string path in Directory.GetFiles( "content\\levels", "*.xml" ) )
            {
                mLevelEntries.Add(new LevelEntry(Path.GetFileNameWithoutExtension(path), path));
            }
        }

        public void Update(float deltaTime)
        {
            GamePadState primaryPadState = GamePad.GetState(0);

            // check for level selection
            if ( primaryPadState.IsButtonDown(Buttons.A) )
            {
                GameState.GameMode = GameState.Mode.Game;
                GameState.Level = new Level();
                return;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = new Vector2(15, 25);

            for ( int i = 0; i < mLevelEntries.Count; ++i )
            {
                Color color = ( i == mSelectedIdx ) ? Color.Red : Color.White;
                spriteBatch.DrawString(GameState.PixelFont, mLevelEntries[i].Label, position, color);
                position += new Vector2(0, GameState.PixelFont.LineSpacing + 5);
            }
        }
    }
}
