using Anotation_system.Test;
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

                if (typeof(T).Name == "Book")
                {
                    tableName = "Book";
                    selectQuery = string.Format("Select * From {0} \n Where id = {1};", tableName, primaryKey.ToString());
                }
                else if (typeof(T).Name == "Publisher")
                {
                    tableName = "Publisher";
                    selectQuery = string.Format("Select * From {0} \n Where id = {1};", tableName, primaryKey.ToString());
                }
                else
                    throw new Exception(" T type error");
                Console.WriteLine("{0}", selectQuery);
                SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlite);
                SQLiteDataReader r = selectCommand.ExecuteReader();

                T res = (T)Activator.CreateInstance(typeof(T));
                while (r.Read())
                {
                    FieldInfo[] fields = typeof(T).GetFields();
                    fields[0].SetValue(res, r.GetInt32(0));
                    if (typeof(T).Name == "Book")
                    {
                        fields[1].SetValue(res, r.GetString(1));
                        var ep = new EntityManager<Publisher>();
                        fields[2].SetValue(res, ep.find(r.GetInt32(2)));
                    }
                    else if (typeof(T).Name == "Publisher")
                    {
                        fields[1].SetValue(res, r.GetString(1));
                        if (r.GetString(2) == "true")
                        {
                            var eb = new EntityManager<Book>();
                            List<Book> lb = new List<Book>();
                            lb = eb.findList(new Key("Fk_Id", r.GetInt32(0).ToString()));
                            fields[2].SetValue(res, lb);
                        }
                    }
                    else
                        throw new Exception(" T type error");
                }
                sqlite.Close();
                return (T)res;
            }
        }

        private List<T> findList(Key keyObject)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string selectQuery = "";

                if (typeof(T).Name == "Book")
                {
                    tableName = "Book";
                    selectQuery = string.Format("Select * From {0} \n Where {1} = {2};", tableName, keyObject.name, keyObject.value);

                    Console.WriteLine("{0}", selectQuery);
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlite);
                    SQLiteDataReader r = selectCommand.ExecuteReader();
                    List<T> ListObj = new List<T>();
                    while (r.Read())
                    {
                        T res = (T)Activator.CreateInstance(typeof(T));
                        ListObj.Add(res);
                        FieldInfo[] fields = typeof(T).GetFields();
                        fields[0].SetValue(res, r.GetInt32(0));
                        fields[1].SetValue(res, r.GetString(1));
                        //var ep = new EntityManager<Publisher>();
                        //fields[2].SetValue(res, ep.find(r.GetInt32(2)));

                    }
                    sqlite.Close();
                    return (List<T>)ListObj;
                }
                else
                    throw new Exception(" T type error");

            }
        }

        public void persist(T entity)
        {
            using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
            {
                sqlite.Open();
                string tableName = "";
                string insertQuery = "";
                FieldInfo[] fields = typeof(T).GetFields();
                string values = fields[0].GetValue(entity).ToString();

                if (entity is Book)
                {
                    tableName = "Book";
                    values = string.Format("{0},'{1}'", values, fields[1].GetValue(entity).ToString());
                    values = string.Format("{0},'{1}'", values, ((Publisher)fields[fields.Length - 1].GetValue(entity)).id);
                    insertQuery = string.Format("Insert Into {0} \n Values({1});", tableName, values);
                }
                else if (entity is Publisher)
                {
                    tableName = "Publisher";
                    values = string.Format("{0},'{1}'", values, fields[1].GetValue(entity).ToString());
                    if ((bool)fields[fields.Length - 1].GetValue(entity))
                        values = string.Format("{0},'true'", values);
                    else
                        values = string.Format("{0},'false'", values);
                    insertQuery = string.Format("Insert Into {0} \n Values({1});", tableName, values);
                }
                else
                    throw new Exception(" entity type error");
                Console.WriteLine("{0}", insertQuery);
                SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, sqlite);
                insertCommand.ExecuteNonQuery();
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

                if (entity is Book)
                {
                    tableName = "Book";
                    deleteQuery = string.Format("Delete From {0} \n Where id = {1};", tableName, values);
                }
                else if (entity is Publisher)
                {
                    tableName = "Publisher";
                    deleteQuery = string.Format("Delete From {0} \n Where id = {1};", tableName, values);
                }
                Console.WriteLine("{0}", deleteQuery);
                SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, sqlite);
                deleteCommand.ExecuteNonQuery();
                sqlite.Close();
            }
        }
    }
}
