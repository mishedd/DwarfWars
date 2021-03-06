﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using DwarfWars.Library;
using Lidgren.Network;
using System;

namespace DwarfWars.Server
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ServerGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Server server;
        SpriteFont font;

        public ServerGame(Server server)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.server = server;
            IsMouseVisible = true;
            
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Texture2D temp = new Texture2D(GraphicsDevice, 1, 1);
            temp.SetData(new Color[] { Color.White });

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach(ServerPlayer p in server._clients)
            {
                spriteBatch.Draw(temp, new Rectangle(p.Avatar.XPos, p.Avatar.YPos, 50, 50), Color.Red);
                spriteBatch.DrawString(font, p.ID.ToString(), new Vector2(p.Avatar.XPos, p.Avatar.YPos), Color.Black);
            }
            spriteBatch.DrawString(font, server._clients.Count.ToString(), new Vector2(0, 0), Color.Black);
            base.Draw(gameTime);
            
            spriteBatch.End();
        }
        
    }
}
