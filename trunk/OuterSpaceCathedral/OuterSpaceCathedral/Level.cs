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
        const int skMaxPlayers = 4;
        
        List<Background> backgrounds = new List<Background>();
        List<Background> foregrounds = new List<Background>();
        List<Player> players = new List<Player>() { null, null, null, null };
        List<Enemy> enemies = new List<Enemy>();
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();
        List<Effect> mEffects = new List<Effect>();
        LevelData mLevelData    = null;

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
            BuildInstancesForLevelData(mLevelData);

            // activate player 1
            players[0] = new Player(PlayerIndex.One);
        }

        public virtual void Update(float deltaTime)
        {
            // Update Level
            if ( mLevelData != null )
            {
                mLevelData.Update(deltaTime, enemies);
            }

            // Check for player activations
            for ( int i = 0; i < skMaxPlayers; ++i )
            {
                if ( players[i] == null )
                {
                    GamePadState gpad = GameState.GetGamePadState( (PlayerIndex)i );
                    if ( gpad.Buttons.A == ButtonState.Pressed )
                    {
                        players[i] = new Player( (PlayerIndex)i );
                    }
                }
            }

            List<Player> activePlayers = GetActivePlayers();

            //Update Objects
            backgrounds.ForEach( x => x.Update(deltaTime) );
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
            RemoveAll(enemies, x => x.ReadyForRemoval());
            RemoveAll(playerBullets, x => x.ReadyForRemoval());
            RemoveAll(enemyBullets, x => x.ReadyForRemoval());
            RemoveAll(mEffects, x => x.ReadyForRemoval());

            ElapsedLevelTime += deltaTime;
        }

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            List<Player> activePlayers = GetActivePlayers();

            backgrounds.ForEach( x => x.Draw(spriteBatch) );
            enemies.ForEach( x => x.Draw(spriteBatch) );
            mEffects.ForEach( x => x.Draw(spriteBatch) );
            playerBullets.ForEach( x => x.Draw(spriteBatch) );
            enemyBullets.ForEach( x => x.Draw(spriteBatch) );
            activePlayers.ForEach( x => x.Draw(spriteBatch) );
            foregrounds.ForEach(x => x.Draw(spriteBatch));

            DrawRejoinText(spriteBatch);
        }

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

        public float ElapsedLevelTime { get; private set; }

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
            const string rejoinText = "Press A";

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
        /// Build the sprite rectangle for a specified art piece.
        /// </summary>
        /// <param name="artId">string identifier for the art piece.</param>
        /// <returns>Valid sprite rectangle.</returns>
        private static Rectangle BuildSpriteForArtPiece(string artId)
        {
            switch ( artId )
            {
                case "city_clouds_background":      return new Rectangle(0, 1024 + 300, 960, 270);
                case "city_background":             return new Rectangle(0, 1024 + 270 * 2, 960, 270);
                case "clouds_foreground":           return new Rectangle(0, 1024 + 150, 960, 270);

                default:
                    throw new Exception("Failed to find art piece for id");
            }
        }

        /// <summary>
        /// Rate a rate string. Expecting form "x y"
        /// </summary>
        private static Vector2 ParseRate(string rate)
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
        private static Color ParseColor(string color)
        {
            if ( color.Contains(',') )
            {
                throw new Exception("Do not put commas in color string");
            }

            string [] colorTkns = color.Split( new char [] { ' ' } );
            return new Color( float.Parse(colorTkns[0]), float.Parse(colorTkns[1]), float.Parse(colorTkns[2]), colorTkns.Length > 3 ? float.Parse(colorTkns[3]) : 1.0f );
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
                            GetListForArtPiece(artPiece.Layer).Add( new SolidColorBackground( Level.ParseColor(artPiece.Color) * artPiece.ColorMod ) );
                        }
                        break;

                    case "scrolling":
                        {
                            GetListForArtPiece(artPiece.Layer).Add( new ScrollingBackground( Level.BuildSpriteForArtPiece(artPiece.ArtId), 
                                                                                             Level.ParseRate(artPiece.Rate),
                                                                                             Level.ParseColor(artPiece.Color) * artPiece.ColorMod ) );
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
