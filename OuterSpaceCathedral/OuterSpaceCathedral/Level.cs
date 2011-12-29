using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    class Level
    {
        List<Background> backgrounds = new List<Background>();
        List<Player> players = new List<Player>();
        List<Enemy> enemies = new List<Enemy>();
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();

        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            backgrounds.Add(new SolidColorBackground(Color.CornflowerBlue));
            backgrounds.Add(new ScrollingBackground(new Vector2(0, 200)));
            backgrounds.Add(new ScrollingBackground(new Vector2(0, 100)));
        }

        public virtual void Update(float deltaTime)
        {
            //Update Objects
            foreach (Background background in backgrounds)
            {
                background.Update(deltaTime);
            }

            foreach (Player player in players)
            {
                player.Update(deltaTime);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(deltaTime);
            }

            foreach (Bullet bullet in playerBullets)
            {
                bullet.Update(deltaTime);
            }

            //Remove objects
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (players[i].ReadyForRemoval())
                {
                    players.RemoveAt(i);
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].ReadyForRemoval())
                {
                    enemies.RemoveAt(i);
                }
            }

            for (int i = playerBullets.Count - 1; i >= 0; i--)
            {
                if (playerBullets[i].ReadyForRemoval())
                {
                    playerBullets.RemoveAt(i);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (Background background in backgrounds)
            {
                background.Draw(spriteBatch);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            foreach (Bullet bullet in playerBullets)
            {
                bullet.Draw(spriteBatch);
            }

            foreach (Player player in players)
            {
                player.Draw(spriteBatch);
            }
        }

        public List<Bullet> PlayerBullets
        {
            get
            {
                return playerBullets;
            }
        }
    }
}
