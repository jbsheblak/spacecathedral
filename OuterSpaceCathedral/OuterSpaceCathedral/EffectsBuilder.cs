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

            GameState.Level.AddEffect(new Effect(position, new AnimFrameManager(1/15f ,animFrames), 0.333f));
        }
    }
}
