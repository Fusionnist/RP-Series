using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Collections.Generic;


namespace Roleplay
{
    public class TileSheet
    {
        public string[] tiles;
        public TileSheet(string[] tiles_)
        {
            tiles = tiles_;
        }
    }
}
