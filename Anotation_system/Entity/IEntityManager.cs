using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.Entity
{
    interface IEntityManager<T>
    {
        void persist(T entity);
        void remove(T entity);
        T find(Object primaryKey);
        Query<T> createQuery(String query);
    }
}
