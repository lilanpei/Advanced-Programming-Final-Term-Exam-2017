using Annotation_system.Annotations;
using Annotation_system.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.Parser
{
    class TestParser
    {
        public List<Table> Test()
        {
            string text = System.IO.File.ReadAllText(@"test.txt");
            var tokens = new AnnotationTokenizer(text).Tokenize();
            var parser = new AnnotationParse(tokens);
            return parser.Parse();
        }
    }
}
