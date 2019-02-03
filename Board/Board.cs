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
        Cross,
        None
    }

    public enum Outlines
    {
        Default,
        Highlighted,
        None
    }

    public enum GameStates
    {
        Playing,
        SwapAnimation,
        SwapBackwardsAnimation,
        FallAnimation,
    }

    class Board
    {
        public static Random rand = new Random();

        public static int rowHeight = 96;
        public static int columnWidth = 128;

        public static float swapSpeed = 300f;
        public static float fallSpeed = 400f;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Dictionary<Tiles, Texture2D> tileTextures;
        public static Dictionary<Outlines, Texture2D> outlineTextures;
        private Texture2D whiteBoxTexture;

        private List<List<Tile>> board;

        private Tile highlightedTile;
        private Tile targetTile;

        private GameStates state;

        private List<Tile> fallingTiles;

        float remainingTime;

        public Board()
        {
            graphics = MatchThreeGame.graphics;
            tileTextures = new Dictionary<Tiles, Texture2D>();
            outlineTextures = new Dictionary<Outlines, Texture2D>();
        }

        public void Initialize()
        {
            fallingTiles = new List<Tile>();

            do
            {
                GenerateBoard();
                FindMultiples();
            } while (DeleteMarkedTiles());

            MatchThreeGame.score = 0;
            remainingTime = 60f;

            state = GameStates.Playing;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            spriteBatch = MatchThreeGame.spriteBatch;

            tileTextures.Add(Tiles.Rectangle, content.Load<Texture2D>("Figures/square0000"));
            tileTextures.Add(Tiles.Triangle, content.Load<Texture2D>("Figures/triangle0000"));
            tileTextures.Add(Tiles.Circle, content.Load<Texture2D>("Figures/circle0000"));
            tileTextures.Add(Tiles.Trapezium, content.Load<Texture2D>("Figures/trapezium0000"));
            tileTextures.Add(Tiles.Cross, content.Load<Texture2D>("Figures/cross0000"));
            tileTextures.Add(Tiles.None, CreateOutlineTexture(Color.Transparent));

            outlineTextures.Add(Outlines.Default, CreateOutlineTexture(Color.LightGray));
            outlineTextures.Add(Outlines.Highlighted, CreateOutlineTexture(Color.Yellow, 5));
            outlineTextures.Add(Outlines.None, CreateOutlineTexture(Color.Transparent));

            whiteBoxTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            whiteBoxTexture.SetData(new Color[]{ Color.White });
        }

        public bool Update(GameTime gameTime)
        {
            remainingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (remainingTime <= 0f)
            {
                board.Clear();
                highlightedTile = null;
                targetTile = null;
                fallingTiles.Clear();
                return true;
            }

            switch (state)
            {
                case GameStates.Playing:
                    HandlePlaying(gameTime);
                    break;
                case GameStates.SwapAnimation:
                case GameStates.SwapBackwardsAnimation:
                    HandleSwapAnimation(gameTime);
                    break;
                case GameStates.FallAnimation:
                    HandleFallAnimation(gameTime);
                    break;
            }

            return false;
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board[i].Count; j++)
                    board[i][j].Draw(spriteBatch);

            for (int i = 0; i < fallingTiles.Count; i++)
                fallingTiles[i].Draw(spriteBatch);

            spriteBatch.Draw(whiteBoxTexture,
                new Rectangle(0, 0, columnWidth * 8, rowHeight),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0.5f
            );

            spriteBatch.DrawString(MatchThreeGame.font, 
                "Score: " + MatchThreeGame.score, 
                new Vector2(columnWidth, rowHeight / 2),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString(MatchThreeGame.font,
                "Remaining time: " + Math.Round(remainingTime),
                new Vector2(columnWidth * 5, rowHeight / 2),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public static Texture2D CreateOutlineTexture(Color color, int size = 1)
        {
            Texture2D outline = new Texture2D(MatchThreeGame.graphics.GraphicsDevice, columnWidth, rowHeight);

            Color[] pixels = new Color[rowHeight * columnWidth];
            for (int i = 0; i < rowHeight; i++)
                for (int j = 0; j < columnWidth; j++)
                    if (i < size || j < size || i > rowHeight - size - 1 || j > columnWidth - size - 1)
                        pixels[i * columnWidth + j] = color;
                    else
                        pixels[i * columnWidth + j] = Color.Transparent;
            outline.SetData(pixels);
            return outline;
        }

        private void GenerateBoard()
        {
            highlightedTile = null;

            if (board != null)
                board.Clear();

            board = new List<List<Tile>>();
            for (int i = 0; i < 8; i++)
                board.Add(new List<Tile>());

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i].Add(new Tile(i, j, (Tiles)rand.Next(0, 5), Outlines.Default));
        }

        private void RemoveSelection()
        {
            if (highlightedTile.tile != Tiles.None)
                highlightedTile.outline = Outlines.Default;
            if (targetTile != null && targetTile.tile != Tiles.None)
                targetTile.outline = Outlines.Default;
            highlightedTile = null;
            targetTile = null;
        }

        private void HandlePlaying(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && MatchThreeGame.previousMouseButtonState != ButtonState.Pressed)
            {
                int x = mouseState.X / columnWidth;
                int y = (mouseState.Y - rowHeight) / rowHeight;

                if (mouseState.Y > rowHeight && x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    if (highlightedTile != null)
                    {
                        if (highlightedTile.IsNeighbour(board[x][y]))
                        {
                            state = GameStates.SwapAnimation;
                            targetTile = board[x][y];
                            highlightedTile.target = targetTile;
                            targetTile.target = highlightedTile;
                        }
                        else
                        {
                            RemoveSelection();
                        }
                    }
                    else
                    {
                        board[x][y].outline = (board[x][y].outline == Outlines.Default) ? Outlines.Highlighted : Outlines.Default;
                        highlightedTile = board[x][y];
                    }
                }
            }
        }

        private void HandleSwapAnimation(GameTime gameTime)
        {
            highlightedTile.Move(gameTime, true);
            targetTile.Move(gameTime, true);

            if ((targetTile.coordinates - targetTile.target.EstimatedCoordinates).Length() < 5f)
            {
                highlightedTile.target = null;
                targetTile.target = null;

                highlightedTile.Swap(targetTile);

                if (state == GameStates.SwapBackwardsAnimation)
                {
                    state = GameStates.Playing;
                    highlightedTile.coordinates = highlightedTile.EstimatedCoordinates;
                    targetTile.coordinates = targetTile.EstimatedCoordinates;
                    RemoveSelection();
                }

                if (state == GameStates.SwapAnimation)
                {
                    FindMultiples();
                    if (DeleteMarkedTiles())
                    {
                        state = GameStates.FallAnimation;
                        RemoveSelection();

                        PrepareFalling();
                    }
                    else
                    {
                        state = GameStates.SwapBackwardsAnimation;

                        int x = highlightedTile.x, y = highlightedTile.y;
                        int hX = targetTile.x, hY = targetTile.y;

                        highlightedTile = board[hX][hY];
                        targetTile = board[x][y];
                        highlightedTile.target = targetTile;
                        targetTile.target = highlightedTile;
                    }
                }
            }
        }

        private void ClearBuffer(List<Tile> buffer)
        {
            if (buffer.Count > 2)
                for (int k = 0; k < buffer.Count; k++)
                    buffer[k].markedAsDead = true;
            buffer.Clear();
        }

        private void FindMultiples()
        {
            // Horizontal
            for (int i = 0; i < 8; i++)
            {
                List<Tile> buffer = new List<Tile>();
                for (int j = 0; j < 8; j++)
                {
                    if (buffer.Count == 0)
                    {
                        buffer.Add(board[i][j]);
                        continue;
                    }

                    if (buffer[0].tile == board[i][j].tile)
                        buffer.Add(board[i][j]);
                    else
                    {
                        ClearBuffer(buffer);
                        buffer.Add(board[i][j]);
                    }
                }
                ClearBuffer(buffer);
            }

            // Vertical
            for (int j = 0; j < 8; j++)
            {
                List<Tile> buffer = new List<Tile>();
                for (int i = 0; i < 8; i++)
                {
                    if (buffer.Count == 0)
                    {
                        buffer.Add(board[i][j]);
                        continue;
                    }

                    if (buffer[0].tile == board[i][j].tile)
                        buffer.Add(board[i][j]);
                    else
                    {
                        ClearBuffer(buffer);
                        buffer.Add(board[i][j]);
                    }
                }
                ClearBuffer(buffer);
            }
        }

        private bool DeleteMarkedTiles()
        {
            bool result = false;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board[i][j].markedAsDead)
                    {
                        MatchThreeGame.score++;
                        result = true;
                        board[i][j].markedAsDead = false;
                        board[i][j].outline = Outlines.None;
                        board[i][j].tile = Tiles.None;
                    }
                }
            return result;
        }

        private void PrepareFalling()
        {
            for (int i = 0; i < 8; i++)
            {
                int current = 7;

                for (int j = 7; j >= 0; j--)
                {
                    if (board[i][j].tile != Tiles.None)
                    {
                        if (j != current)
                            board[i][j].target = board[i][current];
                        current--;
                    }
                }

                int offset = -1;
                while (current >= 0)
                {
                    Tile newOne = new Tile(i, offset, (Tiles)rand.Next(0, 5), Outlines.Default);
                    newOne.target = board[i][current];
                    current--;
                    offset--;
                    fallingTiles.Add(newOne);
                }
            }
        }

        private void HandleFallAnimation(GameTime gameTime)
        {
            bool isFalling = false;

            for (int i = 0; i < 8; i++)
                for (int j = 7; j >= 0; j--)
                {
                    if (board[i][j].target != null)
                    {
                        isFalling = true;
                        board[i][j].Move(gameTime);

                        if ((board[i][j].coordinates - board[i][j].target.EstimatedCoordinates).Length() < 5f)
                        {
                            board[i][j].target.Swap(board[i][j]);
                            board[i][j].target.outline = Outlines.Default;
                            board[i][j].target = null;
                        }
                    }
                }

            for (int i = 0; i < fallingTiles.Count; i++)
            {
                if (fallingTiles[i].target != null)
                {
                    isFalling = true;
                    fallingTiles[i].Move(gameTime);

                    if ((fallingTiles[i].coordinates - fallingTiles[i].target.EstimatedCoordinates).Length() < 5f)
                    {
                        fallingTiles[i].target.tile = fallingTiles[i].tile;
                        fallingTiles[i].target.outline = Outlines.Default;
                        fallingTiles[i].target = null;
                    }
                }
            }

            if (!isFalling)
            {
                fallingTiles.Clear();
                FindMultiples();
                if (DeleteMarkedTiles())
                    PrepareFalling();
                else
                    state = GameStates.Playing;
            }
        }
    }
}
