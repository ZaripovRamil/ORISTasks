using System.Data.SqlClient;
using HttpServerTask.Attributes;
using HttpServerTask.Models;
using HttpServerTask.ORM;

namespace HttpServerTask.Controllers
{
    [ApiController("accounts")]
    public class AccountController
    {
        private static AccountRepository repo;

        public AccountController(string connectionString)
        {
            repo = new AccountRepository(connectionString);
        }

        [HttpPOST]
        public static void SaveAccount(string login, string password)
        {
            repo.Insert(new Account(login, password.GetHashCode()));
        }

        [HttpGET(@"\d")]
        public static Account GetAccountById(int id)
        {
            return repo.Select(id);
        }

        [HttpGET]
        public static Account[] GetAccounts()
        {
            return repo.Select();
        }
    }
}