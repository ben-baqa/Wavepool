using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Wavepool
{
    public class WaveGrid
    {
        Texture2D texture;
        SpriteBatch spriteBatch;

        Func<Vector2, Vector2> GetOffset;

        Vector2 drawOffset;
        Vector2 drawScale;
        float drawSize;

        Vector2 position;
        Vector2 size;
        Vector2 spacing;

        int rows;
        int columns;

        public WaveGrid(Vector2 position, Vector2 size, int rows, int columns, float drawSize, Func<Vector2, Vector2> GetOffset)
        {
            this.position = position;
            this.size = size;
            this.rows = rows;
            this.columns = columns;
            this.drawSize = drawSize;

            spacing = new Vector2(size.X / rows, size.Y / columns);
            this.GetOffset = GetOffset;
        }

        public void OnLoad(Texture2D texture, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;

            drawOffset = new Vector2(texture.Width / 2, texture.Height / 2);
            drawScale = Vector2.One * drawSize / texture.Height;
        }

        public void DrawGrid()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    DrawPoint(GetPoint(i, j));
                }
            }
        }

        public Vector2 GetPoint(int rowIndex, int columnIndex)
        {
            Vector2 point = new Vector2(rowIndex * spacing.X, columnIndex * spacing.Y);
            point += position;
            return point + GetOffset(point);
        }

        public void DrawPoint(Vector2 position)
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