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

        public ScrollingBackground(Rectangle sourceRectangle, Vector2 velocity, Color color)
        {
            position = GameConstants.RenderTargetCenter;
            this.velocity = velocity;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
        }

        public override void Update(float deltaTime)
        {
            positionOffset += velocity * deltaTime;

            if (velocity.X == 0)
            {
                position.X = GameConstants.RenderTargetCenter.X;
            }
            else
            {
                position.X = positionOffset.X % GameConstants.RenderTargetWidth + GameConstants.RenderTargetWidth;
            }
            //position.Y = 270;

            base.Update(deltaTime);
        }
    }
}
