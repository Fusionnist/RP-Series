using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public enum ActionType { Skill, Move, Item, TakeDamage, Idle }
    public class SingleAction
    {
        public ActionType type;
        public int key, actorKey, damage;
        public Point location;

        public SingleAction(ActionType type_, int key_, Point location_, int actorKey_)
        {
            actorKey = actorKey_;
            type = type_;
            key = key_;
            location = location_;
        }
        public SingleAction(int damage_, int actorKey_)
        {
            actorKey = actorKey_;
            damage = damage_;
            type = ActionType.TakeDamage;
        }
    }
}
