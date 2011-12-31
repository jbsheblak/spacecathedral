using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public class CustomStub : GameObject
    {
        private string mCustomId;

        public CustomStub( string customId )
        {
            mCustomId = customId;
        }

        public override void Update(float deltaTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {   
        }
    }
}
