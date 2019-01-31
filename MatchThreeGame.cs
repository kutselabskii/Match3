﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    public class MatchThreeGame : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static ButtonState previousMouseButtonState;

        Board board;

        public MatchThreeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            board = new Board();
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 864;
            graphics.ApplyChanges();

            IsMouseVisible = true;
            previousMouseButtonState = ButtonState.Released;

            board.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            board.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            board.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            board.Update(gameTime);
            previousMouseButtonState = Mouse.GetState().LeftButton;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            board.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
