using Annotation_system.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotation_system.Parser
{
    class KeywordToken:StringToken
    {
        public KeywordEnum keyword { get; set; }
    }
}
