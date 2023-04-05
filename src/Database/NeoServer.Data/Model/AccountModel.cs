using System;
using System.Collections.Generic;
using NeoServer.Data.Enums;
using NeoServer.Data.Helpers;

namespace NeoServer.Data.Model;

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
    public bool AllowManyOnline { get; set; }
    public AccountType Type { get; set; }
    
    public DateTime? BanishedAt { get; set; }
    public string BanishmentReason { get; set; }
    public uint? BannedBy { get; set; }

    public virtual ICollection<PlayerModel> Players { get; set; }
    public virtual ICollection<AccountVipListModel> VipList { get; set; }

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

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}