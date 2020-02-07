using System.Collections.Generic;

public interface ICommandHandler<T> where T : ICommand
{
    void Handle(T command);
}
