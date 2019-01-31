using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    class Board
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D squareTexture;

        public void Initialize()
        {
            graphics = MatchThreeGame.graphics;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            spriteBatch = MatchThreeGame.spriteBatch;
            squareTexture = content.Load<Texture2D>("Figures/square0000");
        }

        public void UnloadContent()
        {
        
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(squareTexture, new Rectangle(0, 96, 128, 96), Color.White);
        }
    }
}
