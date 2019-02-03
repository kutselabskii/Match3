using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    enum MenuStates
    {
        MainMenu,
        Gameplay,
        EndScreen
    }


    public class MatchThreeGame : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static ButtonState previousMouseButtonState;

        public static SpriteFont font;
        public static int score;

        Board board;
        MainMenu mainMenu;
        EndScreen endScreen;

        MenuStates state;

        public MatchThreeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            board = new Board();
            mainMenu = new MainMenu();
            endScreen = new EndScreen();
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 864;
            graphics.ApplyChanges();

            IsMouseVisible = true;
            previousMouseButtonState = ButtonState.Released;

            state = MenuStates.MainMenu;
            mainMenu.Initialize();
            endScreen.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Arial");
            
            board.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case MenuStates.MainMenu:
                    if (mainMenu.Update(gameTime))
                    {
                        state = MenuStates.Gameplay;
                        board.Initialize();
                    }
                    break;
                case MenuStates.Gameplay:
                    if (board.Update(gameTime))
                    {
                        state = MenuStates.EndScreen;
                    }
                    break;
                case MenuStates.EndScreen:
                    if (endScreen.Update(gameTime))
                    {
                        state = MenuStates.MainMenu;
                    }
                    break;
            }

            previousMouseButtonState = Mouse.GetState().LeftButton;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            switch (state)
            {
                case MenuStates.MainMenu:
                    mainMenu.Draw(gameTime);
                    break;
                case MenuStates.Gameplay:
                    board.Draw(gameTime);
                    break;
                case MenuStates.EndScreen:
                    endScreen.Draw(gameTime);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
