using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    public enum Tiles
    {
        Rectangle,
        Triangle,
        Circle,
        Trapezium,
        Cross
    }

    public enum Outlines
    {
        Default,
        Highlighted
    }

    
    class Tile
    {
        public int x, y;
        public Tiles tile;
        public Outlines outline;

        public Tile(int _x, int _y, Tiles _tile, Outlines _outline)
        {
            x = _x;
            y = _y;
            tile = _tile;
            outline = _outline;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Board.tileTextures[tile], new Rectangle(x, y, Board.columnWidth, Board.rowHeight), Color.White);
            spriteBatch.Draw(Board.outlineTextures[outline], new Rectangle(x, y, Board.columnWidth, Board.rowHeight), Color.White);
        }

    }

    class Board
    {
        public static int rowHeight = 96;
        public static int columnWidth = 128;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Dictionary<Tiles, Texture2D> tileTextures;
        public static Dictionary<Outlines, Texture2D> outlineTextures;

        private List<List<Tile>> board;

        public void Initialize()
        {
            graphics = MatchThreeGame.graphics;
            tileTextures = new Dictionary<Tiles, Texture2D>();
            outlineTextures = new Dictionary<Outlines, Texture2D>();

            board = new List<List<Tile>>();
            for (int i = 0; i < 8; i++)
                board.Add(new List<Tile>());


            Random rand = new Random();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i].Add(new Tile(j * columnWidth, (i + 1) * rowHeight, (Tiles)rand.Next(0, 5), Outlines.Default));
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            spriteBatch = MatchThreeGame.spriteBatch;

            tileTextures.Add(Tiles.Rectangle, content.Load<Texture2D>("Figures/square0000"));
            tileTextures.Add(Tiles.Triangle, content.Load<Texture2D>("Figures/triangle0000"));
            tileTextures.Add(Tiles.Circle, content.Load<Texture2D>("Figures/circle0000"));
            tileTextures.Add(Tiles.Trapezium, content.Load<Texture2D>("Figures/trapezium0000"));
            tileTextures.Add(Tiles.Cross, content.Load<Texture2D>("Figures/cross0000"));

            outlineTextures.Add(Outlines.Default, CreateOutlineTexture(Color.SlateGray));
        }

        public void UnloadContent()
        {
        
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board[i].Count; j++)
                    board[i][j].Draw(spriteBatch);
        }

        private Texture2D CreateOutlineTexture(Color color)
        {
            Texture2D outline = new Texture2D(graphics.GraphicsDevice, columnWidth, rowHeight);

            Color[] pixels = new Color[rowHeight * columnWidth];
            for (int i = 0; i < rowHeight; i++)
                for (int j = 0; j < columnWidth; j++)
                    if (i == 0 || j == 0)
                        pixels[i * columnWidth + j] = color;
                    else
                        pixels[i * columnWidth + j] = Color.Transparent;
            outline.SetData(pixels);
            return outline;
        }
    }
}
