namespace NeoServer.Web.Shared.ViewModels.Request;

public class AccountPostRequest
{
    public string Password { get; set; }
    public string Email { get; set; }
    public int PremiumDays { get; set; }
}