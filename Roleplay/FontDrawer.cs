using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class FontDrawer
    {
        Texture2D src;
        int charWidth, charHeight;
        string letters;
        public FontDrawer(Texture2D src_, int charWidth_, int charHeight_, string letters_)
        {
            src = src_;
            charWidth = charWidth_;
            charHeight = charHeight_;
            letters = letters_;
        }
        public void DrawLetter(char c_, Vector2 pos_, SpriteBatch sb_, float scale_)
        {
            for (int y = 0; y < src.Height / charHeight; y++)
            {
                for (int x = 0; x < src.Width / charWidth; x++)
                {
                    if ((src.Width / charWidth) * y + x >= letters.Length) { break; }
                    if(letters[(src.Width / charWidth) * y + x] == c_)
                    {
                        Rectangle srcr = new Rectangle(x * charWidth, y * charHeight, charWidth, charHeight);
                        sb_.Draw(texture: src, sourceRectangle: srcr, position: pos_ * scale_, scale: new Vector2(scale_));
                        break;
                    }
                }
            }
        }
        public void DrawText(Vector2 pos_, string text_, int frameHeight_, int frameWidth_, SpriteBatch sb_, float scale_)
        {
            for(int x = 0; x < text_.Length; x++)
            {
                DrawLetter(text_[x], new Vector2(pos_.X + charWidth * x, pos_.Y), sb_, scale_);
            }
        }
    }
}
