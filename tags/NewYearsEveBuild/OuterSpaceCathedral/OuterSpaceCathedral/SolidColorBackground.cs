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
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            this.color = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, GameConstants.RenderTargetRect, sourceRectangle, color);

            //base.Draw(spriteBatch);
        }
    }
}
