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
        public int[] tileTIDs, tileIDs;
        public string[] tileNames;
        public TileSheet(int[] tileTIDs_, string[] names_, int[] tileIDs_)
        {
            tileNames = names_;
            tileTIDs = tileTIDs_;
            tileIDs = tileIDs_;
        }
    }
}
