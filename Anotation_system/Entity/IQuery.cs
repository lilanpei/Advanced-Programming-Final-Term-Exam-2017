using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.Entity
{
    interface IQuery<T>
    {
        List<T> getResultList();
        void execute();
    }
}
