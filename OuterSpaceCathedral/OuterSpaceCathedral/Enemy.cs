using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    internal class Enemy : GameObject
    {
        private const int skSpriteWidth  = 16;
        private const int skSpriteHeight = 16;
        
        private Vector2 mVelocity = Vector2.Zero;

        public Enemy(Vector2 initialPosition, Vector2 velocity)
        {
            sourceRectangle = new Rectangle(0, 0, skSpriteWidth, skSpriteHeight);
            position = initialPosition;
            mVelocity = velocity;
        }

        public override void Update(float deltaTime)
        {
            position += (mVelocity * deltaTime);

            RemoveIfOffscreen();
        }

        private void RemoveIfOffscreen()
        {
            if ( !GameConstants.RenderTargetRect.Intersects(PositionRectangle) )
            {
                RemoveObject();
            }
        }
    }
}
