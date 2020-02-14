public class CharlistOutput : OutputMessage
{

    public CharlistOutput(Account account, uint[] xtea) : base(6)
    {
        LoadOutput(account, xtea);
    }
    private void LoadOutput(Account account, uint[] xtea)
    {
        AddCharList(account);
        Xtea.Encrypt(this, xtea);
        AddHeader(true);
    }

    private void AddCharList(Account account)
    {
        var charListOutput = GetCharList(account);
        var payloadLength = charListOutput.Length;

        AddUInt16((ushort)payloadLength);

        for (int i = 0; i < payloadLength; i++)
        {
            AddByte(charListOutput.Buffer[i]);
        }
    }
    private OutputMessage GetCharList(Account account)
    {

        var output = new OutputMessage(0);
        //output.AddByte(0x14); todo: modt
        output.AddByte(0x64); //todo charlist
        output.AddByte((byte)account.Players.Count);
        foreach (var player in account.Players)
        {
            output.AddString(player.Name);
            output.AddString(GameDefinition.ServerName);
            output.AddByte(127);
            output.AddByte(0);
            output.AddByte(0);
            output.AddByte(1);
            output.AddUInt16(7172);
           

        }
         output.AddUInt16((ushort)account.PremiumTime);
        return output;
    }

    private void AddWorlds()
    {
        AddByte(1); // number of worlds
        AddByte(0); // world id
        AddString(GameDefinition.ServerName);
        AddString("localhost");
        AddUInt16(7171);
        AddByte(0);
    }
}