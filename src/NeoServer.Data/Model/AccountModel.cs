using NeoServer.Data.Enums;
using NeoServer.Data.Helpers;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;

namespace NeoServer.Data.Model
{

    public class AccountModel
    {
        public AccountModel()
        {
            Players = new HashSet<PlayerModel>();
        }

        public int AccountId { get; set; }
        public int _creation { get; set; }
        public string Email { get; set; }
        public uint _lastday { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int PremiumTime { get; set; }
        public string Secret { get; set; }
        public AccountType Type { get; set; }


        public virtual ICollection<PlayerModel> Players { get; set; }

        public DateTime Creation
        {
            get => DateTimeHelper.FromUnixTime(_creation);

            set => _creation = DateTimeHelper.ToUnixTimeInt(value);
        }

        public DateTime LastDay
        {
            get => DateTimeHelper.FromUnixTime(_lastday);

            set => _lastday = (uint)DateTimeHelper.ToUnixTimeInt(value);
        }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Name) &&
            !string.IsNullOrWhiteSpace(Password);
    }
}
