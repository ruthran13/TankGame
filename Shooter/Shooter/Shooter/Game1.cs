using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Handler handler;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Texture2D backgroundTexture, gameBackgroundTexture, tankTexture, brickWallTexture, stoneWallTexture, waterTexture,exitButtonTexture, joinButtonTexture, tryAgainTexture, exitTexture, lifePackTexture, coinPileTexture, bulletTexture;
        int screenWidth, screenHight;
        int joinedCounter;  ///to check whether the player has got connected to the server 0-not tried 1-tried,not joined, 2-joined
        Rectangle gameScreen, gameBackScreen, statisticScreen, exitButton, joinButton, tryAgainButton, exit;
        SpriteFont statistics;
        int updateCounter, timeSinceCoinDrawn, timeSinceLifeDrawn, coinDrawnCount, lifePackDrawnCount, time;
        Boolean started;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            handler = new Handler();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //.SynchronizeWithVerticalRetrace = false;
            started = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1);
            coinDrawnCount = lifePackDrawnCount = 0;
            updateCounter = 1;
            joinedCounter = 0;
            time = 0;
            this.IsMouseVisible = true;
            base.Initialize();
            ///Server s=new Server();
            ///s.send();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary> 

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            graphics.PreferredBackBufferWidth = 1150;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Shooter";
            backgroundTexture = Content.Load<Texture2D>("background");
            gameBackgroundTexture = Content.Load<Texture2D>("gameBackground");
            screenHight = device.PresentationParameters.BackBufferHeight;
            screenWidth = device.PresentationParameters.BackBufferWidth;
            gameScreen = new Rectangle(74, 74, 600, 600);
            gameBackScreen = new Rectangle(50, 50, 648, 648);
            statisticScreen = new Rectangle(gameBackScreen.Right + 50, gameBackScreen.Top, 350, gameBackScreen.Height);
            
            tankTexture = Content.Load<Texture2D>("tank");
            //brickWallTexture = Content.Load<Texture2D>("brickWall");
            brickWallTexture = Content.Load<Texture2D>("brickWallNew");
            stoneWallTexture = Content.Load<Texture2D>("stoneWall");
            waterTexture = Content.Load<Texture2D>("water");
            coinPileTexture = Content.Load<Texture2D>("coinPile3");
            lifePackTexture = Content.Load<Texture2D>("healthPack2");
            bulletTexture = Content.Load<Texture2D>("bullet");

            statistics = Content.Load<SpriteFont>("PlayerStatistics");
            exitButtonTexture = Content.Load<Texture2D>("exitButton");
            exitButton = new Rectangle(statisticScreen.Left + 130, statisticScreen.Bottom - 170, 50, 50);
            joinButtonTexture = Content.Load<Texture2D>("JoinButton");
            joinButton = new Rectangle(graphics.PreferredBackBufferWidth/2 -100,  graphics.PreferredBackBufferHeight/2 -50, 200, 100);
            tryAgainTexture = Content.Load<Texture2D>("tryAgain");
            tryAgainButton = new Rectangle(graphics.PreferredBackBufferWidth / 2 - 225, graphics.PreferredBackBufferHeight / 2 - 50, 200, 100);
            exitTexture = Content.Load<Texture2D>("exitButton2");
            exit = new Rectangle(graphics.PreferredBackBufferWidth / 2 + 25, graphics.PreferredBackBufferHeight / 2 - 50, 200, 100);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.Milliseconds;

            //Console.WriteLine("abcd");
            // Allows the game to exit
            processMouseInput();
            //processKeyBoardInput();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            
            if (time % 100 == 0)
            {
               handler.updateBullets();
            }

            if (time >990 && joinedCounter == 2)
            {
                //updateCounter = 1;
                //this.Draw(gameTime);
                //started = handler.receive(gameScreen);
                //Console.WriteLine(time + gameTime.ElapsedGameTime.Milliseconds);
                if (started)
                {
                    //handler.moveTank();
                    handler.receive(gameScreen);
                }
                else
                {
                    started = handler.receive(gameScreen);
                }
                //this.Draw(gameTime);
                //Console.WriteLine(time + gameTime.ElapsedGameTime.Milliseconds);
                time = 0;
            }
            base.Update(gameTime);
            //updateCounter++;








            //if (updateCounter % 900 == 0 && joinedCounter == 2)
            //{
            //    updateCounter = 1;
            //    //this.Draw(gameTime);
            //    started = handler.receive(gameScreen);
            //    if (started)
            //    {
            //        //handler.moveTank();
            //        handler.receive(gameScreen);
            //    }
            //    else
            //    {
            //        started = handler.receive(gameScreen);
            //    }   
            //    //this.Draw(gameTime);
            //}
            //base.Update(gameTime);
            //updateCounter++;


            //Console.WriteLine(gameTime.ElapsedGameTime.Seconds + "\t" + gameTime.ElapsedGameTime.Milliseconds);
            if (coinDrawnCount > 0)
            {
                //coinDrawnCount = 0;

                /*foreach (Coins pile in handler.coinPiles)
                {
                    pile.spentTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (pile.spentTime >= pile.lifeTime)
                    {
                        coinDrawnCount++;
                        pile.isAlive = false;
                        handler.coinPiles.Remove(pile);
                    }
                }*/
                if (handler.updateCoinPile(gameTime.ElapsedGameTime.Milliseconds) == 0)
                {
                    coinDrawnCount = 0;
                }
            }
            if (lifePackDrawnCount > 0)
            {
                /*foreach (LifePack pack in handler.lifePacks)
                {
                    timeSinceLifeDrawn = gameTime.ElapsedGameTime.Milliseconds;
                    if (timeSinceLifeDrawn >= pack.lifeTime)
                    {
                        lifePackDrawnCount++;
                        pack.isAlive = false;
                        handler.lifePacks.Remove(pack);
                    }
                }*/
                if (handler.updateLifePacks(gameTime.ElapsedGameTime.Milliseconds) == 0)
                {
                    lifePackDrawnCount = 0;
                }
            }


            //this.Draw(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            drawBackground();
            if (joinedCounter == 0)
            { 
                drawButton(joinButtonTexture, joinButton);
            }
            else if (joinedCounter == 1)
            {
                drawButton(tryAgainTexture, tryAgainButton);
                drawButton(exitTexture, exit);
            }
            
            else if(joinedCounter == 2)
            {
                //create();
                drawGameBackGround();
                drawObstacles();
                drawButton(exitButtonTexture, exitButton);
                if (started)
                {
                    drawText();
                    drawTanks();
                    drawcoinPiles();
                    drawLifePacks();
                    drawBullets();
                    
                }

                
            }
            spriteBatch.End();
            base.Draw(gameTime);
            //Console.WriteLine("dhff");
        }

        private void drawBackground()
        {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, screenWidth, screenHight), Color.White);
            
        }

        private void drawGameBackGround()
        {
            spriteBatch.Draw(gameBackgroundTexture, statisticScreen, Color.White);
            spriteBatch.Draw(gameBackgroundTexture, gameBackScreen, Color.White);
            
        }

        private void drawTanks()
        {
            foreach (Tank tank in handler.tanks)
            {
                if (tank.health>0)
                {
                    Vector2 newPosition = new Vector2(tank.position.X * 30 + gameScreen.X+15, tank.position.Y * 30 + gameScreen.Y+15);
                    spriteBatch.Draw(tankTexture, newPosition, null, tank.color, tank.direction*MathHelper.PiOver2, new Vector2(15, 15), 1, SpriteEffects.None, 0);
                }
            }
        }

        private void drawObstacles()
        {
            foreach (Obstacle obstacle in handler.obstacles)
            {
                if (obstacle != null)
                {
                    Vector2 newPosition = new Vector2(obstacle.position.X * 30 + gameScreen.X, obstacle.position.Y * 30 + gameScreen.Y);
                    if (obstacle.type == "brickWall")
                    {
                        if (obstacle.damageLevel == 0)
                        {
                            spriteBatch.Draw(brickWallTexture, newPosition, null, Color.Brown, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                        }
                        else if (obstacle.damageLevel == 1)
                        {
                            spriteBatch.Draw(brickWallTexture, newPosition, null, Color.Brown, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                        }
                        else if (obstacle.damageLevel == 2)
                        {
                            spriteBatch.Draw(brickWallTexture, newPosition, null, Color.Brown, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                        }
                        else if (obstacle.damageLevel == 3)
                        {
                            spriteBatch.Draw(brickWallTexture, newPosition, null, Color.Brown, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(brickWallTexture, newPosition, null, Color.Brown, 0, new Vector2(0, 0), 0, SpriteEffects.None, 0);
                        }

                    }
                    else if (obstacle.type == "stoneWall")
                    {
                        spriteBatch.Draw(stoneWallTexture, newPosition, null, Color.Gray, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                    }
                    else if (obstacle.type == "water")
                    {
                        spriteBatch.Draw(waterTexture, newPosition, null, Color.Blue, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
                    }
                }
            }
        }
        private void drawcoinPiles()
        {
            foreach (Coins pile in handler.coinPiles)
            {
                if (pile.isAlive)
                {
                    Vector2 newPos = new Vector2(pile.position.X * 30 + gameScreen.X, pile.position.Y * 30 + gameScreen.Y);
                    coinDrawnCount = 1;
                    spriteBatch.Draw(coinPileTexture, newPos, Color.White);
                }
            }
        }

        private void drawLifePacks()
        {
            foreach (LifePack pack in handler.lifePacks)
            {
                Vector2 newPos = new Vector2(pack.position.X * 30 + gameScreen.X, pack.position.Y * 30 + gameScreen.Y);
                lifePackDrawnCount = 1;
                spriteBatch.Draw(lifePackTexture, newPos, Color.White);
            }
        }

        private void drawBullets()
        {
            foreach (Bullet bullet in handler.bullets)
            {
                  
                Vector2 pos = new Vector2(bullet.position.X + gameScreen.X+15, bullet.position.Y + gameScreen.Y+15);
                //Console.WriteLine(bullet.position.X + "\t" + bullet.position.Y);
                try
                {
                    //if (handler.grid[((int)pos.X - 15) / 30, ((int)pos.Y - 15) / 30].IsWalkable(5))
                    //{
                        //Console.WriteLine(((int)pos.Y - 15) / 30);
                        spriteBatch.Draw(bulletTexture, pos, null, Color.White, bullet.direction * MathHelper.PiOver2, new Vector2(15, 15), 1, SpriteEffects.None, 0);
                    //}
                    //else
                    //{
                       // bullet.hit = true;
                    //}
                }
                catch (NullReferenceException e)
                {
                    //Console.WriteLine(((int)pos.Y - 15) / 30);
                    spriteBatch.Draw(bulletTexture, pos, null, Color.White, bullet.direction * MathHelper.PiOver2, new Vector2(15, 15), 1, SpriteEffects.None, 0);
                }
                
                
            }
            //Console.WriteLine();
        }

        private void drawText()
        {
            spriteBatch.DrawString(statistics,"Player Statistics", new Vector2(statisticScreen.Left+85,statisticScreen.Top+50),Color.White);
            spriteBatch.DrawString(statistics, "Points   Coins   Health",new Vector2(statisticScreen.Left + 100, statisticScreen.Top + 100), Color.White,0,new Vector2(0,0),0.85f,0,0);
            int i = 0;
            foreach (Tank tank in handler.tanks)
            {
                if (tank.health==0)
                {
                    spriteBatch.DrawString(statistics, tank.name + "            " + tank.points + "          " + tank.coins + "         " + tank.health + "         dead", new Vector2(statisticScreen.Left + 50, statisticScreen.Top + 150 + (50 * i)), Color.White, 0, new Vector2(0, 0), 0.85f, 0, 0);
                }
                else
                {
                    spriteBatch.DrawString(statistics, tank.name + "            " + tank.points + "          " + tank.coins + "         " + tank.health, new Vector2(statisticScreen.Left + 50, statisticScreen.Top + 150 + (50 * i)), Color.White, 0, new Vector2(0, 0), 0.85f, 0, 0);
                }
                i++;
                
            }
        }

        public void drawButton(Texture2D aTexture, Rectangle aRectangle)
        {
            spriteBatch.Draw(aTexture, aRectangle, Color.White);
        }


        public void processKeyBoardInput()
        {
            KeyboardState s = Keyboard.GetState();
            if (s.IsKeyDown(Keys.Enter)){
                while (Keyboard.GetState().IsKeyDown(Keys.Enter)){
                }
                handler.shoot();
            }
        }
            
        
        public void processMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (joinedCounter == 0)
                {
                    if (mouseState.X >= joinButton.Left && mouseState.X <= joinButton.Right && mouseState.Y >= joinButton.Top && mouseState.Y <= joinButton.Bottom)
                    {
                        while (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                           // Console.WriteLine("Button Pressed");
                        }
                            Boolean isJoined = handler.join(gameScreen);
                            if (isJoined)
                            {
                                joinedCounter = 2;
                                Tick();
                            }
                            else
                            {
                                joinedCounter = 1;
                                Tick();
                            }
                        
                    }
                }
                else if (joinedCounter == 1)
                {
                    if (mouseState.X >= exit.Left && mouseState.X <= exit.Right && mouseState.Y >= exit.Top && mouseState.Y <= exit.Bottom)
                    {
                        this.Exit();
                    }
                    if (mouseState.X >= tryAgainButton.Left && mouseState.X <= tryAgainButton.Right && mouseState.Y >= tryAgainButton.Top && mouseState.Y <= tryAgainButton.Bottom)
                    {
                        Boolean isJoined = handler.join(gameScreen);
                        if (isJoined)
                        {
                            joinedCounter = 2;
                            Tick();
                        }
                        else
                        {
                            joinedCounter = 1;
                            Tick();
                        }
                    }
                }
                else if (joinedCounter == 2)
                {
                    if (mouseState.X >= exitButton.Left && mouseState.X <= exitButton.Right && mouseState.Y >= exitButton.Top && mouseState.Y <= exitButton.Bottom)
                    {
                        this.Exit();
                    }
                }
                else
                {
                }
                
            }
        }


    }
}
