using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class FontDrawer //draws custom fonts
    {
        Font font;
        public FontDrawer(Font f_)
        {
            font = f_;
        }
        public void DrawText(Vector2 pos_, string text_, int frameHeight_, int frameWidth_, SpriteBatch sb_, float scale_)
        {
            int offset = 0;
            for(int x = 0; x < text_.Length; x++)
            {
                offset += font.DrawCharacter(text_[x], new Vector2(pos_.X + offset, pos_.Y), sb_, scale_);
            }
        }
    }
}
