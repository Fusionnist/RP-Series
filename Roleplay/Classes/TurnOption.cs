using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public enum OptionType { Skill, Move, Item }
    public class TurnOption
    {
        public OptionType type;
        public int key, actorKey;
        public Point location;

        public TurnOption(OptionType type_, int key_, Point location_, int actorKey_)
        {
            actorKey = actorKey_;
            type = type_;
            key = key_;
            location = location_;
        }
    }
}
