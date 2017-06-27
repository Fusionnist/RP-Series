using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Roleplay
{
    class FrameDrawer
    {
        bool hasStyle;
        MagicTexture UL, UR, DL, DR, L, R, U, D, M;
        public FrameDrawer()
        {
           
        }
        public void GetStyle(
            MagicTexture UL_, 
            MagicTexture UR_, 
            MagicTexture L_, 
            MagicTexture R_, 
            MagicTexture DL_, 
            MagicTexture DR_, 
            MagicTexture U_,
            MagicTexture D_, 
            MagicTexture M_)
        {
            UL = UL_;
            UR = UR_;
            L = L_;
            R = R_;
            DL = DL_;
            DR = DR_;
            M = M_;
            U = U_;
            D = D_;

            hasStyle = true;
        }
        public void Draw(Rectangle rec_, SpriteBatch sb_)
        {
            if (hasStyle)
            {
                int l = 0;
                int r = 0;
                int u = 0;
                int d = 0;

                for(int x = 0; x*L.frame.Width < rec_.Width; x++)
                {
                    l = x + 1;
                }
                for (int x = 0; x * R.frame.Width < rec_.Width; x++)
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

                //draw corners
                UL.Draw(sb_, new Vector2(rec_.X, rec_.Y), 1f, false);
                UR.Draw(sb_, new Vector2(rec_.X + U.frame.Width*u + UL.frame.Width, rec_.Y), 1f, false);
                DL.Draw(sb_, new Vector2(rec_.X, rec_.Y+L.frame.Height*l+UL.frame.Height), 1f, false);
                DR.Draw(sb_, new Vector2(rec_.X + U.frame.Width * u +UL.frame.Width, rec_.Y + L.frame.Height * l+UL.frame.Height), 1f, false);

                for (int x = 0; x < u; x++)
                {
                    U.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * x, rec_.Y), 1f, false);
                    D.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * x, rec_.Y + L.frame.Height * l + UL.frame.Height), 1f, false);
                }
                for (int x = 0; x < l; x++)
                {
                    L.Draw(sb_, new Vector2(rec_.X, rec_.Y + L.frame.Height * x + UL.frame.Height), 1f, false);
                    R.Draw(sb_, new Vector2(rec_.X + UL.frame.Width + U.frame.Width * u, rec_.Y + L.frame.Height * x + UL.frame.Height), 1f, false);
                }
                for(int x = 0; x < u; x++)
                {
                    for(int y = 0; y< l; y++)
                    {
                        M.Draw(sb_, new Vector2(rec_.X+UL.frame.Width + M.frame.Width * x, rec_.X+UL.frame.Height + M.frame.Height * y), 1f, false);
                    }
                }
            }
        }
    }
}
