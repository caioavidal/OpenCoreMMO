using System;
using System.Text;

public class InputMessage
{
    public byte[] Buffer { get; private set; }
    public int BytesRead {get{
        return _position;
    }}
    private int _position;
    public InputMessage(byte[] buffer)
    {
        Buffer = buffer;

        //var login = new LoginInput(loginData, handler);
    }

    public ushort GetUInt16()
    {
        var to = _position + 2;
        var value = BitConverter.ToUInt16(Buffer[_position..to], 0);
        _position = to;
        return value;

    }
    public uint GetUInt32()
    {
        var to = _position + 4;
        var value = BitConverter.ToUInt32(Buffer[_position..to], 0);
        _position = to;
        return value;
    }

    public byte GetByte(){
         var to = _position + 1;
        var value = Buffer[_position..to];
        _position = to;
        return value[0];
    }
    public byte[] GetBytes(int length){
         var to = _position + length;
        var value = Buffer[_position..to];
        _position = to;
        return value;
    }

    public string GetString(){
    
        var length = GetUInt16();
        var to = _position + length;

        var value = Encoding.UTF8.GetString(Buffer[_position..to]);
        _position = to;

        return value;
    }


}