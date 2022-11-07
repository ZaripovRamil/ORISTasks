using ORMTask.Attributes;

namespace ORMTask.Models;

public class Account
{
    [Id("id")]
    public int? Id { get; }
    [ValueColumn("username")]
    public string Username { get; }
    [ValueColumn("password")]
    public int Password { get; set; }

    public Account(string username, int password)
    {
        Username = username;
        Password = password;
    }

    [DbRecordCtor]
    public Account(int id, string username, int password)
    {
        Id = id;
        Username = username;
        Password = password;
    }
}