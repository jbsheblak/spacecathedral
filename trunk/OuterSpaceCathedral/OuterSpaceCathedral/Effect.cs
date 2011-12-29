using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public class Effect : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;

        private AnimFrameManager mAnimMgr = null;
        private float            mLifeTime = 0;
        
        public Effect(Vector2 initialPosition, AnimFrameManager animFrameManager, float lifeTime, Color color)
        {
            mAnimMgr = animFrameManager;
            mLifeTime = lifeTime;

            sourceRectangle = mAnimMgr.FrameRectangle;
            position = initialPosition;
            this.color = color;
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
