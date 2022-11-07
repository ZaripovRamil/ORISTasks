using ORMTask.Models;

namespace ORMTask.ORM;

internal class AccountRepository
{
    private readonly Dictionary<int, Account> _repository;
    private MyOrm Orm { get; }

    public AccountRepository(string connectionString)
    {
        Orm = new MyOrm(connectionString);
        _repository = Orm.Select<Account>().ToDictionary(account => account.Id!.Value, account => account);
    }

    public Account[] Select() => _repository.Values.ToArray();

    public Account Select(int id) => _repository[id];

    public void Insert(Account account)
    {
        Orm.Insert(account);
        var accountWithId = Orm.Select<Account>("username", account.Username).FirstOrDefault();
        _repository.Add(accountWithId!.Id!.Value, accountWithId);
    }

    public void UpdatePassword(int id, int password)
    {
        _repository[id].Password = password;
        Orm.Update<Account>(id, "password", password);
    }

    public void Delete(int id)
    {
        _repository.Remove(id);
        Orm.Delete<Account>("id", id);
    }
}