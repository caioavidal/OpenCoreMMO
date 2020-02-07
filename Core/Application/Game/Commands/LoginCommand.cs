public class LoginCommand: ICommand
{
    public Authentication Auth { get; private set; }
    public LoginCommand(Authentication auth)
    {
        Auth = auth;
    }
    public LoginCommand()
    {
    }
}