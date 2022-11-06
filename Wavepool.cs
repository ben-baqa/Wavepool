using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Wavepool
{
    public class Wavepool
    {
        List<Ripple> ripples;
        List<Ripple> doomedRipples;

        WaveGrid waveGrid;

        SpriteBatch spriteBatch;

        float halfX;


        public Wavepool(Vector2 position, Vector2 size, int rows, int columns, float drawSize)
        {
            ripples = new List<Ripple>();
            doomedRipples = new List<Ripple>();

            halfX = size.X / 2;

            waveGrid = new WaveGrid(position, size, rows, columns, drawSize, GetOffset);
        }

        public void Load(Texture2D texture, GraphicsDevice graphicsDevice)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            waveGrid.OnLoad(texture, spriteBatch);
        }

        public void Update(float deltaTime)
        {
            foreach(Ripple ripple in ripples)
            {
                ripple.Update(deltaTime);
            }

            foreach(Ripple ripple in doomedRipples)
            {
                ripples.Remove(ripple);
            }
        }

        public void Draw()
        {
            spriteBatch.Begin();

            waveGrid.DrawGrid();

            spriteBatch.End();
        }

        public void AddRipple(Vector2 origin, SoundEffect sound)
        {
            float panning = (origin.X - halfX) / halfX;
            if (panning < -1) panning = -1;
            else if(panning > 1) panning = 1;
            ripples.Add(new Ripple(origin, sound, panning, DestroyRipple));
        }
        public void DestroyRipple(Ripple ripple)
        {
            doomedRipples.Add(ripple);
        }

        Vector2 GetOffset(Vector2 position)
        {
            Vector2 offset = Vector2.Zero;

            foreach (Ripple ripple in ripples)
            {
                offset += ripple.GetOffset(position);
            }

            return offset;
        }
    }
}