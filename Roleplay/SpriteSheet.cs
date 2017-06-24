using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Roleplay
{
    public class SpriteSheet
    {
        public TileSheet tileSheet;
        public CreatureSheet crSheet;
        public Texture2D[] sourceTex;

        //tex
        public int[] srcIDs;
        public int[] mTexIDs;
        public Rectangle[] sourceRects;
        public Facing[] facing;
        //anim
        public bool[] isAnimated;
        public float[] frameTime;
        public int[] frameCount;

        public SpriteSheet(XDocument campaignDoc, ContentManager c_)
        {
            //load texture data
            XDocument tDoc = XDocument.Load(campaignDoc.Element("Campaign").Element("Textures").Attribute("path").Value);

            List<Texture2D> sourceTexL = new List<Texture2D>();
            //tex data
            {
                List<int> srcIDsL = new List<int>();
                List<int> mTexIDsL = new List<int>();
                List<Rectangle> sourceRectsL = new List<Rectangle>();
                List<Facing> facingL = new List<Facing>();
                List<bool> isAnimatedL = new List<bool>();
                List<float> frameTimeL = new List<float>();
                List<int> frameCountL = new List<int>();

                foreach (XElement tEl in tDoc.Element("Root").Elements("Textures"))
                {
                    int id = int.Parse(tEl.Attribute("id").Value);
                    sourceTexL.Add(c_.Load<Texture2D>(tEl.Attribute("src").Value));

                    foreach (XElement ttEl in tEl.Elements("Texture"))
                    {
                        srcIDsL.Add(id);
                        mTexIDsL.Add(int.Parse(ttEl.Attribute("id").Value));
                        //rect
                        sourceRectsL.Add(new Rectangle(
                            int.Parse(ttEl.Attribute("x").Value),
                            int.Parse(ttEl.Attribute("y").Value),
                            int.Parse(ttEl.Attribute("w").Value),
                            int.Parse(ttEl.Attribute("h").Value)));
                        //facing
                        if (ttEl.Attribute("id").Value == "R")
                            facingL.Add(Facing.R);
                        else if (ttEl.Attribute("id").Value == "L")
                            facingL.Add(Facing.L);
                        else
                            facingL.Add(Facing.N);

                        if (bool.Parse(ttEl.Attribute("anim").Value))
                        {
                            isAnimatedL.Add(true);
                            frameTimeL.Add(int.Parse(ttEl.Attribute("ft").Value));
                            frameCountL.Add(int.Parse(ttEl.Attribute("fc").Value));
                        }
                        else
                        {
                            isAnimatedL.Add(false);
                            frameTimeL.Add(0);
                            frameCountL.Add(1);
                        }
                    }
                }//gets and adds all tex values

                sourceTex = sourceTexL.ToArray();
                srcIDs = srcIDsL.ToArray();
                mTexIDs = mTexIDsL.ToArray();
                sourceRects = sourceRectsL.ToArray();
                facing = facingL.ToArray();
                isAnimated = isAnimatedL.ToArray();
                frameTime = frameTimeL.ToArray();
                frameCount = frameCountL.ToArray();
            }

            XDocument dDoc = XDocument.Load(campaignDoc.Element("Campaign").Element("Data").Attribute("path").Value);

            //creature data
            {
                List<int> maxhp = new List<int>();
                List<int> hp = new List<int>();
                List<bool> isActive = new List<bool>();

                List<string> names = new List<string>();
                List<int> IDs = new List<int>();
                List<int[]> tIDs = new List<int[]>();
                List<string[]> tuses = new List<string[]>();

                foreach (XElement El in dDoc.Element("Data").Element("Creatures").Elements("Creature"))
                {
                    maxhp.Add(int.Parse(El.Attribute("maxhp").Value));
                    hp.Add(int.Parse(El.Attribute("hp").Value));
                    IDs.Add(int.Parse(El.Attribute("id").Value));
                    isActive.Add(bool.Parse(El.Attribute("active").Value));
                    names.Add(El.Attribute("name").Value);

                    List<int> tIDsA = new List<int>();
                    List<string> tusesA = new List<string>();

                    foreach (XElement tEl in El.Elements("Texture"))
                    {
                        tIDsA.Add(int.Parse(tEl.Attribute("id").Value));
                        tusesA.Add(tEl.Attribute("name").Value);
                    }

                    tIDs.Add(tIDsA.ToArray());
                    tuses.Add(tusesA.ToArray());
                }
                crSheet = new CreatureSheet(
                    hp.ToArray(),
                    maxhp.ToArray(),
                    names.ToArray(),
                    isActive.ToArray(),
                    tIDs.ToArray(),
                    tuses.ToArray(),
                    IDs.ToArray());
            }

            //tile data
            {
                List<int> tileTIDs = new List<int>();
                List<int> tileIDs = new List<int>();
                List<string> tileNames = new List<string>();

                foreach (XElement El in dDoc.Element("Data").Element("Tiles").Elements("Tile"))
                {
                    tileTIDs.Add(int.Parse(El.Element("Texture").Attribute("id").Value));
                    tileIDs.Add(int.Parse(El.Attribute("id").Value));
                    tileNames.Add(El.Attribute("name").Value);
                }
                tileSheet = new TileSheet(tileTIDs.ToArray(), tileNames.ToArray(), tileIDs.ToArray());
            }

        }
        public Creature getCreature(int ID_)
        {
            for(int x = 0; x < crSheet.IDs.Length; x++)
            {
                if (crSheet.IDs[x] == ID_)
                {
                    List<MagicTexture> ts = new List<MagicTexture>();
                    for (int y = 0; y < crSheet.tIDs[x].Length; y++)
                    {
                        ts.Add(getTex(crSheet.tIDs[x][y]));
                        ts[y].GetName(crSheet.tuses[x][y]);
                    }
                    return new Creature(
                        ts.ToArray(),
                        Vector2.Zero,
                        Point.Zero,
                        crSheet.hp[x],
                        crSheet.maxhp[x],
                        crSheet.names[x],
                        new List<Skill>(),
                        crSheet.isActive[x],
                        5,
                        ID_
                        );
                }
            }
            return null;
        }
        public Tile getTile(int ID_)
        {
            for(int x = 0; x < tileSheet.tileIDs.Length; x++)
            {
                if(tileSheet.tileIDs[x] == ID_)
                {
                    return new Tile(
                        getTex(tileSheet.tileTIDs[x]),
                        Vector2.Zero,
                        Point.Zero,
                        tileSheet.tileNames[x],
                        ID_
                        );
                }
            }
            return null;
        }

        public MagicTexture getTex(int ID)
        {
            for(int i = 0; i < mTexIDs.Length; i++)
            {
                if(ID == mTexIDs[i]) {
                    if (isAnimated[i])
                        return new MagicTexture(sourceTex[srcIDs[i]], sourceRects[i], facing[i], frameCount[i], frameTime[i], 0, mTexIDs[i]);
                    else
                        return new MagicTexture(sourceTex[srcIDs[i]], sourceRects[i], facing[i], mTexIDs[i]);
                }
            }
            return null;
        }

    }
}
