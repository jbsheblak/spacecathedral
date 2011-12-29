using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
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

        public void RemoveObject()
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
