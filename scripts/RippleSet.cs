using Microsoft.Xna.Framework;

namespace Wavepool
{
    public class RippleSet
    {
        RippleParameters[] rippleParams;
        RippleParameters centreRippleParams;
        Wavepool wavepool;
        Vector2 centre;

        public RippleSet(Wavepool wavepool, RippleParameters[] rippleParams, RippleParameters centreRippleParams, Vector2 screenSize)
        {
            this.wavepool = wavepool;
            this.rippleParams = rippleParams;
            this.centreRippleParams = centreRippleParams;

            centre = screenSize / 2;
        }

        public void SpawnCentreRipple()
        {
            wavepool.AddRipple(new Ripple(centre, centreRippleParams));
        }

        public void SpawnRipple(Vector2 origin, int radialIndex, float pitch)
        {
            RippleParameters p = rippleParams[0];
            if (radialIndex >= 0 && radialIndex < rippleParams.Length)
                p = rippleParams[radialIndex];

            float pitchRatio = System.MathF.Pow(2, pitch);
            p.wavelength /= pitchRatio;
            p.amplitude /= pitchRatio;

            wavepool.AddRipple(new Ripple(origin, p));
        }
    }

    public struct RippleParameters
    {
        public float radius;
        public float amplitude;
        public float speed;
        public float wavelength;
        public float crestCount;
        public float decayRate;

        public RippleParameters(float radius, float amplitude, float speed, float wavelength, float crestCount, float decayRate)
        {
            this.radius = radius;
            this.amplitude = amplitude;
            this.speed = speed;
            this.wavelength = wavelength;
            this.crestCount = crestCount;
            this.decayRate = decayRate;
        }

        public RippleParameters Lerp(RippleParameters target, float f)
        {
            RippleParameters result = new RippleParameters();
            result.radius = MathHelper.Lerp(radius, target.radius, f);
            result.amplitude = MathHelper.Lerp(amplitude, target.amplitude, f);
            result.speed = MathHelper.Lerp(speed, target.speed, f);
            result.wavelength = MathHelper.Lerp(wavelength, target.wavelength, f);
            result.crestCount = MathHelper.Lerp(crestCount, target.crestCount, f);
            result.decayRate = MathHelper.Lerp(decayRate, target.decayRate, f);
            return result;
        }
    }
}