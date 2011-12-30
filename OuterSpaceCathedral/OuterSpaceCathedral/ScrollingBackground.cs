using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class ScrollingBackground : Background
    {
        Vector2 velocity = new Vector2(-200, 0);
        Vector2 positionOffset;

        public ScrollingBackground(Vector2 velocity)
        {
            position = GameConstants.RenderTargetCenter;
            this.velocity = velocity;
            sourceRectangle = new Rectangle(0, 754, 960, 540);
            this.color = color * 0.6f;
        }

        public override void Update(float deltaTime)
        {
            positionOffset += velocity * deltaTime;
            
            position.X = positionOffset.X % GameConstants.RenderTargetWidth + GameConstants.RenderTargetWidth;
            position.Y = 270;

            base.Update(deltaTime);
        }
    }
}
