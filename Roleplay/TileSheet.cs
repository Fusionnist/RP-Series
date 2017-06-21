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
        public string[] tileTNames, tileNames;
        public TileSheet(string[] tiles_, string[] names_)
        {
            tileNames = names_;
            tileTNames = tiles_;
        }
    }
}
