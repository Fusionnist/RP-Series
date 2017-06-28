using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    enum ItemType { Consumable, Material, Weapon };

    class Item
    {
        public int ID;
        public string name { get; set; }
        MagicTexture icon;

        public Item(MagicTexture icon_, int ID_, string name_)
        {
            icon = icon_;
            name = name_;
            ID = ID_;
        }
        public void DrawIcon(SpriteBatch sb_, int dim_, Vector2 pos_)
        {
            icon.Draw(sb_, pos_, (float)dim_ / icon.frame.Width, false);
        }
    }
}
