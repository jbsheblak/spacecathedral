using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    /// <summary>
    /// Movement strategy interface.
    /// </summary>
    interface IEnemyMovementStrategy
    {
        Vector2     Position { get; set; }
        bool        Complete { get; }

        void        Update(float deltaTime);
    }

    /// <summary>
    /// Compositing utility for movement strategies. Allows enemy to queue up movements.
    /// </summary>
    internal class EnemyCompositeMovementStrategy : IEnemyMovementStrategy
    {
        private List<IEnemyMovementStrategy> mStrategies = null;
        private int                          mStrategyIndex = 0;

        public EnemyCompositeMovementStrategy(List<IEnemyMovementStrategy> strategies)
        {
            mStrategies = strategies;
        }

        public Vector2 Position
        {
            get { return CurrentStrategy.Position; }
            set { CurrentStrategy.Position = value; }
        }

        public bool Complete
        {
            get { return false; }
        }
        
        private IEnemyMovementStrategy CurrentStrategy
        {
            get { return mStrategies[mStrategyIndex]; }
        }

        public void Update(float deltaTime)
        {
            CurrentStrategy.Update(deltaTime);

            if ( mStrategyIndex < mStrategies.Count - 1 )
            {
                // is the current strategy finished?
                if ( CurrentStrategy.Complete )
                {
                    // pass the current strategies position to the new strategy
                    Vector2 curPosition = CurrentStrategy.Position;
                    ++mStrategyIndex;
                    CurrentStrategy.Position = curPosition;
                }
            }
        }
    }

    /// <summary>
    /// Simple 'Move to Point' strategy.
    /// </summary>
    internal class EnemyMoveToLocationStrategy : IEnemyMovementStrategy
    {
        private Vector2 mPosition;
        private Vector2 mTarget;
        private float   mMaxSpeed;

        const float skCloseEnough = 0.01f;

        public EnemyMoveToLocationStrategy(Vector2 initialPosition, Vector2 targetPosition, float maxSpeed)
        {
            mPosition = initialPosition;
            mTarget = targetPosition;
            mMaxSpeed = maxSpeed;
        }

        public Vector2 Position 
        { 
            get { return mPosition; } 
            set { mPosition = value; }
        }

        public bool Complete 
        { 
            get { return (mTarget - mPosition).LengthSquared() < skCloseEnough; } 
        }

        public void Update(float deltaTime)
        {
            if ( !Complete )
            {
                Vector2 dirToTarget = (mTarget - mPosition);
                dirToTarget.Normalize();

                // move towards target
                mPosition.X = ConvergeToValue(mPosition.X, mTarget.X, dirToTarget.X * mMaxSpeed, deltaTime);
                mPosition.Y = ConvergeToValue(mPosition.Y, mTarget.Y, dirToTarget.Y * mMaxSpeed, deltaTime);
            }
        }

        private static float ConvergeToValue( float pos, float target, float vel, float deltaTime )
        {
            float newPos = pos + vel * deltaTime;
            return ( ( target - pos ) < 0 ) ? Math.Max(target, newPos) : Math.Min(target, newPos);
        }
    }
    
    /// <summary>
    /// Movement strategy which circles around a starting point.
    /// </summary>
    internal class EnemyCircularMovementStrategy : IEnemyMovementStrategy
    {
        private Vector2 mPosition;
        private Vector2 mCenter;
        private float   mRotationRateDegrees;

        public EnemyCircularMovementStrategy(Vector2 initialPosition, float rotationRateDegrees)
        {
            mCenter                 = initialPosition;
            mPosition               = initialPosition;
            mRotationRateDegrees    = rotationRateDegrees;
        }

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public bool Complete
        {
            get { return false; }
        }

        public void Update(float deltaTime)
        {
            float radRate = (float)(mRotationRateDegrees * Math.PI / 180.0f);
            float radRot = radRate * deltaTime;

            float cosRot = (float)Math.Cos(radRot);
            float sinRot = (float)Math.Sin(radRot);

            Vector2 localPosition = mPosition - mCenter;
            
            float rotLocalX = cosRot * localPosition.X - sinRot * localPosition.Y;
            float rotLocalY = sinRot * localPosition.X + cosRot * localPosition.Y;

            mPosition = new Vector2(rotLocalX, rotLocalY) + mCenter;
        }
    }

    internal class Enemy : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;
        
        private IEnemyMovementStrategy mMovementStrategy;

        public Enemy(IEnemyMovementStrategy movementStrategy)
        {
            sourceRectangle = new Rectangle(32, 0, skSpriteWidth, skSpriteHeight);

            mMovementStrategy = movementStrategy;
            position = mMovementStrategy.Position;
        }

        public override void Update(float deltaTime)
        {   
            mMovementStrategy.Update(deltaTime);

            position = mMovementStrategy.Position;
            RemoveIfOffscreen();
        }

        public override void RemoveObject()
        {
            base.RemoveObject();
            GameState.Level.AddEffect(new Effect(position));
        }

        private void RemoveIfOffscreen()
        {
            if ( !GameConstants.RenderTargetRect.Intersects(PositionRectangle) )
            {
                RemoveObject();
            }
        }
    }
}
