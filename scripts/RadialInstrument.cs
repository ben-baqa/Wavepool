using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Wavepool
{
    public class RadialInstrument
    {
        int[] majorPitches = new int[] { -9, -4, 0, 3, 5, 10 };
        int[] minorPitches = new int[] { -10, -5, 0, 4, 5, 9 };
        int[] Pitches => isMajor ? majorPitches : minorPitches;

        SoundEffect[] RadialSounds => isMajor? majorSounds: minorSounds;
        SoundEffect[] majorSounds;
        SoundEffect[] minorSounds;
        SoundEffect innerSound;

        public System.Action<bool> OnMiddleClicked;

        Vector2 centre;
        float outerRadius;
        float innerRadius;

        public bool isMajor;

        SpriteBatch spriteBatch;
        RippleSet rippleSet;

        public RadialInstrument(GraphicsDevice graphicsDevice, SoundEffect[] majorSounds, SoundEffect[] minorSounds,
            SoundEffect innerSound, Vector2 screenSize, float innerRadius, RippleSet rippleSet)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            this.majorSounds = majorSounds;
            this.minorSounds = minorSounds;
            isMajor = true;

            this.innerSound = innerSound;
            this.innerRadius = innerRadius;

            centre = screenSize / 2;
            outerRadius = centre.Y;

            this.rippleSet = rippleSet;
        }

        public void DrawGuides()
        {
            Color guideColor = Color.WhiteSmoke;
            float guideThickness = 2;

            spriteBatch.Begin();
            spriteBatch.DrawCircle(centre, innerRadius, 32, guideColor, guideThickness * 5);

            for(int i = 0; i < 5; i++)
            {
                Vector2 radii = new Vector2(innerRadius + (i + 1) * (centre.X - innerRadius) / 6,
                    innerRadius + (i + 1) * (centre.Y - innerRadius) / 6);
                spriteBatch.DrawEllipse(centre, radii, 32, guideColor, guideThickness);
            }

            float radStep = 2 * System.MathF.PI / RadialSounds.Length;
            for (int i = 0; i < RadialSounds.Length; i++)
            {
                float angle = i * radStep;
                Vector2 direction = new Vector2(System.MathF.Cos(angle), System.MathF.Sin(angle));
                direction.Normalize();
                spriteBatch.DrawLine(centre + direction * innerRadius, centre + direction * outerRadius, guideColor, guideThickness);
            }
            spriteBatch.End();
        }

        public void OnClick(Vector2 position)
        {
            Vector2 diff = position - centre;
            float dist = diff.Length();

            SoundEffectInstance sound;
            if(dist < innerRadius)
            {
                sound = innerSound.CreateInstance();

                rippleSet.SpawnCentreRipple();

                isMajor = !isMajor;
                OnMiddleClicked(isMajor);
            }
            else
            {
                float angle = System.MathF.Atan2(diff.Y, diff.X) + System.MathF.PI;
                int soundIndex = (int)(angle / (2 * System.MathF.PI) * (RadialSounds.Length));

                if (soundIndex == RadialSounds.Length)
                    soundIndex--;

                sound = RadialSounds[soundIndex].CreateInstance();
                sound.Pan = GetPanning(position);
                float pitch = GetPitch(position);
                sound.Pitch = pitch;

                rippleSet.SpawnRipple(position, soundIndex, pitch);
            }

            sound.Play();
        }

        public float GetPanning(Vector2 position)
        {
            float panning = (position.X - centre.X) / centre.X;
            if (panning < -1) return -1;
            else if (panning > 1) return 1;
            return panning;
        }

        public float GetPitch(Vector2 point)
        {
            int pitchIndex = 5;

            Vector2 radii = Vector2.One * innerRadius;
            Vector2 radiiStep = (centre - radii) / 6;
            while (pitchIndex > 0)
            {
                radii += radiiStep;
                if (InEllipse(centre, radii, point))
                    break;

                pitchIndex--;
            }

            return Pitches[pitchIndex] / 12f;
        }

        bool InEllipse(Vector2 origin, Vector2 radii, Vector2 point)
        {
            float a = EllipseHelper(origin.X, point.X, radii.X);
            float b = EllipseHelper(origin.Y, point.Y, radii.Y);

            return a + b <= 1;
        }

        float EllipseHelper(float origin, float point, float radius)
        {
            float res = point - origin;
            res *= res;
            res /= (radius * radius);
            return res;
        }
    }
}