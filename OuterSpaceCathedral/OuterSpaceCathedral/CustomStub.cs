using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public class CustomStub : GameObject
    {
        private string mCustomId;

        bool flashScreen = false;

        List<Firework> fireworks = new List<Firework>();

        float mElapsed;

        float flashFadeElapsed = 0.1f;

        int fireworksPopped = 0;

        float fireWorkTimer;
        float fireWorkInterval = 0.1f;

        public CustomStub( string customId )
        {
            mCustomId = customId;
            AudioManager.PlayNewYearSong();
        }

        public override void Update(float deltaTime)
        {
            

            if (flashScreen)
            {
                if (flashFadeElapsed < 0f)
                {
                    flashScreen = false;
                }

                flashFadeElapsed -= deltaTime;
            }

            foreach (Firework fw in fireworks)
            {
                fw.Update(deltaTime);
            }

            for (int i = fireworks.Count - 1; i >= 0; i--)
            {
                if (fireworks[i].Dead)
                {
                    fireworks.RemoveAt(i);
                }
            }

            if (fireworksPopped == 150)
            {
                fireWorkInterval = 1.5f;
            }

            if (fireworksPopped == 100)
            {
                fireWorkInterval = 0.5f;
            }

            if (fireWorkTimer > fireWorkInterval)
            {
                PopFirework();
                fireWorkTimer -= fireWorkInterval;
            }

            fireWorkTimer += deltaTime;

            mElapsed += deltaTime;
        }

        public void PopFirework()
        {
            Vector2 location = new Vector2(GameUtility.Random.Next(480), GameUtility.Random.Next(270));
            Color color = new Color(GameUtility.Random.Next(100, 256), GameUtility.Random.Next(100, 256), GameUtility.Random.Next(100, 256));

            fireworks.Add(new Firework(location, color));

            AudioManager.PlayFireworkPopSFX();

            flashScreen = true;

            fireworksPopped++;
            flashFadeElapsed = 0.1f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {   
            if (flashScreen)
            {
                spriteBatch.Draw(GameState.SpriteSheet, GameConstants.RenderTargetRect, new Rectangle(0, 0, 32, 32), Color.White * (flashFadeElapsed / 0.1f) * 0.3f);
            }

            foreach (Firework fw in fireworks)
            {
                fw.Draw(spriteBatch);
            }
        }
    }

    class Firework
    {
        List<Spark> sparks = new List<Spark>();

        float lifeTime = 1.5f;
        float alpha = 1f;

        public Firework(Vector2 location, Color color)
        {
            for (int i = 0; i < 100; i++)
            {
                sparks.Add(new Spark(location, new Vector2(GameUtility.Random.Next(-100, 100), GameUtility.Random.Next(-100, 100)), color));
            }
        }

        public void Update(float deltaTime)
        {
            foreach (Spark s in sparks)
            {
                s.Update(deltaTime);
            }

            lifeTime -= deltaTime;

            if (lifeTime < 0)
            {
                Dead = true;
            }
            else if (lifeTime < 0.5f)
            {
                alpha = lifeTime / 0.5f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Spark s in sparks)
            {
                s.Draw(spriteBatch, alpha);
            }
        }

        public bool Dead { get; private set;}
    }

    class Spark
    {
        Rectangle fireworkSpark = new Rectangle(160, 8, 3, 3);

        Vector2 location, velocity;

        Color color;

        public Spark(Vector2 location, Vector2 velocity, Color color)
        {
            this.location = location;
            this.velocity = velocity;

            this.color = color;
        }

        public void Update(float deltaTime)
        {
            location += velocity * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch, float alpha)
        {
            spriteBatch.Draw(GameState.SpriteSheet, new Rectangle((int)location.X, (int)location.Y, 3, 3), fireworkSpark, color * alpha);
        }
    }
}
