using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;

/*
 * higher notes closer to center
 * change frequency by pitch
 * exclusive centre zone
 * centre zone with big wuwoh and long splash
 * blending between two zones at any point 
*/

namespace Wavepool
{
    public class Game1 : Game
    {
        Vector2 gameResolution = new Vector2(1920, 1080);
        FullScreen fullScreenManager;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        Wavepool wavepool;
        SoundEffect pingSound;
        bool canClick = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Vector2 poolMargin = Vector2.One * 20;
            wavepool = new Wavepool(poolMargin + 4 * Vector2.One, gameResolution - poolMargin * 2, 128, 72, 8);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            fullScreenManager = new FullScreen(gameResolution, Color.CornflowerBlue, graphics, GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D circleTexture = Content.Load<Texture2D>("circle");
            wavepool.Load(circleTexture, spriteBatch);

            pingSound = Content.Load<SoundEffect>("ping");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            wavepool.Update(deltaTime);

            var mouse = Mouse.GetState();
            if (canClick && mouse.LeftButton == ButtonState.Pressed)
            {
                canClick = false;
                Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

                // ignore clicks outside of the screen
                if (mousePos.X > 0 && mousePos.X < graphics.PreferredBackBufferWidth &&
                    mousePos.Y > 0 && mousePos.Y < graphics.PreferredBackBufferHeight)
                {
                    mousePos = fullScreenManager.ScreenToGamePoint(mousePos);
                    wavepool.AddRipple(mousePos, pingSound);
                }
            }
            else if (mouse.LeftButton == ButtonState.Released)
                canClick = true;

            fullScreenManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            fullScreenManager.Push();

            wavepool.Draw();

            fullScreenManager.Pop();
            base.Draw(gameTime);
        }
    }
}