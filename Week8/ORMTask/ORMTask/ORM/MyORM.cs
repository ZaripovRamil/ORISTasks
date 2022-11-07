using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ORMTask.Attributes;

namespace ORMTask.ORM;

internal class MyOrm
{
    private readonly string _connectionString;

    public MyOrm(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<T> Select<T>()
    {
        var table = $"{typeof(T).Name}s";
        var query = $"SELECT * FROM {table}";
        using var connection = new SqlConnection(_connectionString);
        var queryData = RunReadableQuery(query, connection);
        return ParseSelectResult<T>(queryData).ToList();
    }

    public IEnumerable<T> Select<T>(string column, object value)
    {
        var table = $"{typeof(T).Name}s";
        var query = $"SELECT * FROM {table} WHERE {column} = '{value.ToString()}'";
        using var connection = new SqlConnection(_connectionString);
        var queryData = RunReadableQuery(query, connection);
        return ParseSelectResult<T>(queryData).ToList();
    }

    public void Insert<T>(T item)
    {
        var table = $"{typeof(T).Name}s";
        var lineData = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute(typeof(ColumnName)) != null)
            .ToDictionary(p => ((ColumnName) p.GetCustomAttribute(typeof(ColumnName))!).columnName,
                p => p.GetValue(item)?.ToString());

        var query =
            $"INSERT INTO {table} " +
            $"({string.Join(", ", lineData.Keys)}) VALUES ('{string.Join("', '", lineData.Values)}')";

        using var connection = new SqlConnection(_connectionString);
        RunNonReturningQuery(query, connection);
    }

    public int Update<T>(T oldT, T newT)
    {
        var idColumn = typeof(T).GetProperties().FirstOrDefault(p => Attribute.IsDefined(p, typeof(Id)));
        if (idColumn is null) throw new Exception("no id column in model");
        var table = $"{typeof(T).Name}s";
        var lineData = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute(typeof(ColumnName)) != null)
            .ToDictionary(p => ((ColumnName) p.GetCustomAttribute(typeof(ColumnName))!).columnName,
                p => p.GetValue(newT)?.ToString());
        var query =
            $"UPDATE {table} SET {string.Join(", ", lineData.Select(pair => $"{pair.Key} = '{pair.Value}'"))} WHERE " +
            $"{((Id) idColumn.GetCustomAttribute(typeof(Id))!).columnName} = {idColumn.GetValue(oldT)}";
        using var connection = new SqlConnection(_connectionString);
        return RunNonReturningQuery(query, connection);
    }

    public int Update<T>(int id, string column, object value)
    {
        var table = $"{typeof(T).Name}s";
        var query = $"UPDATE {table} SET {column} = '{value}' WHERE id = {id}";
        using var connection = new SqlConnection(_connectionString);
        return RunNonReturningQuery(query, connection);
    }

    public int Delete<T>(string column, object value)
    {
        var table = $"{typeof(T)}s";
        var query = $"DELETE FROM {table} WHERE {column} = '{value}'";
        using var connection = new SqlConnection(_connectionString);
        return RunNonReturningQuery(query, connection);
    }

    private static IEnumerable<T> ParseSelectResult<T>(IDataReader queryData)
    {
        var ctor = typeof(T).GetConstructors()
            .FirstOrDefault(ctor => Attribute.IsDefined(ctor, typeof(DbRecordCtor)));
        if (ctor is null)
            throw new InvalidDataException("No defined constructor to create from database args");
        while (queryData.Read())
            yield return (T) ctor.Invoke(ParseLine(queryData).ToArray());
        queryData.Close();
    }

    private static IEnumerable<object> ParseLine(IDataRecord queryData)
    {
        return Enumerable
            .Range(0, queryData.FieldCount)
            .Select(queryData.GetValue);
    }

    private static SqlDataReader RunReadableQuery(string query, SqlConnection connection)
    {
        connection.Open();
        Console.WriteLine(query);
        var result = new SqlCommand(query, connection).ExecuteReader();
        return result;
    }

    private static int RunNonReturningQuery(string query, SqlConnection connection)
    {
        connection.Open();
        return new SqlCommand(query, connection).ExecuteNonQuery();
    }
}