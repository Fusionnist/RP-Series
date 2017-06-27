using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
