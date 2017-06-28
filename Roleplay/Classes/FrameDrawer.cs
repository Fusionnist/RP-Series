using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace Roleplay
{
    class FrameDrawer
    {
        bool hasStyle;
        List<MagicTexture[]> styles;
        public FrameDrawer()
        {
            styles = new List<MagicTexture[]>();
        }
        public void GetStyle(MagicTexture[] style_)
        {
            styles.Add(style_);
            hasStyle = true;
        }
        public MagicTexture GetPiece(string name_)
        {
            foreach(MagicTexture m in styles[0])
            {
                if(m.name == name_) { return m; }
            }
            return null;
        }
        public void Draw(Rectangle rec_, SpriteBatch sb_)
        {
            if (hasStyle)
            {
                int l = 0;
                int r = 0;
                int u = 0;
                int d = 0;
                int mx = 0;
                int my = 0;

                MagicTexture L = GetPiece("L");
                MagicTexture R = GetPiece("R");
                MagicTexture U = GetPiece("U");
                MagicTexture D = GetPiece("D");
                MagicTexture UR = GetPiece("UR");
                MagicTexture UL = GetPiece("UL");
                MagicTexture DL = GetPiece("DL");
                MagicTexture DR = GetPiece("DR");
                MagicTexture M = GetPiece("M");

                MagicTexture UC = GetPiece("UC");
                
                for (int x = 0; x * L.frame.Height < rec_.Height; x++)
                {
                    l = x + 1;
                }
                for (int x = 0; x * R.frame.Height < rec_.Height; x++)
                {
                    r = x + 1;
                }
                for (int x = 0; x * U.frame.Width < rec_.Width; x++)
                {
                    u = x + 1;
                }
                for (int x = 0; x * D.frame.Width < rec_.Width; x++)
                {
                    d = x + 1;
                }
                for (int x = 0; x * D.frame.Width < rec_.Width; x++)
                {
                    mx = x + 1;
                }
                for (int x = 0; x * M.frame.Height < rec_.Height; x++)
                {
                    my = x + 1;
                }

                //draw corners
                UL.Draw(sb_, new Vector2(rec_.X, rec_.Y), 1f, false);
                UR.Draw(sb_, new Vector2(rec_.X + U.frame.Width*u + UL.frame.Width, rec_.Y), 1f, false);
                DL.Draw(sb_, new Vector2(rec_.X, rec_.Y+L.frame.Height*l+UL.frame.Height), 1f, false);
                DR.Draw(sb_, new Vector2(rec_.X + U.frame.Width * u +UL.frame.Width, rec_.Y + L.frame.Height * l+UL.frame.Height), 1f, false);

                for (int x = 0; x < u; x++) //draw up parts
                {
                    U.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * x, rec_.Y), 1f, false);
                }
                if (UC!=null)
                {
                    UC.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + (U.frame.Width * u) / 2 - UC.frame.Width / 2, rec_.Y), 1f, false);
                }
                for (int x = 0; x < l; x++) //draw left parts 
                {
                    L.Draw(sb_, new Vector2(rec_.X, rec_.Y + L.frame.Height * x + UL.frame.Height), 1f, false);
                }
                for (int x = 0; x < d; x++) //draw down parts
                {
                    D.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * x, rec_.Y + L.frame.Height * l + UL.frame.Height), 1f, false);
                }
                for (int x = 0; x < r; x++) //draw right parts
                {
                    R.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * u, rec_.Y + L.frame.Height * x + UL.frame.Height), 1f, false);
                }
                for (int x = 0; x < mx; x++) //draw middle
                {
                    for(int y = 0; y< my; y++)
                    {
                        M.Draw(sb_, new Vector2(rec_.X+UL.frame.Width + M.frame.Width * x, rec_.X+UL.frame.Height + M.frame.Height * y), 1f, false);
                    }
                }
            }
        }
        public Rectangle getInteriorDim(Rectangle rec_)
        {
            return new Rectangle(
                rec_.X + GetPiece("UL").frame.Width,
                rec_.Y + GetPiece("UL").frame.Height, 
                rec_.Width - GetPiece("L").frame.Width - GetPiece("R").frame.Width,
                rec_.Height - GetPiece("U").frame.Height - GetPiece("D").frame.Height);
        }
    }
}
