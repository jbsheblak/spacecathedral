using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class Player : GameObject
    {
        PlayerIndex playerIndex;
        float movementSpeed = 200;
        Vector2 invertY = new Vector2(1, -1);
        GamePadState oldGamePadState, gamePadState;
        float fireInterval = 0.1f;

        float fireIntervalCounterElapsed = 0f;

        public Player(PlayerIndex pi)
        {
            playerIndex = pi;
            sourceRectangle = new Rectangle(0 + 16 * (int)playerIndex, 0, 16, 16);
            position = new Vector2(15, 35 + (int)playerIndex * 50);
        }

        public override void Update(float deltaTime)
        {
            gamePadState = GamePad.GetState(playerIndex);


            Vector2 movement = gamePadState.ThumbSticks.Left * invertY * movementSpeed;

            if (gamePadState.IsButtonDown(Buttons.A) && fireIntervalCounterElapsed >= fireInterval)
            {
                DefaultFire(movement);
            }

            position += movement * deltaTime;
            
            int halfWidth  = sourceRectangle.Width  / 2;
            int halfHeight = sourceRectangle.Height / 2;

            // clamp position to be onscreen
            position.X = Math.Max( halfWidth,  Math.Min( position.X, GameConstants.RenderTargetWidth  - halfWidth  ) );
            position.Y = Math.Max( halfHeight, Math.Min( position.Y, GameConstants.RenderTargetHeight - halfHeight ) );

            if (fireIntervalCounterElapsed < fireInterval)
            {
                fireIntervalCounterElapsed += deltaTime;
            }

            oldGamePadState = gamePadState;
        }

        public void DefaultFire(Vector2 playerMoveSpeed)
        {
        #if true
            playerMoveSpeed = Vector2.Zero;
        #endif

            GameState.Level.PlayerBullets.Add(new DefaultBullet(position,                       new Vector2(300,  0)     + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(new DefaultBullet(position + new Vector2(0, -3),  new Vector2(300, -22.5f) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(new DefaultBullet(position + new Vector2(0,  3),  new Vector2(300,  22.5f) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(new DefaultBullet(position + new Vector2(0, -6),  new Vector2(300, -45)    + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(new DefaultBullet(position + new Vector2(0,  6),  new Vector2(300,  45)    + playerMoveSpeed, playerIndex));
            
            fireIntervalCounterElapsed = 0f;
        }
    }
}
