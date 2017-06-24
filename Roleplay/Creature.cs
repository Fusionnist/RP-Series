using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Roleplay
{
    public class Creature : Entity
    {
        public MagicTexture[] textures;
        public List<Skill> skills;
        public string name;
        public int maxAP, AP;
        public int hp, maxHp;
        public bool isPlayer, isActive;
        public Creature(MagicTexture[] tex_, Vector2 pos_, Point tsPos_, int hp_, int maxHp_, string name_, List<Skill> skills_, bool isActive_, int maxAP_, int ID_) : base(tex_[0], pos_, tsPos_, ID_)
        {
            skills = skills_;
            name = name_;
            hp = hp_;
            maxHp = maxHp_;
            tsPos = tsPos_;
            isActive = isActive_;
            maxAP = maxAP_;
            ResetAP();

            textures = tex_;
        }
        public void SelectTexture(string name)
        {
            foreach(MagicTexture m in textures)
            {
                if(m.name == name)
                    tex = m;
            }
        }
        public override void Update(GameTime gt_)
        {
            if (hp < 0)
            { SelectTexture("dead"); }
            ToggleActivity();
            base.Update(gt_);
        }
        public void ResetAP()
        {
            AP = maxAP;
        }
        public void LearnSkill(Skill skill_)
        {
            skills.Add(skill_);
        }
        public void ToggleActivity()
        {
            if(hp <= 0) { isActive = false; }
            if(hp > 0) { isActive = true; }
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
