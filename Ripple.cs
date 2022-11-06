using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Wavepool
{
    public class Ripple
    {
        public bool alive;

        float waveIntensity = 30;
        float waveSpeed = 200;
        float wavePeriod = 30;
        float waveCount = 3;
        float decayRate = 0.2f;

        Vector2 origin;
        float startingRadius;
        float radius;
        float strength;
        float decay;

        SoundEffectInstance soundInstance;

        public Ripple(Vector2 origin, float startingRadius = 15, float waveIntensity = 30,
            float waveSpeed = 200, float wavePeriod = 30, float waveCount = 3, float decayRate = 0.2f)
        {
            Init(origin, startingRadius);

            this.waveIntensity = waveIntensity;
            this.waveSpeed = waveSpeed;
            this.wavePeriod = wavePeriod;
            this.waveCount = waveCount;
            this.decayRate = decayRate;
        }

        public Ripple(Vector2 origin, float startingRadius, SoundEffect sound, float panning)
        {
            Init(origin, startingRadius);
            //origin = point;
            //radius = 15;
            //strength = 1;

            soundInstance = sound.CreateInstance();
            soundInstance.Volume = 0.5f;

            int rand = Random.Shared.Next(3);
            if (rand == 1)
                soundInstance.Pitch = 5 / (float)12;
            if (rand == 2)
                soundInstance.Pitch = -5 / (float)12;

            soundInstance.Pan = panning;
            soundInstance.Play();

            //decay = 1;

        }
        public Ripple(Vector2 origin, float startingRadius = 15) => Init(origin, startingRadius);
        void Init(Vector2 origin, float startingRadius = 15)
        {
            this.origin = origin;
            this.startingRadius = startingRadius;
            radius = startingRadius;
            strength = 1;
            decay = 1;

            alive = true;
        }

        public void Update(float deltaTime)
        {
            radius += waveSpeed * deltaTime;
            strength = 15 / radius;
            decay -= decayRate * deltaTime;

            strength *= decay * decay;
            soundInstance.Volume = strength / 2;

            UpdateConstants();

            if (decay < 0)
                alive = false;
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

            if (diffLength > waveLimit || diffLength < waveLowerLimit)
            {
                return Vector2.Zero;
            }

            diffLength = MathF.Sqrt(diffLength);

            float scaling = 1 + diffLength / wavePeriod;

            float oscillation = MathF.Cos((diffLength - radius) / wavePeriod);

            diff.Normalize();
            return diff * baseScaling * oscillation * scaling;
        }
    }
}