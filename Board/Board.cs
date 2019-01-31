using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    class Board
    {
        private int rowHeight = 96;
        private int columnWidth = 128;

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
            Texture2D outline = new Texture2D(graphics.GraphicsDevice, columnWidth, rowHeight);

            Color[] pixels = new Color[rowHeight * columnWidth];
            for (int i = 0; i < rowHeight; i++)
                for (int j = 0; j < columnWidth; j++)
                    if (i == 0 || j == 0)
                        pixels[i * columnWidth + j] = Color.SlateGray;
                    else
                        pixels[i * columnWidth + j] = Color.Transparent;
            outline.SetData(pixels);

            spriteBatch.Draw(squareTexture, new Rectangle(0, rowHeight, columnWidth, rowHeight), Color.White);

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    spriteBatch.Draw(outline, new Rectangle(i * columnWidth, (j + 1) * rowHeight, columnWidth, rowHeight), Color.White);
        }
    }
}
