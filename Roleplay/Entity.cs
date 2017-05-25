using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Entity:Sprite
    {
        public Point tsPos;
        public Entity(MagicTexture tex_, Vector2 pos_, Point tsPos_) : base(tex_, pos_)
        {
            tsPos = tsPos_;
        }
    }
}
