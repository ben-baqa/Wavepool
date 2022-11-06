using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Wavepool
{
    public class Ripple
    {
        const float waveIntensity = 30;
        const float wavePeriod = 30;
        const float waveCount = 3;
        const float waveSpeed = 200;
        const float decayRate = 0.2f;

        Vector2 origin;
        float radius;
        float strength;
        float decay;

        Action<Ripple> Destroy;
        SoundEffectInstance soundInstance;

        public Ripple(Vector2 point, SoundEffect sound, float panning, Action<Ripple> destroyAction)
        {
            origin = point;
            radius = 15;
            strength = 1;
            Destroy = destroyAction;

            soundInstance = sound.CreateInstance();
            soundInstance.Volume = 0.5f;

            int rand = Random.Shared.Next(3);
            if (rand == 1)
                soundInstance.Pitch = 5 / (float)12;
            if (rand == 2)
                soundInstance.Pitch = -5 / (float)12;

            soundInstance.Pan = panning;
            soundInstance.Play();

            decay = 1;
        }

        public void Update(float deltaTime)
        {
            radius += waveSpeed * deltaTime;
            strength = 15 / radius;
            decay -= decayRate * deltaTime;

            //strength *= MathF.Sqrt(MathF.Pow(1 - decayBuildup, 3));
            strength *= decay * decay;
            soundInstance.Volume = strength / 2;

            UpdateConstants();

            if (decay < 0)
                Destroy(this);
        }

        float waveLowerLimit;
        float waveLimit;
        float baseScaling;

        void UpdateConstants()
        {
            waveLowerLimit = radius - ((4 * waveCount - 3) * wavePeriod * MathF.PI) / 2;
            if (waveLowerLimit < 0)
                waveLowerLimit = 0;
            else
                waveLowerLimit = waveLowerLimit * waveLowerLimit;

            waveLimit = radius + (wavePeriod * MathF.PI) / 2;
            waveLimit *= waveLimit;

            baseScaling = strength * waveIntensity;
        }

        public Vector2 GetOffset(Vector2 point)
        {
            Vector2 diff = point - origin;
            float diffLength = diff.LengthSquared();

            //float scaling = 1;

            if (diffLength > waveLimit || diffLength < waveLowerLimit)
            {
                //scaling = (1 + (radius - diffLength) + (radius / wavePeriod));
                //if (scaling < 0)
                return Vector2.Zero;
            }

            diffLength = MathF.Sqrt(diffLength);
            //else

            float scaling = 1 + diffLength / wavePeriod;

            float oscillation = MathF.Cos((diffLength - radius) / wavePeriod);

            diff.Normalize();
            return diff * baseScaling * oscillation * scaling;


            //return diff * strength * MathF.Cos((2 * MathF.PI * wavePeriod * diffLength / radius) - MathF.PI) *
            //    (1 + (diffLength / wavePeriod));



            //if(diffLength < radius)
            //{
            //    diffLength += 2 * (radius - diffLength);
            //}

            //diff.Normalize();

            //float f2 = ((radius / wavePeriod) - MathF.Abs((diffLength - radius) / wavePeriod));
            //if (f2 < 0)
            //    return Vector2.Zero;

            //return strength * (diff * waveIntensity * MathF.Cos((diffLength / wavePeriod) - radius) / radius) * f2;
        }

        // jiggle
        // diff * wavePush * MathF.Cos((diffLength / wavePeriod) - radius) / radius;
    }
}