using System;

public class LoadServerCommand
{
    public LoadServerCommand()
    {
      
    }

    public void Execute(){
        Game.Initialize();
        Console.WriteLine(Game.InfoMessage);

        GameConfiguration.Load();

        VocationConfig.Load();
    }
}