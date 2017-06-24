using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public enum Facing { L, R, N }

    public class MagicTexture
    {
        Facing face;
        Texture2D source;
        Rectangle sourceRect;
        public Rectangle frame;
        int frameCount, frameCounter;
        float frameTime, frameTimer;
        public int ID;
        public string name;

        public void GetName(string name_)
        {
            name = name_;
        }
        public MagicTexture(Texture2D source_, Rectangle sourceRect_, Facing face_, int ID_)
        {
            ID = ID_;
            face = face_;
            source = source_;
            sourceRect = sourceRect_;
            frame = sourceRect_;
        }
        public MagicTexture(Texture2D source_, Rectangle sourceRect_, Facing face_, int frameCount_, float frameTime_, float delay_, int ID_)
        {
            ID = ID_;
            face = face_;
            source = source_;
            sourceRect = sourceRect_;
            frameCount = frameCount_;   
            frameTime = frameTime_;
            frameTimer += delay_;
            frame = sourceRect_;
        }
        public void Update(GameTime gt_)
        {
            frameTimer -= (float)gt_.ElapsedGameTime.TotalSeconds;

            if(frameTimer < 0) { frameTimer = frameTime; frameCounter++; }
            if(frameCounter >= frameCount) { frameCounter = 0; }
        }
        public void Draw(SpriteBatch sb_, Vector2 pos_, float zoom_, bool shouldResize_)
        {
            //calculate the correct source rect
            float resizer = 1f;
            if (shouldResize_) { resizer = 200f / frame.Width; }
            frame = new Rectangle(sourceRect.X + frameCounter * sourceRect.Width, sourceRect.Y, sourceRect.Width, sourceRect.Height);
            sb_.Draw(source, sourceRectangle: frame, position: pos_*zoom_,scale:new Vector2(zoom_) * (resizer));            
        }
        public Vector2 getMiddle()
        {
            return new Vector2(frame.Width / 2, frame.Height / 2);
        }
    }
}
