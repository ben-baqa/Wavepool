using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Wavepool
{
    public class RadialInstrument
    {
        SoundEffect[] radialSounds;
        SoundEffect innerSound;
        Vector2 screenSize;
        float innerRadius;
        float outerRadius;

        Vector2 centre;

        RippleSet rippleSet;

        public RadialInstrument(SoundEffect[] radialSounds, SoundEffect innerSound, Vector2 screenSize, float innerRadius, RippleSet rippleSet)
        {
            this.radialSounds = radialSounds;
            this.innerSound = innerSound;
            this.screenSize = screenSize;
            this.innerRadius = innerRadius;

            centre = screenSize / 2;
            outerRadius = centre.Y;

            this.rippleSet = rippleSet;
        }

        public void OnClick(Vector2 position)
        {
            Vector2 diff = position - centre;
            float dist = diff.Length();

            SoundEffectInstance sound;
            if(dist < innerRadius)
            {
                sound = innerSound.CreateInstance();

                rippleSet.SpawnCentreRipple(innerRadius);
            }
            else
            {
                float angle = MathF.Atan2(diff.Y, diff.X);
                int soundIndex = (int)(angle / (2 * MathF.PI) * radialSounds.Length);

                sound = radialSounds[soundIndex].CreateInstance();
                sound.Pan = GetPanning(position);
                sound.Pitch = GetPitch(dist);

                rippleSet.SpawnRipple(position, soundIndex);
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

        // will return either -10/12, -5/12, 0, 5/12 or 10/12
        public float GetPitch(float distance)
        {
            distance = distance - innerRadius;
            float totalDistanceRatio = distance / (outerRadius - innerRadius);
            int pitchIndex = (int)(totalDistanceRatio * 5f);

            if (pitchIndex > 4)
                pitchIndex = 4;
                     
            Debug.Write("Pitch index: " + pitchIndex);
            return (10 - pitchIndex * 5) / 12f;
        }
    }
}