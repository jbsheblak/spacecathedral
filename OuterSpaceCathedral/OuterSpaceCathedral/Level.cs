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
        
        static int mTempEnemyIndex = 0;
        static Vector2 mEnemyVelocity = new Vector2(0, 100);

        private static Enemy BuildCircularEnemy(Vector2 initialPosition, Vector2 initialTarget)
        {
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>()
            {
                new EnemyMoveToLocationStrategy(initialPosition, initialTarget, 100),
                new EnemyCircularMovementStrategy(initialPosition, 180)
            };
            
            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }

        private void BuildCircularPack(Vector2 initialPosition)
        {
            int skSpread = 75;

            enemies.Add( BuildCircularEnemy( initialPosition, initialPosition + new Vector2(-skSpread,  0) ) );
            enemies.Add( BuildCircularEnemy( initialPosition, initialPosition + new Vector2(+skSpread,  0) ) );
            enemies.Add( BuildCircularEnemy( initialPosition, initialPosition + new Vector2( 0, -skSpread) ) );
            enemies.Add( BuildCircularEnemy( initialPosition, initialPosition + new Vector2( 0, +skSpread) ) );
        }

        private static Enemy BuildEnemy(Vector2 initialPosition)
        {
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>()
            {
                new EnemyMoveToLocationStrategy(initialPosition, initialPosition + new Vector2(0, 25), 20),
                new EnemyCircularMovementStrategy(initialPosition, 180)
            };
            
            IEnemyMovementStrategy strat = new EnemyCompositeMovementStrategy(strats);
            return new Enemy(strat);
        }

        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            BuildCircularPack(new Vector2( GameConstants.RenderTargetWidth/2, GameConstants.RenderTargetHeight/2 ));
            //enemies.Add(BuildEnemy(new Vector2( GameConstants.RenderTargetWidth/2, GameConstants.RenderTargetHeight/2 ) ) );

            backgrounds.Add(new SolidColorBackground(Color.CornflowerBlue));
            backgrounds.Add(new ScrollingBackground(new Vector2(0, 200)));
            backgrounds.Add(new ScrollingBackground(new Vector2(0, 100)));
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
                ++mTempEnemyIndex;
                enemies.Add( BuildEnemy( new Vector2( (mTempEnemyIndex % 5) * GameConstants.RenderTargetWidth / 5, GameConstants.RenderTargetHeight/2 ) ) );
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
