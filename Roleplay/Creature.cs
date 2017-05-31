using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public class Creature : Entity
    {
        public string name;
        public int hp, maxHp;
        public bool isPlayer;
        public Creature(MagicTexture tex_, Vector2 pos_, Point tsPos_, int hp_, int maxHp_, string name_) : base(tex_, pos_, tsPos_)
        {
            name = name_;
            hp = hp_;
            maxHp = maxHp_;
            tsPos = tsPos_;
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
