using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    enum InventoryType { Storage, Loot };

    class Inventory
    {
        int size;
        List<Item> items;

        public Inventory(int size_)
        {
            items = new List<Item>();
            size = size_;
        }
        public bool AddItem(Item i_)
        {
            if(i_ != null)
                if(items.Count < size)
                {
                    items.Add(i_);
                    return true; //managed to add item to inventory
                }
            return false; //could not add item to inventory
        }
        public void Update(Vector2 mousePos_)
        {

        }
        public void Draw(SpriteBatch sb_, FontDrawer fd_, FrameDrawer frd_, Vector2 pos_)
        {

        }
    }
}
