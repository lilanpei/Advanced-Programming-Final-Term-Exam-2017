﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotation_system.Annotations
{
    class One2Many:Relation
    {
        public string MappedBy { get; set; }
    }
}
