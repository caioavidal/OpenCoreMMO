public class LoginErrorOutput : OutputMessage
{
    public LoginErrorOutput(string message, uint[] xtea) : base(6)
    {
        AddUInt16((ushort)(message.Length + 3));
        AddByte(0x0A);
        AddString(message);

        Xtea.Encrypt(this, xtea);

        AddHeader(true);
    }
}