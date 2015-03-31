using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Microsoft.Samples.Kinect.XnaBasics
{
    class Log : Entity
    {
        public static Texture2D texture;
        public static List<Log> logList = new List<Log>();

        public Log(Vector2 originalPosition, float speed, Vector2 direction) :
            base(originalPosition,speed, direction)
        {
            drawRectangle = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, (int)(((float)texture.Width / (float)texture.Height) * XnaBasics.GridSizePixelsY), XnaBasics.GridSizePixelsY);
        }

        public static void Update(GameTime gametime)
        {
            Vector2 spawnPosition;
            Random r = new Random();
            Vector2 direction;
            float closestDistanceFromSpawnPosition;
            float distanceFromSpawner;

            //spawn logs
            for (int y = 6; y < 9; y++)
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
                for (int i = 0; i < logList.Count; i++)
                {
                    if (logList[i].direction.X == direction.X)
                    {
                        distanceFromSpawner = (logList[i].currentPosition - spawnPosition).Length();
                        if (distanceFromSpawner < closestDistanceFromSpawnPosition)
                            closestDistanceFromSpawnPosition = distanceFromSpawner;
                    }
                }
                //spawn car if distance du plus proche car est plus grand que "random entre 100 et 500"
                if (closestDistanceFromSpawnPosition > r.Next(300, 800))
                {
                    logList.Add(new Log(spawnPosition, 0.10f, direction));
                }
            }

            bool isOnLog=false;
            for (int i = 0; i < logList.Count; i++)
            {
                Vector2 logDeplacement = logList[i].UpdateEntity(gametime);
                if (Rectangle.Intersect(logList[i].drawRectangle, new Rectangle((int)XnaBasics.myFrog.position.X + 20, (int)XnaBasics.myFrog.position.Y + 20, XnaBasics.GridSizePixelsX - 40, XnaBasics.GridSizePixelsY - 40)) != Rectangle.Empty)
                {
                    isOnLog = true;
                    XnaBasics.myFrog.position += logDeplacement;
                }               

                if ((logList[i].currentPosition - logList[i].originalPosition).Length() > XnaBasics.Width + 200)
                {
                    logList.RemoveAt(i);
                    i--;
                }
            }

            if (isOnLog)
            {
                XnaBasics.myFrog.IsOnLog = true;
            }
            else
            {
                XnaBasics.myFrog.IsOnLog = false;
            }

            if (!XnaBasics.myFrog.IsOnLog && XnaBasics.myFrog.IsOnWater())
            {
                XnaBasics.myFrog.IsDead = true;
                XnaBasics.myFrog.deathTime = (float)gametime.TotalGameTime.TotalMilliseconds;
            }
            
        }

        public static void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("buche");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, Color.White);
        }
    }
}
