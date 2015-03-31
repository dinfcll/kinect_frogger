using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class Car : Entity
    {
        public static Texture2D[] carTextures;
        private static string[] CarModels = new string[] { "auto", "autobusEcole" };       
        private Texture2D texture;
        private static Random r;
        public static List<Car> carList = new List<Car>();

        public Car(Vector2 originalPosition, float speed, Vector2 direction) :
            base(originalPosition, speed, direction)
        {                      
            texture = carTextures[r.Next(carTextures.Length)];
            drawRectangle = new Rectangle((int)currentPosition.X, (int)currentPosition.Y,(int)(((float)texture.Width / (float)texture.Height) * XnaBasics.GridSizePixelsY), XnaBasics.GridSizePixelsY);
        }

        public static void Update(GameTime gametime)
        {
            Vector2 spawnPosition;
            Random r = new Random();
            Vector2 direction;
            float closestDistanceFromSpawnPosition;
            float distanceFromSpawner;

            //spawn cars
            for (int y = 1; y < 5; y++)
            {
                if (y % 2 == 0)
                {
                    direction = new Vector2(1, 0);
                    spawnPosition = new Vector2(0, XnaBasics.Height + 20 - (y * XnaBasics.GridSizePixelsY));
                }
                else
                {
                    spawnPosition = new Vector2(XnaBasics.Width, XnaBasics.Height + 20 - (y * XnaBasics.GridSizePixelsY));
                    direction = new Vector2(-1, 0);
                }

                closestDistanceFromSpawnPosition = 999999;
                for (int i = 0; i < carList.Count; i++)
                {
                    if (carList[i].direction.X == direction.X)
                    {
                        distanceFromSpawner = (carList[i].currentPosition - spawnPosition).Length();
                        if (distanceFromSpawner < closestDistanceFromSpawnPosition)
                            closestDistanceFromSpawnPosition = distanceFromSpawner;
                    }
                }
                //spawn car if distance du plus proche car est plus grand que "random entre 100 et 500"
                if (closestDistanceFromSpawnPosition > r.Next(300, 800))
                {
                    carList.Add(new Car(spawnPosition, 0.10f, direction));
                }
            }

            for (int i = 0; i < carList.Count; i++)
            {
                carList[i].UpdateEntity(gametime);                
                if ((carList[i].currentPosition - carList[i].originalPosition).Length() > XnaBasics.Width + 200)
                {
                    carList.RemoveAt(i);
                    i--;
                }

            }
        }

        public static void LoadContent(ContentManager content)
        {
            r = new Random();
            carTextures = new Texture2D[CarModels.Length];
            for (int i = 0; i < CarModels.Length; i++)
            {
                carTextures[i] = content.Load<Texture2D>(CarModels[i]);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (direction.X == -1)
            {
                spriteBatch.Draw(texture, drawRectangle, null, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
            else spriteBatch.Draw(texture, drawRectangle, null, Color.White, 0.0f, new Vector2(0,0), SpriteEffects.None,0 );
        }
    }
}
