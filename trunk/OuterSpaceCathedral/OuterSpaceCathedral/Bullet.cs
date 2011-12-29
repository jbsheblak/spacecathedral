﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public class Bullet : GameObject
    {
        protected Vector2 velocity = new Vector2(0, -200);

        public Bullet(Vector2 initialPosition)
        {
            position = initialPosition;
            sourceRectangle = new Rectangle(160, 0, 4, 4);
        }

        public override void Update(float deltaTime)
        {
            position += velocity * deltaTime;

            // check if bullet is still onscreen
            if ( !GameConstants.RenderTargetRect.Intersects(PositionRectangle) )
            {
                RemoveObject();
            }

            base.Update(deltaTime);
        }
    }
}
