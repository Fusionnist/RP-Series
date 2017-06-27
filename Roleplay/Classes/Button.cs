using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Button : Sprite
    {
        public string action, keyname;
        public int key;

        public Button(MagicTexture tex_, Vector2 pos_, string action_):base(tex_, pos_)
        {
            action = action_;
        }
        public Button(MagicTexture tex_, Vector2 pos_, string action_, string keyname_, int key_) : base(tex_, pos_)
        {
            keyname = keyname_;
            key = key_;
            action = action_;
        }
        public void Draw(SpriteBatch sb_, float zoom_, FontDrawer fd)
        {
            base.Draw(sb_, zoom_);
            fd.DrawText(pos, action, 1000, 1000, sb_, 1f);
        }
    }
}
