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

        public string[] mtexNames;
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
            if(sheetEl.Element("Tilesheet") != null)
            {
                foreach (XElement el in sheetEl.Element("Tilesheet").Elements("Tile"))
                {
                    tn.Add(el.Attribute("name").Value);
                }
            }
            

            List<string> crn = new List<string>();
            List<int> crhp = new List<int>();
            List<int> crmhp = new List<int>();
            List<bool> cra = new List<bool>();

            if (sheetEl.Element("Creatures") != null)
            {
                foreach (XElement el in sheetEl.Element("Creatures").Elements("Creature"))
                {
                    crn.Add(el.Attribute("name").Value);
                    crhp.Add(int.Parse(el.Attribute("hp").Value));
                    crmhp.Add(int.Parse(el.Attribute("maxhp").Value));
                    cra.Add(bool.Parse(el.Attribute("active").Value));
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

            mtexNames = texNameList.ToArray();
            tileSheet = new TileSheet(tn.ToArray());
            crSheet = new CreatureSheet(crhp.ToArray(), crmhp.ToArray(), crn.ToArray(), cra.ToArray());
            sourceRects = ps.ToArray();
            facing = facingList.ToArray();
            isAnimated = isAnimList.ToArray();
            frameCount = frameCountList.ToArray();
            frameTime = frameTimeList.ToArray();
        }
        public MagicTexture getTex(string n_)
        {
            for(int i = 0; i < mtexNames.Length; i++)
            {
                if(n_ == mtexNames[i]) {
                    if (isAnimated[i])
                        return new MagicTexture(sourceTex, sourceRects[i], facing[i], frameCount[i], frameTime[i], 0,name);
                    else
                        return new MagicTexture(sourceTex, sourceRects[i], facing[i],name);
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
                    return new Creature(getTex(name_), Vector2.Zero, Point.Zero, crSheet.hp[x], crSheet.maxhp[x], name_, skills, crSheet.isActive[x]);
                }
            }
            return null;
        }
    }
}
