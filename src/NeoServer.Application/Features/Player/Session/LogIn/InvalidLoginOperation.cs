using System.ComponentModel;

namespace NeoServer.Application.Features.Player.Session.LogIn;

public enum InvalidLoginOperation
{
    None,
    
    [Description("You may only login with one character of your account at the same time.")]
    CannotLogInWithMultipleCharacters,
    
    PlayerAlreadyLoggedIn,

    [Description("Account name or password is not correct.")]
    AccountOrPasswordIncorrect,

    [Description("Your account has been banished.")]
    AccountIsBanned,

    [Description("This player type is not supported by the game.")]
    PlayerTypeNotSupported,

    [Description("You must enter your account name.")]
    InvalidAccountName,

    [Description("Only clients with protocol {{version}} allowed!")]
    ClientVersionNotAllowed,

    [Description("Game World is starting up. Please wait.")]
    GameWorldIsStartingUp,

    [Description("Game World is under maintenance. Please re-connect in a while.")]
    GameWorldIsUnderMaintenance,

    [Description("Server is currently closed.\nPlease try again later.")]
    ServerIsCurrentlyClosed
}

public static class InvalidLoginOperationExtension
{
    public static bool IsValid(this InvalidLoginOperation invalidLoginOperation) =>
        invalidLoginOperation is InvalidLoginOperation.None;
    
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var memInfo = type.GetMember(value.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        var attribute = attributes.Length > 0 ? (DescriptionAttribute)attributes[0] : null;
        return attribute?.Description;
    }
}