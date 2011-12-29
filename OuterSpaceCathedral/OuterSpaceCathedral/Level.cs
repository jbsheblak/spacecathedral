using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace OuterSpaceCathedral
{
    class Level
    {
        List<Background> backgrounds = new List<Background>();
        List<Player> players = new List<Player>();
        List<Enemy> enemies = new List<Enemy>();
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();
        List<Effect> mEffects = new List<Effect>();
        
        static int mEnemyWave = 2;

        private static Enemy BuildCircularEnemy(Vector2 initialPosition, float radius, float initialRotationDegrees)
        {
            float targetX = radius * (float)Math.Cos( initialRotationDegrees * Math.PI / 180.0f );
            float targetY = radius * (float)Math.Sin( initialRotationDegrees * Math.PI / 180.0f );
            Vector2 target = initialPosition + new Vector2(targetX, targetY);

            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>()
            {
                new EnemyMoveToLocationStrategy(initialPosition, target, 100),
                new EnemyParentedMovementStrategy( new EnemyFixedMovementStrategy(initialPosition), new EnemyCircularMovementStrategy(180, initialRotationDegrees, radius) ),
            };
            
            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }

        private Enemy BuildLinearEnemy( Vector2 initialPosition, Vector2 targetPosition, float toTargetSpeed, Vector2 linearVelocity, float goTime)
        {
            float distToTravel = (targetPosition - initialPosition).Length();
            float timeToTarget = distToTravel / toTargetSpeed;            
            float waitTime = goTime - timeToTarget;

            // move to position, wait, then go...
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>()
            {
                new EnemyMoveToLocationStrategy(initialPosition, targetPosition, toTargetSpeed),
                new EnemyTimeDelayedMovementStrategy(waitTime),
                new EnemyLinearMovementStrategy(initialPosition, linearVelocity)
            };

            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }

        private Enemy BuildWaveEnemy( Vector2 initialPosition )
        {
            Vector2 linearVelocity = new Vector2(-100, 0);
            Vector2 waveDisplacement = new Vector2(0, 35);

            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>()
            {
                new EnemyParentedMovementStrategy( new EnemyLinearMovementStrategy(initialPosition, linearVelocity), new EnemyWaveMovementStrategy(waveDisplacement, 360, 0) )
            };
            
            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }

        private void BuildEnemyWave(int enemyWaveIdx)
        {
            switch ( enemyWaveIdx % 3 )
            {
                case 0:
                    {
                        int skSpread = 75;

                        // circular
                        Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth/2, GameConstants.RenderTargetHeight/2 );

                        for ( int i = 0; i < 8; ++i )
                        {
                            enemies.Add( BuildCircularEnemy( initialPosition, 75, 360/8 * i ) );
                        }
                    }
                    break;

                case 1:
                    {
                        // linear
                        Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth - 32, 32 );                        
                        Vector2 enemyVelocity = new Vector2(-200, 0);
                        float enemyToTargetSpeed = 100;
                        int enemyCount = 8;
                        int maxDistance = ( enemyCount - 1 ) * 32;
                        float waitTime = Math.Abs( maxDistance / enemyToTargetSpeed );
                        for ( int i = 0; i < enemyCount; ++i )
                        {
                            enemies.Add( BuildLinearEnemy( initialPosition, initialPosition + new Vector2(0, 32 * i), enemyToTargetSpeed, enemyVelocity, waitTime) );
                        }
                    }
                    break;

                case 2:
                    {
                        for ( int i = 0; i < 8; ++i )
                        {
                            Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth - 32, (i+1) * 32 );
                            enemies.Add( BuildWaveEnemy(initialPosition) );
                        }
                    }
                    break;
            }
        }
        
        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            BuildEnemyWave(mEnemyWave);

            backgrounds.Add(new SolidColorBackground(new Color(33, 73, 90)));
            //backgrounds.Add(new ScrollingBackground(new Vector2(0, 200)));
            //backgrounds.Add(new ScrollingBackground(new Vector2(0, 100)));
        }

        public virtual void Update(float deltaTime)
        {
            //Update Objects
            backgrounds.ForEach( x => x.Update(deltaTime) );
            players.ForEach( x => x.Update(deltaTime) );
            enemies.ForEach( x => x.Update(deltaTime) );
            playerBullets.ForEach( x => x.Update(deltaTime) );
            mEffects.ForEach( x => x.Update(deltaTime) );

            //Check collisions
            CheckCollisions(enemies, playerBullets, true);
            CheckCollisions(players, enemies, false);

            //Remove dead objects
            players.RemoveAll(x => x.ReadyForRemoval());
            enemies.RemoveAll(x => x.ReadyForRemoval());
            playerBullets.RemoveAll(x => x.ReadyForRemoval());
            mEffects.RemoveAll(x => x.ReadyForRemoval());

            // TEMP TEMP TEMP, to be removed
            if ( enemies.Count == 0 )
            {
                // clear all bullets so we can see the next pattern...
                playerBullets.ForEach( x => x.RemoveObject() );
                playerBullets.RemoveAll( x => x.ReadyForRemoval() );

                ++mEnemyWave;
                BuildEnemyWave(mEnemyWave);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            backgrounds.ForEach( x => x.Draw(spriteBatch) );
            enemies.ForEach( x => x.Draw(spriteBatch) );
            mEffects.ForEach( x => x.Draw(spriteBatch) );
            playerBullets.ForEach( x => x.Draw(spriteBatch) );
            players.ForEach( x => x.Draw(spriteBatch) );
        }

        public List<Bullet> PlayerBullets
        {
            get
            {
                return playerBullets;
            }
        }

        public void AddEffect(Effect effect)
        {
            mEffects.Add(effect);
        }

        #region Private

        /// <summary>
        /// Check to see if a list of target objects intersect a list of dangerous objects.
        /// Intersecting target objects will be removed.
        /// </summary>
        private void CheckCollisions(IEnumerable<GameObject> targetObjects, IEnumerable<GameObject> dangerObjects, bool removeDangerObjectOnImpact)
        {
            foreach ( GameObject target in targetObjects )
            {
                foreach ( GameObject danger in dangerObjects )
                {
                    // check for bound intersection
                    if ( target.PositionRectangle.Intersects(danger.PositionRectangle) )
                    {
                        target.RemoveObject();
                        if ( removeDangerObjectOnImpact )
                        {
                            danger.RemoveObject();
                        }
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
