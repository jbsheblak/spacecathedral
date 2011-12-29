using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    /// <summary>
    /// Movement strategy interface.
    /// </summary>
    public interface IEnemyMovementStrategy
    {
        Vector2     Position { get; set; } // get/set the current position
        bool        Complete { get; }      // is the movement complete?

        void        Update(float deltaTime); // update the movement
    }

    /// <summary>
    /// Compositing utility for movement strategies. Allows enemy to queue up movements.
    /// </summary>
    public class EnemyCompositeMovementStrategy : IEnemyMovementStrategy
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
        
        /// <summary>
        /// The current executing movement strategy.
        /// </summary>
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
    /// Parenting movement class. Allows for movement relative to another movement.
    /// </summary>
    public class EnemyParentedMovementStrategy : IEnemyMovementStrategy
    {
        private IEnemyMovementStrategy  mGlobalMovement;
        private IEnemyMovementStrategy  mLocalMovement;
        private Vector2                 mPosition;
        
        public EnemyParentedMovementStrategy(IEnemyMovementStrategy globalMovement, IEnemyMovementStrategy localMovement)
        {
            mGlobalMovement = globalMovement;
            mLocalMovement  = localMovement;
            mPosition       = Vector2.Zero;
        }

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public bool Complete
        {
            get { return mGlobalMovement.Complete && mLocalMovement.Complete; }
        }

        public void Update(float deltaTime)
        {
            mGlobalMovement.Update(deltaTime);
            mLocalMovement.Update(deltaTime);
            mPosition = mGlobalMovement.Position + mLocalMovement.Position;
        }
    }

    /// <summary>
    /// Movement strategy to cause enemy to wait for a certain amount of time.
    /// </summary>
    public class EnemyTimeDelayedMovementStrategy : IEnemyMovementStrategy
    {
        private float mTimeDelay = 0.0f;

        public EnemyTimeDelayedMovementStrategy(float timeDelay)
        {
            mTimeDelay = timeDelay;
        }

        public Vector2 Position
        {
            get; set;
        }

        public bool Complete
        {
            get { return mTimeDelay == 0.0f; }
        }

        public void Update(float deltaTime)
        {
            mTimeDelay = Math.Max(0.0f, mTimeDelay - deltaTime);
        }
    }

    /// <summary>
    /// Simple 'Move to Point' strategy.
    /// </summary>
    public class EnemyMoveToLocationStrategy : IEnemyMovementStrategy
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
    /// Fixed movement strategy. Does not move position.
    /// </summary>
    public class EnemyFixedMovementStrategy : IEnemyMovementStrategy
    {
        private Vector2 mPosition;

        public EnemyFixedMovementStrategy(Vector2 initialPosition)
        {
            mPosition = initialPosition;
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
            // No movement.
        }
    }

    /// <summary>
    /// Linear movement strategy.
    /// </summary>
    public class EnemyLinearMovementStrategy : IEnemyMovementStrategy
    {
        private Vector2 mPosition;
        private Vector2 mVelocity;

        public EnemyLinearMovementStrategy(Vector2 initialPosition, Vector2 velocity)
        {
            mPosition = initialPosition;
            mVelocity = velocity;
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
            mPosition += mVelocity * deltaTime;
        }
    }
    
    /// <summary>
    /// Movement strategy which circles around a starting point.
    /// </summary>
    public class EnemyCircularMovementStrategy : IEnemyMovementStrategy
    {
        private Vector2                 mPosition;
        private float                   mRotationRateDegrees;
        private float                   mRotationDegrees;
        private float                   mRadius;

        public EnemyCircularMovementStrategy(float rotationRateDegrees, float startRotation, float startRadius)
        {   
            mPosition               = Vector2.Zero;
            mRotationRateDegrees    = rotationRateDegrees;
            mRotationDegrees        = startRotation;
            mRadius                 = startRadius;
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
            mRotationDegrees += mRotationRateDegrees * deltaTime;

            // rotation relative to (1,0)
            float radRot = (float)(mRotationDegrees * Math.PI / 180.0f);
            mPosition = mRadius * ( new Vector2( (float)Math.Cos(radRot), (float)Math.Sin(radRot) ) );
        }
    }

    /// <summary>
    /// Movement strategy that waves on a sin curve along the wave displacement vector.
    /// </summary>
    public class EnemyWaveMovementStrategy : IEnemyMovementStrategy
    {
        private Vector2                 mPosition;
        private Vector2                 mWaveDisplacement;
        private float                   mRotationRateDegrees;
        private float                   mRotationDegrees;

        public EnemyWaveMovementStrategy(Vector2 waveDisplacement, float rotationRateDegrees, float startRotation)
        {   
            mPosition               = Vector2.Zero;
            mRotationRateDegrees    = rotationRateDegrees;
            mRotationDegrees        = startRotation;
            mWaveDisplacement       = waveDisplacement;
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
            mRotationDegrees += mRotationRateDegrees * deltaTime;

            // rotation relative to (1,0)
            float radRot = (float)(mRotationDegrees * Math.PI / 180.0f);
            mPosition = mWaveDisplacement * (float)Math.Sin(radRot);
        }
    }

    public class Enemy : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;

        private int health = 15;

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

        public override void CollisionReaction()
        {
            Damage(1);
            base.CollisionReaction();
        }

        public void Damage(int damageStrength)
        {
            health -= damageStrength;

            if (health <= 0)
            {
                RemoveObject();
            }
        }

        public override void RemoveObject()
        {
            base.RemoveObject();
            EffectsBuilder.BuildExplosion(position);
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
