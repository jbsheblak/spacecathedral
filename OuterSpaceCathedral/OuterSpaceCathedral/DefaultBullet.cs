using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class DefaultBullet : Bullet
    {
        PlayerIndex playerIndex;
        
        public DefaultBullet(Vector2 initialPosition, Vector2 velocity, PlayerIndex playerIndex)
            : base(initialPosition)
        {
            this.velocity = velocity;
            this.playerIndex = playerIndex;
            sourceRectangle = new Rectangle(160, 0, 4, 4);

            switch (playerIndex)
            {
                case PlayerIndex.One:
                    color = Color.LightYellow;
                    break;
                case PlayerIndex.Two:
                    color = Color.Orange;
                    break;
                case PlayerIndex.Three:
                    color = Color.Violet;
                    break;
                case PlayerIndex.Four:
                    color = Color.Cyan;
                    break;
            }
        }

        public override void RemoveObject()
        {
            EffectsBuilder.BuildBulletHitEvaporation(position, color);
            base.RemoveObject();
        }
    }
}
