using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    public enum OptionType { Skill, Move, Item}
    public class TurnOption
    {
        public OptionType type;
        public int key;
        public Point location;

        public TurnOption(OptionType type_, int key_, Point location_)
        {
            type = type_;
            key = key_;
            location = location_;
        }
    }
}
