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
using Anotation_system.Test;

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
            //Type booktype = results.CompiledAssembly.GetType("Book");                   
            //object book = results.CompiledAssembly.CreateInstance("Book");
            //Obj.Add(book);
            //MethodInfo method = type.GetMethod("Execute");
            //method.Invoke(null, new object[] { });
            //Type publishertype = results.CompiledAssembly.GetType("Publisher");
            //object publisher = results.CompiledAssembly.CreateInstance("Publisher");
            /*
            T res = (T)Activator.CreateInstance(typeof(T));
            FieldInfo[] fields = typeof(T).GetFields();
            fields[fields.Length - 1].SetValue(res, result1);
            */
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
            Publisher p = new Publisher();
            p.id = 1;
            p.name = "BBC";
            Publisher q = new Publisher();
            q.id = 2;
            q.name = "CNN";
            Book b = new Book();
            b.id = 1;
            b.title = "CS";
            b.publisher = p;
            Book c = new Book();
            c.id = 2;
            c.title = "AI";
            c.publisher = p;
            p.books = new List<Book>();
            p.books.Add(b);                
            var ep = new EntityManager<Publisher>();
            ep.remove(p);
            ep.persist(p);
            ep.remove(q);
            ep.persist(q);
            var eb = new EntityManager<Book>();
            eb.remove(b);
            eb.persist(b);
            eb.remove(c);
            eb.persist(c);
            Book rb = eb.find(1);
            Publisher rp = ep.find(1);
            string query1 = string.Format("Delete From book \n Where id = 2; ");
            string query2 = string.Format("Select * From book \n Where id = 1; ");
            string query3 = string.Format("Update book \n Set title = 'CNN' \n Where id = 1; ");
            List<Book> ObjList1 = new List<Book>();
            List<Book> ObjList2 = new List<Book>();
            Query<Book> q1 = eb.createQuery(query3);
            Query<Book> q2 = eb.createQuery(query2);
            q1.execute();
            ObjList2 = q2.getResultList();
            return;
        }
    }
}
