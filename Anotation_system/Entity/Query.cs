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
                    SQLiteCommand Command = new SQLiteCommand(query, sqlite);
                    Command.ExecuteNonQuery();
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
                SQLiteCommand Command = new SQLiteCommand(query, sqlite);
                SQLiteDataReader r = Command.ExecuteReader();
                while (r.Read())
                {
                    T Obj = (T)Activator.CreateInstance(typeof(T));

                    ObjList.Add(Obj);

                    FieldInfo[] fields = typeof(T).GetFields();
                    fields[0].SetValue(Obj, r.GetInt32(0));
                    if (typeof(T).Name == "Book")
                    {
                        fields[0].SetValue(Obj, r.GetInt32(0));
                        fields[1].SetValue(Obj, r.GetString(1));
                        var en = new EntityManager<Publisher>();
                        fields[2].SetValue(Obj, en.find(r.GetInt32(2)));
                    }
                    else if (typeof(T).Name == "Publisher")
                    {
                        fields[0].SetValue(Obj, r.GetInt32(0));
                        fields[1].SetValue(Obj, r.GetString(1));
                    }
                    else
                        throw new Exception(" T type error");
                }
                sqlite.Close();
            }
            return ObjList;
        }
    }
}
