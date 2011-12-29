using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    /// <summary>
    /// Utility class for managing animation key frame updates.
    /// </summary>
    class AnimFrameManager
    {
        private int                 mFrameIndex;
        private List<Rectangle>     mFrames;
        private float               mTimePerFrame;
        private float               mTimeAccumulation;

        public AnimFrameManager(float timePerFrame, List<Rectangle> frameRectangles)
        {
            mFrameIndex = 0;
            mFrames = frameRectangles;
            mTimePerFrame = timePerFrame;
            mTimeAccumulation = 0.0f;
        }
        
        /// <summary>
        /// Tick the anim manager. This may cause the class to move to a new anim frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            mTimeAccumulation += deltaTime;
            if ( mTimeAccumulation > mTimePerFrame )
            {
                mTimeAccumulation -= mTimePerFrame;
                mFrameIndex = ( mFrameIndex + 1 ) % mFrames.Count;
            }
        }

        /// <summary>
        /// Sprite sheet rectangle for the current frame.
        /// </summary>
        public Rectangle FrameRectangle
        {
            get { return mFrames[mFrameIndex]; }
        }
    };

    class GameObject
    {
        protected Vector2 position;             //Object's center in the world
        protected Rectangle sourceRectangle;    //Rectangle Location of object's art in the spriteSheet
        protected Color color = Color.White;    //Color to draw the object in
        
        private bool markForRemoval;            //Private variable used to tell the level when it is okay remove this object its list

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, PositionRectangle, sourceRectangle, color);
        }

        public virtual void CollisionReaction()
        {
        }

        public virtual void RemoveObject()
        {
            markForRemoval = true;
        }

        public bool ReadyForRemoval()
        {
            return markForRemoval;
        }

        public Rectangle PositionRectangle
        {
            get
            {
                return new Rectangle((int)(position.X - sourceRectangle.Width / 2), (int)(position.Y - sourceRectangle.Height / 2), sourceRectangle.Width, sourceRectangle.Height);
            }
        }
    }
}
