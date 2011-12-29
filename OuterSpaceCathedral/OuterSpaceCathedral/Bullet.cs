using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class Bullet : GameObject
    {
        protected Vector2 velocity = new Vector2(0, -200);

        public Bullet(Vector2 initialPosition)
        {
            position = initialPosition;
            sourceRectangle = new Rectangle(64, 0, 4, 4);
        }

        public override void Update(float deltaTime)
        {
            position += velocity * deltaTime;

            if (!new Rectangle(0, 0, 480, 270).Intersects(PositionRectangle))
            {
                RemoveObject();
            }

            base.Update(deltaTime);
        }
    }
}
