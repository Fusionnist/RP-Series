using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    class Button : Sprite
    {
        string action;
        public Button(MagicTexture tex_, Vector2 pos_, string action_):base(tex_, pos_)
        {
            action = action_;
        }
    }
}
