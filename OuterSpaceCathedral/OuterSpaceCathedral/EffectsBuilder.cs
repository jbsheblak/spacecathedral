using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OuterSpaceCathedral
{
    static class EffectsBuilder
    {
        public static void BuildExplosion(Vector2 position)
        {
            List<Rectangle> animFrames = new List<Rectangle>()
            {
                GameConstants.CalcRectFor32x32Sprite(1, 0),
                GameConstants.CalcRectFor32x32Sprite(1, 1),
                GameConstants.CalcRectFor32x32Sprite(1, 2),
                GameConstants.CalcRectFor32x32Sprite(1, 3),
                GameConstants.CalcRectFor32x32Sprite(1, 4),
            };

            GameState.Level.AddEffect(new Effect(position, new AnimFrameManager(1/15f ,animFrames), 0.333f, Color.White));
        }

        public static void BuildBulletHitEvaporation(Vector2 position, Color bulletColor)
        {
            List<Rectangle> animFrames = new List<Rectangle>()
            {
                new Rectangle(176, 0, 8, 8),
                new Rectangle(184, 0, 8, 8),
                new Rectangle(176, 8, 8, 8),
                new Rectangle(184, 8, 8, 8),
            };

            GameState.Level.AddEffect(new Effect(position, new AnimFrameManager(1 / 30f, animFrames), 0.1333f, bulletColor));
        }
    }
}
