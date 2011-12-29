using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    internal class Effect : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;

        private AnimFrameManager mAnimMgr = null;
        private float            mLifeTime = 0;
        
        public Effect(Vector2 initialPosition)
        {
            List<Rectangle> animFrames = new List<Rectangle>()
            {
                GameConstants.CalcRectFor32x32Sprite(1, 0),
                GameConstants.CalcRectFor32x32Sprite(1, 1),
                GameConstants.CalcRectFor32x32Sprite(1, 2),
                GameConstants.CalcRectFor32x32Sprite(1, 3),
                GameConstants.CalcRectFor32x32Sprite(1, 4),
            };

            mAnimMgr = new AnimFrameManager(1/15.0f, animFrames);       
            mLifeTime = 0.333f;

            sourceRectangle = mAnimMgr.FrameRectangle;
            position = initialPosition;
        }

        public override void Update(float deltaTime)
        {
            mAnimMgr.Update(deltaTime);
            sourceRectangle = mAnimMgr.FrameRectangle;

            mLifeTime = Math.Max(0, mLifeTime - deltaTime);
            if ( mLifeTime == 0.0f )
            {
                RemoveObject();
            }
        }
    }
}
