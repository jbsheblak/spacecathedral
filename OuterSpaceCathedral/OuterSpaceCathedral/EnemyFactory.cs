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
            ScreenRightMiddle = new Vector2(GameConstants.RenderTargetWidth + 32, GameConstants.RenderTargetHeight/2);
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
                case "move_in_shallow_then_circle":     MoveInShallowThenCircle(movementStrategies); break;
                case "move_to_boss_then_circle":        MoveToBossThenCircle(movementStrategies); break;
                case "line_up_then_move":               LineUpThenMove(movementStrategies); break;
                case "wave_line":                       WaveLine(movementStrategies); break;
                case "wave_line_small":                 WaveLineSmall(movementStrategies); break;
                case "uncluttered_line":                UnclutteredLine(movementStrategies); break;
                case "uncluttered_line_easy":           UnclutteredLineEasy(movementStrategies); break;
                case "uncluttered_line_medium":         UnclutteredLineMedium(movementStrategies); break;
                case "asteroid_mob":                    AsteroidMob(movementStrategies); break;
                case "ddr_insanity":                    DDRInsanity(movementStrategies); break;
                case "ddr_insanity_and_im_serious":     DDRInsanityAndImSeriousThisTime(movementStrategies); break;
                case "flying_v":                        FlyingV(movementStrategies); break;
                case "snake_wave":                      SnakeWave(movementStrategies); break;
                case "lone_linear_destroyer":           LoneDestroyer(movementStrategies); break;
                case "single_fast_linear":              SingleFastLinear(movementStrategies); break;
                case "dual_guarded":                    DualGuarded(movementStrategies); break;
                case "boss":                            Boss(movementStrategies); break;
                case "boss_bebop":                      BossBebop(movementStrategies); break;
            }

            // build attack pattern
            BuildAttackDelegate attackDelegate = null;
            switch ( attackId )
            {
                case "attack1234":                      attackDelegate = new BuildAttackDelegate(Build_1_2_3_4_Pattern); break;
                case "1sec_periodic":                   attackDelegate = new BuildAttackDelegate(Build_1_sec_periodic); break;
                case "circular_gap":                    attackDelegate = new BuildAttackDelegate(Build_Circular_Gap); break;
                case "circular_gap_bebop":              attackDelegate = new BuildAttackDelegate(Build_Circular_Gap_Bebop); break;
                case "heavy_dual_horiz_line":           attackDelegate = new BuildAttackDelegate(Build_Heavy_Dual_Horiz_Line); break;
                case "gapped_forward_spray":            attackDelegate = new BuildAttackDelegate(Build_Gapped_Forward_Spray); break;
                case "happy_new_year":                  attackDelegate = new BuildAttackDelegate(Build_Happy_New_Year); break;
            }   

            // build unit description
            BuildEnemyDelegate buildEnemy = null;
            switch (actorId)
            {
                case "asteroid":                        buildEnemy = new BuildEnemyDelegate(BuildAsteroid); break;
                case "leaf_tron":                       buildEnemy = new BuildEnemyDelegate(BuildLeafTron); break;
                case "leaf_tron_miniboss":              buildEnemy = new BuildEnemyDelegate(BuildLeafTronMiniBoss); break;
                case "anime_punch":                     buildEnemy = new BuildEnemyDelegate(BuildAnimePunch); break;
                case "random_ddr_arrow":                buildEnemy = new BuildEnemyDelegate(BuildRandomDDRArrow); break;
                case "countdown_ten":                   buildEnemy = new BuildEnemyDelegate(BuildCountdownTen); break;
                case "countdown_nine":                  buildEnemy = new BuildEnemyDelegate(BuildCountdownNine); break;
                case "countdown_eight":                 buildEnemy = new BuildEnemyDelegate(BuildCountdownEight); break;
                case "countdown_seven":                 buildEnemy = new BuildEnemyDelegate(BuildCountdownSeven); break;
                case "countdown_six":                   buildEnemy = new BuildEnemyDelegate(BuildCountdownSix); break;
                case "countdown_five":                  buildEnemy = new BuildEnemyDelegate(BuildCountdownFive); break;
                case "countdown_four":                  buildEnemy = new BuildEnemyDelegate(BuildCountdownFour); break;
                case "countdown_three":                 buildEnemy = new BuildEnemyDelegate(BuildCountdownThree); break;
                case "countdown_two":                   buildEnemy = new BuildEnemyDelegate(BuildCountdownTwo); break;
                case "countdown_one":                   buildEnemy = new BuildEnemyDelegate(BuildCountdownOne); break;
                case "evil_heart":                      buildEnemy = new BuildEnemyDelegate(BuildHeart); break;
                case "evil_heart_miniboss":             buildEnemy = new BuildEnemyDelegate(BuildHeartMiniboss); break;
                case "dreamcast":                       buildEnemy = new BuildEnemyDelegate(BuildDreamcast); break;
                case "coco":                            buildEnemy = new BuildEnemyDelegate(BuildCoco); break;
                case "bebop_cola":                      buildEnemy = new BuildEnemyDelegate(BuildBebopCola); break;
                case "calendar_boss":                   buildEnemy = new BuildEnemyDelegate(BuildCalendarBoss); break;
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
            MoveToPositionThenCircle(movementStrategies, ScreenRightMiddle, GameConstants.RenderTargetCenter, DefaultEnemyMoveVelocity);
        }

        // move to the center of screen, then split and rotation around center
        private static void MoveInShallowThenCircle( List<IEnemyMovementStrategy> movementStrategies )
        {
            MoveToPositionThenCircle(movementStrategies, ScreenRightMiddle, ScreenRightMiddle + new Vector2(-128, 0), DefaultEnemyMoveVelocity);
        }

        private static void MoveToBossThenCircle( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 position = new Vector2(GameConstants.RenderTargetWidth + 64, GameConstants.RenderTargetHeight / 2);
            Vector2 target = position + new Vector2(-160, 0);

            MoveToPositionThenCircle(movementStrategies, position, target, Vector2.Zero);
        }

        private static void MoveToPositionThenCircle( List<IEnemyMovementStrategy> movementStrategies, Vector2 initialPosition, Vector2 moveToPosition, Vector2 moveVelocity )
        {
            const int enemyCount = 8;
            for ( int i = 0; i < enemyCount; ++i )
            {
                movementStrategies.Add( BuildMoveToLocationThenCircle(initialPosition, moveToPosition, moveVelocity, 75, 360/enemyCount * i) );
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
            WaveLineWithDisplacement( movementStrategies, DefaultEnemyMoveVelocity, new Vector2(0, 35) );
        }

        // vertical line of wave movers
        private static void WaveLineSmall( List<IEnemyMovementStrategy> movementStrategies )
        {
            WaveLineWithDisplacement( movementStrategies, DefaultEnemyMoveVelocity * 0.5f, new Vector2(0, 10) );
        }

        // vertical line of wave movers
        private static void WaveLineWithDisplacement( List<IEnemyMovementStrategy> movementStrategies, Vector2 linearVelocity, Vector2 waveDisplacement )
        {   
            float rotRateDegrees = 180.0f;

            for ( int i = 0; i < 8; ++i )
            {
                Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth + 32, (i+1) * 32 );
                movementStrategies.Add( BuildWave(initialPosition, linearVelocity, waveDisplacement, rotRateDegrees, 0) );
            }
        }

        // uncluttered line of movers
        private static void UnclutteredLine(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;

            for (int i = 0; i < 8; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 32 + 256 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        // easy uncluttered line of movers
        private static void UnclutteredLineEasy(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;

            for (int i = 0; i < 8; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 256 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        // medium uncluttered line of movers
        private static void UnclutteredLineMedium(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;

            for (int i = 0; i < 16; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 128 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        private static void AsteroidMob(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity * 2;

            for (int i = 0; i < 32; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 256 + 64 * i, GameUtility.Random.Next(1, 9) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        private static void DDRInsanity(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = new Vector2(-1000, 0);

            for (int i = 0; i < 198; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 80 + 256 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        private static void DDRInsanityAndImSeriousThisTime(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = new Vector2(-2000, 0);

            for (int i = 0; i < 208; ++i)
            {
                Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 80 + 256 * i, GameUtility.Random.Next(1, 8) * 32);
                movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
            }
        }

        // uncluttered line of movers
        private static void SingleFastLinear(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 linearVelocity = new Vector2(-500, 0);
            Vector2 initialPosition = new Vector2(GameConstants.RenderTargetWidth + 64, GameConstants.RenderTargetHeight / 2);

            movementStrategies.Add(BuildLinearMove(initialPosition, linearVelocity));
        }

        // two enemies that move just on screen and hover
        private static void DualGuarded(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 topPosition = new Vector2(GameConstants.RenderTargetWidth + 64, 1 * GameConstants.RenderTargetHeight / 4);
            Vector2 botPosition = new Vector2(GameConstants.RenderTargetWidth + 64, 3 * GameConstants.RenderTargetHeight / 4);
            Vector2 topTarget = topPosition + new Vector2(-128, 0);
            Vector2 botTarget = botPosition + new Vector2(-128, 0);
            Vector2 bobDisp = new Vector2(0, 5);

            movementStrategies.Add( BuildMoveToLocationThenBob(topPosition, topTarget, bobDisp, 100, 180.0f) );
            movementStrategies.Add( BuildMoveToLocationThenBob(botPosition, botTarget, bobDisp, 100, 180.0f) );
        }

        // one enemy that moves out and hovers
        private static void Boss(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 position = new Vector2(GameConstants.RenderTargetWidth + 64, 1 * GameConstants.RenderTargetHeight / 2);            
            Vector2 target = position + new Vector2(-160, 0);            
            Vector2 bobDisp = new Vector2(0, 10);

            movementStrategies.Add( BuildMoveToLocationThenBob(position, target, bobDisp, 100, 180.0f) );
        }

        // one enemy that moves out and hovers
        private static void BossBebop(List<IEnemyMovementStrategy> movementStrategies)
        {
            Vector2 position = new Vector2(GameConstants.RenderTargetWidth + 64, 1 * GameConstants.RenderTargetHeight / 2);            
            Vector2 target = position + new Vector2(-160, 0);            
            Vector2 bobDisp = new Vector2(0, 50);

            movementStrategies.Add( BuildMoveToLocationThenBob(position, target, bobDisp, 100, 240.0f) );
        }

        // Might Ducks flying v
        private static void FlyingV( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;

            float xSpacing = 50;
            float ySpacing = 25;
            float yOffsetFromCenter = 0;

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
        
        // lone destroyer that moves across screen
        private static void LoneDestroyer( List<IEnemyMovementStrategy> movementStrategies )
        {
            Vector2 linearVelocity = DefaultEnemyMoveVelocity;            
            Vector2 headPosition = new Vector2( GameConstants.RenderTargetWidth + 40, GameConstants.RenderTargetHeight/2 );
            
            movementStrategies.Add( BuildLinearMove(headPosition, linearVelocity) );
        }

        #endregion

        #region Movement Strategy Builders

        private static IEnemyMovementStrategy BuildMoveToLocation(Vector2 initialPosition, Vector2 targetLocation, float moveSpeed)
        {
            // move to position, then wait
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, initialPosition, targetLocation, moveSpeed);
            return new EnemyCompositeMovementStrategy(strats);
        }

        // move to a location and then bob in place
        private static IEnemyMovementStrategy BuildMoveToLocationThenBob(Vector2 initialPosition, Vector2 targetLocation, Vector2 bobDisplacement, float moveSpeed, float bobRate)
        {
            // move to position, then wait
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, initialPosition, targetLocation, moveSpeed);
            MoveWave(strats, targetLocation, Vector2.Zero, bobDisplacement, bobRate, 0.0f);
            return new EnemyCompositeMovementStrategy(strats);
        }

        private static IEnemyMovementStrategy BuildMoveToLocationThenCircle(Vector2 initialCenter, Vector2 targetCenter, Vector2 moveVelocity, float radius, float initialRotationDegrees)
        {
            // calculate radial position
            float targetX = radius * (float)Math.Cos( initialRotationDegrees * Math.PI / 180.0f );
            float targetY = radius * (float)Math.Sin( initialRotationDegrees * Math.PI / 180.0f );
            Vector2 radialPosition = targetCenter + new Vector2(targetX, targetY);

            // move to position, expand into circle, then rotation around center
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, initialCenter, targetCenter,   100);
            MoveToLocation(strats, targetCenter,  radialPosition, 100);
            MovingRotateAboutPoint(strats, moveVelocity, targetCenter, 180, initialRotationDegrees, radius);

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

            return new EnemyAttackStrategy( new EnemyFixedAttackTargetStrategy(fireVelocity),  new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern), 1 );
        }

        // slow period attack pattern
        private static IEnemyAttackStrategy Build_1_sec_periodic( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 1.0f;

            return new EnemyAttackStrategy( new EnemyFixedAttackTargetStrategy(fireVelocity),  new EnemyPeriodicAttackRateStrategy(periodTime, 0.0f), 1 );
        }

        // circular pattern with gaps for evasion
        private static IEnemyAttackStrategy Build_Circular_Gap( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.005f;
            float rotationRate      = 360;
            int [] attackPattern    = new int [] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            
            return new EnemyAttackStrategy( new EnemyCircularAttackTargetStrategy(fireVelocity, rotationRate, 0.0f), new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern), 1 );
        }

        // circular pattern with gaps for evasion
        private static IEnemyAttackStrategy Build_Circular_Gap_Bebop( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.0025f;
            float rotationRate      = 800;
            int [] attackPattern    = new int [] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            
            return new EnemyAttackStrategy( new EnemyCircularAttackTargetStrategy(fireVelocity, rotationRate, 0.0f), new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern), 1 );
        }

        // heavy fire with gaps, dual line
        private static IEnemyAttackStrategy Build_Heavy_Dual_Horiz_Line( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.05f;
            int [] attackPattern    = new int [] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            
            return new EnemyAttackStrategy( new EnemyLineAttackTargetStrategy(fireVelocity, 2), new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern), 2 );
        }

        // sprays in arc with some gaps
        private static IEnemyAttackStrategy Build_Gapped_Forward_Spray( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.005f;
            float rotationRate      = 180;
            float rotationMin       = 135;
            float rotationMax       = 225;
            int [] attackPattern    = new int [] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            
            return new EnemyAttackStrategy( new EnemyArcAttackStrategy(fireVelocity, rotationRate, rotationMax, rotationMin, rotationMax), new EnemyPeriodicPatternedAttackRateStrategy(periodTime, attackPattern), 1 );
        }

        // happy new year
        private static IEnemyAttackStrategy Build_Happy_New_Year( int enemyIndex, int enemyCount )
        {   
            float fireVelocity      = DefaultFireSpeed;
            float periodTime        = 0.05f;
            int [] attackPattern    = new int [] {1};
            
            return new EnemyTextureAttackStrategy( GameState.HappyNewYearsTexture, new EnemyPeriodicAttackRateStrategy(periodTime, 0.0f), fireVelocity );
        }

        #endregion

        #region Enemy Builder Delegates

        private delegate Enemy BuildEnemyDelegate( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy);

        private static Enemy BuildLeafTron( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy )
        {
            return BuildLeafTronGeneric(enemyIndex, enemyCount, buildAttackDelegate, movementStrategy, 25);
        }

        private static Enemy BuildLeafTronMiniBoss( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy )
        {
            return BuildLeafTronGeneric(enemyIndex, enemyCount, buildAttackDelegate, movementStrategy, 200);
        }

        private static Enemy BuildLeafTronGeneric( int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy, int health )
        {
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

        private static Enemy BuildCountdownTen(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(0, 112, 48, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            AudioManager.PlayCountdownSFX();

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownNine(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(48, 112, 32, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownEight(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(80, 112, 32, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownSeven(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(112, 112, 32, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownSix(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(144, 112, 32, 48),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownFive(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(10f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(176, 112, 64, 80),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownFour(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(10f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(240, 112, 80, 80),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownThree(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(10f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(0, 160, 80, 112),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownTwo(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(10f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(80, 160, 80, 112),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCountdownOne(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10000;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(0, 272, 48, 144),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildAsteroid(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 15;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(224, 32, 32, 32),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildRandomDDRArrow(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 20;

            AnimFrameManager animFrameMgr = new AnimFrameManager(1f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(320, 64 + 80 * GameUtility.Random.Next(0, 4), 80, 80),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }


        private static Enemy BuildHeart(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            return BuildHeartGeneric(enemyIndex, enemyCount, buildAttackDelegate, movementStrategy, 10);
        }

        private static Enemy BuildHeartMiniboss(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            return BuildHeartGeneric(enemyIndex, enemyCount, buildAttackDelegate, movementStrategy, 100);
        }

        private static Enemy BuildHeartGeneric(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy, int health)
        {   
            AnimFrameManager animFrameMgr = new AnimFrameManager(1/4.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(0,  96, 16, 16),
                                                                        new Rectangle(16, 96, 16, 16),
                                                                        new Rectangle(32, 96, 16, 16),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }
        private static Enemy BuildDreamcast(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 10;

            AnimFrameManager animFrameMgr = new AnimFrameManager( 1.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(192, 0, 32, 32)
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCoco(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 300;
            
            AnimFrameManager animFrameMgr = new AnimFrameManager(1/4.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(160,  192, 48, 64),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }


        private static Enemy BuildBebopCola(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 1000;
            
            AnimFrameManager animFrameMgr = new AnimFrameManager(1/4.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(48, 272, 80, 144),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            AudioManager.PlayBebopBossSong();
            AudioManager.PlayBebopSFX();

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        private static Enemy BuildCalendarBoss(int enemyIndex, int enemyCount, BuildAttackDelegate buildAttackDelegate, IEnemyMovementStrategy movementStrategy)
        {
            int health = 600;
            
            AnimFrameManager animFrameMgr = new AnimFrameManager(1/4.0f,
                                                                    new List<Rectangle>()
                                                                    {
                                                                        new Rectangle(208, 192, 64, 64),
                                                                    }
                                                                );

            IEnemyAttackStrategy attack = null;
            if (buildAttackDelegate != null)
            {
                attack = buildAttackDelegate(enemyIndex, enemyCount);
            }

            return new Enemy(movementStrategy, attack, animFrameMgr, health);
        }

        #endregion Enemy Builder Delegates
    }
}
