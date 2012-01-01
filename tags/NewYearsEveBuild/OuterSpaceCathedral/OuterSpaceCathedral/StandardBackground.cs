using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class StandardBackground : Background
    {
        public StandardBackground(Rectangle sourceRectangle)
        {
            this.sourceRectangle = sourceRectangle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, GameConstants.RenderTargetRect, sourceRectangle, color);

            base.Draw(spriteBatch);
        }
    }
}
