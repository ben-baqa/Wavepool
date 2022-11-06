using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;

namespace Wavepool
{
    public class Game1 : Game
    {
        Point gameResolution = new Point(1920, 1080);

        RenderTarget2D renderTarget;
        Rectangle renderTargetDestination;
        bool canToggleFullscreen = true;

        Color letterboxingColor = new Color(0, 0, 0);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        Wavepool wavepool;
        SoundEffect pingSound;
        Vector2 screenSize;
        bool canClick = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //SetFullscreen();

            // TODO: Add your initialization logic here
            //screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            screenSize = new Vector2(gameResolution.X, gameResolution.Y);
            //Debug.WriteLine(screenSize);

            Vector2 poolMargin = Vector2.One * 20;
            wavepool = new Wavepool(poolMargin + 4 * Vector2.One, screenSize - poolMargin * 2, 128, 72, 8);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = gameResolution.X;
            graphics.PreferredBackBufferHeight = gameResolution.Y;

            graphics.ApplyChanges();

            renderTarget = new RenderTarget2D(GraphicsDevice, gameResolution.X, gameResolution.Y);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

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
                    mousePos.X *= (float)gameResolution.X / graphics.PreferredBackBufferWidth;
                    mousePos.Y *= (float)gameResolution.Y / graphics.PreferredBackBufferHeight;
                    wavepool.AddRipple(mousePos, pingSound);
                }
            }
            else if (mouse.LeftButton == ButtonState.Released)
                canClick = true;

            var keyboard = Keyboard.GetState();
            if (canToggleFullscreen && keyboard.IsKeyDown(Keys.F11))
            {
                canToggleFullscreen = false;
                ToggleFullScreen();
            }
            else
                canToggleFullscreen = true;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(letterboxingColor);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            wavepool.Draw();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(letterboxingColor);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void ToggleFullScreen()
        {
            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = gameResolution.X;
                graphics.PreferredBackBufferHeight = gameResolution.Y;
            }
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();

            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            float resolutionRatio = (float)resolution.X / resolution.Y;
            float screenRatio;
            Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
            screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            Rectangle rectangle = new Rectangle();

            if (resolutionRatio < screenRatio)
                scale = (float)bounds.Y / resolution.Y;
            else if (resolutionRatio > screenRatio)
                scale = (float)bounds.X / resolution.X;
            else
            {
                // Resolution and window/screen share aspect ratio
                rectangle.Size = bounds;
                return rectangle;
            }
            rectangle.Width = (int)(resolution.X * scale);
            rectangle.Height = (int)(resolution.Y * scale);
            return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
        }

        static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            Point delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }
    }
}