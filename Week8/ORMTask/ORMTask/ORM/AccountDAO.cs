using ORMTask.Models;

namespace ORMTask.ORM;

internal class AccountDao
{
    private MyOrm Orm { get; }

    public AccountDao(string connectionString, string tableName)
    {
        Orm = new MyOrm(connectionString);
    }

    public IEnumerable<Account> Select() => Orm.Select<Account>();

    public Account? SelectById(int id) => Orm.Select<Account>("id", id).FirstOrDefault();
    
    public Account? SelectByUsername(string username) => Orm.Select<Account>("username", username).FirstOrDefault();

    public void Insert(Account account) => Orm.Insert(account);

    public void DeleteById(int id) => Orm.Delete<Account>("id", id);

    public void DeleteByUsername(string username) => Orm.Delete<Account>("username", username);
}