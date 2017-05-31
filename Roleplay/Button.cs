using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Button : Sprite
    {
        public string action;
        public Button(MagicTexture tex_, Vector2 pos_, string action_):base(tex_, pos_)
        {
            action = action_;
        }
    }
}
