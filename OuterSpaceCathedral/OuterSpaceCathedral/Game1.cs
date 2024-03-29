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
        RenderTarget2D renderTarget2;
        float mElapsedTime = 0.0f;

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
            renderTarget  = new RenderTarget2D(GraphicsDevice, GameConstants.RenderTargetWidth, GameConstants.RenderTargetHeight);
            renderTarget2 = new RenderTarget2D(GraphicsDevice, GameConstants.RenderTargetWidth, GameConstants.RenderTargetHeight);
            
            graphics.PreferredBackBufferWidth = GameConstants.BackBufferWidth;
            graphics.PreferredBackBufferHeight = GameConstants.BackBufferHeight;

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.ApplyChanges();

            ControllerInput.Initialize();

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
            Texture2D happyNewYears = Content.Load<Texture2D>("textures\\HappyNewYears");

            GameState.Initialize(spriteSheet, happyNewYears, pixelFont);
            AudioManager.Initialize(Content);
            GameState.GameMode = GameState.Mode.FrontEnd;

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

            mElapsedTime += deltaTime;
            GameState.Update(deltaTime);
            
            base.Update(gameTime);
        }
        
        /// <summary>
        /// Render the trippy front end background
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void RenderTitleScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameState.SpriteSheet, GameConstants.RenderTargetRect, new Rectangle(0, 0, 16, 16), new Color(26, 48, 78));

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(i * 64, j * 64, 64, 64), new Rectangle(352, 0, 64, 64), Color.White);
                }
            }

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    spriteBatch.Draw(GameState.SpriteSheet, new Rectangle(-32 + i * 64 - (int)((mElapsedTime * 32) % 64), j * 64, 64, 64), new Rectangle(352, 0, 64, 64), Color.White);
                }
            }

            // draw title screen
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.9f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.9f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.6f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.6f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.25f, (float)Math.Sin(mElapsedTime - 0.3f) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f - 0.3f) / 10f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 175), new Rectangle(416, 0, 128, 128), Color.White * 0.8f, (float)Math.Sin(mElapsedTime) / 3f, new Vector2(64, 64), 1f + ((float)Math.Sin(mElapsedTime * 5) + 2f) / 10f, SpriteEffects.None, 0);
            
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(300, 30), new Rectangle(544, 0, 128, 16), Color.White, 0f, new Vector2(64, 8), 2f + ((float)Math.Sin(mElapsedTime * 7) + 2f) / 9f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(450, 30), new Rectangle(544, 16, 128, 16), Color.SkyBlue, 0f, new Vector2(64, 8), 2f + ((float)Math.Sin((mElapsedTime + Math.PI) * 7) + 2f) / 9f, SpriteEffects.None, 0);
            spriteBatch.Draw(GameState.SpriteSheet, new Vector2(325, 70), new Rectangle(544, 32, 128, 16), Color.Lerp(Color.White, Color.Yellow, ((float)Math.Sign(mElapsedTime * 10) + 1f) / 2f), 0f, new Vector2(64, 8), 2f + ((float)Math.Sin((mElapsedTime + Math.PI / 2f) * 7) + 2f) / 9f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if ( GameState.Level != null && GameState.Level.ScreenTransition != null )
            {
                BlendState blend;

                DepthStencilState depthState = DepthStencilState.None;

                // render trippy background
                GraphicsDevice.SetRenderTarget(renderTarget2);
                GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                RenderTitleScreen(spriteBatch);
                spriteBatch.End();

                // draw game
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                GameState.Draw(spriteBatch);
                spriteBatch.End();

                // bind back buffer
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear( new Color(255, 0, 0, 0) );
                
                // Color = render target color
                // Alpha = zero                
                blend = new BlendState();
                blend.ColorBlendFunction = BlendFunction.Add;
                blend.ColorSourceBlend = Blend.One;
                blend.ColorDestinationBlend = Blend.Zero;
                blend.AlphaBlendFunction = BlendFunction.Add;
                blend.AlphaSourceBlend = Blend.Zero;
                blend.AlphaDestinationBlend = Blend.Zero;

                // draw trippy background to back buffer
                spriteBatch.Begin(SpriteSortMode.Deferred, blend, SamplerState.PointClamp, depthState, null);
                spriteBatch.Draw(renderTarget2, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();

                ScreenTransition screenTransition = GameState.Level.ScreenTransition;

                // setup masked pattern transition
                Vector2 transitionMaskCenter = new Vector2(transitionMask.Width/2, transitionMask.Height/2);
                int transitionWidth  = (int)( transitionMask.Width  * screenTransition.Scale );
                int transitionHeight = (int)( transitionMask.Height * screenTransition.Scale );
                Vector2 transMaskOrigin = GameConstants.BackBufferCenter;

                // Color = Current BackBuffer Color
                // Alpha = Texture Alpha
                blend = new BlendState();
                blend.ColorBlendFunction = BlendFunction.Add;
                blend.ColorSourceBlend = Blend.Zero;
                blend.ColorDestinationBlend = Blend.One;
                blend.AlphaBlendFunction = BlendFunction.Add;
                blend.AlphaSourceBlend = Blend.One;
                blend.AlphaDestinationBlend = Blend.Zero;                

                // draw transition mask into alpha channel
                spriteBatch.Begin(SpriteSortMode.Deferred, blend, SamplerState.PointClamp, depthState, null);
                spriteBatch.Draw(transitionMask, new Rectangle( (int)transMaskOrigin.X, (int)transMaskOrigin.Y, transitionWidth, transitionHeight), null, Color.White, screenTransition.Rotation, transitionMaskCenter, SpriteEffects.None, 0.0f ); 
                spriteBatch.End();

                // Color = RenderTarget * DestAlpha + BackBuffer * InvDestAlpha
                blend = new BlendState();
                blend.ColorBlendFunction = BlendFunction.Add;
                blend.ColorSourceBlend = Blend.DestinationAlpha;
                blend.ColorDestinationBlend = Blend.InverseDestinationAlpha;
                blend.AlphaBlendFunction = BlendFunction.Add;
                blend.AlphaSourceBlend = Blend.Zero;
                blend.AlphaDestinationBlend = Blend.Zero;
                                
                // blend back buffer with render target during the transition mask alpha channel
                spriteBatch.Begin(SpriteSortMode.Deferred, blend, SamplerState.PointClamp, depthState, null);
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            else
            {   
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

                // draw game
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                if ( GameState.GameMode == GameState.Mode.FrontEnd )
                {
                    RenderTitleScreen(spriteBatch);
                }
                GameState.Draw(spriteBatch);
                spriteBatch.End();

                // draw render target to back buffer
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
