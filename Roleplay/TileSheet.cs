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
        public List<int> tileTIDs, tileIDs;
        public List<string> tileNames;
        public TileSheet(List<int> tileTIDs_, List<string> names_, List<int> tileIDs_)
        {
            tileNames = names_;
            tileTIDs = tileTIDs_;
            tileIDs = tileIDs_;
        }
    }
}
