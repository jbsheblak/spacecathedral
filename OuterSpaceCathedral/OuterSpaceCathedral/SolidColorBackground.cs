using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class SolidColorBackground : Background
    {
        public SolidColorBackground(Color color)
        {
            sourceRectangle = new Rectangle(80, 0, 8, 8);
            this.color = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(0, 0, 480, 270), sourceRectangle, color);

            //base.Draw(spriteBatch);
        }
    }
}
