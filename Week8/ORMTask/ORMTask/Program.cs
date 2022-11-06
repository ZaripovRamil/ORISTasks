using ORMTask.Models;
using ORMTask.ORM;

Console.WriteLine(nameof(Account));
Console.WriteLine($"{nameof(Account)}s");
var orm = new MyOrm(
    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=steamDB;Integrated Security=True;");
orm.Insert(new Account("name2", 1843567));
foreach (var account in orm.Select<Account>())
{
    Console.WriteLine($"{account.Id} {account.Username} {account.Password}");
}

