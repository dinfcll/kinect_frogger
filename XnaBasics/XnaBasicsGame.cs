using System.Collections.Generic;
//------------------------------------------------------------------------------
// <copyright file="XnaBasicsGame.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.XnaBasics
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// The main Xna game implementation.
    /// </summary>
    public class XnaBasics : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// This is used to adjust the window size.
        /// </summary>
        public static int Width = 1200;
        public static int Height = ((Width - 2) / 4) * 3;

        public static Frog myFrog;

        public static int GridSizePixelsX;
        public static int GridSizePixelsY;
        public static int GridWidth = 11;
        /// <summary>
        /// The graphics device manager provided by Xna.
        /// </summary>
        private readonly GraphicsDeviceManager graphics;
        
        /// <summary>
        /// This control selects a sensor, and displays a notice if one is
        /// not connected.
        /// </summary>
        private readonly KinectChooser chooser;

        /// <summary>
        /// This manages the rendering of the color stream.
        /// </summary>
      //  private readonly ColorStreamRenderer colorStream;

        /// <summary>
        /// This manages the rendering of the depth stream.
        /// </summary>
        private readonly DepthStreamRenderer depthStream;

        /// <summary>
        /// This is the location of the color stream when minimized.
        /// </summary>
        private readonly Vector2 colorSmallPosition;

        /// <summary>
        /// This is the location of the depth stream when minimized;
        /// </summary>
        private readonly Vector2 depthSmallPosition;

        /// <summary>
        /// This is the minimized size for both streams.
        /// </summary>
        private readonly Vector2 minSize;

        /// <summary>
        /// This is the viewport of the streams.
        /// </summary>
        public readonly Rectangle viewPortRectangle;

        private Texture2D background;
        /// <summary>
        /// This is the SpriteBatch used for rendering the header/footer.
        /// </summary>
        private SpriteBatch spriteBatch;


        /// <summary>
        /// This is the font for the footer.
        /// </summary>
        private SpriteFont font;


        /// <summary>
        /// Initializes a new instance of the XnaBasics class.
        /// </summary>
        public XnaBasics()
        {           
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "Frogger";
            
            // This sets the width to the desired width
            // It also forces a 4:3 ratio for height
            // Adds 110 for header/footer
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = Width;
            this.graphics.PreferredBackBufferHeight = ((Width / 4) * 3) + 110;
            this.graphics.PreparingDeviceSettings += this.GraphicsDevicePreparingDeviceSettings;
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.viewPortRectangle = new Rectangle(10, 190, Width - 20, Height -85);
            GridSizePixelsX = (viewPortRectangle.Width / GridWidth);
            GridSizePixelsY = (viewPortRectangle.Height / 10);

            Content.RootDirectory = "Content";

            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            this.Services.AddService(typeof(KinectChooser), this.chooser);
            
            // Calculate the minimized size and location
            this.depthStream = new DepthStreamRenderer(this);
            this.depthStream.Size = new Vector2(240, 180);
            this.depthStream.Position = new Vector2(Width - this.depthStream.Size.X - 15, 10);

            // Store the values so we can animate them later
            this.minSize = this.depthStream.Size;
            this.depthSmallPosition = this.depthStream.Position;
            this.colorSmallPosition = new Vector2(15, 85);

            this.Components.Add(this.chooser);
        }

        /// <summary>
        /// Loads the Xna related content.
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), this.spriteBatch);
            background = Content.Load<Texture2D>("background1");
            this.font = Content.Load<SpriteFont>("Segoe16");
            myFrog = new Frog(Content, viewPortRectangle);
            Car.LoadContent(Content);
            Log.LoadContent(Content);
            base.LoadContent();
        }

        /// <summary>
        /// Initializes class and components
        /// </summary>
        protected override void Initialize()
        {
            this.Components.Add(this.depthStream);
          //  this.Components.Add(this.colorStream);

            base.Initialize();
        } 

        /// <summary>
        /// This method updates the game state. Including monitoring
        /// keyboard state and the transitions.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Update(GameTime gameTime)
        {

            Log.Update(gameTime);
            Car.Update(gameTime);

            if (!myFrog.isInvincible)
            {
                for (int i = 0; i < Car.carList.Count; i++)
	            {
	                if (Rectangle.Intersect(Car.carList[i].drawRectangle, new Rectangle((int)myFrog.position.X+20, (int)myFrog.position.Y+20, GridSizePixelsX-40, GridSizePixelsY-40)) != Rectangle.Empty)
	                {
	                    myFrog.IsDead = true;
	                    myFrog.deathTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
	                }
	            }
            }

            myFrog.Update(gameTime, depthStream.skeletonStream.player1Coords, viewPortRectangle);



            base.Update(gameTime); 
        }

        /// <summary>
        /// This method renders the current state.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen
            GraphicsDevice.Clear(Color.White);

            // Render header/footer
            this.spriteBatch.Begin();
            spriteBatch.Draw(background, viewPortRectangle, Color.White);               
            this.spriteBatch.DrawString(this.font, depthStream.skeletonStream.skelCoords1, new Vector2(10, 50), Color.Black);

            if (!myFrog.hasWin)
            {
                for (int i = 0; i < Log.logList.Count; i++)
                {
                    Log.logList[i].Draw(spriteBatch);
                }

                myFrog.Draw(spriteBatch, viewPortRectangle);

                for (int i = 0; i < Car.carList.Count; i++)
                {
                    Car.carList[i].Draw(spriteBatch);
                }
            }
            else
            {
                this.spriteBatch.DrawString(this.font, "Vous avez gagné !!", new Vector2(100, 100), Color.Black);
            }
            

            this.spriteBatch.End();
            this.depthStream.DrawOrder = 2;

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the SkeletonStreamRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
    }
}
