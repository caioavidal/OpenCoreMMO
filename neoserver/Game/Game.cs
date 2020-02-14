public class Game
{
    public static GameState State { get; private set; }

    public void Load()
    {

    }

    public static void Initialize()
    {
        State = GameState.STARTUP;
    }

    public static string InfoMessage { get; } = $"{GameDefinition.ServerName} - Version {GameDefinition.ServerVersion}";
    //todo: add more info here!!!


}