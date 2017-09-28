using Annotation_system.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.CodeGenerator
{
    class CodeGenerator
    {
        public IEnumerator<Table> _tables;
        public IEnumerable<Table> tables;
        public string output;
        public virtual string path { get; set; }
        public CodeGenerator() { }
        public string GenerateCode()
        {
            while (_tables.MoveNext())
            {
                output = string.Format("{0} {1} \n", output, Generate());
            }
            File.WriteAllText(path, output);
            return output;
        }
        public string Generate()
        {
            return string.Format("{0} {1}", GenerateTitle(), GenerateBody());
        }
        public virtual string GenerateBody() { return ""; }
        public virtual string GenerateTitle() { return ""; }
        public virtual string GenerateId() { return ""; }
        public virtual string GenerateColumn() { return ""; }
        public virtual string GenerateRelation() { return ""; }
    }
}
