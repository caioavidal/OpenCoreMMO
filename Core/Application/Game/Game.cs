public class Game
{
    public GameState State { get; private set; }
    
    public void Load(){

    }

    public void Start(){
        State = GameState.STARTUP;
    }

    public string GetInfoMessage(){
        return $"{GameDefinition.ServerName} - Version {GameDefinition.ServerVersion}";
        //todo: add more info here!!!
    }

}