using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;

namespace NeoServer.Data.Model
{
  
    public class AccountModel : IAccountModel
    {
        public AccountModel(string accountName, string password)
        {
            AccountName = accountName;
            Password = password;
        }

        public AccountModel()
        {

        }

        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int PremiumTime { get; set; }
        public IEnumerable<IPlayerModel> Players { get; set; } = new List<PlayerModel>();
        public IEnumerable<PlayerModel> Characters { get; set; } = new List<PlayerModel>();

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(AccountName) &&
            !string.IsNullOrWhiteSpace(Password);

    }
}
