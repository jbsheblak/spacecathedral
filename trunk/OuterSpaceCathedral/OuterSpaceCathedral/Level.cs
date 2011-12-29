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

        static int mTempEnemyIndex = 0;

        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            enemies.Add(new Enemy(new Vector2( GameConstants.RenderTargetWidth/2, 0 ) ) );

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

            //Check collisions
            CheckBulletCollisions(enemies, playerBullets);

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


            // TEMP TEMP TEMP, to be removed
            if ( enemies.Count == 0 )
            {
                ++mTempEnemyIndex;
                enemies.Add( new Enemy( new Vector2( (mTempEnemyIndex % 5) * GameConstants.RenderTargetWidth / 5, 0 ) ) );
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

        #region Private

        /// <summary>
        /// Check if a list of bullets collides with a list of target objects.
        /// All colliding targets will be marked for removal.
        /// </summary>
        /// <param name="targetObjects">Non-null list of target objects.</param>
        /// <param name="bullets">Non-null list of bullet objects.</param>
        void CheckBulletCollisions(IEnumerable<GameObject> targetObjects, IEnumerable<GameObject> bullets)
        {
            foreach ( GameObject target in targetObjects )
            {
                foreach ( GameObject bullet in bullets )
                {
                    // check for bound intersection
                    if ( target.PositionRectangle.Intersects(bullet.PositionRectangle) )
                    {
                        target.RemoveObject();
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
