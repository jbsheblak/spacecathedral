﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace OuterSpaceCathedral
{
    class Player : GameObject
    {
        const float maxShotAngle = 80f;
        const float playerBounceSpeed = 10f;
        const float playerBounceDistance = 4f;
        const float invincibilityTimeTotal = 1.5f;

        bool invincible = true;
        float invincibilityTimeElapsed;

        PlayerIndex playerIndex;
        float movementSpeed = 200;
        Vector2 invertY = new Vector2(1, -1);
        float fireInterval = 0.1f;

        Vector2 nonFloatPosition;

        float fireIntervalCounterElapsed = 0f;

        float bulletStreamAngleScalar = 0f;
        bool isFiring;

        public Player(PlayerIndex pi)
        {
            playerIndex = pi;
            sourceRectangle = new Rectangle(32 + 32 * (int)playerIndex, 0, 32, 32);
            position = new Vector2(15, 35 + (int)playerIndex * 50);
        }

        public override void CollisionReaction(CollisionMessage collisionMessage)
        {
            if (!invincible)
            {
                AudioManager.PlayPlayerDeathSFX();
                EffectsBuilder.BuildPlayerDeathExplosion(position);
                GameState.Level.PlayerStatsManager.Players[(int)playerIndex].Deaths++;
                RemoveObject();
            }

            base.CollisionReaction(collisionMessage);
        }

        public override void Update(float deltaTime)
        {
            ControllerInput.IController controller = GameState.GetController(playerIndex);
            
            Vector2 movement = controller.GetAnalogDirection(ControllerInput.AnalogAction.Game_Move) * invertY * movementSpeed;

            bool fireInput = controller.GetButtonState(ControllerInput.ButtonAction.Game_Fire) == ButtonState.Pressed;

            if (fireInput && fireIntervalCounterElapsed >= fireInterval)
            {
                bulletStreamAngleScalar = 1 - controller.GetAnalogDirection(ControllerInput.AnalogAction.Game_StreamControl).X;
                DefaultFire(movement);
            }

            if (fireInput)
            {
                isFiring = true;
            }
            else
            {
                isFiring = false;
            }

            //position += movement * deltaTime;
            nonFloatPosition += movement * deltaTime;

            int halfWidth  = sourceRectangle.Width  / 2;
            int halfHeight = sourceRectangle.Height / 2;

            // clamp position to be onscreen
            nonFloatPosition.X = Math.Max(halfWidth, Math.Min(nonFloatPosition.X, GameConstants.RenderTargetWidth - halfWidth));
            nonFloatPosition.Y = Math.Max(halfHeight, Math.Min(nonFloatPosition.Y, GameConstants.RenderTargetHeight - halfHeight));

            position = nonFloatPosition + new Vector2(0, (float)Math.Sin(GameState.Level.ElapsedLevelTime * playerBounceSpeed) * playerBounceDistance);

            if (fireIntervalCounterElapsed < fireInterval)
            {
                fireIntervalCounterElapsed += deltaTime;
            }

            //Handle spawn invincibility
            if (invincible)
            {
                if (invincibilityTimeElapsed >= invincibilityTimeTotal)
                {
                    invincible = false;
                    color = Color.White;
                }
                else
                {
                    color = Color.White * (((float)Math.Sin(GameState.Level.ElapsedLevelTime * 50) + 1) / 2f);
                    invincibilityTimeElapsed += deltaTime;
                }
            }
        }

        public void DefaultFire(Vector2 playerMoveSpeed)
        {
        #if true
            playerMoveSpeed = Vector2.Zero;
        #endif

            GameState.Level.PlayerBullets.Add(Bullet.BuildPlayerBullet(nonFloatPosition, new Vector2(320, 0 * bulletStreamAngleScalar) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(Bullet.BuildPlayerBullet(nonFloatPosition + new Vector2(0, -3), new Vector2(310, -maxShotAngle / 2 * bulletStreamAngleScalar) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(Bullet.BuildPlayerBullet(nonFloatPosition + new Vector2(0, 3), new Vector2(310, maxShotAngle / 2 * bulletStreamAngleScalar) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(Bullet.BuildPlayerBullet(nonFloatPosition + new Vector2(0, -6), new Vector2(300, -maxShotAngle * bulletStreamAngleScalar) + playerMoveSpeed, playerIndex));
            GameState.Level.PlayerBullets.Add(Bullet.BuildPlayerBullet(nonFloatPosition + new Vector2(0, 6), new Vector2(300, maxShotAngle * bulletStreamAngleScalar) + playerMoveSpeed, playerIndex));
            
            fireIntervalCounterElapsed = 0f;
        }

        public bool IsFiring
        {
            get
            {
                return isFiring;
            }
        }
    }
}
