using System;
using System.Collections.Generic;

public class LoginHandler : ICommandHandler<LoginCommand>
{
    public async void Handle(LoginCommand command)
    {
        var socket = command.Auth.Socket;

          new LoginErrorOutput($"Invalid account name.", command.Auth.Xtea)
            .Send(socket);

        if(string.IsNullOrWhiteSpace(command.Auth.AccountName)){
            new LoginErrorOutput($"Invalid account name.", command.Auth.Xtea)
            .Send(socket);
            return;
        }
        if(string.IsNullOrWhiteSpace(command.Auth.Password)){
            new LoginErrorOutput($"Invalid password.", command.Auth.Xtea)
            .Send(socket);
            return;
        }
        //execute
        if (command.Auth.Version != GameDefinition.ClientVersion)
        {

            new LoginErrorOutput($"Only clients with protocol {GameDefinition.ClientVersion} allowed!", command.Auth.Xtea)
            .Send(socket);
            return;
        }


        var account = await new AccountRepository().Get(command.Auth.AccountName,
         command.Auth.Password);

        if (account == null)
        {
            new LoginErrorOutput("Account name or password is not correct.", command.Auth.Xtea)
            .Send(socket);
        }
        else
        {
            new CharlistOutput(account, command.Auth.Xtea)
            .Send(socket);

        }
    }
}