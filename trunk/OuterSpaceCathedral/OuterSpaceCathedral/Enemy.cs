using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    internal class Enemy : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;
        
        private Vector2 mVelocity = Vector2.Zero;

        public Enemy(Vector2 initialPosition, Vector2 velocity)
        {
            sourceRectangle = new Rectangle(32, 0, skSpriteWidth, skSpriteHeight);
            position = initialPosition;
            mVelocity = velocity;
        }

        public override void Update(float deltaTime)
        {
            position += (mVelocity * deltaTime);

            RemoveIfOffscreen();
        }

        public override void RemoveObject()
        {
            base.RemoveObject();
            GameState.Level.AddEffect(new Effect(position));
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
