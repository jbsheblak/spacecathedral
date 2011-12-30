using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OuterSpaceCathedral
{
    /// <summary>
    /// Pattern of enemies all released simultaneously.
    /// </summary>
    public class EnemyPattern
    {
        public EnemyPattern()
        {
            PatternId = string.Empty;
            TimeOffset = 0.0f;
        }

        [XmlAttribute("PatternId")]
        public string PatternId
        { 
            get; 
            set; 
        }

        [XmlAttribute("ActorId")]
        public string ActorId
        {
            get;
            set;
        }

        [XmlAttribute("TimeOffset")]
        public float TimeOffset
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// Wave of enemies consisting of a set of EnemyPatterns released over time.
    /// </summary>
    public class EnemyWave
    {   
        private int    mPatternIdx = 0;
        private float  mTimeOffset = 0.0f;

        public EnemyWave()
        {
            Name = string.Empty;
            EnemyPatterns = null;
            StartCondition = null;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }
        
        [XmlAttribute("StartCondition")]
        public string StartCondition
        {
            get;
            set;
        }

        public List<EnemyPattern> EnemyPatterns
        {
            get;
            set;
        }

        public bool ArePatternsComplete()
        {
            return mPatternIdx >= EnemyPatterns.Count;
        }

        public void Update(float deltaTime, List<Enemy> enemyList)
        {
            mTimeOffset += deltaTime;

            if ( EnemyPatterns != null )
            {
                if ( mPatternIdx < EnemyPatterns.Count )
                {
                    EnemyPattern currentPattern = EnemyPatterns[mPatternIdx];
                    if ( mTimeOffset >= currentPattern.TimeOffset )
                    {
                        // spawn pattern
                        EnemyFactory.BuildPattern(currentPattern.ActorId, currentPattern.PatternId, enemyList);

                        // move to next pattern
                        ++mPatternIdx;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Data container for a given level.
    /// </summary>
    public class LevelData
    {
        private int     mWaveIdx   = -1;
        private float   mTimeOffset = 0;

        public LevelData()
        {
            Name = string.Empty;
            EnemyWaves = null;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        public List<EnemyWave> EnemyWaves
        {
            get;
            set;
        }

        public void Update(float deltaTime, List<Enemy> enemiesList)
        {
            mTimeOffset += deltaTime;

            if ( EnemyWaves != null )
            {
                if ( mWaveIdx < EnemyWaves.Count )
                {
                    // check for next wave condition
                    if ( mWaveIdx < EnemyWaves.Count - 1 )
                    {
                        EnemyWave nextWave = EnemyWaves[mWaveIdx+1];
                     
                        bool startNextWave = (nextWave.StartCondition == null);
                        if ( nextWave.StartCondition != null )
                        {
                            switch ( nextWave.StartCondition )
                            {
                                case "PrevWaveComplete":
                                    {   
                                        bool prevWaveComplete = ( mWaveIdx >= 0 ) ? EnemyWaves[mWaveIdx].ArePatternsComplete() : true;
                                        startNextWave = ( prevWaveComplete && (enemiesList.Count == 0) );
                                    }
                                    break;
                            }
                        }

                        if ( startNextWave )
                        {
                            ++mWaveIdx;
                        }
                    }

                    // update current wave
                    if ( mWaveIdx >= 0 && mWaveIdx < EnemyWaves.Count )
                    {
                        EnemyWave currentWave = EnemyWaves[mWaveIdx];
                        currentWave.Update(deltaTime, enemiesList);
                    }
                }
            }
        }
    }

    public class Level
    {
        List<Background> backgrounds = new List<Background>();
        List<Player> players = new List<Player>();
        List<Enemy> enemies = new List<Enemy>();
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();
        List<Effect> mEffects = new List<Effect>();
        LevelData mLevelData = null;

        /// <summary>
        /// Load a level data instance from an xml file.
        /// </summary>
        private static LevelData LoadLevelData(string levelPath)
        {
            try
            {
                XmlSerializer xmlDeserializer = new XmlSerializer(typeof(LevelData));
                using ( TextReader textReader = new StreamReader(levelPath) )
                {
                    return (LevelData)xmlDeserializer.Deserialize(textReader);
                }
            }
            catch ( System.Exception)
            {
                return null;
            }
        }
        
        public Level()
        {
            players.Add(new Player(PlayerIndex.One));
            players.Add(new Player(PlayerIndex.Two));
            players.Add(new Player(PlayerIndex.Three));
            players.Add(new Player(PlayerIndex.Four));

            mLevelData = Level.LoadLevelData("Content/levels/testLevel.xml");

            backgrounds.Add(new SolidColorBackground(new Color(33, 73, 90)));
            backgrounds.Add(new ScrollingBackground(new Vector2(-500, 0)));
            //backgrounds.Add(new ScrollingBackground(new Vector2(0, 100)));
        }

        public virtual void Update(float deltaTime)
        {
            // Update Level
            if ( mLevelData != null )
            {
                mLevelData.Update(deltaTime, enemies);
            }

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
                        target.CollisionReaction();
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
