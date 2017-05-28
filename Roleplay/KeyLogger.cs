using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roleplay
{
    class KeyLogger
    {
        Keys k;
        bool p,pb;
        public string name;
        public KeyLogger(Keys k_, string name_)
        {
            k = k_;
            name = name_;
        }
        public void Update(KeyboardState kbs_)
        {
            if (kbs_.IsKeyUp(k)) { pb = false; }
            if (kbs_.IsKeyDown(k) && !pb) { pb = true; p = true; }
            else if (kbs_.IsKeyDown(k)) { p = false; }
        }
        public bool isPressed()
        {
            
            return p;
        }
    }
}
