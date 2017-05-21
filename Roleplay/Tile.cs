using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Roleplay
{
    public class Tile : Sprite
    {
        public string name;
        public Tile(MagicTexture tex_, Vector2 pos_, string name_):base(tex_, pos_)
        {
            name = name_;
        }
    }
}
