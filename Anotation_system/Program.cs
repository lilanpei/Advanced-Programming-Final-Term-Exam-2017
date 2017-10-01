using Annotation_system.Annotations;
using Annotation_system.Parser;
using Anotation_system.CodeGenerator;
using Anotation_system.Entity;
using Anotation_system.Parser;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Annotation_system
{
    class Program
    {
        static void Main(string[] args)
        {
            var res = new TestParser().Test();
            new CsharpCodeGenerator(res).GenerateCode();
            new SQLCodeGenerator(res).GenerateCode();
            CompilerResults cr = (new CsharpCodeCompiler()).CompilerCsharpCode();

            var book1 = cr.CompiledAssembly.CreateInstance("foo.Book");
            var book2 = cr.CompiledAssembly.CreateInstance("foo.Book");

            var publisher1 = cr.CompiledAssembly.CreateInstance("foo.Publisher");
            var publisher2 = cr.CompiledAssembly.CreateInstance("foo.Publisher");

            FieldInfo[] fields_book = book1.GetType().GetFields();
            FieldInfo[] fields_publisher = publisher1.GetType().GetFields();

            fields_publisher[0].SetValue(publisher1, 1);
            fields_publisher[1].SetValue(publisher1, "BBC");
            fields_publisher[0].SetValue(publisher2, 2);
            fields_publisher[1].SetValue(publisher2, "CNN");
            fields_book[0].SetValue(book1, 1);
            fields_book[1].SetValue(book1, "CS");
            fields_book[2].SetValue(book1, publisher1);
            fields_book[0].SetValue(book2, 2);
            fields_book[1].SetValue(book2, "AI");
            fields_book[2].SetValue(book2, publisher1);

            Type type = typeof(List<>).MakeGenericType(fields_publisher[2].FieldType.GetGenericArguments()[0]);
            var books = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("Add");

            method.Invoke(books, new object[] { book1 });
            method.Invoke(books, new object[] { book2 });
            fields_publisher[2].SetValue(publisher1, books);

            if (!System.IO.File.Exists("annotation.sqlite"))
            {
                SQLiteConnection.CreateFile("annotation.sqlite");
                using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=annotation.sqlite"))
                {
                    sqlite.Open();
                    string createTableQuery = File.ReadAllText(@"SQLcode.txt");
                    SQLiteCommand command = new SQLiteCommand(createTableQuery, sqlite);
                    command.ExecuteNonQuery();
                    sqlite.Close();
                }
            }

            Type type_p = typeof(EntityManager<>).MakeGenericType(publisher1.GetType());
            var ep = Activator.CreateInstance(type_p);

            MethodInfo find_p = type_p.GetMethod("find");
            MethodInfo remove_p = type_p.GetMethod("remove");
            MethodInfo persist_p = type_p.GetMethod("persist");
            MethodInfo createQuery_p = type_p.GetMethod("createQuery");

            remove_p.Invoke(ep, new object[] { publisher1 });
            remove_p.Invoke(ep, new object[] { publisher1 });
            persist_p.Invoke(ep, new object[] { publisher1 });
            remove_p.Invoke(ep, new object[] { publisher2 });
            persist_p.Invoke(ep, new object[] { publisher2 });

            Type type_b = typeof(EntityManager<>).MakeGenericType(book1.GetType());
            var eb = Activator.CreateInstance(type_b);

            MethodInfo find_b = type_b.GetMethod("find");
            MethodInfo remove_b = type_b.GetMethod("remove");
            MethodInfo persist_b = type_b.GetMethod("persist");
            MethodInfo createQuery_b = type_b.GetMethod("createQuery");

            remove_b.Invoke(eb, new object[] { book1 });
            persist_b.Invoke(eb, new object[] { book1 });
            remove_b.Invoke(eb, new object[] { book2 });
            persist_b.Invoke(eb, new object[] { book2 });

            find_b.Invoke(eb, new object[] { 1 });
            find_p.Invoke(ep, new object[] { 1 });

            string query1 = string.Format("Delete From book \n Where id = 2; ");
            string query2 = string.Format("Select * From book \n Where id = 1; ");
            string query3 = string.Format("Update book \n Set title = 'CNN' \n Where id = 1; ");

            Type type_q = typeof(Query<>).MakeGenericType(book1.GetType());

            MethodInfo execute = type_q.GetMethod("execute");
            MethodInfo getResultList = type_q.GetMethod("getResultList");
            execute.Invoke(createQuery_b.Invoke(eb, new object[] { query3 }), new object[] { });
            var resgrl = getResultList.Invoke(createQuery_b.Invoke(eb, new object[] { query2 }), new object[] { });

            return;
        }
    }
}
