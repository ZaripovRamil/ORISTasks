using ORMTask.Models;
using ORMTask.ORM;

Console.WriteLine(nameof(Account));
Console.WriteLine($"{nameof(Account)}s");
var orm = new MyOrm(
    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=steamDB;Integrated Security=True;");
var oldAcc = orm.Select<Account>("id", 2).FirstOrDefault();
var newAcc = new Account("updatedName2", 564651813);
orm.Update(oldAcc, newAcc);
foreach (var account in orm.Select<Account>())
    Console.WriteLine($"{account.Id} {account.Username} {account.Password}");

