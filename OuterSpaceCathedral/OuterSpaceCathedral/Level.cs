﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        [XmlAttribute("AttackId")]
        public string AttackId
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
            StartAction = null;
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

        [XmlAttribute("StartAction")]
        public string StartAction
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
                        EnemyFactory.BuildPattern(currentPattern.ActorId, currentPattern.PatternId, currentPattern.AttackId, enemyList);

                        // move to next pattern
                        ++mPatternIdx;
                    }
                }
            }
        }
    }

    /// <summary>
    /// art piece in level.
    /// </summary>
    public class ArtPiece
    {
        public ArtPiece()
        {
            Color = "1 1 1 1";
            ColorMod = 1;
            Layer = "background";
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("Type")]
        public string Type
        {
            get;
            set;
        }

        [XmlAttribute("ArtId")]
        public string ArtId
        {
            get;
            set;
        }

        [XmlAttribute("Layer")]
        public string Layer
        {
            get;
            set;
        }
                
        
        [XmlAttribute("Rate")]
        public string Rate
        {
            get;
            set;
        }

        [XmlAttribute("Color")]
        public string Color
        {
            get;
            set;
        }

        [XmlAttribute("ColorMod")]
        public float  ColorMod
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Level transition data
    /// </summary>
    public class TransitionData
    {
        [XmlAttribute("Text")]
        public string Text
        {
            get;
            set;
        }

        [XmlAttribute("Time")]
        public float Time
        {
            get;
            set;
        }

        [XmlAttribute("Color")]
        public string Color
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Data container for a given level.
    /// </summary>
    public class LevelData
    {
        public LevelData()
        {
            Name = string.Empty;
            EnemyWaves = null;
            ArtPieces = null;
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

        public List<ArtPiece> ArtPieces
        {
            get;
            set;
        }

        public TransitionData Intro
        {
            get;
            set;
        }

        public TransitionData Outro
        {
            get;
            set;
        }
    }

    public static class ParseUtil
    {
        /// <summary>
        /// Rate a rate string. Expecting form "x y"
        /// </summary>
        public static Vector2 ParseRate(string rate)
        {   
            if ( rate.Contains(',') )
            {
                throw new Exception("Do not put commas in rate string");
            }

            string [] rateTkns = rate.Split( new char [] { ' ' } );
            return new Vector2( float.Parse(rateTkns[0]), float.Parse(rateTkns[1]) );
        }

        /// <summary>
        /// Parse a color string. Expecting form "r g b" or "r g b a"
        /// </summary>
        public static Color ParseColor(string color)
        {
            if ( color.Contains(',') )
            {
                throw new Exception("Do not put commas in color string");
            }

            string [] colorTkns = color.Split( new char [] { ' ' } );
            return new Color( float.Parse(colorTkns[0]), float.Parse(colorTkns[1]), float.Parse(colorTkns[2]), colorTkns.Length > 3 ? float.Parse(colorTkns[3]) : 1.0f );
        }
    }

    /// <summary>
    /// Management of spawing enemy waves.
    /// </summary>
    public class EnemyWaveManager
    {
        private List<EnemyWave> mEnemyWaves;
        private int             mWaveIdx   = -1;
        private float           mTimeOffset = 0;

        public EnemyWaveManager(List<EnemyWave> enemyWaves)
        {
            mEnemyWaves = enemyWaves;
        }

        public void Update(float deltaTime, List<Enemy> enemiesList)
        {
            mTimeOffset += deltaTime;

            if ( mEnemyWaves != null )
            {
                if ( mWaveIdx < mEnemyWaves.Count )
                {
                    // check for next wave condition
                    if ( mWaveIdx < mEnemyWaves.Count - 1 )
                    {
                        EnemyWave nextWave = mEnemyWaves[mWaveIdx+1];
                     
                        bool startNextWave = (nextWave.StartCondition == null);
                        if ( !string.IsNullOrEmpty(nextWave.StartCondition) )
                        {
                            if ( nextWave.StartCondition == "PrevWaveComplete" )
                            {
                                bool prevWaveComplete = ( mWaveIdx >= 0 ) ? mEnemyWaves[mWaveIdx].ArePatternsComplete() : true;
                                startNextWave = ( prevWaveComplete && (enemiesList.Count == 0) );
                            }
                            else if ( nextWave.StartCondition.Contains("TimeQuery") )
                            {
                                string [] queryTkns = nextWave.StartCondition.Split( new char [] { '=' } );

                                int timeQuery = GameUtility.GetQueryTimeValue(queryTkns[1]);
                                int timeNow   = GameUtility.GetCurrentTimeValue();

                                // has the query time passed?
                                if ( timeNow >= timeQuery )
                                {
                                    startNextWave = true;
                                }
                            }
                        }

                        if ( startNextWave )
                        {
                            if ( !string.IsNullOrEmpty(nextWave.StartAction) )
                            {
                                switch ( nextWave.StartAction )
                                {
                                    case "ClearEnemies":
                                        {
                                            foreach ( Enemy e in enemiesList )
                                            {
                                                e.Damage(1000000, null);
                                            }
                                        }
                                        break;

                                    case "AddCelebrationStub":
                                        {
                                            if ( GameState.Level != null )
                                            {
                                                GameState.Level.AddCelebration();
                                            }
                                        }
                                        break;
                                }
                            }

                            ++mWaveIdx;
                        }
                    }

                    // update current wave
                    if ( mWaveIdx >= 0 && mWaveIdx < mEnemyWaves.Count )
                    {
                        EnemyWave currentWave = mEnemyWaves[mWaveIdx];
                        currentWave.Update(deltaTime, enemiesList);
                    }
                }
            }
        }

        /// <summary>
        /// Check if all the waves have completed.
        /// </summary>
        /// <returns></returns>
        public bool AreAllWavesComplete()
        {
            // check patterns
            foreach ( EnemyWave ew in mEnemyWaves )
            {
                if ( !ew.ArePatternsComplete() )
                {
                    return false;
                }
            }
            
            return true;
        }
    }

    /// <summary>
    /// Manages logic through transtions.
    /// </summary>
    public class TransitionDataManager
    {
        private float           mTimeRemaining;
        private TransitionData  mData;

        public TransitionDataManager(TransitionData data)
        {
            mData = data;
            mTimeRemaining = data.Time;
        }

        public bool Complete
        {
            get { return mTimeRemaining == 0.0f; }
        }

        public void Update(float deltaTime)
        {
            mTimeRemaining = Math.Max(0.0f, mTimeRemaining - deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            Vector2 textSize = GameState.PixelFont.MeasureString(mData.Text);
            Vector2 textPos = GameConstants.RenderTargetCenter - new Vector2( textSize.X / 2, 0 );

            //Prevent letters getting chopped off
            textPos = new Vector2((int)textPos.X, (int)textPos.Y);

            Vector2 shadowPos = textPos + new Vector2(-1, -1);
            Color textColor = ParseUtil.ParseColor(mData.Color);
            Color shadowColor = Color.Black;
            
            spriteBatch.DrawString(GameState.PixelFont, mData.Text, shadowPos, shadowColor);
            spriteBatch.DrawString(GameState.PixelFont, mData.Text, textPos,   textColor);
        }
    }

    /// <summary>
    /// Utility class for showing level stats.
    /// </summary>
    public class TransitionStatsManager
    {
        private float                   mTimeRemaining;
        private string                  mLine0;
        private string                  mLine1;

        public bool Complete
        {
            get { return mTimeRemaining == 0.0f; }
        }

        public TransitionStatsManager(PlayerStatsManager data)
        {   
            mTimeRemaining = 5.0f;

            string [] playerLabels = new string []
            {
                "Player 1",
                "Player 2",
                "Player 3",
                "Player 4",
            };

            string mostKills        = playerLabels[0];
            string leastKills       = playerLabels[0];
            string mostDeaths       = playerLabels[0];
            string leastDeaths      = playerLabels[0];
            
            int mostKillsValue      = data.Players[0].Kills;
            int leastKillValue      = data.Players[0].Kills;
            int leastDeathsValue    = data.Players[0].Deaths;
            int mostDeathsValue     = data.Players[0].Deaths;

            // collect stats
            for ( int i = 1; i < data.Players.Count; ++i )
            {
                PlayerStats ps = data.Players[i];

                if ( ps.Joins > 0 )
                {
                    if ( ps.Kills > mostKillsValue )
                    {
                        mostKillsValue = ps.Kills;
                        mostKills = playerLabels[i];
                    }

                    if ( ps.Kills < leastKillValue )
                    {
                        leastKillValue = ps.Kills;
                        leastKills = playerLabels[i];
                    }

                    if ( ps.Deaths > mostDeathsValue )
                    {
                        mostDeathsValue = ps.Deaths;
                        mostDeaths = playerLabels[i];
                    }

                    if ( ps.Deaths < leastDeathsValue )
                    {
                        leastDeathsValue = ps.Deaths;
                        leastDeaths = playerLabels[i];
                    }
                }
            }

            // format text
            mLine0 = string.Format("Most  Kills: {0}            Least Deaths: {1}", mostKills,  leastDeaths);
            mLine1 = string.Format("Least Kills: {0}            Most  Deaths: {1}", leastKills, mostDeaths);
        }

        public void Update(float deltaTime)
        {
            mTimeRemaining = Math.Max(0.0f, mTimeRemaining - deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 textSize = GameState.PixelFont.MeasureString(mLine0);
            Vector2 textPos0 = GameConstants.RenderTargetCenter - new Vector2( textSize.X / 2, 0 );
            Vector2 textPos1 = textPos0 + new Vector2( 0, GameState.PixelFont.LineSpacing );
            Vector2 shadowOffset = new Vector2(-1,-1);
            Color shadowColor = Color.Black;
            
            spriteBatch.DrawString(GameState.PixelFont, mLine0, textPos0 + shadowOffset, shadowColor);
            spriteBatch.DrawString(GameState.PixelFont, mLine1, textPos1 + shadowOffset, shadowColor);

            spriteBatch.DrawString(GameState.PixelFont, mLine0, textPos0, Color.White);
            spriteBatch.DrawString(GameState.PixelFont, mLine1, textPos1, Color.Red);
        }
    }

    /// <summary>
    /// Screen Transition Info
    /// </summary>
    public class ScreenTransition
    {
        private float mScaleRate;
        private float mRotationRate;

        public ScreenTransition(float startScale, float scaleRate, float rotationRate)
        {
            mScaleRate = scaleRate;
            mRotationRate = rotationRate;
            Scale = startScale;
            Rotation = 0;
        }

        public float Scale      { get; private set; }
        public float Rotation   { get; private set; }

        public void Update(float deltaTime)
        {
            Scale += mScaleRate * deltaTime;
            Rotation += mRotationRate * deltaTime;
        }

        public bool IsComplete()
        {
            return ( mScaleRate < 0 ) ? Scale < GameConstants.ScreenTransitionScaleMin : Scale > GameConstants.ScreenTransitionScaleMax;
        }
    }

    /// <summary>
    /// Level Logic and Data
    /// </summary>
    public class Level
    {
        const int skMaxPlayers = 4;
        
        public enum State
        {
            IntroScreen,
            Intro,
            Play,
            Outro,
            OutroStats,
            OutroScreen,
        };

        List<Background> backgrounds = new List<Background>();
        List<Background> foregrounds = new List<Background>();
        List<Player> players = new List<Player>() { null, null, null, null };
        List<Enemy> enemies = new List<Enemy>();
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();
        List<Effect> mEffects = new List<Effect>();
        List<CustomStub> mCustomStubs = new List<CustomStub>();
        LevelData mLevelData    = null;
        EnemyWaveManager mWaveManager = null;
        State mState = State.Intro;
        TransitionDataManager mTransitionManager = null;
        ScreenTransition mScreenTransition = null;
        TransitionStatsManager mTransitionStats = null;
        
        /// <summary>
        /// Construct a level instance from a configuration file.
        /// </summary>
        /// <param name="levelPath">Path to a level data file to load.</param>
        /// <returns>Valid level instance on success, null otherwise.</returns>
        public static Level BuildLevelFromFile(string levelPath)
        {
            LevelData levelData = LoadLevelData(levelPath);
            if ( levelData != null )
            {
                return new Level(levelData);
            }

            return null;
        }

        private Level(LevelData levelData)
        {   
            mLevelData = levelData;
            mWaveManager = new EnemyWaveManager(levelData.EnemyWaves);
            BuildInstancesForLevelData(mLevelData);
            PlayerStatsManager = new PlayerStatsManager();

            mState = State.IntroScreen;
            mScreenTransition = new ScreenTransition(GameConstants.ScreenTransitionScaleMin, GameConstants.ScreenTransitionScaleRate, GameConstants.ScreenTransitionRotationRate);
            
            // activate player 1
            AddPlayer( PlayerIndex.One );
        }

        public virtual void Update(float deltaTime)
        {
            if ( mState == State.Play )
            {
                // Update Level
                mWaveManager.Update(deltaTime, enemies);

                // check for play completion
                if ( mWaveManager.AreAllWavesComplete() && enemies.Count == 0 )
                {
                    if ( mLevelData.Outro != null )
                    {
                        mState = State.Outro;
                        mTransitionManager = new TransitionDataManager(mLevelData.Outro);
                    }
                    else
                    {
                        SetReturnToFrontend();
                        return;
                    }
                }
            }

            // check if we need to transtion out
            if ( CheckTransitions(deltaTime) )
            {
                return;
            }

            // Check for player activations
            for ( int i = 0; i < skMaxPlayers; ++i )
            {
                if ( players[i] == null )
                {
                    GamePadState gpad = GameState.GetGamePadState( (PlayerIndex)i );
                    if ( gpad.Buttons.Start == ButtonState.Pressed )
                    {
                        AddPlayer( (PlayerIndex)i );
                        AudioManager.PlayPlayerJoinSFX();
                    }
                }
            }

            List<Player> activePlayers = GetActivePlayers();

            //Update Objects
            backgrounds.ForEach( x => x.Update(deltaTime) );
            mCustomStubs.ForEach( x => x.Update(deltaTime) );
            activePlayers.ForEach( x => x.Update(deltaTime) );
            enemies.ForEach( x => x.Update(deltaTime) );
            playerBullets.ForEach( x => x.Update(deltaTime) );
            enemyBullets.ForEach( x => x.Update(deltaTime) );
            mEffects.ForEach( x => x.Update(deltaTime) );
            foregrounds.ForEach(x => x.Update(deltaTime));

            //Check collisions
            CheckCollisions(enemies, playerBullets, true);
            CheckCollisions(activePlayers, enemyBullets, true);
            CheckCollisions(activePlayers, enemies, false);

            //Remove dead objects
            for ( int i = 0; i < players.Count; ++i )
            {
                if ( players[i] != null && players[i].ReadyForRemoval() )
                {
                    players[i] = null;
                }
            }

            RemoveAll(backgrounds, x => x.ReadyForRemoval());
            RemoveAll(foregrounds, x => x.ReadyForRemoval());
            RemoveAll(mCustomStubs, x => x.ReadyForRemoval());
            RemoveAll(enemies, x => x.ReadyForRemoval());
            RemoveAll(playerBullets, x => x.ReadyForRemoval());
            RemoveAll(enemyBullets, x => x.ReadyForRemoval());
            RemoveAll(mEffects, x => x.ReadyForRemoval());

            ElapsedLevelTime += deltaTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            List<Player> activePlayers = GetActivePlayers();

            backgrounds.ForEach( x => x.Draw(spriteBatch) );
            mCustomStubs.ForEach( x => x.Draw( spriteBatch ) );
            enemyBullets.ForEach(x => x.Draw(spriteBatch));
            enemies.ForEach( x => x.Draw(spriteBatch) );
            mEffects.ForEach( x => x.Draw(spriteBatch) );
            playerBullets.ForEach( x => x.Draw(spriteBatch) ); 
            activePlayers.ForEach( x => x.Draw(spriteBatch) );
            foregrounds.ForEach(x => x.Draw(spriteBatch));

            DrawRejoinText(spriteBatch);

            if ( mTransitionManager != null )
            {
                mTransitionManager.Draw(spriteBatch);
            }

            if ( mTransitionStats != null )
            {
                mTransitionStats.Draw(spriteBatch);
            }


        #if DEBUG
            DrawDebugText(spriteBatch);
        #endif
        }

        public PlayerStatsManager PlayerStatsManager { private set; get; }

        public List<Bullet> PlayerBullets
        {
            get
            {
                return playerBullets;
            }
        }

        public List<Bullet> EnemyBullets
        {
            get
            {
                return enemyBullets;
            }
        }

        public void AddEffect(Effect effect)
        {
            mEffects.Add(effect);
        }

        public void AddCelebration()
        {
            mCustomStubs.Add( new CustomStub("Celebration") );
        }

        public bool IsPlayerFiring()
        {
            foreach (Player player in players)
            {
                if (player != null && player.IsFiring)
                {
                    return true;
                }
            }

            return false;
        }

        public float ElapsedLevelTime { get; private set; }

        public ScreenTransition ScreenTransition
        {
            get { return mScreenTransition; }
        }

        #region Private

        /// <summary>
        /// Check to see if a list of target objects intersect a list of dangerous objects.
        /// Intersecting target objects will be removed.
        /// </summary>
        private void CheckCollisions<A, B>(IEnumerable<A> targetObjects, IEnumerable<B> dangerObjects, bool removeDangerObjectOnImpact) where A : GameObject where B : GameObject
        {
            foreach ( GameObject target in targetObjects )
            {
                foreach ( GameObject danger in dangerObjects )
                {
                    // check for bound intersection
                    if ( target.PositionRectangle.Intersects(danger.PositionRectangle) )
                    {
                        target.CollisionReaction(danger.CollisionMessage);
                        if ( removeDangerObjectOnImpact )
                        {
                            danger.RemoveObject();
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of all of the currently active players.
        /// </summary>
        /// <returns></returns>
        private List<Player> GetActivePlayers()
        {
            // Gather active players
            List<Player> activePlayers = new List<Player>();
            foreach ( Player p in players )
            {
                if ( p != null )
                {
                    activePlayers.Add(p);
                }
            }

            return activePlayers;
        }

        /// <summary>
        /// Draw the text that prompts the players to rejoin the game.
        /// </summary>
        private void DrawRejoinText(SpriteBatch spriteBatch)
        {
            const string rejoinText = "Press Start";

            // draw rejoin text
            
            const int skPadding = 15;
            Vector2 rejoinTextSize = GameState.PixelFont.MeasureString(rejoinText);
            for ( int i = 0; i < skMaxPlayers; ++i )
            {
                if ( players[i] == null )
                {
                    Vector2 textPos = Vector2.Zero;
                    switch ( i )
                    {
                        case 0:
                            textPos = new Vector2( skPadding, skPadding );
                            break;

                        case 1:
                            textPos = new Vector2( GameConstants.RenderTargetWidth - skPadding - rejoinTextSize.X, skPadding );
                            break;

                        case 2:
                            textPos = new Vector2( skPadding, GameConstants.RenderTargetHeight - skPadding - rejoinTextSize.Y );
                            break;

                        case 3:
                            textPos = GameConstants.RenderTargetMax - rejoinTextSize - new Vector2(skPadding, skPadding);
                            break;
                    }

                    spriteBatch.DrawString(GameState.PixelFont, rejoinText, textPos, GameConstants.GetColorForPlayer( (PlayerIndex)i ));
                }
            }
        }

        /// <summary>
        /// Draw debug stats.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawDebugText(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2( GameConstants.RenderTargetWidth/2, GameState.PixelFont.LineSpacing );

            spriteBatch.DrawString(GameState.PixelFont, string.Format("Enemies:         {0}", enemies.Count), textPosition, Color.Red);       textPosition.Y += GameState.PixelFont.LineSpacing;
            spriteBatch.DrawString(GameState.PixelFont, string.Format("EnemyBullets:    {0}", enemyBullets.Count), textPosition, Color.Red);  textPosition.Y += GameState.PixelFont.LineSpacing;
            spriteBatch.DrawString(GameState.PixelFont, string.Format("PlayerBullets:   {0}", playerBullets.Count), textPosition, Color.Red); textPosition.Y += GameState.PixelFont.LineSpacing;
            spriteBatch.DrawString(GameState.PixelFont, string.Format("Player 1 Kills:  {0}", PlayerStatsManager.Players[0].Kills), textPosition, Color.Red); textPosition.Y += GameState.PixelFont.LineSpacing;
            spriteBatch.DrawString(GameState.PixelFont, string.Format("Player 1 Deaths: {0}", PlayerStatsManager.Players[0].Deaths), textPosition, Color.Red); textPosition.Y += GameState.PixelFont.LineSpacing;
        }

        /// <summary>
        /// Build the sprite rectangle for a specified art piece.
        /// </summary>
        /// <param name="artId">string identifier for the art piece.</param>
        /// <returns>Valid sprite rectangle.</returns>
        private static Rectangle BuildSpriteForArtPiece(string artId)
        {
            switch ( artId )
            {
                case "city_clouds_background":          return new Rectangle(0, 1024 + 300, 960, 270);
                case "city_clouds_background2":         return new Rectangle(0, 1024 + 350, 960, 270);
                case "city_background":                 return new Rectangle(0, 1024 + 270 * 2, 960, 270);
                case "purple_clouds_background":        return new Rectangle(960, 1564, 960, 270);
                case "clouds_foreground":               return new Rectangle(0, 1024 + 150, 960, 270);
                case "hills_background":                return new Rectangle(960, 1294, 960, 270);


                default:
                    throw new Exception("Failed to find art piece for id");
            }
        }

        /// <summary>
        /// Get the art piece layer for identifier.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private List<Background> GetListForArtPiece(string layer)
        {
            return layer == "background" ? backgrounds : foregrounds;
        }

        /// <summary>
        /// Build all instance data for a level.
        /// </summary>
        /// <param name="levelData"></param>
        private void BuildInstancesForLevelData(LevelData levelData)
        {
            // build art pieces
            foreach ( ArtPiece artPiece in levelData.ArtPieces )
            {
                switch ( artPiece.Type )
                {
                    case "solid":
                        {
                            GetListForArtPiece(artPiece.Layer).Add( new SolidColorBackground( ParseUtil.ParseColor(artPiece.Color) * artPiece.ColorMod ) );
                        }
                        break;

                    case "scrolling":
                        {
                            GetListForArtPiece(artPiece.Layer).Add( new ScrollingBackground( Level.BuildSpriteForArtPiece(artPiece.ArtId), 
                                                                                             ParseUtil.ParseRate(artPiece.Rate),
                                                                                             ParseUtil.ParseColor(artPiece.Color) * artPiece.ColorMod ) );
                        }
                        break;
                }
            }
        }

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

        /// <summary>
        /// Mark us for a return to the front end
        /// </summary>
        private void SetReturnToFrontend()
        {
            AudioManager.StopPlayerFireSFX();
            GameState.GameMode = GameState.Mode.FrontEnd;
        }

        /// <summary>
        /// Check on transitions.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns>true if we need to bail out of the level</returns>
        private bool CheckTransitions(float deltaTime)
        {
            switch ( mState )
            {
                case State.IntroScreen:
                    mScreenTransition.Update(deltaTime);
                    if ( mScreenTransition.IsComplete() )
                    {   
                        mState = State.Intro;
                        mTransitionManager = new TransitionDataManager(mLevelData.Intro);
                        mScreenTransition = null;
                    }
                    break;
                    
                case State.Intro:
                    mTransitionManager.Update(deltaTime);
                    if ( mTransitionManager.Complete )
                    {
                        mTransitionManager = null;
                        mState = State.Play;
                    }
                    break;

                case State.Outro:
                    mTransitionManager.Update(deltaTime);
                    if ( mTransitionManager.Complete )
                    {
                        mTransitionManager = null;
                        mState = State.OutroStats;
                        mTransitionStats = new TransitionStatsManager(PlayerStatsManager);
                    }
                    break;

                case State.OutroStats:
                    mTransitionStats.Update(deltaTime);
                    if ( mTransitionStats.Complete )
                    {
                        mTransitionStats = null;
                        mState = State.OutroScreen;
                        mScreenTransition = new ScreenTransition(GameConstants.ScreenTransitionScaleMax, -GameConstants.ScreenTransitionScaleRate, -GameConstants.ScreenTransitionRotationRate);
                    }
                    break;

                case State.OutroScreen:
                    mScreenTransition.Update(deltaTime);
                    if ( mScreenTransition.IsComplete() )
                    {
                        SetReturnToFrontend();
                        return true;
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// add a player to the game
        /// </summary>
        /// <param name="playerIndex"></param>
        private void AddPlayer(PlayerIndex playerIndex)
        {
            players[ (int)playerIndex ] = new Player(playerIndex);
            PlayerStatsManager.Players[ (int)playerIndex ].Joins += 1;        
        }

        /// <summary>
        /// Remove all instaces based on a specified predicate.
        /// </summary>
        public void RemoveAll<A>(List<A> gameObjects, Predicate<A> predicate) where A : GameObject
        {
            for (int i = gameObjects.Count - 1; i >= 0; --i)
            {
                if (predicate(gameObjects[i]))
                {
                    gameObjects.RemoveAt(i);
                }
            }
        }

        #endregion Private
    }
}
