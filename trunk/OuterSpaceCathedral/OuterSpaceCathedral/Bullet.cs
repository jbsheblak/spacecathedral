using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public enum BulletFirer { PlayerOne, PlayerTwo, PlayerThree, PlayerFour, Enemy }

    /// <summary>
    /// Base bullet class.
    /// </summary>
    public class Bullet : GameObject
    {
        protected Vector2 mVelocity = Vector2.Zero;
        
        public static Bullet BuildPlayerBullet(Vector2 initialPosition, Vector2 velocity, PlayerIndex playerIndex)
        {
            return new Bullet(initialPosition, velocity, GameConstants.GetColorForPlayer(playerIndex), new Rectangle(160, 0, 4, 4), (BulletFirer)playerIndex);
        }

        public static Bullet BuildEnemyBullet(Vector2 initialPosition, Vector2 velocity)
        {
            return new Bullet(initialPosition, velocity, Color.White, new Rectangle(164, 0, 6, 6), BulletFirer.Enemy);
        }

        public Bullet(Vector2 initialPosition, Vector2 velocity, Color bulletColor, Rectangle spriteRect, BulletFirer bulletFirer)
        {
            position = initialPosition;
            mVelocity = velocity;
            color = bulletColor;
            sourceRectangle = spriteRect;

            CollisionMessage = new BulletCollisionMessage(bulletFirer);
        }
        
        public override void Update(float deltaTime)
        {
            position += mVelocity * deltaTime;

            // check if bullet is still onscreen
            if ( !GameConstants.RenderTargetRect.Intersects(PositionRectangle) )
            {
                RemoveObject();
            }

            base.Update(deltaTime);
        }

        public override void RemoveObject()
        {
            EffectsBuilder.BuildBulletHitEvaporation(position, color);
            base.RemoveObject();
        }
    }
}
