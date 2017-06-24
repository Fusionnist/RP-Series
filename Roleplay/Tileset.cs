using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roleplay
{
    public class Tileset
    {
        public Tile[,] tiles;
        public int width, height, tw, th;
        public Tileset(Tile[,] tiles_, int width_, int height_, int tx_, int ty_)
        {
            tiles = tiles_;
            width = width_;
            height = height_;
            tw = tx_;
            th = ty_;
            PlaceTiles();
        }
        public void Draw(SpriteBatch sb_,float zoom_)
        {
            for(int x = 0; x<width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].Draw(sb_,zoom_);
                }
            }
        }
        public void PlaceTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].pos = new Vector2(x * 100 - y * 100, x * 51 + y * 51);
                    tiles[x, y].tsPos = new Point(x, y);
                }
            }
        }
    }
}
