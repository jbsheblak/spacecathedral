﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OuterSpaceCathedral
{
    #region Enemy Movement Strategies

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

    #endregion Enemy Movement Strategies

    #region Enemy Attack Strategies

    /// <summary>
    /// Strategy pattern which dictates where an how an enemy attacks.
    /// </summary>
    public interface IEnemyAttackStrategy
    {
        bool Complete { get; }

        void Update( Enemy parent, float deltaTime );
    }

    /// <summary>
    /// Strategy to determine where to fire attacks.
    /// </summary>
    public interface IEnemyAttackTargetStrategy
    {
        void        Update( Enemy parent, float deltaTime );
        Vector2     GetPosition( Enemy parent );
        Vector2     GetVelocity( Enemy parent );
    }

    /// <summary>
    /// Strategy to determine when to fire attacks.
    /// </summary>
    public interface IEnemyAttackRateStrategy
    {
        void        Update ( Enemy parent, float deltaTime );
        bool        ShouldFire( Enemy parent );
    }

    /// <summary>
    /// Composition class for attack strategies.
    /// </summary>
    public class EnemyCompositeAttackStrategy
    {
        private List<IEnemyAttackStrategy> mStrategies = null;
        private int                        mStrategyIndex = 0;

        bool Complete 
        { 
            get { return false; }
        }
        
        public void Update( Enemy parent, float deltaTime )
        {
            // update the current strategy
            mStrategies[mStrategyIndex].Update(parent, deltaTime);

            // check for completion
            if ( mStrategies[mStrategyIndex].Complete )
            {
                // move to next stategy, but not past last
                mStrategyIndex = Math.Min( mStrategyIndex + 1, mStrategies.Count - 1 );
            }
        }
    }
    
    /// <summary>
    /// Attack strategy to cause enemy to wait for a certain amount of time.
    /// </summary>
    public class EnemyTimeDelayedAttackStrategy : IEnemyAttackStrategy
    {
        private float mTimeDelay = 0.0f;

        public EnemyTimeDelayedAttackStrategy(float timeDelay)
        {
            mTimeDelay = timeDelay;
        }
        
        public bool Complete
        {
            get { return mTimeDelay == 0.0f; }
        }

        public void Update(Enemy parent, float deltaTime)
        {
            mTimeDelay = Math.Max(0.0f, mTimeDelay - deltaTime);
        }
    }

    /// <summary>
    /// Basic attack strategy which combines an attack and rate strategy.
    /// </summary>
    public class EnemyAttackStrategy : IEnemyAttackStrategy
    {
        private IEnemyAttackTargetStrategy mTargetStrategy = null;
        private IEnemyAttackRateStrategy   mRateStrategy = null;

        public EnemyAttackStrategy(IEnemyAttackTargetStrategy targetStrategy, IEnemyAttackRateStrategy rateStrategy)
        {
            mTargetStrategy = targetStrategy;
            mRateStrategy = rateStrategy;
        }

        public bool Complete
        {
            get { return false; }
        }

        public void Update(Enemy parent, float deltaTime)
        {
            mTargetStrategy.Update(parent, deltaTime);
            mRateStrategy.Update(parent, deltaTime);

            if ( mRateStrategy.ShouldFire(parent) )
            {
                parent.FireBullet( mTargetStrategy.GetPosition(parent), mTargetStrategy.GetVelocity(parent) );
            }
        }
    }

    /// <summary>
    /// Fixed target strategy. Fires from parent to the left
    /// </summary>
    public class EnemyFixedAttackTargetStrategy : IEnemyAttackTargetStrategy
    {
        private float mFireVelocity;

        public EnemyFixedAttackTargetStrategy(float fireVelocity)
        {
            mFireVelocity = fireVelocity;
        }

        public void Update( Enemy parent, float deltaTime )
        {
        }

        public Vector2 GetPosition( Enemy parent )
        {
            return parent.Position;
        }

        public Vector2 GetVelocity( Enemy parent )
        {
            return new Vector2(-mFireVelocity, 0);
        }
    }

    /// <summary>
    /// Attack Rate strategy that fires on a period curve.
    /// </summary>
    public class EnemyPeriodicAttackRateStrategy : IEnemyAttackRateStrategy
    {
        private float mPeriodTime;
        private float mTimeUntilFire;

        public EnemyPeriodicAttackRateStrategy( float periodTime, float periodOffset )
        {
            mPeriodTime = periodTime;
            mTimeUntilFire = periodTime - periodOffset;
        }

        public void Update ( Enemy parent, float deltaTime )
        {
            if ( mTimeUntilFire <= 0.0f )
            {
                mTimeUntilFire += mPeriodTime;
            }

            mTimeUntilFire -= deltaTime;
        }
        
        public bool ShouldFire( Enemy parent )
        {
            return mTimeUntilFire <= 0.0f;
        }
    }

    #endregion Enemy Attack Strategies

    /// <summary>
    /// Base enemy class.
    /// </summary>
    public class Enemy : GameObject
    {
        private const int skSpriteWidth  = 32;
        private const int skSpriteHeight = 32;
        
        private IEnemyMovementStrategy  mMovementStrategy = null;
        private IEnemyAttackStrategy    mAttackStrategy = null;
        private AnimFrameManager        mAnimFrameManager = null;
        private int                     mHealth = 0;

        /// <summary>
        /// Strategy that determines how the enemy moves and where it is positioned.
        /// </summary>
        public IEnemyMovementStrategy MovementStrategy
        {
            get { return mMovementStrategy; }
            set { mMovementStrategy = value; }
        }

        /// <summary>
        /// Strategy that determines where and how enemy attacks.
        /// </summary>
        public IEnemyAttackStrategy AttackStrategy
        {
            get { return mAttackStrategy; }
            set { mAttackStrategy = value; }
        }

        /// <summary>
        /// Key frame manager.
        /// </summary>
        public AnimFrameManager FrameManager
        {
            get { return mAnimFrameManager; }
            set { mAnimFrameManager = value; }
        }

        /// <summary>
        /// Enemy health status.
        /// </summary>
        public int Health
        {
            get { return mHealth; }
            set { mHealth = value; }
        }

        public Enemy(IEnemyMovementStrategy movement, IEnemyAttackStrategy attack, AnimFrameManager frames, int health)
        {
            mMovementStrategy = movement;
            mAttackStrategy = attack;
            mAnimFrameManager = frames;
            mHealth = health;
        }

        public override void Update(float deltaTime)
        {
            if (mAnimFrameManager != null)
            {
                mAnimFrameManager.Update(deltaTime);
                sourceRectangle = mAnimFrameManager.FrameRectangle;
            }

            if (mMovementStrategy != null)
            {
                mMovementStrategy.Update(deltaTime);
                position = mMovementStrategy.Position;
            }

            if (mAttackStrategy != null)
            {
                mAttackStrategy.Update(this, deltaTime);
            }

            RemoveIfOffscreen();
        }

        public override void CollisionReaction()
        {
            Damage(1);
            base.CollisionReaction();
        }

        public void Damage(int damageStrength)
        {
            mHealth -= damageStrength;

            if (mHealth <= 0)
            {
                RemoveObject();
            }
        }

        public void FireBullet(Vector2 position, Vector2 velocity)
        {
            GameState.Level.EnemyBullets.Add(Bullet.BuildEnemyBullet(position, velocity));
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
