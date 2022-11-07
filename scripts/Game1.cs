using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
        Vector2 gameResolution = new Vector2(1200, 1200);
        Vector2 margin = Vector2.One * 20;

        Color backgroundColour = new Color(65, 75, 100);
        Color dotColour = new Color(180, 190, 220);
        Color guideColour = new Color(100, 110, 120);


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
            fullScreenManager = new FullScreen(gameResolution, backgroundColour, graphics, GraphicsDevice);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (object sender, System.EventArgs args) =>
            {
                fullScreenManager.OnWindowSizeChanged(Window.ClientBounds);
            };

            // TODO: Add your initialization logic here
            wavepool = new Wavepool(margin, gameResolution - 2 * margin, 150, 150, 6, dotColour);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            Texture2D circleTexture = Content.Load<Texture2D>("circle");
            wavepool.Load(circleTexture, GraphicsDevice);


            RippleSet rippleSet = new RippleSet(wavepool, new RippleParameters[]
            {
                new RippleParameters(10, 25, 200, 20, 1, 0.2f),
                new RippleParameters(15, 30, 200, 22, 1, 0.2f),
                new RippleParameters(15, 30, 200, 22, 1, 0.2f),
                new RippleParameters(25, 35, 200, 35, 1, 0.2f)
            }, new RippleParameters(50, 50, 200, 50, 5, 0.1f),
            gameResolution);

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

            SoundEffect majorA = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveA");
            SoundEffect majorB = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveB");
            SoundEffect majorC = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveC");
            SoundEffect majorD = Content.Load<SoundEffect>("Pings/MajorSets/MajorWaveD");


            SoundEffect minorA = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveA");
            SoundEffect minorB = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveB");
            SoundEffect minorC = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveC");
            SoundEffect minorD = Content.Load<SoundEffect>("Pings/MinorSets/MinorWaveD");
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
            }, splash, rippleSet, gameResolution, 150, margin);

            instrument.OnMiddleClicked = isMajor =>
            {
                MediaPlayer.Play(isMajor ? majorAmbience : minorAmbience);
            };
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
                mousePos = fullScreenManager.ScreenToGamePoint(mousePos);

                // ignore clicks outside of the screen
                if (mousePos.X > 0 && mousePos.X < gameResolution.X &&
                    mousePos.Y > 0 && mousePos.Y < gameResolution.Y)
                {
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
            instrument.DrawGuides(guideColour);
            wavepool.Draw();

            fullScreenManager.Pop();
            base.Draw(gameTime);
        }
    }
}