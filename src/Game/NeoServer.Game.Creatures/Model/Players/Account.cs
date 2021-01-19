using NeoServer.Server.Model.Players;
using System.Collections.Generic;

namespace NeoServer.Server.Model
{
    public class Account
    {
        public Account(string accountName, string password)
        {
            AccountName = accountName;
            Password = password;
        }

        public Account()
        {

        }

        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int PremiumTime { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(AccountName) &&
            !string.IsNullOrWhiteSpace(Password);

    }
}
