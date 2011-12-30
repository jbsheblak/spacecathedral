using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    public static class EnemyFactory
    {
        static EnemyFactory()
        {
            ScreenRightMiddle = new Vector2(GameConstants.RenderTargetWidth - 32, GameConstants.RenderTargetHeight/2);
            DefaultEnemyMoveVelocity = new Vector2(-100, 0);
            DefaultFireSpeed = 320;
        }

        private static readonly Vector2 ScreenRightMiddle;
        private static readonly Vector2 DefaultEnemyMoveVelocity;
        private static readonly float   DefaultFireSpeed;
        
        /// <summary>
        /// Build an enemy attack pattern.
        /// </summary>
        /// <param name="actorId">Actor identifier for enemies.</param>
        /// <param name="patternId">Movement pattern identifier.</param>
        /// <param name="attackId">Attack pattern identifier.</param>
        /// <param name="enemiesList">List of outgoing enemies.</param>
        public static void BuildPattern(string actorId, string patternId, string attackId, List<Enemy> enemiesList)
        {
            List<IEnemyMovementStrategy> movementStrategies = new List<IEnemyMovementStrategy>();

            // build movement patterns
            switch ( patternId )
            {
                case "move_center_then_circle":         MoveCenterThenCircle(movementStrategies); break;
                case "line_up_then_move":               LineUpThenMove(movementStrategies); break;
                case "wave_line":                       WaveLine(movementStrategies); break;
                case "uncluttered_line":                UnclutteredLine(movementStrategies); break;
                case "flying_v":                        FlyingV(movementStrategies); break;
                case "snake_wave":                      SnakeWave(movementStrategies); break;
            }

            // build attack pattern
            BuildAttackDelegate attackDelegate = null;
            switch ( attackId )
            {
                case "attack1234":                      attackDelegate = new BuildAttackDelegate(Build_1_2_3_4_Pattern); break;
                case "1sec_periodic":                   attackDelegate = new BuildAttackDelegate(Build_1_sec_periodic); break;
            }   

            // build unit description
            BuildEnemyDelegate buildEnemy = null;
            switch (actorId)
            {
                case "leaf_tron":                       buildEnemy = new BuildEnemyDelegate(BuildLeafTron); break;
                case "anime_punch":                     buildEnemy = new BuildEnemyDelegate(BuildAnimePunch); break;
            }

            // build enemies
            if ( buildEnemy != null )
            {
                for ( int i = 0; i < movementStrategies.Count; ++i )
                {
                    enemiesList.Add( buildEnemy(i, movementStrategies.Count, attackDelegate, movementStrategies[i]) );
                }
            }
        }

        #region Movement Pattern Builders

        // move to the center of screen, then split and rotation around center
        private static void MoveCenterThenCircle( List<IEnemyMovementStrategy> movementStrategies )
        {
            const int enemyCount = 8;
            for ( int i = 0; i < enemyCount; ++i )
            {
                movementStrategies.Add( BuildMoveToLocationThenCircle(ScreenRightMiddle, GameConstants.RenderTargetCenter, 75, 360/enemyCount * i) );
            }
        }

        // line up on right edge of screen then sweep across screen
        private static void LineUpThenMove( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth - 32, 32 );                        
            Vector2 enemyVelocity = new Vector2(-200, 0);
            float enemyToTargetSpeed = 100;
            int enemyCount = 8;
            int maxDistance = ( enemyCount - 1 ) * 32;
            float waitTime = Math.Abs( maxDistance / enemyToTargetSpeed );
            for ( int i = 0; i < enemyCount; ++i )
            {
                movementStrategies.Add( BuildLineUpThenMove( initialPosition, initialPosition + new Vector2(0, 32 * i), enemyToTargetSpeed, enemyVelocity, waitTime) );
            }
        }
       
        // vertical line of wave movers
        private static void WaveLine( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;
            Vector2 waveDisplacement = new Vector2(0, 35);
            float rotRateDegrees = 180.0f;

            for ( int i = 0; i < 8; ++i )
            {
                Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth - 32, (i+1) * 32 );
                movementStrategies.Add( BuildWave(initialPosition, linearVelocity, waveDisplacement, rotRateDegrees, 0) );
            }
        }

        // uncluttered line of movers
        private static void UnclutteredLine(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;
            Vector2 waveDisplacement = new Vector2(0, 35);

            for (int i = 0; i < 8; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 256 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        // Might Ducks flying v
        private static void FlyingV( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;

            float xSpacing = 50;
            float ySpacing = 25;
            float yOffsetFromCenter = 25;

            Vector2 headPosition = new Vector2( GameConstants.RenderTargetWidth + 40, GameConstants.RenderTargetHeight/2 - yOffsetFromCenter );

                    // can't stop the flying v
                    movementStrategies.Add( BuildLinearMove( headPosition + new Vector2(2.0f * xSpacing, 2.0f * ySpacing), linearVelocity ) );    
                movementStrategies.Add( BuildLinearMove( headPosition + new Vector2(1.0f * xSpacing, 1.0f * ySpacing), linearVelocity ) );
            movementStrategies.Add( BuildLinearMove( headPosition, linearVelocity) );
                movementStrategies.Add( BuildLinearMove( headPosition + new Vector2(1.0f * xSpacing, -1.0f * ySpacing), linearVelocity ) );
                    movementStrategies.Add( BuildLinearMove( headPosition + new Vector2(2.0f * xSpacing, -2.0f * ySpacing), linearVelocity ) );
        }

        // line of enemies snaking up/down across screen
        private static void SnakeWave( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;
            Vector2 waveDisplacement = new Vector2(0, GameConstants.RenderTargetHeight/2);
            Vector2 headPosition = new Vector2( GameConstants.RenderTargetWidth + 40, GameConstants.RenderTargetHeight/2 );
            
            float xSpacing = 50;
            float rotSpacing = 25;
            float rotRateDegrees = 180;

            for ( int i = 0; i < 8; ++i )
            {
                movementStrategies.Add( BuildWave(headPosition + new Vector2(i * xSpacing, 0), linearVelocity, waveDisplacement, rotRateDegrees, i * rotSpacing) );
            }
        }
        
        #endregion

        #region Movement Strategy Builders

        private static IEnemyMovementStrategy BuildMoveToLocationThenCircle(Vector2 initialCenter, Vector2 targetCenter, float radius, float initialRotationDegrees)
        {
            // calculate radial position
            float targetX = radius * (float)Math.Cos( initialRotationDegrees * Math.PI / 180.0f );
            float targetY = radius * (float)Math.Sin( initialRotationDegrees * Math.PI / 180.0f );
            Vector2 radialPosition = targetCenter + new Vector2(targetX, targetY);

            // move to position, expand into circle, then rotation around center
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, initialCenter, targetCenter,   100);
            MoveToLocation(strats, targetCenter,  radialPosition, 100);
            MovingRotateAboutPoint(strats, new Vector2(-100, 0), targetCenter, 180, initialRotationDegrees, radius);

            return new EnemyCompositeMovementStrategy(strats);
        }

        private static IEnemyMovementStrategy BuildLineUpThenMove( Vector2 lineUpStartPosition, Vector2 lineUpTargetPosition, float lineUpSpeed, Vector2 moveVelocity, float goTime)
        {
            float distToTravel = (lineUpTargetPosition - lineUpStartPosition).Length();
            float timeToTarget = distToTravel / lineUpSpeed;            
            float waitTime = goTime - timeToTarget;

            // move to position, wait, then go...
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, lineUpStartPosition, lineUpTargetPosition, lineUpSpeed);
            Wait(strats, waitTime);
            MoveLinear(strats, lineUpTargetPosition, moveVelocity);

            return new EnemyCompositeMovementStrategy(strats);
        }

        private static IEnemyMovementStrategy BuildLinearMove( Vector2 initialPosition, Vector2 moveVelocity )
        {
            return new EnemyLinearMovementStrategy(initialPosition, moveVelocity);
        }

        private static IEnemyMovementStrategy BuildWave( Vector2 initialLocation, Vector2 linearVelocity, Vector2 waveDisplacement, float rotRateDegrees, float initialRotDegrees )
        {
            // move in wave pattern linearly
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveWave(strats, initialLocation, linearVelocity, waveDisplacement, rotRateDegrees, initialRotDegrees);
            
            return new EnemyCompositeMovementStrategy(strats);
        }

        #region Commands

        private static void MoveToLocation(List<IEnemyMovementStrategy> strategies, Vector2 initialLocation, Vector2 targetLocation, float moveSpeed)
        {
            strategies.Add( new EnemyMoveToLocationStrategy(initialLocation, targetLocation, moveSpeed) );
        }

        private static void MoveLinear(List<IEnemyMovementStrategy> strategies, Vector2 initialLocation, Vector2 velocity)
        {
            strategies.Add( new EnemyLinearMovementStrategy(initialLocation, velocity) );
        }

        private static void MoveWave(List<IEnemyMovementStrategy> strategies, Vector2 initialLocation, Vector2 velocity, Vector2 waveDisplacement, float rotRateDegrees, float initialRotDegrees)
        {
            strategies.Add( new EnemyParentedMovementStrategy( new EnemyLinearMovementStrategy(initialLocation, velocity), new EnemyWaveMovementStrategy(waveDisplacement, rotRateDegrees, initialRotDegrees) ) );
        }

        private static void FixedRotateAboutPoint(List<IEnemyMovementStrategy> strategies, Vector2 circleCenter, float rotRateDegrees, float initialRotDegrees, float rotRadius)
        {
            strategies.Add( new EnemyParentedMovementStrategy( new EnemyFixedMovementStrategy(circleCenter), new EnemyCircularMovementStrategy(rotRateDegrees, initialRotDegrees, rotRadius) ) );
        }

        private static void MovingRotateAboutPoint(List<IEnemyMovementStrategy> strategies, Vector2 moveVelocity, Vector2 circleCenter, float rotRateDegrees, float initialRotDegrees, float rotRadius)
        {
            strategies.Add( new EnemyParentedMovementStrategy( new EnemyLinearMovementStrategy(circleCenter, moveVelocity), new EnemyCircularMovementStrategy(rotRateDegrees, initialRotDegrees, rotRadius) ) );
        }

        private static void Wait(List<IEnemyMovementStrategy> strategies, float waitTime)
        {
            strategies.Add( new EnemyTimeDelayedMovementStrategy(waitTime) );
        }

        #endregion

        #endregion Movement Strategy Builders

        #region Attack Pattern Builders

        private delegate IEnemyAttackStrategy BuildAttackDelegate( int enemyIndex, int enemyCount );

        // attack pattern 1 fire, 3 wait, 2 fire, 3 wait, 3 fire, 3 wait, 4 fire
        private static IEnemyAttackStrategy Build_1_2_3_4_Pattern( int enemyIndex, int enemyCount )
        {
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.1f;
            int [] attackPattern    = new int [] { 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 };

            return new EnemyAttackStrategy( new EnemyFixedAttackTargetStrategy(fireVelocity),  new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern) );
        }

        // slow period attack pattern
        private static IEnemyAttackStrategy Build_1_sec_periodic( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 1.0f;

            return new EnemyAttackStrategy( new EnemyFixedAttackTargetStrategy(fireVelocity),  new EnemyPeriodicAttackRateStrategy(periodTime, 0.0f) );
        }

        #endregion

        #region Enemy Builder Delegates

        private delegate Enemy BuildEnemyDelegate( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy);

        private static Enemy BuildLeafTron( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy )
        {   
            int health = 25;

            AnimFrameManager animFrameMgr = new AnimFrameManager(   1/10.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 0),
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 0),
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 0),
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 1),
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 2),
                                                                        GameConstants.CalcRectFor32x32Sprite(2, 1),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if ( buildAttackDelegate != null )
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildAnimePunch(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {   
            int health = 50;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1 / 10.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(96, 64, 64, 48),
                                                                        new Rectangle(160, 64, 64, 48),
                                                                        new Rectangle(224, 64, 64, 48),
                                                                        new Rectangle(160, 64, 64, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if ( buildAttackDelegate != null )
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        #endregion Enemy Builder Delegates
    }
}
