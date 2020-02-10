public class LoginCommand: ICommand
{
    public LoginInput Auth { get; private set; }
    
    public LoginCommand(LoginInput auth)
    {
        Auth = auth;
    }
    public LoginCommand()
    {
    }
}