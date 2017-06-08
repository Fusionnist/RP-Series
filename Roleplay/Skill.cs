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
        public SkillTrajectory traj;
        public int range;
        public int damage;
        public string name;

        public Skill(SkillTrajectory traj_, int range_, int damage_, string name_)
        {
            traj = traj_;
            name = name_;
            damage = damage_;
            range = range_;
        }
    }
}
