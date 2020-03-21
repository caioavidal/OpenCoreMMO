using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model
{
    public class AccountModel
    {
        public AccountModel(string accountName, string password)
        {
            AccountName = accountName;
            Password = password;
        }

        public AccountModel()
        {
            
        }

        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int PremiumTime { get; set; }
        public ICollection<PlayerModel> Players { get; set; } = new List<PlayerModel>();

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(AccountName) &&
            !string.IsNullOrWhiteSpace(Password);


    }
}
