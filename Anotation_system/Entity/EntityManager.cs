using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Anotation_system.Entity
{
    class EntityManager<T> : IEntityManager<T>
    {
        public Query<T> createQuery(string query)
        {
            return new Query<T>(query);
        }

        public T find(object primaryKey)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string selectQuery = "";
                tableName = typeof(T).Name.ToLower();
                T res = (T)Activator.CreateInstance(typeof(T));
                selectQuery = string.Format("Select * From {0} \n Where id = {1};", tableName, primaryKey.ToString());
                Console.WriteLine("{0}", selectQuery);

                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlite))
                {
                    using (SQLiteDataReader r = selectCommand.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            FieldInfo[] fields = typeof(T).GetFields();
                            Type type;

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

                                        object result1 = method.Invoke(obj, new object[] { (new Key("Fk_Id", r[0].ToString())), res });
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
                    }
                }
                sqlite.Close();
                return (T)res;
            }
        }

        public List<T> findList(Key keyObject, object v)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string selectQuery = "";

                tableName = typeof(T).Name.ToLower();
                selectQuery = string.Format("Select * From {0} \n Where {1} = {2};", tableName, keyObject.name, keyObject.value);
                Console.WriteLine("{0}", selectQuery);
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlite))
                {
                    using (SQLiteDataReader r = selectCommand.ExecuteReader())
                    {
                        List<T> ListObj = new List<T>();
                        while (r.Read())
                        {
                            T res = (T)Activator.CreateInstance(typeof(T));
                            ListObj.Add(res);
                            FieldInfo[] fields = typeof(T).GetFields();

                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (!(fields[i].FieldType.Namespace.ToString() == "System"))
                                {
                                        fields[fields.Length - 1].SetValue(res, v);
                                }
                                else
                                    fields[i].SetValue(res, r[i]);
                            }
                        }
                        sqlite.Close();
                        return (List<T>)ListObj;
                    }
                }
            }
        }

        public void persist(T entity)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string insertQuery = "";
                Type type;
                FieldInfo[] fields = typeof(T).GetFields();
                string values = fields[0].GetValue(entity).ToString();

                tableName = typeof(T).Name.ToLower();
                for (int i = 1; i < fields.Length; i++)
                {
                    if (!(fields[i].FieldType.Namespace.ToString() == "System"))
                    {
                        if (!(fields[i].FieldType.IsGenericType))
                        {
                            type = typeof(EntityManager<>).MakeGenericType(fields[i].FieldType);
                            object fk = fields[fields.Length - 1].GetValue(entity);
                            FieldInfo[] fk_fields = fk.GetType().GetFields();
                            values = string.Format("{0},'{1}'", values, fk_fields[0].GetValue(fk).ToString());
                        }
                    }
                    else
                        values = string.Format("{0},'{1}'", values, fields[1].GetValue(entity).ToString());
                }
                insertQuery = string.Format("Insert Into {0} \n Values({1});", tableName, values);
                Console.WriteLine("{0}", insertQuery);
                using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, sqlite))
                {
                    insertCommand.ExecuteNonQuery();
                }
                sqlite.Close();
            }
        }

        public void remove(T entity)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string deleteQuery = "";
                FieldInfo[] fields = typeof(T).GetFields();
                string values = fields[0].GetValue(entity).ToString();

                tableName = typeof(T).Name.ToLower();
                deleteQuery = string.Format("Delete From {0} \n Where id = {1};", tableName, values);

                Console.WriteLine("{0}", deleteQuery);
                using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, sqlite))
                {
                    deleteCommand.ExecuteNonQuery();
                }
                sqlite.Close();
            }
        }
    }
}
