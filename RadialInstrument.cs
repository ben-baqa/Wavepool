using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Wavepool
{
    public class RadialInstrument
    {
        SoundEffect[] radialSounds;
        SoundEffect innerSound;
        Vector2 screenSize;
        float innerRadius;

        Vector2 centre;

        public RadialInstrument(SoundEffect[] radialSounds, SoundEffect innerSound, Vector2 screenSize, float innerRadius)
        {
            this.radialSounds = radialSounds;
            this.innerSound = innerSound;
            this.screenSize = screenSize;
            this.innerRadius = innerRadius;

            centre = screenSize / 2;
        }

        public void OnClick(Vector2 position)
        {

        }
    }
}