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
        int frameW, frameH;
        Vector2 pos;
        int iconW;

        public Inventory(int size_)
        {
            items = new List<Item>();
            size = size_;

            frameW = 800;
            frameH = 400;

            iconW = 100;
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
        public void Draw(SpriteBatch sb_, FontDrawer fd_, FrameDrawer frd_)
        {
            frd_.Draw(new Rectangle(0, 0, frameW, frameH), sb_);
            Rectangle interior = frd_.getInteriorDim(new Rectangle((int)pos.X, (int)pos.Y, frameW, frameH));
            int iconPerW = 0;
            for(int x = 0; x*iconW < interior.Width; x++)
            {
                iconPerW = x + 1;
            }
            int y = 0;
            for(int x = 0; x<items.Count; x++)
            {
                if ((x - y * iconPerW) >= iconPerW) { y++; }
                items[x].DrawIcon(sb_, iconW, new Vector2((x - y * iconPerW) * iconW+interior.X, interior.Y+y*iconW));
            }
        }
    }
}
