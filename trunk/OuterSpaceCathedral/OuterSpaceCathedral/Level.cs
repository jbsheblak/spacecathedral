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
        static Vector2 mEnemyVelocity = new Vector2(0, 100);

        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            enemies.Add(new Enemy(new Vector2( GameConstants.RenderTargetWidth/2, 0 ), mEnemyVelocity ) );

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
            CheckCollisions(enemies, playerBullets);
            CheckCollisions(players, enemies);

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
                enemies.Add( new Enemy( new Vector2( (mTempEnemyIndex % 5) * GameConstants.RenderTargetWidth / 5, 0 ), mEnemyVelocity ) );
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
        /// Check to see if a list of target objects intersect a list of dangerous objects.
        /// Intersecting target objects will be removed.
        /// </summary>
        void CheckCollisions(IEnumerable<GameObject> targetObjects, IEnumerable<GameObject> dangerObjects)
        {
            foreach ( GameObject target in targetObjects )
            {
                foreach ( GameObject danger in dangerObjects )
                {
                    // check for bound intersection
                    if ( target.PositionRectangle.Intersects(danger.PositionRectangle) )
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
