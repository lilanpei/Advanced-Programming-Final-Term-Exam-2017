using Annotation_system.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Anotation_system.CodeGenerator
{
    class CsharpCodeGenerator : CodeGenerator
    {
        public override string path { get; set; }
        public CsharpCodeGenerator(IEnumerable<Table> tb)
        {
            _tables = tb.GetEnumerator();
            tables = tb;
            path = @"CsharpCode.txt";
        }

        public override string GenerateBody()
        {
            return string.Format("{{ \n {0} \n {1} {2} \n }}", GenerateId(), GenerateColumn(), GenerateRelation());
        }

        public override string GenerateRelation()
        {
            if (_tables.Current.relation is Many2One)
            {
                return string.Format("  public {0} {1};", _tables.Current.relation.Target, _tables.Current.relation.Name);
            }
            else if (_tables.Current.relation is One2Many)
            {
                return string.Format("  public List<{0}> {1};", _tables.Current.relation.Target, _tables.Current.relation.Name);
            }
            else
                throw new Exception(" Invaild relation Type");
        }

        public override string GenerateColumn()
        {
            int i = _tables.Current.columns.Count();
            string cols = "";
            while (i > 0)
            {
                i--;
                cols = string.Format(" {0} public string {1};\n", cols, _tables.Current.columns[i].Name);
            }
            return cols;
        }

        public override string GenerateId()
        {
            return string.Format("  public int {0};", _tables.Current.id.Name);
        }

        public override string GenerateTitle()
        {
            return string.Format("class {0}", _tables.Current.type);
        }
    }
}
