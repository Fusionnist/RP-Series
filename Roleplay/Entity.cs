using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Entity:Sprite
    {
        public Point tsPos;
        public int ID;
        public Entity(MagicTexture tex_, Vector2 pos_, Point tsPos_, int ID_) : base(tex_, pos_)
        {
            ID = ID_;
            tsPos = tsPos_;
        }
        public override void Draw(SpriteBatch sb_, float zoom_)
        {
            tex.Draw(sb_, pos, zoom_, true);
        }
    }
}
