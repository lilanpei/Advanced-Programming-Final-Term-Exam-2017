using Anotation_system.Test;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anotation_system.Entity
{
    class Query<T> : IQuery<T>
    {
        public string query;

        public Query(string _query)
        {
            query = _query;
        }

        public void execute()
        {
            var QueryArry = query.Split();
            if (QueryArry[0].ToLower() == "update" || QueryArry[0].ToLower() == "delete")
            {
                using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
                {
                    sqlite.Open();
                    Console.WriteLine("{0}", query);
                    using (SQLiteCommand Command = new SQLiteCommand(query, sqlite))
                    {
                        Command.ExecuteNonQuery();
                    }
                    sqlite.Close();
                }
            }
        }

        public List<T> getResultList()
        {
            List<T> ObjList = new List<T>();
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                Console.WriteLine("{0}", query);
                using (SQLiteCommand Command = new SQLiteCommand(query, sqlite))
                {
                    using (SQLiteDataReader r = Command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            T res = (T)Activator.CreateInstance(typeof(T));
                            Type type;
                            ObjList.Add(res);
                            FieldInfo[] fields = typeof(T).GetFields();

                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (!(fields[i].FieldType.Namespace.ToString() == "System"))
                                {

                                    if (fields[i].FieldType.IsGenericType)
                                    {
                                        type = typeof(EntityManager<>).MakeGenericType(fields[i].FieldType.GetGenericArguments()[0]);
                                    }
                                    else
                                    {
                                        type = typeof(EntityManager<>).MakeGenericType(fields[i].FieldType);
                                    }
                                    var obj = Activator.CreateInstance(type);

                                    if (i == r.FieldCount)
                                    {
                                        MethodInfo method = type.GetMethod("findList");
                                        object result1 = method.Invoke(obj, new object[] { new Key("Fk_Id", r[0].ToString()) });
                                        fields[fields.Length - 1].SetValue(res, result1);
                                    }
                                    else
                                    {
                                        MethodInfo method = type.GetMethod("find");
                                        object result = method.Invoke(obj, new object[] { r[i] });
                                        fields[fields.Length - 1].SetValue(res, result);
                                    }
                                }
                                else
                                    fields[i].SetValue(res, r[i]);
                            }
                        }
                        sqlite.Close();
                    }
                    return ObjList;
                }
            }
        }
    }
}
