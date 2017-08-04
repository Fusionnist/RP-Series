using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay
{
    public class ItemSheet
    {   
        public List<int> IDs, iconIDs;
        public List<List<ItemType>> types;
        public List<string> names;

        public ItemSheet(List<int> IDs_, List<int> iconIDs_, List<List<ItemType>> types_, List<string> names_)
        {
            IDs = IDs_;
            iconIDs = iconIDs_;
            types = types_;
            names = names_;
        }
    }
}