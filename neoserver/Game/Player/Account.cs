using System.Collections.Generic;

public class Account {
    public string AccountName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public int PremiumTime { get; set; }
    public ICollection<Player> Players { get; set; } = new List<Player>();
}