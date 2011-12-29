using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    internal class Enemy : GameObject
    {
        private const int skSpriteWidth  = 16;
        private const int skSpriteHeight = 16;

        public Enemy(Vector2 initialPosition)
        {
            sourceRectangle = new Rectangle(0 , 0, skSpriteWidth, skSpriteHeight);
            position = initialPosition;
        }
    }
}
