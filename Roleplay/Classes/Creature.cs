using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Roleplay
{
    public class Creature : Entity
    {
        public MagicTexture[] textures;
        public ActionType currentAction, prevAction;
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
        public bool ActionEnded()
        {
            return tex.animCompleted;
        }
        public void SelectTexture(string name)
        {
            foreach(MagicTexture m in textures)
            {
                if(m.name == name)
                    tex = m;
                tex.Reset();
            }
        }
        public override void Update(GameTime gt_)
        {
            if (hp < 0)
            { SelectTexture("dead"); } //temp?
            if (currentAction != ActionType.Idle && ActionEnded()) { SelectTexture("idle"); } //Revert to idle appearance
            ToggleActivity();
            base.Update(gt_);
        }
        void ActionLogic() //select texture when action changes
        {
            if (currentAction == ActionType.TakeDamage) { SelectTexture("hit"); }
        }
        void ChangeAction(ActionType newType_) //change to new action and deal with logic
        {
            prevAction = currentAction;
            currentAction = newType_;
            ActionLogic();
        }
        public void ResetAP() //reset AP + logic(?)
        {
            AP = maxAP;
        }
        public void LearnSkill(Skill skill_) //learn skill logic
        {
            skills.Add(skill_);
        }
        public void ToggleActivity() //activity = if you can act
        {
            if(hp <= 0) { isActive = false; }
            if(hp > 0) { isActive = true; }
        }
        public void BecomePlayer() //this should become fun
        {
            isPlayer = true;
        }
        public void BecomeAI() //revert to AI
        {
            isPlayer = false;
        }
    }
}
