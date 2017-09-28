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
            if (!System.IO.File.Exists("annotation.sqlite"))
            {
                Console.WriteLine("Just entered to create Sync DB");
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
            p.hasbook = true;                    
            var ep = new EntityManager<Publisher>();
            ep.persist(p);
            ep.remove(p);
            ep.persist(q);
            ep.persist(p);
            var eb = new EntityManager<Book>();
            eb.persist(b);
            eb.remove(b);
            eb.persist(c);
            eb.persist(b);
            Book rb = eb.find(1);
            Publisher rp = ep.find(1);
            string query1 = string.Format("Update book Set title = 'CNN' \n Where id = 1; ");
            string query2 = string.Format("Select * From book \n Where id = 2; ");
            List<Book> ObjList1 = new List<Book>();
            List<Book> ObjList2 = new List<Book>();
            Query<Book> q1 = eb.createQuery(query1);
            Query<Book> q2 = eb.createQuery(query2);
            q1.execute();
            ObjList2 = q2.getResultList();
            return;
        }
    }
}
