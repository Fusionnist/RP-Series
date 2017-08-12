using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Roleplay
{
    public class Font
    {
        Texture2D src;
        char[][] characters;
        Rectangle[] frames;

        public Font(Texture2D src_, char[][] characters_, Rectangle[] frames_)
        {
            src = src_;
            characters = characters_;
            frames = frames_;
        }

        public Rectangle getFrame(char characterRequest_)
        {
            for(int x = 0; x < characters.Length; x++)
            {
                foreach(char c in characters[x])
                {
                    if (c == characterRequest_) { return frames[x]; }
                }
            }
            return Rectangle.Empty;
        }

        public void DrawCharacter(char characterResquest_, Vector2 pos_, SpriteBatch sb_)
        {
            sb_.Draw(src, sourceRectangle: getFrame(characterResquest_), position: pos_);
        }
    }
}
