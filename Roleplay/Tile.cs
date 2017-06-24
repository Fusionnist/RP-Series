﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Roleplay
{
    public class Tile : Entity
    {
        public string name;
        public Tile(MagicTexture tex_, Vector2 pos_, Point tsPos_, string name_, int ID_):base(tex_, pos_, tsPos_, ID_)
        {
            name = name_;
        }
    }
}
