using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchThree
{
    public class Tile
    {
        public int x, y;
        public Vector2 coordinates;
        public Tiles tile;
        public Outlines outline;
        public bool markedAsDead;

        public Tile target;

        public Tile()
        {
            markedAsDead = false;
            target = null;
        }

        public Tile(int x, int y, Tiles tile, Outlines outline)
        {
            markedAsDead = false;
            target = null;

            this.x = x;
            this.y = y;
            coordinates = new Vector2(x * Board.columnWidth, (y + 1) * Board.rowHeight);
            this.tile = tile;
            this.outline = outline;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Board.tileTextures[tile],
                new Rectangle((int)coordinates.X, (int)coordinates.Y, Board.columnWidth, Board.rowHeight),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );

            spriteBatch.Draw(Board.outlineTextures[outline],
                new Rectangle((int)coordinates.X, (int)coordinates.Y, Board.columnWidth, Board.rowHeight),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );
        }

        public bool IsNeighbour(Tile other)
        {
            return 
                (x == other.x && Math.Abs(y - other.y) == 1) || 
                (y == other.y && Math.Abs(x - other.x) == 1);
        }

        public void Swap(Tile other)
        {
            Tiles swapperTile = tile;
            tile = other.tile;
            other.tile = swapperTile;

            coordinates = EstimatedCoordinates;
            other.coordinates = other.EstimatedCoordinates;
        }

        public void Move(GameTime gameTime, bool isSwap = false)
        {
            Vector2 moveTo = target.EstimatedCoordinates - coordinates;
            moveTo.Normalize();
            if (isSwap)
                coordinates += moveTo * Board.swapSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                coordinates += moveTo * Board.fallSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public Vector2 EstimatedCoordinates
        {
            get
            {
                return new Vector2(x * Board.columnWidth, (y + 1) * Board.rowHeight);
            }
        }
    }
}
