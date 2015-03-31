using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Microsoft.Samples.Kinect.XnaBasics
{
    public class Frog
    {
        const float jumpThreshold = 0.0f;
        public bool IsDead = false;
        public bool IsOnLog = false;
        public float deathTime = 0;
        public bool isInvincible = false;
        bool firstTimeOut;
        public Vector2 position;
        public Texture2D skin;
        public Texture2D deadFrog;
        private float lastNavigated;
        private float lastMoveHorizontal, lastJump;
        bool init;
        private Point currentGridPosition;
        bool isDown = false;
       public  bool hasWin = false;

        public Frog(ContentManager content, Rectangle viewPortRectangle)
        {
            lastJump = 0;
            lastMoveHorizontal = 0;
            firstTimeOut = true;            
            currentGridPosition.Y = 0;
            currentGridPosition.X = 5;
            position = new Vector2(0, 0);
            position.X = currentGridPosition.X * XnaBasics.GridSizePixelsX + 10;
            position.Y = (110 + viewPortRectangle.Height) - currentGridPosition.Y * XnaBasics.GridSizePixelsY;
            init = false;
            
            skin = content.Load<Texture2D>("frog");
            deadFrog = content.Load<Texture2D>("deadfrog");
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, Rectangle viewPortRectangle)
        {           
            if (!init)
            {
                lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                init = true;
            }
            ///////////test///////////////////
            if (!IsDead)
            {
                if (firstTimeOut || gameTime.TotalGameTime.TotalMilliseconds - lastMoveHorizontal > 250)
	            {
	                 if (playerPosition.X > 0.25 && currentGridPosition.X < XnaBasics.GridWidth - 1)
	                 {
	                     currentGridPosition.X++;
                         position.X += XnaBasics.GridSizePixelsX;              
	                 }
	                 if (playerPosition.X < -0.25 && currentGridPosition.X > 0)
	                 {
	                     currentGridPosition.X--;
                         position.X -= XnaBasics.GridSizePixelsX;           
	                 }
	                 firstTimeOut = false;
	                 lastMoveHorizontal = (int)gameTime.TotalGameTime.TotalMilliseconds;
	            }          
	
	            if (playerPosition.X > -0.20 && playerPosition.X < 0.20)
	            {
	                firstTimeOut = true;               
	            }
	
	            if (gameTime.TotalGameTime.TotalMilliseconds - lastJump > 200)
	            {
	                if (PlayerJumped(playerPosition))
	                {
	                    if (currentGridPosition.Y <= (int)(viewPortRectangle.Height / XnaBasics.GridSizePixelsY) - 2)
	                    {
	                        currentGridPosition.Y++;
                            position.Y -= XnaBasics.GridSizePixelsY;
                            if (currentGridPosition.Y == (int)(viewPortRectangle.Height / XnaBasics.GridSizePixelsY) - 2 && !IsOnWater())
                            {
                                hasWin = true;
                            }
	                    }
	                    lastJump = (int)gameTime.TotalGameTime.TotalMilliseconds;
	                }
	            }

                if (isInvincible && gameTime.TotalGameTime.TotalMilliseconds - deathTime > 4000)
                {
                    isInvincible = false;
                    deathTime = 0;
                }
            }
            else if (!isInvincible && gameTime.TotalGameTime.TotalMilliseconds - deathTime > 2000)
            {
                isInvincible = true;
                IsDead = false;               
            }           
        }

        public bool IsOnWater()
        {
            return (currentGridPosition.Y == 6 || currentGridPosition.Y == 7 || currentGridPosition.Y == 8 || (currentGridPosition.Y == 9 && ((position.X >= 2 * XnaBasics.GridSizePixelsX && position.X <= 3 * XnaBasics.GridSizePixelsX) || (position.X >= 7 * XnaBasics.GridSizePixelsX && position.X <= 8 * XnaBasics.GridSizePixelsX))));
            
        }

        private bool PlayerJumped(Vector2 playerPosition)
        {           
            if (playerPosition.Y < jumpThreshold)
            {
                isDown = true;
                return false;
            }
            else if (isDown && playerPosition.Y > jumpThreshold)
            {
                isDown = false;
                return true;
            }
            else return false;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle viewPortRectangle)
        {
            if (IsDead)
            {
                spriteBatch.Draw(deadFrog, new Rectangle((int)position.X, (int)position.Y, XnaBasics.GridSizePixelsX, XnaBasics.GridSizePixelsY), Color.White);
            }
            else
                spriteBatch.Draw(skin, new Rectangle((int)position.X, (int)position.Y, XnaBasics.GridSizePixelsX, XnaBasics.GridSizePixelsY), Color.White);
        }
    }
}
