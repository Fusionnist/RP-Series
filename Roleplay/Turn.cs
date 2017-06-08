using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay
{
    public class Turn
    {
        public TurnOption[] options;
        public Turn(TurnOption[] options_)
        {
            options = options_;
        }
    }
}
