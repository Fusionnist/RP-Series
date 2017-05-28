using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Sprite
    {
        public MagicTexture tex;
        public Vector2 pos;
        public Sprite(MagicTexture tex_, Vector2 pos_)
        {
            tex = tex_;
            pos = pos_;
        }
        public virtual void Draw(SpriteBatch sb_,float zoom_)
        {
            tex.Draw(sb_, pos,zoom_, false);
        }
        public void Update(GameTime gt_)
        {
            tex.Update(gt_);
        }
        public Rectangle getFrame()
        {
            Rectangle rec = tex.frame;
            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;
            return rec;
        }
        public Vector2 getMiddle()
        {
            return(tex.getMiddle() + pos);
        }
    }
}
