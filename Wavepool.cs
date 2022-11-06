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

        Texture2D texture;
        SpriteBatch spriteBatch;

        Vector2 drawOffset;
        Vector2 drawScale;
        float drawSize;

        Vector2 position;
        Vector2 size;
        Vector2 spacing;

        int rows;
        int columns;


        public Wavepool(Vector2 position, Vector2 size, int rows, int columns, float drawSize)
        {
            ripples = new List<Ripple>();
            doomedRipples = new List<Ripple>();

            this.position = position;
            this.size = size;
            spacing = new Vector2(size.X / rows, size.Y / columns);

            this.rows = rows;
            this.columns = columns;
            this.drawSize = drawSize;
        }

        public void Load(Texture2D texture, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            drawOffset = new Vector2(texture.Width / 2, texture.Height / 2);
            drawScale = Vector2.One * drawSize / texture.Height;
            this.spriteBatch = spriteBatch;
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
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    DrawPoint(GetPoint(i, j));
                }
            }
            spriteBatch.End();
        }

        public void AddRipple(Vector2 origin, SoundEffect sound)
        {
            float halfX = size.X / 2;
            float panning = (origin.X - halfX) / halfX;
            ripples.Add(new Ripple(origin, sound, panning, DestroyRipple));
        }
        public void DestroyRipple(Ripple ripple)
        {
            doomedRipples.Add(ripple);
        }

        Vector2 GetPoint(int rowIndex, int columnIndex)
        {
            Vector2 point = new Vector2(rowIndex * spacing.X, columnIndex * spacing.Y);
            point += position;
            return point + GetOffset(point);
        }

        Vector2 GetOffset(Vector2 position)
        {
            Vector2 offset = Vector2.Zero;

            foreach(Ripple ripple in ripples)
            {
                offset += ripple.GetOffset(position);
            }

            return offset;
        }

        void DrawPoint(Vector2 position)
        {
            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                drawOffset,
                drawScale,
                SpriteEffects.None,
                0
            );
        }
    }
}