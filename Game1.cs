using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

/*
 * higher notes closer to center
 * change frequency by pitch
 * exclusive centre zone
 * centre zone with big wuwoh and long splash
 * blending between two zones at any point 
*/

/*
 * loop ambiance, loop static
 * major waveform C
 * middle button swaps between major and minor
 *
 */

namespace Wavepool
{
    public class Game1 : Game
    {
        Vector2 gameResolution = new Vector2(1920, 1080);
        FullScreen fullScreenManager;

        private GraphicsDeviceManager graphics;


        RadialInstrument instrument;
        Wavepool wavepool;
        
        bool canClick = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            fullScreenManager = new FullScreen(gameResolution, Color.CornflowerBlue, graphics, GraphicsDevice);

            // TODO: Add your initialization logic here
            Vector2 poolMargin = Vector2.One * 20;
            wavepool = new Wavepool(poolMargin + 4 * Vector2.One, gameResolution - poolMargin * 2, 128, 72, 8);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            Texture2D circleTexture = Content.Load<Texture2D>("circle");
            wavepool.Load(circleTexture, GraphicsDevice);


            RippleSet rippleSet = new RippleSet(wavepool, new RippleParameters[]
            {
                new RippleParameters(15, 30, 200, 30, 1, 0.2f),
                new RippleParameters(15, 30, 200, 30, 1, 0.2f),
                new RippleParameters(15, 30, 200, 30, 1, 0.2f),
                new RippleParameters(15, 30, 200, 30, 1, 0.2f)
            }, new RippleParameters(75, 50, 200, 50, 5, 0.1f),
            gameResolution);

            //SoundEffect pingSound = Content.Load<SoundEffect>("ping");

            Song majorAmbience = Content.Load<Song>("Ambiance/WavegameAmbianceMajorStereo");
            Song minorAmbience = Content.Load<Song>("Ambiance/WavegameAmbianceMinorStereo");
            SoundEffect staticAmbience = Content.Load<SoundEffect>("Ambiance/WavegameAmbianceStaticStereo");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(majorAmbience);
            MediaPlayer.Volume = 0.35f;

            SoundEffectInstance staticInstance = staticAmbience.CreateInstance();
            staticInstance.IsLooped = true;
            staticInstance.Volume = 0.1f;
            staticInstance.Play();


            SoundEffect splash = Content.Load<SoundEffect>("Ambiance/WavegamePingSplashZone");

            SoundEffect majorA = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveformA/WavegamePingMajorWaveANote3");
            SoundEffect majorB = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveformB/WavegamePingMajorWaveBNote3");
            SoundEffect majorC = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveformC/WavegamePingMajorWaveCNote3");
            SoundEffect majorD = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveformD/WavegamePingMajorWaveDNote3");


            SoundEffect minorA = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveformA/WavegamePingMinorWaveANote3");
            SoundEffect minorB = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveformB/WavegamePingMajorWaveBNote3");
            SoundEffect minorC = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveformC/WavegamePingMajorWaveCNote3");
            SoundEffect minorD = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveformD/WavegamePingMajorWaveDNote3");
            instrument = new RadialInstrument(GraphicsDevice, new SoundEffect[]
            {
                majorA,
                majorB,
                majorC,
                majorD
            }, new SoundEffect[]
            {
                minorA,
                minorB,
                minorC,
                minorD
            }, splash, gameResolution, 150, rippleSet);
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
                    //wavepool.AddRipple(mousePos, pingSound);
                    instrument.OnClick(mousePos);
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

            instrument.DrawGuides();
            wavepool.Draw();

            fullScreenManager.Pop();
            base.Draw(gameTime);
        }
    }
}