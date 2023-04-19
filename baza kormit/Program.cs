using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace baza_kormit
{
    class Program
    {
        static void Main(string[] args)
        {
            InUser1(1);

            InUser1(0);
        }

        static ICollection<TRecord> ReadFromTable<TRecord>(
            string connectionString,
            string sql, 
            Func<IDataReader, TRecord> func,
            IEnumerable<SQLiteParameter> parameters = null)
        {
            Console.WriteLine(nameof(ReadFromTable));
            using var connection = new SQLiteConnection(connectionString);
            using var comand = new SQLiteCommand(sql, connection);
            
            if (parameters != null)
            {
                foreach (var z in parameters)
                {
                    comand.Parameters.Add(z);
                }
            }

            connection.Open();
            using DbDataReader reader = comand.ExecuteReader();

            List<TRecord> items = new List<TRecord>();

            while (reader.Read())
            {
                var u = func(reader);
                items.Add(u);

            }

            return items;
        }

        static void InUser0()
        {
            Console.WriteLine(nameof(InUser0));
           var users = ReadFromTable(
                "Data Source = somebody",
                 @"Select id, name
                      From Users",
                 reader => new User
                 {
                     Id = reader.GetInt32(0),
                     Name = reader.GetString(1)
                 });
            

            foreach (var u in users)
            {
                Console.WriteLine($"id: {u.Id} name: {u.Name}");
            }
        }

        static void InUser1(int id)
        {
            Console.WriteLine(nameof(InUser1));
            var users = ReadFromTable(
                 "Data Source = somebody",
                  @"Select id, name
                      From Users
                    where id = @id",
                  reader => new User
                  {
                      Id = reader.GetInt32(0),
                      Name = reader.GetString(1)
                  },
                  new[] { new SQLiteParameter("@id", id) });


            foreach (var u in users)
            {
                Console.WriteLine($"id: {u.Id} name: {u.Name}");
            }
        }

        static void InUser()
        {
            using var connection = new SQLiteConnection("Data Source = somebody");
            using var comand = new SQLiteCommand(
                    @"Select id, name
                      From Users", connection);
            
            connection.Open();
            using DbDataReader reader = comand.ExecuteReader();

            List<User> users = new List<User>();

            while (reader.Read())
            {
                var u = new User();
                u.Id = reader.GetInt32(0);
                u.Name = reader.GetString(1);
                users.Add(u);
                
            }

            foreach (var u in users)
            {
                Console.WriteLine($"id: {u.Id} name: {u.Name}");
            }
        }

        static void Init()
        {
            SQLiteConnection.CreateFile("somebody");
            using var connection = new SQLiteConnection("Data Source = somebody");
            using var comand = new SQLiteCommand("create table Users(id int, name varchar(20))", connection);

            connection.Open();
            comand.ExecuteNonQuery();

            using var comandinsert = new SQLiteCommand(@"
                                        insert into Users 
                                        values (0, 'cherepaha'), 
                                                (1, 'verblud')
                                                 ", connection);
            
           var res = comandinsert.ExecuteNonQuery();
            Console.WriteLine(res);
        }

    }
}
