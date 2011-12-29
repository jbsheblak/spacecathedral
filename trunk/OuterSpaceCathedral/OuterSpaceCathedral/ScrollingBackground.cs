using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class ScrollingBackground : Background
    {
        Vector2 velocity = new Vector2(0, 200);
        Vector2 positionOffset;

        public ScrollingBackground(Vector2 velocity)
        {
            this.velocity = velocity;
            sourceRectangle = new Rectangle(544, 0, 480, 540);
            this.color = color * 0.6f;
        }

        public override void Update(float deltaTime)
        {
            positionOffset += velocity * deltaTime;

            position.Y = positionOffset.Y % GameConstants.RenderTargetHeight;

            base.Update(deltaTime);
        }
    }
}
