using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace Wavepool
{
    public class Ripple
    {
        public bool alive;

        float amplitude = 30;
        float speed = 200;
        float wavelength = 30;
        float crestCount = 1;
        float decayRate = 0.2f;
        const float twoPI = System.MathF.PI * 2;

        Vector2 origin;
        float startingRadius;
        float radius;
        float strength;
        float decay;

        public Ripple(Vector2 origin, RippleParameters parameters)
        {
            Init(origin, parameters.radius);

            amplitude = parameters.amplitude;
            speed = parameters.speed;
            wavelength = parameters.wavelength;
            crestCount = parameters.crestCount;
            decayRate = parameters.decayRate;
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
            radius += speed * deltaTime;
            strength = startingRadius / radius;
            decay -= decayRate * deltaTime;

            strength *= decay * decay;

            UpdateConstants();

            if (decay < 0)
                alive = false;
        }

        float waveLimit;
        float waveLowerLimit;
        float waveTailLimit;
        float waveTailLimitSquared;
        float baseScaling;

        void UpdateConstants()
        {
            waveLowerLimit = radius - twoPI * wavelength * (crestCount - 0.75f);
            if (waveLowerLimit < 0)
                waveLowerLimit = 0;
            else
                waveLowerLimit *= waveLowerLimit;

            waveTailLimit = radius - twoPI * wavelength * (crestCount - 0.25f);
            if(waveTailLimit < 0)
                waveTailLimit = 0;
            waveTailLimitSquared = waveTailLimit * waveTailLimit;

            waveLimit = radius + (wavelength * System.MathF.PI) / 2;
            waveLimit *= waveLimit;

            baseScaling = strength * amplitude;
        }

        public Vector2 GetOffset(Vector2 point)
        {
            Vector2 diff = point - origin;
            float dist = diff.LengthSquared();

            if (dist == 0)
                return Vector2.Zero;

            if (dist > waveLimit || dist < waveLowerLimit)
            {
                if(dist < waveLowerLimit && dist > waveTailLimitSquared)
                {
                    return GetTailOffset(diff, dist);
                }
                return Vector2.Zero;
            }

            dist = System.MathF.Sqrt(dist);

            float scaling = 1 + dist / wavelength;

            float oscillation = System.MathF.Cos((dist - radius) / wavelength);

            return Vector2.Normalize(diff) * baseScaling * oscillation * scaling;
        }

        public Vector2 GetTailOffset(Vector2 dir, float distSquared)
        {
            float dist = System.MathF.Sqrt(distSquared);

            float scaling = (dist - waveTailLimit) / (wavelength * System.MathF.PI);
            scaling *= 1 + dist / wavelength;

            float oscillation = System.MathF.Cos((dist - radius) / wavelength);

            return Vector2.Normalize(dir) * baseScaling * oscillation * scaling;
        }
    }
}