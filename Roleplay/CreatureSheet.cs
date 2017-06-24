using System.Collections.Generic;

namespace Roleplay
{
    public class CreatureSheet
    {
        public List<int> maxap;
        public List<int> maxhp;
        public List<int> hp;
        public List<bool> isActive;

        public List<string> names;
        public List<int> IDs;
        public List<List<int>> tIDs;
        public List<List<string>> tuses;

        public CreatureSheet(List<int> hp_, List<int> maxHp_, List<string> names_, List<bool> isActive_, List<List<int>> tIDs_, List<List<string>> tuses_, List<int> IDs_, List<int> maxap_)
        {
            maxap = maxap_;
            IDs = IDs_;
            tuses = tuses_;
            tIDs = tIDs_;
            maxhp = maxHp_;
            hp = hp_;
            names = names_;
            isActive = isActive_;
        }
    }
}
