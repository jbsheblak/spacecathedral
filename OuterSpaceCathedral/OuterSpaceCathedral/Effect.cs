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
                GameConstants.CalcRectFor32x32Sprite(0, 2),
                GameConstants.CalcRectFor32x32Sprite(1, 2),
            };

            mAnimMgr = new AnimFrameManager(1/10.0f, animFrames);       
            mLifeTime = 4.0f;

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
