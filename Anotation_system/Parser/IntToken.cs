using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotation_system.Parser
{
    class IntToken:Token
    {
        private readonly int _value;

        public IntToken(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }
    }
}
