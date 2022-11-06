using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Wavepool
{
    public class Ripple
    {
        public bool alive;

        float amplitude = 30;
        float speed = 200;
        float period = 30;
        float crestCount = 1;
        float decayRate = 0.2f;

        Vector2 origin;
        float startingRadius;
        float radius;
        float strength;
        float decay;

        //SoundEffectInstance soundInstance;

        //public Ripple(Vector2 origin, float startingRadius = 15, float waveIntensity = 30,
        //    float waveSpeed = 200, float wavePeriod = 30, float waveCount = 3, float decayRate = 0.2f)
        //{
        //    Init(origin, startingRadius);

        //    this.amplitude = waveIntensity;
        //    this.speed = waveSpeed;
        //    this.period = wavePeriod;
        //    this.crestCount = waveCount;
        //    this.decayRate = decayRate;
        //}

        public Ripple(Vector2 origin, RippleParameters parameters)
        {
            Init(origin, parameters.radius);

            amplitude = parameters.amplitude;
            speed = parameters.speed;
            period = parameters.period;
            crestCount = parameters.crestCount;
            decayRate = parameters.decayRate;
        }

        //public Ripple(Vector2 origin, float startingRadius, SoundEffect sound, float panning)
        //{
        //    Init(origin, startingRadius);
        //    //origin = point;
        //    //radius = 15;
        //    //strength = 1;

        //    soundInstance = sound.CreateInstance();
        //    soundInstance.Volume = 0.5f;

        //    int rand = Random.Shared.Next(3);
        //    if (rand == 1)
        //        soundInstance.Pitch = 5 / (float)12;
        //    if (rand == 2)
        //        soundInstance.Pitch = -5 / (float)12;

        //    soundInstance.Pan = panning;
        //    soundInstance.Play();

        //    //decay = 1;

        //}
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
            radius += speed * deltaTime;
            strength = startingRadius / radius;
            decay -= decayRate * deltaTime;

            strength *= decay * decay;
            //soundInstance.Volume = strength / 2;

            UpdateConstants();

            if (decay < 0)
                alive = false;
        }

        float waveLowerLimit;
        float waveLimit;
        float baseScaling;

        void UpdateConstants()
        {
            waveLowerLimit = radius - ((4 * crestCount - 3) * period * MathF.PI) / 2;
            if (waveLowerLimit < 0)
                waveLowerLimit = 0;
            else
                waveLowerLimit = waveLowerLimit * waveLowerLimit;

            waveLimit = radius + (period * MathF.PI) / 2;
            waveLimit *= waveLimit;

            baseScaling = strength * amplitude;
        }

        public Vector2 GetOffset(Vector2 point)
        {
            Vector2 diff = point - origin;
            float dist = diff.LengthSquared();

            if (dist > waveLimit || dist < waveLowerLimit)
            {
                return Vector2.Zero;
            }

            dist = MathF.Sqrt(dist);

            float scaling = 1 + dist / period;

            float oscillation = MathF.Cos((dist - radius) / period);

            diff.Normalize();
            return diff * baseScaling * oscillation * scaling;
        }
    }
}