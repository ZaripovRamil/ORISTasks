using ORMTask.Models;

namespace ORMTask.ORM;

internal class AccountRepository
{
    private readonly Dictionary<int, Account> _repository;
    private readonly MyOrm _orm;

    public AccountRepository(string connectionString)
    {
        _orm = new MyOrm(connectionString);
        _repository = _orm.Select<Account>().ToDictionary(account => (int)account.Id, account => account);
    }

    public Account[] Select() => _repository.Values.ToArray();

    public Account Select(int id) => _repository[id];

    public void Insert(Account account)
    {
        _orm.Insert(account);
        var accountWithId = _orm.Select<Account>("username", account.Username).FirstOrDefault();
        _repository.Add((int)accountWithId.Id, accountWithId);
    }

    public void Delete(int id)
    {
        _repository.Remove(id);
        _orm.Delete<Account>("id",id);
    }
}