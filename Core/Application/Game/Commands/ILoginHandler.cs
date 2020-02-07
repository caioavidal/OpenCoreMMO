using System;

public class LoginHandler : ICommandHandler<LoginCommand>
{
    public void Handle(LoginCommand command)
    {
        //execute
        Console.WriteLine("Login");
    }
}