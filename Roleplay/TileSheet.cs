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
        public int w, h;
        public Texture2D src;
        public string[] tiles;
        public Point[] tpos;
        public TileSheet(string docpath_, ContentManager c_)
        {
            XDocument sheetDoc = XDocument.Load(docpath_);
            List<string> names = new List<string>();
            List<Point> ps = new List<Point>();
            w = int.Parse(sheetDoc.Element("Tilesheet").Attribute("w").Value);
            h = int.Parse(sheetDoc.Element("Tilesheet").Attribute("h").Value);

            src = c_.Load<Texture2D>(sheetDoc.Element("Tilesheet").Attribute("path").Value);

            foreach(XElement el in sheetDoc.Element("Tilesheet").Elements("Tile"))
            {
                names.Add(el.Attribute("name").Value);
                ps.Add(new Point(int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value)));
            }
            tiles = names.ToArray();
            tpos = ps.ToArray();
        }
    }
}
