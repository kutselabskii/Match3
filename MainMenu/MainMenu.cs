using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    class MainMenu
    {
        private int x = 384;
        private int y = 384;
        private int width = 256;
        private int height = 128;

        private Texture2D texture;

        public void Initialize()
        {
            texture = Board.CreateOutlineTexture(Color.Blue, 2);
        }

        public bool Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && MatchThreeGame.previousMouseButtonState != ButtonState.Pressed)
            {
                if (x < mouseState.X && mouseState.X < x + width && y < mouseState.Y && mouseState.Y < y + height)
                    return true;
            }

            return false;
        }

        public void Draw(GameTime gameTime)
        {
            MatchThreeGame.spriteBatch.Draw(texture, new Rectangle(x, y, width, height), Color.White);
            MatchThreeGame.spriteBatch.DrawString(MatchThreeGame.font, "Play", new Vector2(x + width / 2.5f, y + height / 3), Color.Black);
        }
    }


}
