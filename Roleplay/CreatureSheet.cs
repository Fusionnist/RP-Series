using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay
{
    public class CreatureSheet
    {
        public int[] maxhp;
        public int[] hp;
        public bool[] isActive;

        public string[] names;
        public string[][] tnames;
        public string[][] tuses;

        public CreatureSheet(int[] hp_, int[] maxHp_, string[] names_, bool[] isActive_, string[][] tnames_, string[][] tuses_)
        {
            tuses = tuses_;
            tnames = tnames_;
            maxhp = maxHp_;
            hp = hp_;
            names = names_;
            isActive = isActive_;
        }
    }
}
