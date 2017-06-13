using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay
{
    public enum SkillTrajectory { AOE, Linear }

    public class Skill
    {
        public bool damages;
        public SkillTrajectory traj;
        public int range;
        public int damage;
        public string name;
        public int AP;

        public Skill(SkillTrajectory traj_, int range_, int damage_, string name_, int AP_)
        {
            damages = true;

            traj = traj_;
            name = name_;
            damage = damage_;
            range = range_;
            AP = AP_;
        }
    }
}
