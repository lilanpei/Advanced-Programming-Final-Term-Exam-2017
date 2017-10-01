using Annotation_system.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.CodeGenerator
{
    class SQLCodeGenerator : CodeGenerator
    {
        public override string path { get; set; }
        public SQLCodeGenerator(IEnumerable<Table> tb)
        {
            _tables = tb.GetEnumerator();
            tables = new List<Table>(tb);
            path = @"SQLCode.txt";
        }

        public override string GenerateBody()
        {
            return string.Format("( \n {0} \n {1} {2} );", GenerateId(), GenerateColumn(), GenerateRelation());
        }

        public override string GenerateRelation()
        {
            if (_tables.Current.relation is One2Many)
            {
                return string.Format("  PRIMARY KEY({0}) \n", _tables.Current.id.Name);
            }
            else if (_tables.Current.relation is Many2One)
            {
                var ele = Array.FindAll(tables.ToArray(), x => x.type == _tables.Current.relation.Target).ToList();
                var r = ele.GetEnumerator();
                string pr = string.Format("  PRIMARY KEY({0}) \n", _tables.Current.id.Name);
                string rel = "";
                while (r.MoveNext())
                    rel = string.Format(" {0} Fk_Id INT, \n   FOREIGN KEY(Fk_Id) REFERENCES {1}({2}), \n", rel, r.Current.Name, r.Current.id.Name);
                return string.Format("{0} {1}", rel, pr);
            }
            else
                throw new Exception("Invalid relation type");
        }

        public override string GenerateColumn()
        {
            int i = _tables.Current.columns.Count();
            string cols = "";
            while (i > 0)
            {
                i--;
                cols = string.Format(" {0} {1} VARCHAR({2}),\n", cols, _tables.Current.columns[i].Name, _tables.Current.columns[i].Length);
            }
            return cols;
        }

        public override string GenerateId()
        {
            return string.Format("  {0} INT,", _tables.Current.id.Name);
        }

        public override string GenerateTitle()
        {
            return string.Format("CREATE TABLE {0}", _tables.Current.Name);
        }
    }
}
