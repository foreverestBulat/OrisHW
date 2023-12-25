using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORM;

public class MyDataContext : IMyDataContext
{
    public string ConnectionString { get; }

    public MyDataContext(string Host, string User, string DBname, string Port, string Password)
    {
        ConnectionString =
            String.Format(
                "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                Host,
                User,
                DBname,
                Port,
                Password);
    }

    long IMyDataContext.Count<T>()
    {
        long count = 0;

        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var type = typeof(T);
            var tableName = type.Name;
            var sqlExpression = $"SELECT COUNT(*) FROM {tableName}s";
            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);

            count = (long)command.ExecuteScalar();
        }

        return count;
    }

    void IMyDataContext.Add<T>(T row)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();

            var type = row?.GetType();
            var properties = type?.GetProperties();
            var tableName = type?.Name;
            var sqlExpression = $"SELECT * FROM {tableName}s";

            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
            StringBuilder sbInsert = new StringBuilder($"INSERT INTO {tableName}s (");
            StringBuilder sbValues = new StringBuilder($"VALUES(");
            NpgsqlDataReader reader = command.ExecuteReader();

            var nameFields = new List<string>();
            var values = new List<object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);

                sbInsert.Append($"{name}");

                if (i < reader.FieldCount - 1)
                    sbInsert.Append(',');
                sbValues.Append($"@{name}");
                if (i < reader.FieldCount - 1)
                    sbValues.Append(',');

                object value = null;
                foreach (var property in properties)
                    if (property?.Name.ToLower() == name.ToLower())
                        value = property?.GetValue(row);

                nameFields.Add(name);
                values.Add(value);
            }
            reader.Close();

            sbValues.Append(')');
            var sqlExpressionInsert = sbInsert.Append($") {sbValues.ToString()}").ToString();
            command.CommandText = sqlExpressionInsert;

            foreach (var (name, value) in nameFields.Zip(values))
            {
                NpgsqlParameter param = new NpgsqlParameter($"@{name}", value);
                command.Parameters.Add(param);
            }

            command.ExecuteNonQuery();
        }
    }

    void IMyDataContext.Update<T>(T row)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();

            var type = row?.GetType();
            var properties = type?.GetProperties();
            var tableName = type?.Name;
            var sqlExpression = $"SELECT * FROM {tableName}s";

            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
            StringBuilder sbUpdate = new StringBuilder($"UPDATE {tableName}s SET ");
            NpgsqlDataReader reader = command.ExecuteReader();

            string field = null;
            int id = -1;
            while (reader.Read())
                if (reader.GetValue(0).Equals(properties?[0].GetValue(row)))
                {
                    field = reader.GetName(0);
                    id = (int)reader.GetValue(0);
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        sbUpdate.Append($"{reader.GetName(j)} = '{properties?[j].GetValue(row)}'");
                        if (j < reader.FieldCount - 1)
                            sbUpdate.Append(',');
                    }
                    break;
                }
            reader.Close();

            sbUpdate.Append($" WHERE {field} = {id}");
            var sqlExpressionUpdate = sbUpdate.ToString();
            command.CommandText = sqlExpressionUpdate;
            command.ExecuteNonQuery();
        }
    }

    void IMyDataContext.Delete<T>(int id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var type = typeof(T);
            var tableName = type.Name;
            var sqlExpression = $"DELETE FROM {tableName}s WHERE {type.GetProperties()[0].Name} = {id}";
            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
        }
    }

    List<T> IMyDataContext.Select<T>()
    {
        List<T> list = new List<T>();
        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();

            var type = typeof(T);
            var properties = type.GetProperties();
            var tableName = type.Name;
            var sqlExpression = $"SELECT * FROM {tableName}s";

            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            ConstructorInfo info = type.GetConstructor(new Type[] { });
            while (reader.Read())
            {
                object obj = info.Invoke(new object[] { });
                for (int i = 0; i < reader.FieldCount; i++)
                    for (int j = 0; j < properties.Length; j++)
                        if (properties[j].Name.ToLower() == reader.GetName(i).ToLower())
                        {
                            properties[j].SetValue(obj, reader.GetValue(i)); // i <--> j
                            break;
                        }

                list.Add((T)obj);
            }
            reader.Close();
            command.ExecuteNonQuery();
        }
        return list;
    }

    T IMyDataContext.SelectByID<T>(int id)
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        var tableName = type.Name;

        ConstructorInfo info = type.GetConstructor(new Type[] { });
        object obj = info.Invoke(new object[] { });

        using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var sqlExpression = $"SELECT * FROM {tableName}s WHERE {properties[0].Name.ToLower()} = {id}";

            NpgsqlCommand command = new NpgsqlCommand(sqlExpression, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        if (properties[i].Name.ToLower() == reader.GetName(j).ToLower())
                        {
                            properties[i].SetValue(obj, reader.GetValue(j));
                            break;
                        }
                    }
                }
            }

            reader.Close();
            command.ExecuteNonQuery();
        }

        return (T)obj;
    }
}