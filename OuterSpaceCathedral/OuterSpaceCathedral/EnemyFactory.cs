﻿using System;
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
        }

        private static readonly Vector2 ScreenRightMiddle;

        public static void BuildPattern(string patternId, List<Enemy> enemiesList)
        {
            switch ( patternId )
            {
                case "move_center_then_circle":
                    {
                        const int enemyCount = 8;
                        for ( int i = 0; i < enemyCount; ++i )
                        {
                            enemiesList.Add( BuildMoveToLocationThenCircleEnemy(ScreenRightMiddle, GameConstants.RenderTargetCenter, 75, 360/enemyCount * i) );
                        }
                    }
                    break;

                case "line_up_then_move":
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
                            enemiesList.Add( BuildLineUpThenMoveEnemy( initialPosition, initialPosition + new Vector2(0, 32 * i), enemyToTargetSpeed, enemyVelocity, waitTime) );
                        }
                    }
                    break;

                case "wave_line":
                    {
                        Vector2 linearVelocity = new Vector2(-100, 0);
                        Vector2 waveDisplacement = new Vector2(0, 35);
                        float rotRateDegrees = 180.0f;

                        for ( int i = 0; i < 8; ++i )
                        {
                            Vector2 initialPosition = new Vector2( GameConstants.RenderTargetWidth - 32, (i+1) * 32 );
                            enemiesList.Add( BuildWaveEnemy(initialPosition, linearVelocity, waveDisplacement, rotRateDegrees, 0) );
                        }
                    }
                    break;
            }
        }
        
        private static Enemy BuildMoveToLocationThenCircleEnemy(Vector2 initialCenter, Vector2 targetCenter, float radius, float initialRotationDegrees)
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

            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }

        private static Enemy BuildLineUpThenMoveEnemy( Vector2 lineUpStartPosition, Vector2 lineUpTargetPosition, float lineUpSpeed, Vector2 moveVelocity, float goTime)
        {
            float distToTravel = (lineUpTargetPosition - lineUpStartPosition).Length();
            float timeToTarget = distToTravel / lineUpSpeed;            
            float waitTime = goTime - timeToTarget;

            // move to position, wait, then go...
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveToLocation(strats, lineUpStartPosition, lineUpTargetPosition, lineUpSpeed);
            Wait(strats, waitTime);
            MoveLinear(strats, lineUpTargetPosition, moveVelocity);

            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
        }
    
        private static Enemy BuildWaveEnemy( Vector2 initialLocation, Vector2 linearVelocity, Vector2 waveDisplacement, float rotRateDegrees, float initialRotDegrees )
        {
            // move in wave pattern linearly
            List<IEnemyMovementStrategy> strats = new List<IEnemyMovementStrategy>();
            MoveWave(strats, initialLocation, linearVelocity, waveDisplacement, rotRateDegrees, initialRotDegrees);
            
            return new Enemy( new EnemyCompositeMovementStrategy(strats) );
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
    }
}
