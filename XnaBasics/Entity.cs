using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    abstract class Entity
    {
        public Vector2 currentPosition;
        public Vector2 direction;
        public Vector2 originalPosition;
        public float speed;
        
        public Rectangle drawRectangle;

        public Entity(Vector2 originalPosition, float speed, Vector2 direction)
        {
            this.direction = direction;
            this.originalPosition = originalPosition;
            this.currentPosition = originalPosition;
            this.speed = speed;
        }

        public Vector2 UpdateEntity(GameTime gameTime)
        {
            Vector2 deplacement = (float)(gameTime.ElapsedGameTime.TotalMilliseconds) * speed * direction;
            currentPosition += deplacement;
            drawRectangle.X = (int)(currentPosition.X);
            drawRectangle.Y = (int)(currentPosition.Y);
            return deplacement;
        }
    }
}
