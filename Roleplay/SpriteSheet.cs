using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;



namespace Roleplay
{
    public class SpriteSheet
    {
        public string name;
        public TileSheet tileSheet;
        public CreatureSheet crSheet;
        public Texture2D sourceTex;

        public string[] mtexTNames;
        public Rectangle[] sourceRects;
        public Facing[] facing;
        //anim
        public bool[] isAnimated;
        public float[] frameTime;
        public int[] frameCount;

        public SpriteSheet(string docpath_, ContentManager c_, string name_)
        {
            name = name_;
            XDocument sheetDoc = XDocument.Load(docpath_);
            List<Rectangle> ps = new List<Rectangle>();
            XElement sheetEl = null;
            foreach (XElement el in sheetDoc.Element("Root").Elements("Textures"))
            {
                if (name_ == el.Attribute("name").Value) { sheetEl = el;  }
            }
            sourceTex = c_.Load<Texture2D>(sheetEl.Attribute("src").Value);

            List<string> tn = new List<string>();
            List<string> tN = new List<string>();
            List<string> texn = new List<string>();
            if (sheetEl.Element("Tilesheet") != null)
            {
                foreach (XElement el in sheetEl.Element("Tilesheet").Elements("Tile"))
                {
                    tn.Add(el.Attribute("tname").Value);
                    tN.Add(el.Attribute("name").Value);
                }
            }
            

            List<string> crn = new List<string>();
            List<int> crhp = new List<int>();
            List<int> crmhp = new List<int>();
            List<bool> cra = new List<bool>();

            List<string[]> crTexNames = new List<string[]>();
            List<string[]> crTexUses = new List<string[]>();

            if (sheetEl.Element("Creatures") != null)
            {
                foreach (XElement el in sheetEl.Element("Creatures").Elements("Creature"))
                {
                    crn.Add(el.Attribute("name").Value);
                    crhp.Add(int.Parse(el.Attribute("hp").Value));
                    crmhp.Add(int.Parse(el.Attribute("maxhp").Value));
                    cra.Add(bool.Parse(el.Attribute("active").Value));

                    List<string> textures = new List<string>();
                    List<string> uses = new List<string>();
                    foreach (XElement ell in el.Elements("Texture"))
                    {
                        textures.Add(ell.Attribute("tname").Value);
                        uses.Add(ell.Attribute("name").Value);
                    }
                    crTexNames.Add(textures.ToArray());
                    crTexUses.Add(uses.ToArray());
                }
            }

            List<string> texNameList = new List<string>();
            List<Facing> facingList = new List<Facing>();
            List<bool> isAnimList = new List<bool>();
            List<int> frameCountList = new List<int>();
            List<float> frameTimeList = new List<float>();
            foreach (XElement el in sheetEl.Elements("Texture"))
            {
                texNameList.Add(el.Attribute("name").Value);

                ps.Add(new Rectangle(int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value), int.Parse(el.Attribute("w").Value), int.Parse(el.Attribute("h").Value)));

                Facing f = Facing.N;
                if (el.Attribute("fa").Value == "L") { f = Facing.L; }
                if (el.Attribute("fa").Value == "R") { f = Facing.R; }

                facingList.Add(f);

                int tempFrameCount = 1;
                float tempFrameTime = 0;
                if (bool.Parse(el.Attribute("anim").Value) == true)
                {
                    isAnimList.Add(true);
                    tempFrameCount = int.Parse(el.Attribute("f").Value);
                    tempFrameTime = float.Parse(el.Attribute("t").Value);
                } else { isAnimList.Add(false); }
                frameCountList.Add(tempFrameCount);
                frameTimeList.Add(tempFrameTime);
            }

            mtexTNames = texNameList.ToArray();
            tileSheet = new TileSheet(tn.ToArray(), tN.ToArray());
            crSheet = new CreatureSheet(crhp.ToArray(), crmhp.ToArray(), crn.ToArray(), cra.ToArray(), crTexNames.ToArray(), crTexUses.ToArray());
            sourceRects = ps.ToArray();
            facing = facingList.ToArray();
            isAnimated = isAnimList.ToArray();
            frameCount = frameCountList.ToArray();
            frameTime = frameTimeList.ToArray();
        }
        public MagicTexture getTex(string n_)
        {
            for(int i = 0; i < mtexTNames.Length; i++)
            {
                if(n_ == mtexTNames[i]) {
                    if (isAnimated[i])
                        return new MagicTexture(sourceTex, sourceRects[i], facing[i], frameCount[i], frameTime[i], 0, name, mtexTNames[i]);
                    else
                        return new MagicTexture(sourceTex, sourceRects[i], facing[i],name, mtexTNames[i]);
                }
            }
            return null;
        }
        public Tile getTile(string name_)
        {
            for(int x = 0; x < tileSheet.tileNames.Length; x++)
            {
                if(tileSheet.tileNames[x] == name_)
                {
                    Tile t = new Tile(getTex(tileSheet.tileTNames[x]), Vector2.Zero, Point.Zero, tileSheet.tileNames[x]);
                    return t;
                }
            }
            return null;
        }
        public Creature getCreature(string name_)
        {
            for(int x = 0; x < crSheet.names.Length; x++)
            {
                if(crSheet.names[x] == name_)
                {
                    List<Skill> skills = new List<Skill>();
                    List<MagicTexture> t = new List<MagicTexture>();
                    for(int y =0; y < crSheet.tnames[x].Length; y++)
                    {
                        t.Add(getTex(crSheet.tnames[x][y]));
                        t[y].GetName(crSheet.tuses[x][y]);
                    }
                    return new Creature(t.ToArray(), Vector2.Zero, Point.Zero, crSheet.hp[x], crSheet.maxhp[x], name_, skills, crSheet.isActive[x], 5);
                }
            }
            return null;
        }
    }
}
