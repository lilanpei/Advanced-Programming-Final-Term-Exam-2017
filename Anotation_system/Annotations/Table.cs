using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotation_system.Annotations
{
    class Table:Annotation
    {
        public Id id { get; set; }
        public List<Column> columns { get; set; }
        public Relation relation { get; set; }
        public string type { get; set; }
    }
}
