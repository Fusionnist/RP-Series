using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay
{
    public class Turn
    {
        public SingleAction[] options;
        public Turn(SingleAction[] options_)
        {
            options = options_;
        }
    }
}
