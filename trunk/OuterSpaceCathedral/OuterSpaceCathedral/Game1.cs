using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OuterSpaceCathedral
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;

        Texture2D spriteSheet;
        Texture2D transitionMask;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            renderTarget = new RenderTarget2D(GraphicsDevice, GameConstants.RenderTargetWidth, GameConstants.RenderTargetHeight);

            graphics.PreferredBackBufferWidth = GameConstants.BackBufferWidth;
            graphics.PreferredBackBufferHeight = GameConstants.BackBufferHeight;

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.ApplyChanges();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>("textures\\spriteSheet");
            transitionMask = Content.Load<Texture2D>("textures\\transitionMask");
            SpriteFont pixelFont = Content.Load<SpriteFont>("fonts\\klobitPixels");

            GameState.Initialize(spriteSheet, pixelFont);
            AudioManager.Initialize(Content);
            GameState.SetGameMode(GameState.Mode.FrontEnd);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GameState.Update(deltaTime);
            
            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            GameState.Draw(spriteBatch);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear( new Color(255, 0, 0, 0) );

            if ( GameState.Level != null && GameState.Level.ScreenTransition != null )
            {   
                // masked transition draw
                ScreenTransition screenTransition = GameState.Level.ScreenTransition;

                Vector2 transitionMaskCenter = new Vector2(transitionMask.Width/2, transitionMask.Height/2);

                int transitionWidth  = (int)( transitionMask.Width  * screenTransition.Scale );
                int transitionHeight = (int)( transitionMask.Height * screenTransition.Scale );

                Vector2 transMaskOrigin = GameConstants.BackBufferCenter;
                
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                spriteBatch.Draw(transitionMask, new Rectangle( (int)transMaskOrigin.X, (int)transMaskOrigin.Y, transitionWidth, transitionHeight), null, Color.White, screenTransition.Rotation, transitionMaskCenter, SpriteEffects.None, 0.0f ); 

                spriteBatch.End();

                BlendState blend = new BlendState();
                blend.ColorBlendFunction = BlendFunction.Add;
                blend.ColorSourceBlend = Blend.DestinationAlpha;
                blend.ColorDestinationBlend = Blend.Zero;
                
                spriteBatch.Begin(SpriteSortMode.Deferred, blend, SamplerState.PointClamp, null, null);
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            else
            {
                // normal draw
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
