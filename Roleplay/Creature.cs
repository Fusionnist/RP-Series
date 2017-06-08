using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Roleplay
{
    public class Creature : Entity
    {
        public List<Skill> skills;
        public string name;
        public int hp, maxHp;
        public bool isPlayer, isActive;
        public Creature(MagicTexture tex_, Vector2 pos_, Point tsPos_, int hp_, int maxHp_, string name_, List<Skill> skills_, bool isActive_) : base(tex_, pos_, tsPos_)
        {
            skills = skills_;
            name = name_;
            hp = hp_;
            maxHp = maxHp_;
            tsPos = tsPos_;
            isActive = isActive_;
        }
        public void LearnSkill(Skill skill_)
        {
            skills.Add(skill_);
        }
        public void BecomePlayer()
        {
            isPlayer = true;
        }
        public void BecomeAI()
        {
            isPlayer = false;
        }

    }
}
